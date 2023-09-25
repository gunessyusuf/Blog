using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;

namespace WebApi.Utils.Bases
{
    public abstract class JwtUtilBase
    {
        private readonly IConfiguration _configuration;

        protected JwtUtilBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual JwtModel CreateJwt(string userName, string roleName, string userId)
        {
            var jwtOptionsSection = _configuration.GetSection("JwtOptions");
            var jwtOptions = new JwtOptionsModel();
            jwtOptionsSection.Bind(jwtOptions);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, roleName)
            };
            var expiration = DateTime.Now.AddMinutes(jwtOptions.ExpirationMinutes);
            var jwtSecurityToken = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, 
                claimList, DateTime.Now, expiration, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            var jwt = new JwtModel()
            {
                Token = token,
                Expiration = expiration
            };
            return jwt;
        }
    }
}
