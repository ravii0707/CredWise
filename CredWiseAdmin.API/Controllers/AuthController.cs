using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            ILogger<AuthController> logger,
            IEmailService emailService)
        {
            _authService = authService;
            _userService = userService;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost("initial-admin-setup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InitialAdminSetup([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                _logger.LogInformation("Starting initial admin setup process");

                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for initial admin setup");
                    return BadRequest(new { 
                        Message = "Invalid input data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                // Check if any admin exists
                if (await _userService.AdminExists())
                {
                    _logger.LogWarning("Initial admin setup attempted when admin already exists");
                    return BadRequest(new { Message = "Initial admin already exists. Please use the regular admin registration process." });
                }

                // Validate email domain
                if (!registerDto.Email.EndsWith("@credwise.com", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid email domain for admin registration: {Email}", registerDto.Email);
                    return BadRequest(new { Message = "Admin email must be from credwise.com domain" });
                }

                // Force Admin role
                registerDto.Role = "Admin";

                // Register the admin
                var result = await _userService.RegisterUserAsync(registerDto);

                _logger.LogInformation("Initial admin created successfully with ID: {UserId}", result.UserId);

                // Send welcome email
                try
                {
                    await _emailService.SendUserRegistrationEmailAsync(result.Email, registerDto.Password);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send welcome email to admin: {Email}", result.Email);
                    // Don't fail the request if email fails
                }

                return Ok(new 
                {
                    UserId = result.UserId,
                    Email = result.Email,
                    Message = "Initial admin created successfully. Please check your email for login credentials."
                });
            }
            catch (Core.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during initial admin setup");
                return BadRequest(new { Message = ex.Message });
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict during initial admin setup");
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during initial admin setup");
                return StatusCode(500, new { Message = "An unexpected error occurred while setting up the initial admin. Please try again later." });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && 
                   phoneNumber.Length == 10 && 
                   phoneNumber.All(char.IsDigit);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto loginDto)
        {
            try
            {
                var response = await _authService.AuthenticateAsync(loginDto);
                return Ok(response);  // Now returns all user details with token
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(500, new { Message = "Login failed" });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                _logger.LogInformation("Starting admin registration process");

                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for admin registration");
                    return BadRequest(new { 
                        Message = "Invalid input data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                // Validate email domain
                if (!registerDto.Email.EndsWith("@credwise.com", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid email domain for admin registration: {Email}", registerDto.Email);
                    return BadRequest(new { Message = "Admin email must be from credwise.com domain" });
                }

                // Force Admin role
                registerDto.Role = "Admin";

                // Register the admin
                var result = await _userService.RegisterUserAsync(registerDto);

                _logger.LogInformation("Admin created successfully with ID: {UserId}", result.UserId);

                // Send welcome email
                try
                {
                    await _emailService.SendUserRegistrationEmailAsync(result.Email, registerDto.Password);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send welcome email to admin: {Email}", result.Email);
                    // Don't fail the request if email fails
                }

                return Ok(new 
                {
                    UserId = result.UserId,
                    Email = result.Email,
                    Message = "Admin registered successfully. Please check your email for login credentials."
                });
            }
            catch (Core.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during admin registration: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict during admin registration: {Message}", ex.Message);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during admin registration. Error details: {ErrorDetails}", ex.ToString());
                return StatusCode(500, new { Message = "An unexpected error occurred while registering the admin. Please try again later.", Details = ex.Message });
            }
        }
    }
}