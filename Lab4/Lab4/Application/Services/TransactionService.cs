using AutoMapper;
using ErrorOr;
using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;

namespace Lab4.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public TransactionService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ErrorOr<bool>> CreateTransactionAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default)
    {
        if(!await IsAccountIdsValid(transactionDto.FromAccountId, transactionDto.ToAccountId))
        {
            return Error.Validation(description: "Invalid account ids");
        }
        
        var transaction = _mapper.Map<Transaction>(transactionDto);
        
        var transactionSuccess = await MakeTransactionBetweenAccountsAsync(transactionDto, transaction);
        if (transactionSuccess.IsError)
        {
            return transactionSuccess.Errors;
        }

        await _repository.CreateAsync(transaction);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<ErrorOr<bool>> MakeTransactionBetweenAccountsAsync(TransactionDto transactionDto, Transaction transaction)
    {
        var accountFrom = await _repository.GetByIdAsync<Account>(transactionDto.FromAccountId);
        var accountTo = await _repository.GetByIdAsync<Account>(transactionDto.ToAccountId);
        if (accountFrom.Balance <= transactionDto.Amount)
        {
            return Error.Validation(description: "Not enough money");
        }

        accountFrom.Balance -= transactionDto.Amount;
        accountTo.Balance += transactionDto.Amount;
        transaction.DateTime = DateTime.Now;
        _repository.Update(accountTo);
        _repository.Update(accountFrom);
        return true;
    }

    private async Task<bool> IsAccountIdsValid(Guid fromAccountId, Guid toAccountId)
    {
        if (fromAccountId == toAccountId)
        {
            return false;
        }
        
        var isFromAccountExists = await _repository.AnyAsync<Account>(a => a.Id == fromAccountId);
        var isToAccountExists = await _repository.AnyAsync<Account>(a => a.Id == toAccountId);
        return isFromAccountExists && isToAccountExists;
    }
}