using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">The login credentials</param>
        /// <returns>A JWT token for authentication</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
        {
            try
            {
                // In a real application, you would validate credentials against a database
                // This is a simplified example for demonstration purposes
                if (IsValidUser(request.Username, request.Password))
                {
                    var token = GenerateJwtToken(request.Username);
                    
                    return Ok(new AuthResponse
                    {
                        Username = request.Username,
                        Token = token,
                        ExpiresIn = 3600 // 1 hour
                    });
                }
                
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", request.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error during authentication");
            }
        }

        /// <summary>
        /// Validates the current token
        /// </summary>
        /// <returns>User information if token is valid</returns>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ValidateToken()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            return Ok(new
            {
                Username = username,
                IsAuthenticated = true,
                Roles = GetUserRoles(username)
            });
        }

        #region Helper Methods

        private bool IsValidUser(string username, string password)
        {
            // In a real application, you would check against a database
            // This is a simplified example for demonstration purposes
            return username == "admin" && password == "admin123";
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "ContentMasterAPISecretKey123!"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            // Add roles
            foreach (var role in GetUserRoles(username))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "ContentMasterAPI",
                audience: _configuration["Jwt:Audience"] ?? "ContentMasterAPIUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IEnumerable<string> GetUserRoles(string username)
        {
            // In a real application, you would get roles from a database
            // This is a simplified example for demonstration purposes
            if (username == "admin")
            {
                return new[] { "Admin", "Editor" };
            }
            
            return new[] { "User" };
        }

        #endregion
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
