using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtConstants = Lab4.Domain.Constants.JwtConstants;

namespace Lab4IntegrationTests.AuthMock;

public static class JwtMock
{
    public static string GenerateJwtToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F4XCU4J99BFRRK1OHHS4WMG3ZHHBV4Y7RFY9E3QD"));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            "http://localhost:5000/",
            "http://localhost:5000/",
            claims,
            expires: DateTime.Now.AddHours(JwtConstants.TokenExpirationTimeInHours),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}