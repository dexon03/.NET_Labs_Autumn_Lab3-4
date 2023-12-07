using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using ErrorOr;

namespace Lab4.Domain.Contracts;

public interface IAccountService
{
    Task<ErrorOr<List<Account>>> GetAccountsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ErrorOr<Account>> CreateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken);
}