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

                // 1. Enhanced null check
                if (registerDto == null) 
                {
                    _logger.LogError("Registration data is null");
                    throw new ArgumentNullException(nameof(registerDto), "Registration data cannot be null");
                }

                // 2. Normalize and validate email
                _logger.LogInformation("Normalizing and validating email");
                registerDto.Email = registerDto.Email?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(registerDto.Email))
                {
                    _logger.LogError("Email is empty or whitespace");
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Email is required");
                }

                // 3. Set default role safely
                _logger.LogInformation("Setting user role");
                registerDto.Role = "Customer"; // Always set to Customer for new registrations

                // 4. Check for existing email
                _logger.LogInformation("Checking if email already exists: {Email}", registerDto.Email);
                if (await _userRepository.EmailExists(registerDto.Email))
                {
                    _logger.LogWarning("Email already exists: {Email}", registerDto.Email);
                    throw new ConflictException($"Email {registerDto.Email} already exists");
                }

                // 5. Map with null check
                _logger.LogInformation("Mapping registration data to user entity");
                try
                {
                    userEntity = _mapper.Map<User>(registerDto);
                    if (userEntity == null)
                    {
                        _logger.LogError("User mapping failed - mapper returned null");
                        throw new InvalidOperationException("User mapping failed - please check the registration data format");
                    }
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Error during user mapping");
                    throw new InvalidOperationException("User mapping failed - please check the registration data format", mapEx);
                }

                // 6. Password validation and hashing
                _logger.LogInformation("Validating and hashing password");
                if (string.IsNullOrWhiteSpace(registerDto.Password))
                {
                    _logger.LogError("Password is empty or whitespace");
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Password is required");
                }

                if (registerDto.Password.Length < 8)
                {
                    _logger.LogError("Password is too short");
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Password must be at least 8 characters long");
                }

                try
                {
                    userEntity.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                }
                catch (Exception hashEx)
                {
                    _logger.LogError(hashEx, "Error hashing password");
                    throw new InvalidOperationException("Error processing password", hashEx);
                }

                // 7. Set audit fields safely
                _logger.LogInformation("Setting audit fields");
                var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Admin";
                userEntity.IsActive = true;
                userEntity.CreatedAt = DateTime.UtcNow;
                userEntity.ModifiedAt = DateTime.UtcNow;
                userEntity.CreatedBy = currentUser;
                userEntity.ModifiedBy = currentUser;

                // 8. Save to database
                _logger.LogInformation("Saving user to database");
                try
                {
                    await _userRepository.AddAsync(userEntity);
                    _logger.LogInformation("User created with ID: {UserId}", userEntity.UserId);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error while saving user");
                    throw new RepositoryException("Error saving user to database", dbEx);
                }

                // 9. Safe email sending (truly non-blocking)
                if (_emailService != null)
                {
                    _logger.LogInformation("Sending registration email");
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendUserRegistrationEmailAsync(
                                 userEntity.Email,
                                 registerDto.Password
                                );
                            _logger.LogInformation("Registration email sent successfully");
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Failed to send welcome email to {Email}",
                                userEntity.Email);
                        }
                    }).ConfigureAwait(false);
                }

                // 10. Get fresh copy from DB to ensure all fields are populated
                _logger.LogInformation("Retrieving saved user from database");
                var savedUser = await _userRepository.GetByIdAsync(userEntity.UserId);
                if (savedUser == null)
                {
                    _logger.LogError("User not found after creation - database operation may have failed");
                    throw new InvalidOperationException("User not found after creation - database operation may have failed");
                }

                // 11. Safe response mapping
                _logger.LogInformation("Mapping user to response DTO");
                try
                {
                    var response = _mapper.Map<UserResponseDto>(savedUser);
                    if (response == null)
                    {
                        _logger.LogError("Response mapping failed - mapper returned null");
                        throw new InvalidOperationException("Response mapping failed - please check the UserResponseDto configuration");
                    }
                    return response;
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Error during response mapping");
                    throw new InvalidOperationException("Error creating response", mapEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed. User state: {UserState}, Error: {ErrorMessage}",
                    userEntity != null ? "Created" : "Not Created",
                    ex.Message);

                // Convert to more specific exception if needed
                if (ex is ArgumentNullException or System.ComponentModel.DataAnnotations.ValidationException or ConflictException)
                    throw;

                throw new ApplicationException($"An error occurred during registration: {ex.Message}", ex);
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