using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
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

        public AuthController(
            IAuthService authService,
            IUserService userService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("initial-admin-setup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InitialAdminSetup([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                if (await _userService.AdminExists())
                {
                    return BadRequest(new { Message = "Initial admin already exists" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                registerDto.Role = "Admin"; // Force Admin role
                var result = await _userService.RegisterUserAsync(registerDto);

                return Ok(new 
                {
                    UserId = result.UserId,
                    Email = result.Email,
                    Message = "Initial admin created successfully"
                });
            }
            catch (Core.Exceptions.ValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Initial admin setup failed");
                return StatusCode(500, new { Message = ex.Message });
            }
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

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                registerDto.Role = "Admin";
                var result = await _userService.RegisterUserAsync(registerDto);
                return CreatedAtAction(nameof(RegisterAdmin), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin registration failed");
                return BadRequest(new { ex.Message });
            }
        }
    }
}