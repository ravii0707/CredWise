using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace CredWiseAdmin.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(AdminLoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);

                if (user == null || !user.IsActive.GetValueOrDefault())
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                var token = GenerateJwtToken(user);

                return new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    Name = user.FirstName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    UserId = user.UserId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed");
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}