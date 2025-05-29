using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CredWiseAdmin.Core.Exceptions; // Assuming your custom exceptions are here
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CredWiseAdmin.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailService emailService,
            ILogger<UserService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AdminExists()
        {
            try
            {
                return await _userRepository.AdminExists();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for existing admin");
                throw;
            }
        }

        public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerDto)
        {
            User userEntity = null;
            try
            {
                _logger.LogInformation("Starting user registration process for email: {Email}", registerDto?.Email);

                // 1. Enhanced validation
                if (registerDto == null) 
                {
                    throw new ArgumentNullException(nameof(registerDto), "Registration data cannot be null");
                }

                // 2. Normalize and validate email
                registerDto.Email = registerDto.Email?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(registerDto.Email))
                {
                    throw new Core.Exceptions.ValidationException("Email is required");
                }

                // 3. Validate phone number
                if (string.IsNullOrWhiteSpace(registerDto.PhoneNumber) || 
                    !registerDto.PhoneNumber.All(char.IsDigit) || 
                    registerDto.PhoneNumber.Length != 10)
                {
                    throw new Core.Exceptions.ValidationException("Invalid phone number format");
                }

                // 4. Check for existing email
                if (await _userRepository.EmailExists(registerDto.Email))
                {
                    throw new ConflictException($"Email {registerDto.Email} is already registered");
                }

                // 5. Set role based on input without authorization check
                if (string.IsNullOrWhiteSpace(registerDto.Role))
                {
                    registerDto.Role = "Customer"; // Default role if not specified
                }

                // 6. Map to entity
                try
                {
                    userEntity = _mapper.Map<User>(registerDto);
                    if (userEntity == null)
                    {
                        throw new InvalidOperationException("User mapping failed");
                    }
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Failed to map registration data to user entity");
                    throw new InvalidOperationException("Failed to process registration data", mapEx);
                }

                // 7. Password hashing with enhanced security
                try
                {
                    userEntity.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, BCrypt.Net.BCrypt.GenerateSalt(12));
                }
                catch (Exception hashEx)
                {
                    _logger.LogError(hashEx, "Failed to hash password");
                    throw new InvalidOperationException("Failed to process password", hashEx);
                }

                // 8. Set audit fields
                userEntity.IsActive = true;
                userEntity.CreatedAt = DateTime.UtcNow;
                userEntity.ModifiedAt = DateTime.UtcNow;
                userEntity.CreatedBy = "System";
                userEntity.ModifiedBy = "System";

                // 9. Save to database
                try
                {
                    await _userRepository.AddAsync(userEntity);
                    _logger.LogInformation("User created successfully with ID: {UserId}", userEntity.UserId);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Failed to save user to database");
                    throw new InvalidOperationException("Failed to save user data", dbEx);
                }

                // 10. Send welcome email asynchronously
                if (_emailService != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendUserRegistrationEmailAsync(userEntity.Email, registerDto.Password);
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Failed to send welcome email to {Email}", userEntity.Email);
                        }
                    });
                }

                // 11. Return response
                try
                {
                    var savedUser = await _userRepository.GetByIdAsync(userEntity.UserId);
                    if (savedUser == null)
                    {
                        throw new InvalidOperationException("Failed to retrieve created user");
                    }
                    return _mapper.Map<UserResponseDto>(savedUser);
                }
                catch (Exception getEx)
                {
                    _logger.LogError(getEx, "Failed to retrieve created user");
                    throw new InvalidOperationException("Failed to retrieve created user", getEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}. Error details: {ErrorDetails}", 
                    registerDto?.Email, ex.ToString());
                throw;
            }
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", id);
                    throw new NotFoundException("User not found");
                }
                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} users", users.Count());
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Attempt to deactivate non-existent user: {UserId}", id);
                    return false;
                }

                if (user.Role == "Admin" && await _userRepository.CountAdmins() <= 1)
                {
                    _logger.LogWarning("Attempt to deactivate last admin: {UserId}", id);
                    throw new InvalidOperationException("Cannot deactivate the last admin");
                }

                user.IsActive = false;
                user.ModifiedAt = DateTime.UtcNow;
                user.ModifiedBy = "System";

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Successfully deactivated user: {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Attempt to update non-existent user: {UserId}", id);
                    return false;
                }

                _mapper.Map(updateDto, user);
                user.ModifiedAt = DateTime.UtcNow;
                user.ModifiedBy = "Admin";

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Successfully updated user: {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                throw;
            }
        }

        public async Task<int> CountAdmins()
        {
            try
            {
                return await _userRepository.CountAdmins();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting admin users");
                throw;
            }
        }
        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Empty email or password provided");
                    return false;
                }

                var normalizedEmail = email.Trim().ToLower();
                var user = await _userRepository.GetByEmailAsync(normalizedEmail);

                if (user == null || !(user.IsActive ?? false))
                {
                    _logger.LogWarning("User not found or inactive: {Email}", normalizedEmail);
                    return false;
                }

                bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

                if (!isValid)
                {
                    _logger.LogWarning("Invalid password for user: {Email}", normalizedEmail);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for {Email}", email);
                return false;
            }
        }
    }
}