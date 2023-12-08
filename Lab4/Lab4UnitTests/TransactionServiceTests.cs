using System.Linq.Expressions;
using AutoMapper;
using Lab4.Application.Services;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using Moq;

namespace Lab4UnitTests;

public class TransactionServiceTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _repositoryMock = new Mock<IRepository>();
        _mapperMock = new Mock<IMapper>();

        _transactionService = new TransactionService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Given_ValidTransaction_CreateTransactionAsync_ShouldReturnTrue()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            FromAccountId = Guid.NewGuid(),
            ToAccountId = Guid.NewGuid(),
            Amount = 100
        };
        
        var mappedTransaction = new Transaction 
        {
            Id = Guid.NewGuid(),
            FromAccountId = transactionDto.FromAccountId,
            ToAccountId = transactionDto.ToAccountId,
            Amount = transactionDto.Amount
        };

        _repositoryMock.Setup(r => r.AnyAsync<Account>(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetByIdAsync<Account>(It.IsAny<Guid>()))
            .ReturnsAsync(new Account { Balance = 2000 });
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Transaction>())).ReturnsAsync(mappedTransaction);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<Transaction>(It.IsAny<TransactionDto>())).Returns(mappedTransaction);

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Given_SameAccountIds_CreateTransactionAsync_ShouldReturnFalse()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var transactionDto = new TransactionDto
        {
            FromAccountId = guid,
            ToAccountId = guid,
            Amount = 100
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Invalid account ids", result.FirstError.Description);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Given_NonExistingAccount_CreateTransactionAsync_ShouldReturnFalse()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            FromAccountId = Guid.NewGuid(),
            ToAccountId = Guid.NewGuid(),
            Amount = 100
        };
        _repositoryMock.Setup(r => r.AnyAsync<Account>(a => a.Id == transactionDto.FromAccountId)).ReturnsAsync(false);

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Invalid account ids", result.FirstError.Description);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task Given_InsufficientBalance_CreateTransactionAsync_ShouldReturnFalse()
    {
        // Arrange
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();
        var transactionDto = new TransactionDto
        {
            FromAccountId = accountFromId,
            ToAccountId = accountToId,
            Amount = 2000
        };
        var accountFrom = new Account { Id = accountFromId, Balance = 1000 };
        var accountTo = new Account { Id = accountToId, Balance = 5000 };
        _repositoryMock.Setup(r => r.AnyAsync<Account>(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetByIdAsync<Account>(accountFromId)).ReturnsAsync(accountFrom);
        _repositoryMock.Setup(r => r.GetByIdAsync<Account>(accountToId)).ReturnsAsync(accountTo);

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Not enough money", result.FirstError.Description);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}