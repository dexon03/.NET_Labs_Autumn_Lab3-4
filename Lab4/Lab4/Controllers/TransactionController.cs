using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

[Authorize]
public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpPost("createTransaction")]
    public async Task<IActionResult> CreateTransactionAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default)
    {
        var result = await _transactionService.CreateTransactionAsync(transactionDto, cancellationToken);
        
        return result.Match<IActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }
}