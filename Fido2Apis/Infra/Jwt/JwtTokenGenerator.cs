using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fido2Apis.Infra.Jwt
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(JwtPayLoadData jwtPayLoadData)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim("Name", jwtPayLoadData.Name) };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], 
                _configuration["Jwt:Issuer"], 
                claims, 
                expires: DateTime.Now.AddMinutes(120), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
