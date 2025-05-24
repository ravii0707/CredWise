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
                _logger.LogInformation("Registering user with email: {Email}", registerDto?.Email);

                // 1. Enhanced null check
                if (registerDto == null) throw new ArgumentNullException(nameof(registerDto));

                // 2. Normalize and validate email
                registerDto.Email = registerDto.Email?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(registerDto.Email))
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Email is required");

                // 3. Set default role safely
                registerDto.Role = string.IsNullOrWhiteSpace(registerDto.Role)
                    ? "Customer"
                    : registerDto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                        ? "Admin"
                        : "Customer";

                // 4. Check for existing email
                if (await _userRepository.EmailExists(registerDto.Email))
                    throw new ConflictException($"Email {registerDto.Email} already exists");

                // 5. Map with null check
                userEntity = _mapper.Map<User>(registerDto) ??
                    throw new InvalidOperationException("User mapping failed");

                // 6. Password validation and hashing
                if (string.IsNullOrWhiteSpace(registerDto.Password))
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Password is required");

                userEntity.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // 7. Set audit fields safely
                var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
                userEntity.IsActive = true;
                userEntity.CreatedAt = DateTime.UtcNow;
                userEntity.ModifiedAt = DateTime.UtcNow;
                userEntity.CreatedBy = currentUser;
                userEntity.ModifiedBy = currentUser;

                // 8. Save to database
                await _userRepository.AddAsync(userEntity);
                _logger.LogInformation("User created with ID: {UserId}", userEntity.UserId);

                // 9. Safe email sending (truly non-blocking)
                if (_emailService != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendUserRegistrationEmailAsync(
                                userEntity.Email,
                                userEntity.FirstName
                            );
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Failed to send welcome email to {Email}",
                                userEntity.Email);
                        }
                    }).ConfigureAwait(false);
                }

                // 10. Get fresh copy from DB to ensure all fields are populated
                var savedUser = await _userRepository.GetByIdAsync(userEntity.UserId);
                if (savedUser == null)
                    throw new InvalidOperationException("User not found after creation");

                // 11. Safe response mapping
                var response = _mapper.Map<UserResponseDto>(savedUser) ??
                    throw new InvalidOperationException("Response mapping failed");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed. User state: {UserState}",
                    userEntity != null ? "Created" : "Not Created");

                // Convert to more specific exception if needed
                if (ex is ArgumentNullException or System.ComponentModel.DataAnnotations.ValidationException or ConflictException)
                    throw;

                throw new ApplicationException("An error occurred during registration", ex);
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
                user.ModifiedBy = "System";

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