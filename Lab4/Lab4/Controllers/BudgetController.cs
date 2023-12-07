using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

public class BudgetController : BaseController
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }
    
    [HttpGet("getBalance/{userId}")]
    public async Task<IActionResult> GetBalanceAsync(Guid userId)
    {
        var result = await _budgetService.GetBalanceAsync(userId);
        return result.Match<IActionResult>(
            balance => Ok(balance),
            error => BadRequest(error)
        );
    }
    
    [HttpPost("addCashFlow/{userId}")]
    public async Task<IActionResult> AddCashFlowAsync(Guid userId, CashFlowDto addCashFlowDto, CancellationToken cancellationToken)
    {
        var result = await _budgetService.AddCashFlowAsync(userId,addCashFlowDto, cancellationToken);
        return result.Match<IActionResult>(
            balance => Ok(balance),
            error => BadRequest(error)
        );
    }
    
    [HttpGet("getReport/{userId}")]
    public async Task<IActionResult> GetReportAsync(Guid userId)
    {
        var result = await _budgetService.GetReportAsync(userId);
        return result.Match<IActionResult>(
            report => Ok(report),
            error => BadRequest(error)
        );
    }
}