using ErrorOr;
using Lab4.Application.Utilities;
using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;

namespace Lab4.Application.Services;

public class AuthService: IAuthService
{
    private readonly UserManager _userManager;
    private readonly IJwtService _jwtService;
    private readonly IRepository _repository;

    public AuthService(UserManager userManager, 
        IJwtService jwtService, 
        IRepository repository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _repository = repository;
    }

    public async Task<ErrorOr<TokenResponse>> LoginAsync(LoginDto loginDto)
    {
        var errorOrUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (errorOrUser.IsError)
        {
            return errorOrUser.Errors;
        }
        if (!await IsLoginRequestValid(errorOrUser.Value, loginDto.Password))
        {
            return Error.Validation(description: "Invalid password");
        }

        var user = errorOrUser.Value;
        var token = GetNewTokenForUser(user);
        return token;
    }

    public async Task<ErrorOr<TokenResponse>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var user = await CreateUser(registerDto);
        var token = GetNewTokenForUser(user);
        await _repository.CreateAsync(user);
        await _repository.SaveChangesAsync(cancellationToken);
        return token;
    }

    public async Task<ErrorOr<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var errorOrUserId = _jwtService.ValidateToken(refreshToken);
        if (errorOrUserId.IsError)
        {
            return errorOrUserId.Errors;
        }
        
        var errorOrUser = await _userManager.FindByIdAsync(Guid.Parse(errorOrUserId.Value));
        if (errorOrUser.IsError)
        {
            return errorOrUser.Errors;
        }
        var token = GetNewTokenForUser(errorOrUser.Value);
        
        return token;
    }

    private async Task<User> CreateUser(RegisterDto request)
    {
        var passwordSalt = PasswordUtility.CreatePasswordSalt();
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordSalt = passwordSalt,
            PasswordHash = PasswordUtility.GetHashedPassword(request.Password, passwordSalt),
        };
        return user;
    }
    
    private TokenResponse GetNewTokenForUser(User user)
    {
        var accessToken = _jwtService.GenerateToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken(user);
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id
        };
    }
    
    private async Task<bool> IsLoginRequestValid(User? user, string password)
    {
        if (user == null)
        {
            return false;
        }
        return await _userManager.CheckPasswordAsync(user, password);
    }
}