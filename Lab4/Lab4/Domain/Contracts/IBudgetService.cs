using ErrorOr;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Domain.Models;

namespace Lab4.Domain.Contracts;

public interface IBudgetService
{
    Task<ErrorOr<double>> GetBalanceAsync(Guid userId);
    Task<ErrorOr<bool>> AddCashFlowAsync(Guid userId, CashFlowDto cashFlowDto, CancellationToken cancellationToken);
    Task<ErrorOr<UserReport>> GetReportAsync(Guid userId);
}