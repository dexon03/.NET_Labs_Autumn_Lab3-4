using System.Linq.Expressions;
using AutoMapper;
using Lab4.Application.Services;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using MockQueryable.Moq;
using Moq;

namespace Lab4UnitTests;

public class AccountServiceTests
{
    private readonly Mock<IRepository> _repositoryMock = new Mock<IRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    
    [Fact]
    public async Task GetAccountsAsync_UserDoesNotExist()
    {
        // Arrange
        Guid testUserId = Guid.NewGuid();
        var service = new AccountService(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act
        var result = await service.GetAccountsAsync(testUserId, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User not found", result.FirstError.Description);
        _repositoryMock.Verify(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAccountAsync_UserDoesNotExist()
    {
        // Arrange
        var testAccountDto = new AccountDto { UserId = Guid.NewGuid() };
        var service = new AccountService(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act
        var result = await service.CreateAccountAsync(testAccountDto, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User not found", result.FirstError.Description);
        _repositoryMock.Verify(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAccountsAsync_UserExists()
    {
        // Arrange
        Guid testUserId = Guid.NewGuid();
        var service = new AccountService(_repositoryMock.Object, _mapperMock.Object);
        var mockAccounts = new List<Account> { new Account { UserId = testUserId } }.AsQueryable().BuildMock();
        _repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.GetAll<Account>()).Returns(mockAccounts);

        // Act
        var result = await service.GetAccountsAsync(testUserId, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(mockAccounts.Count(), result.Value.Count());
        Assert.Equal(mockAccounts.ToList(), result.Value);
        _repositoryMock.Verify(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task CreateAccountAsync_UserExists()
    {
        // Arrange
        var testAccountDto = new AccountDto { UserId = Guid.NewGuid() };
        var service = new AccountService(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.CreateAsync<Account>(It.IsAny<Account>())).ReturnsAsync(new Account());
        _mapperMock.Setup(x => x.Map<Account>(It.IsAny<AccountDto>())).Returns(new Account());

        // Act
        var result = await service.CreateAccountAsync(testAccountDto, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        _repositoryMock.Verify(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        _repositoryMock.Verify(x => x.CreateAsync<Account>(It.IsAny<Account>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}