using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredWiseAdmin.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UsersController(IUserService userService, ILogger<UsersController> logger,IUserRepository userRepository, IEmailService emailService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                _logger.LogInformation("Attempting to register new user");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user registration");
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var user = await _userService.RegisterUserAsync(registerDto);

                // Send email with credentials
                try
                {
                    await _emailService.SendUserRegistrationEmailAsync(user.Email, registerDto.Password);
                    _logger.LogInformation("Registration email sent to {Email}", user.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send registration email to {Email}", user.Email);
                }

                _logger.LogInformation("User registered successfully with ID: {UserId}", user.UserId);
                return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.UserId }, new
                {
                    Success = true,
                    Data = user
                });
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict during user registration");
                return Conflict(new { Success = false, Message = ex.Message });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed during user registration");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error retrieving users"
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID requested: {UserId}", id);
                    return BadRequest(new { Success = false, Message = "Invalid user ID" });
                }

                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", id);
                    return NotFound(new { Success = false, Message = "User not found" });
                }

                return Ok(new { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error retrieving user"
                });
            }
        }

        [HttpPut("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to deactivate user with ID: {UserId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID for deactivation: {UserId}", id);
                    return BadRequest(new { Success = false, Message = "Invalid user ID" });
                }

                var result = await _userService.DeactivateUserAsync(id);

                if (!result)
                {
                    _logger.LogWarning("User not found for deactivation with ID: {UserId}", id);
                    return NotFound(new { Success = false, Message = "User not found" });
                }

                _logger.LogInformation("User deactivated successfully with ID: {UserId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("last admin"))
            {
                _logger.LogWarning(ex, "Attempt to deactivate last admin");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user with ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error deactivating user"
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with ID: {UserId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID for update: {UserId}", id);
                    return BadRequest(new { Success = false, Message = "Invalid user ID" });
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user update");
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = await _userService.UpdateUserAsync(id, updateDto);

                if (!result)
                {
                    _logger.LogWarning("User not found for update with ID: {UserId}", id);
                    return NotFound(new { Success = false, Message = "User not found" });
                }

                _logger.LogInformation("User updated successfully with ID: {UserId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error updating user"
                });
            }
        }
        [AllowAnonymous] // Temporarily allow unauthenticated access for testing
        [HttpPost("test-login")]
        public async Task<IActionResult> TestUserLogin([FromBody] TestLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                // 1. First verify the user exists in database
                var userExists = await _userRepository.EmailExists(loginDto.Email.ToLower().Trim());
                if (!userExists)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "User not found",
                        Data = new { loginDto.Email }
                    });
                }

                // 2. Test password verification
                var isValid = await _userService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);

                // 3. Get user details for verification
                var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLower().Trim());

                return Ok(new
                {
                    Success = true,
                    Message = isValid ? "Credentials valid" : "Invalid password",
                    Data = new
                    {
                        UserId = user?.UserId,
                        Email = user?.Email,
                        IsActive = user?.IsActive,
                        PasswordMatch = isValid,
                        HashedPasswordInDb = user?.Password,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login test failed for {Email}", loginDto?.Email);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Test failed",
                    Error = ex.Message
                });
            }
        }
        [HttpPost("test-send-credentials")]
        [AllowAnonymous] // Temporarily allow unauthenticated access for testing
        public async Task<IActionResult> TestSendCredentials([FromBody] TestEmailRequest request)
        {
            try
            {
                await _emailService.SendUserRegistrationEmailAsync(request.Email, request.Password);
                return Ok(new { Success = true, Message = "Test email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, ex.Message });
            }
        }

        public class TestEmailRequest
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

    }
}