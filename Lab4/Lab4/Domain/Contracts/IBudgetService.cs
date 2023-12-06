using ErrorOr;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Domain.Models;

namespace Lab4.Domain.Contracts;

public interface IBudgetService
{
    Task<ErrorOr<Account>> CreateAccountAsync(AccountDto accountDto); 
    Task<ErrorOr<double>> GetBalanceAsync(Guid userId);
    Task<ErrorOr<bool>> AddIncomeAsync(Guid userId, CashFlowDto cashFlowDto);
    Task<ErrorOr<bool>> AddExpenseAsync(Guid userId, CashFlowDto cashFlowDto);
    Task<ErrorOr<Report>> GetReportAsync(Guid userId);
}