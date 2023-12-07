using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpGet("getAccounts/{userId}")]
    public async Task<IActionResult> GetAccountsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetAccountsAsync(userId, cancellationToken);
        return result.Match<IActionResult>(
            accounts => Ok(accounts),
            error => BadRequest(error)
        );
    }
    
    
    [HttpPost("createAccount")]
    public async Task<IActionResult> CreateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateAccountAsync(accountDto, cancellationToken);
        return result.Match<IActionResult>(
            account => Ok(account),
            error => BadRequest(error)
        );
    }
}