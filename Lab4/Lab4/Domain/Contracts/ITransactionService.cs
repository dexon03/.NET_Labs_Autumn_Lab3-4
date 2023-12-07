using ErrorOr;
using Lab4.Domain.Dtos;

namespace Lab4.Domain.Contracts;

public interface ITransactionService
{
    Task<ErrorOr<bool>> CreateTransactionAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default);
}