using Microsoft.AspNetCore.Http;
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
    public interface IJsonWebTokenService
    {
        /// <summary>
        /// Creates JSON Web Token according to current authenticated user.
        /// This token get limited access to use a service function for a 
        /// defined period of time.
        /// </summary>        
        string Create();
    }

    public class JsonWebTokenService : IJsonWebTokenService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JsonWebTokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string Create()
        {
            var user = httpContextAccessor.HttpContext.User;
            var claims = user == null ? null : new ClaimsIdentity(user.Claims);
            var handler = new JwtSecurityTokenHandler();

            // Specify a start time five minutes earlier to allow for client clock skew.
            var notBefore = DateTime.UtcNow.AddMinutes(-5);
            // Specify a validity period of five minutes starting from now.
            var expires = DateTime.UtcNow.AddMinutes(5);

            var token = handler.CreateJwtSecurityToken(
                issuer: null,    // Not required as no third-party is involved
                audience: null,  // Not required as no third-party is involved                
                notBefore:notBefore, 
                expires: expires,
                subject: claims,
                signingCredentials: new SigningCredentials(
                                        new SymmetricSecurityKey(
                                            Encoding.UTF8.GetBytes(configuration["JwtSecret"])),
                                            SecurityAlgorithms.HmacSha256Signature));

            return handler.WriteToken(token);
        }
    }
}
