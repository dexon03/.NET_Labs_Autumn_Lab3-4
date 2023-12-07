using AutoMapper;
using ErrorOr;
using Lab4.Domain.Contracts;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Application.Services;

public class AccountService : IAccountService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public AccountService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<ErrorOr<List<Account>>> GetAccountsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!await UserExists(userId))
        {
            return Error.NotFound(description: "User not found");
        }
        var accounts = await _repository.GetAll<Account>().Where(a => a.UserId == userId).ToListAsync(cancellationToken);
        
        return accounts;
    }

    public async Task<ErrorOr<Account>> CreateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        if (!await UserExists(accountDto.UserId))
        {
            return Error.NotFound(description: "User not found");
        }
        var account = _mapper.Map<Account>(accountDto);
        var result = await _repository.CreateAsync<Account>(account);
        await _repository.SaveChangesAsync(cancellationToken);
        return result;
    }
    
    private async Task<bool> UserExists(Guid userId)
    {
        return await _repository.AnyAsync<User>(x => x.Id == userId);
    }
}