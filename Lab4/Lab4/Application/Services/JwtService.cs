using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErrorOr;
using Lab4.Domain.Contracts;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.IdentityModel.Tokens;
using JwtConstants = Lab4.Domain.Constants.JwtConstants;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Lab4.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly IRepository _repository;

    public JwtService(IConfiguration configuration, IRepository repository)
    {
        _configuration = configuration;
        _repository = repository;
    }
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
        };
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(JwtConstants.TokenExpirationTimeInHours),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var refreshToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            subject: new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }),
            expires: DateTime.UtcNow.AddHours(JwtConstants.RefreshTokenExpirationTimeInHours),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:RefreshTokenKey"]!)),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(refreshToken);
    }
    
    public ErrorOr<string> ValidateToken(string? token)
    {
        if (token == null)
            return Error.Failure(description: "Token is null");

        var isTokenValid = TryValidateToken(token, out var principal);
        if (!isTokenValid)
        {
            return Error.Validation(description:"Token is invalid");
        }
        
        var userId = principal?.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        
        return userId == null
            ? Error.Validation(description:"User id not found in token")
            : userId;
    }

    private bool TryValidateToken(string token, out ClaimsPrincipal principal)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:RefreshTokenKey"]);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
            };

            principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            return true;
        }
        catch
        {
            principal = null;
            return false;
        }
    }
}