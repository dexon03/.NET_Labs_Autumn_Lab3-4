using ErrorOr;
using Lab4.Domain.Dtos;

namespace Lab4.Domain.Contracts;

public interface IAuthService
{
    Task<ErrorOr<TokenResponse>> LoginAsync(LoginDto loginDto);
    Task<ErrorOr<TokenResponse>>  RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
    
    Task<ErrorOr<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}