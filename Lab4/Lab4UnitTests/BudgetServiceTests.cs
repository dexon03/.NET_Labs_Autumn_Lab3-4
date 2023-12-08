using System.Linq.Expressions;
using AutoMapper;
using Lab4.Application.Services;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Domain.Models;
using Lab4.Infrastructure.Database.Repository;
using MockQueryable.Moq;
using Moq;

namespace Lab4UnitTests;

public class BudgetServiceTests
{
    private readonly Mock<IRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BudgetService _service;

    public BudgetServiceTests()
    {
        _mockRepository = new Mock<IRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new BudgetService(_mockRepository.Object, _mockMapper.Object);
    }
    [Fact]
    public async void GetBalanceAsync_ShouldReturnBalance_WhenUserExistsAndHasBalance()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accounts = new List<Account>
        {
            new Account {UserId = userId, Balance = 50},
            new Account {UserId = userId, Balance = 150}
        }.AsQueryable().BuildMock();

        _mockRepository.Setup(r => r.GetAll<Account>()).Returns(accounts);
        _mockRepository.Setup(r => r.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.GetBalanceAsync(userId);

        // Assert
        Assert.Equal(200, result.Value);
    }

    [Fact]
    public async Task GetBalanceAsync_ReturnsError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepository.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act
        var result = await _service.GetBalanceAsync(userId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User not found", result.FirstError.Description);
    }

    [Fact]
    public async Task AddCashFlowAsync_ReturnsError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cashFlowDto = new CashFlowDto();
        _mockRepository.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act
        var result = await _service.AddCashFlowAsync(userId, cashFlowDto);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User not found", result.FirstError.Description);
    }

    [Fact]
    public async Task GetReportAsync_ReturnsError_UserNotFound()
    {
        // Arrange
        var userId  = Guid.NewGuid();
        _mockRepository.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act
        var result = await _service.GetReportAsync(userId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User not found", result.FirstError.Description);
    }

    [Fact]
    public async Task AddCashFlowAsync_ReturnsTrue_WhenCashFlowAddedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var account = new Account { Id = accountId, UserId = userId, Balance = 100 };
        var cashFlowDto = new CashFlowDto { AccountId = accountId, Amount = 25 };
        _mockRepository.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
        _mockRepository.Setup(x => x.FirstOrDefaultAsync<Account>(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(account);
        _mockRepository.Setup(x => x.CreateAsync<CashFlow>(It.IsAny<CashFlow>())).ReturnsAsync(new CashFlow { });
        _mockMapper.Setup(x => x.Map<CashFlow>(It.IsAny<CashFlowDto>())).Returns(new CashFlow { });

        // Act
        var result = await _service.AddCashFlowAsync(userId, cashFlowDto);

        // Assert
        Assert.False(result.IsError);
        Assert.True(result.Value);
    }
    [Fact]
    public async Task GetReportAsync_ReturnsUserReport_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepository.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetAll<Account>()).Returns(new List<Account> { new Account { UserId = userId } }.AsQueryable().BuildMock());
        _mockRepository.Setup(x => x.GetAll<Transaction>()).Returns(new List<Transaction> { new Transaction { FromAccountId = Guid.NewGuid(), ToAccountId = Guid.NewGuid(), Amount = 100 } }.AsQueryable().BuildMock());

        // Act
        var result = await _service.GetReportAsync(userId);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<UserReport>(result.Value);
    }
}