using AutoMapper;
using ErrorOr;
using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Domain.Models;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ErrorOr<double>> GetBalanceAsync(Guid userId)
    {
        if (!await UserExists(userId))
        {
            return Error.NotFound(description: "User not found");
        }

        var balance = _repository.GetAll<Account>()
            .Where(a => a.UserId == userId)
            .Sum(a => a.Balance);
        return balance;
    }

    public async Task<ErrorOr<bool>> AddCashFlowAsync(Guid userId, CashFlowDto cashFlowDto, CancellationToken cancellationToken = default)
    {
        if (!await UserExists(userId))
        {
            return Error.NotFound(description: "User not found");
        }
        var account = await _repository.FirstOrDefaultAsync<Account>(a => a.Id == cashFlowDto.AccountId && a.UserId == userId);
        if (account is null)
        {
            return Error.NotFound(description: "Account not found");
        }
        var cashFlow = _mapper.Map<CashFlow>(cashFlowDto);
        if(cashFlowDto.Amount < 0 && account.Balance < Math.Abs(cashFlowDto.Amount))
        {
            return Error.Validation(description: "Not enough money");
        }
        account.Balance += cashFlowDto.Amount;
        _repository.Update(account);
        var result = await _repository.CreateAsync(cashFlow);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<ErrorOr<UserReport>> GetReportAsync(Guid userId)
    {
        if (!await UserExists(userId))
        {
            return Error.NotFound(description: "User not found");
        }
        var accounts = (await
            (from account in _repository.GetAll<Account>().Where(a => a.UserId == userId)
                from transaction in _repository.GetAll<Transaction>()
                    .Where(t => t.FromAccountId == account.Id || t.ToAccountId == account.Id)
                select new
                {
                    AccountId = account.Id,
                    AccountName = account.AccountName,
                    TransactionId = transaction.Id,
                    TransactionAmount = transaction.FromAccountId == account.Id ?  -transaction.Amount : transaction.Amount ,
                    TransactionDateTime = transaction.DateTime
                }).ToListAsync())
            .GroupBy(at => new
            {
                at.AccountId,
                at.AccountName
            })
            .Select(gat => new ReportByAccount
            {
                AccountId = gat.Key.AccountId,
                AccountName = gat.Key.AccountName,
                Transactions = gat.Select(at => new ReportTransaction
                {
                    TransactionId = at.TransactionId,
                    Amount = at.TransactionAmount,
                    DateTime = at.TransactionDateTime
                }).ToList()
            });
        return new UserReport
        {
            UserId = userId,
            Report = accounts.ToList()
        };
    }
    private async Task<bool> UserExists(Guid userId)
    {
        return await _repository.AnyAsync<User>(x => x.Id == userId);
    }
    private async Task<bool> AccountExists(Guid accountId)
    {
        return await _repository.AnyAsync<Account>(x => x.Id == accountId);
    }
}