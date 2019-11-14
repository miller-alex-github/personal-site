using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ma.Web.Services
{
    /// <summary>
    /// JSON Web Token service.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Creates JWT (JSON Web Token) authentication token.
        /// </summary>
        /// <param name="user">User identity</param>
        /// <returns>Secure JWT token</returns>
        string CreateToken(ClaimsPrincipal user = null);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;

        public JwtService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateToken(ClaimsPrincipal user = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: null,    // Not required as no third-party is involved
                audience: null,  // Not required as no third-party is involved                
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                subject: user == null ? null : new ClaimsIdentity(user.Claims),
                signingCredentials: new SigningCredentials(
                                        new SymmetricSecurityKey(
                                            Encoding.UTF8.GetBytes(configuration["JwtSecret"])),
                                            SecurityAlgorithms.HmacSha256Signature));

            return tokenHandler.WriteToken(token);
        }
    }
}
