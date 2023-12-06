using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody]LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return result.Match<IActionResult>(
            tokenResponse => Ok(tokenResponse),
            error => BadRequest(error)
        );
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody]RegisterDto registerDto,CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(registerDto,cancellationToken);
        return result.Match<IActionResult>(
            tokenResponse => Ok(tokenResponse),
            error => BadRequest(error)
        );
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody]RefreshTokenRequest refreshTokenDto,CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken,cancellationToken);
        return result.Match<IActionResult>(
            tokenResponse => Ok(tokenResponse),
            error => BadRequest(error)
        );
    }
}