using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Authentication.Service.Interfaces;
using Authentication.Service.Models;

namespace Authentication.Service.Services
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly AuthConfiguration _config;
        
        public JwtAuthenticationService(AuthConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GenerateTokenAsync(Dictionary<string, object> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.SecretKey);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config.Issuer,
                Audience = _config.Audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddHours(_config.TokenExpirationInHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AuthResult> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _config.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                
                var claims = new Dictionary<string, object>();
                foreach (var claim in principal.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }

                return new AuthResult { IsValid = true, Claims = claims };
            }
            catch (Exception ex)
            {
                return new AuthResult { IsValid = false, Error = ex.Message };
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            // Implement token revocation logic if needed
            // For example, you could maintain a list of revoked tokens in memory or a distributed cache
            return true;
        }
    }
} 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authentication.Service.Interfaces;

namespace Authentication.Service.Services
{
    public class PolicyBasedAuthorizationService : IAuthorizationService
    {
        private readonly Dictionary<string, Func<Dictionary<string, object>, bool>> _policies;

        public PolicyBasedAuthorizationService()
        {
            _policies = new Dictionary<string, Func<Dictionary<string, object>, bool>>();
        }

        public void AddPolicy(string policyName, Func<Dictionary<string, object>, bool> policy)
        {
            _policies[policyName] = policy;
        }

        public async Task<bool> AuthorizeAsync(Dictionary<string, object> claims, string resource, string action)
        {
            var policyKey = $"{resource}:{action}";
            return _policies.TryGetValue(policyKey, out var policy) && policy(claims);
        }
    }
} 


using System.Collections.Generic;

namespace Authentication.Service.Models
{
    public class AuthResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, object> Claims { get; set; }
        public string Error { get; set; }
    }
} 

namespace Authentication.Service.Models
{
    public class AuthConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpirationInHours { get; set; } = 1;
    }
}