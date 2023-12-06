using ErrorOr;
using Lab4.Domain.Entities;

namespace Lab4.Domain.Contracts;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken(User user);
    ErrorOr<string> ValidateToken(string? token);
}