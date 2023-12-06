using AutoMapper;
using ErrorOr;
using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Domain.Models;
using Lab4.Infrastructure.Database.Repository;

namespace Lab4.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public BudgetService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public Task<ErrorOr<Account>> CreateAccountAsync(AccountDto accountDto)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<double>> GetBalanceAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<bool>> AddIncomeAsync(Guid userId, CashFlowDto cashFlowDto)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<bool>> AddExpenseAsync(Guid userId, CashFlowDto cashFlowDto)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Report>> GetReportAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}