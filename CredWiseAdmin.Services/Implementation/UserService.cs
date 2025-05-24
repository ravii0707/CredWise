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

namespace CredWiseAdmin.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailService emailService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            try
            {
                _logger.LogInformation("Attempting to register new user: {Email}", registerDto.Email);

                // Validate input
                if (registerDto == null)
                {
                    throw new ArgumentNullException(nameof(registerDto));
                }

                if (await _userRepository.EmailExists(registerDto.Email))
                {
                    _logger.LogWarning("Registration attempt with existing email: {Email}", registerDto.Email);
                    throw new ConflictException("Email already exists");
                }

                // Validate password strength
                if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 8)
                {
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Password must be at least 8 characters");
                }

                var user = _mapper.Map<User>(registerDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                user.IsActive = true;
                user.CreatedAt = DateTime.UtcNow;
                user.ModifiedAt = DateTime.UtcNow;
                user.CreatedBy = "System";
                user.ModifiedBy = "System";

                await _userRepository.AddAsync(user);

                // Send welcome email (without password in production)
                if (_emailService != null)
                {
                    await _emailService.SendUserRegistrationEmailAsync(user.Email, registerDto.Password);
                }

                _logger.LogInformation("Successfully registered new user: {Email} with role {Role}",
                    user.Email, user.Role);

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", registerDto?.Email);
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
    }
}