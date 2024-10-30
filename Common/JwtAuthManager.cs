using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common
{
    public class JwtAuthManager
    {
        public static JwtSecurityToken GenerateJWTToken(string username, string issuer, string audience, string JwtSecretKey)
        {
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,username),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
            var key = Encoding.ASCII.GetBytes(JwtSecretKey);

            var token = new JwtSecurityToken(
                            issuer: issuer,
                            audience: audience,
                            claims: claims,
                            /*notBefore: new DateTimeOffset(DateHelperFunctions.EstTimeNow()).DateTime,
                            expires: new DateTimeOffset(DateHelperFunctions.EstTimeNow().AddDays(1)).DateTime,*/
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                                SecurityAlgorithms.HmacSha256Signature)
                             );
            return token;
        }
    }
}
