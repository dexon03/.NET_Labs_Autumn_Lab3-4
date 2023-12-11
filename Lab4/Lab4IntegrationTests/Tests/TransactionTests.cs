using System.Net;
using System.Net.Http.Json;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;

namespace Lab4IntegrationTests.Tests;

public class TransactionTests : BaseIntegrationTest
{
    public TransactionTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Create_Transaction_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.PostAsJsonAsync("api/Transaction/createTransaction", new TransactionDto());
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Transaction_With_Invalid_Model_Returns_BadRequest()
    {
        // Arrange
        // Act
        var request = await _client.PostAsJsonAsync("api/Transaction/createTransaction", new TransactionDto());
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Transaction_With_Valid_Model_Returns_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.Account.AddAsync(new Account
        {
            Id = accountFromId,
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        await FinanceDbContext.Account.AddAsync(new Account
        {
            Id = accountToId,
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.PostAsJsonAsync("api/Transaction/createTransaction", new TransactionDto
        {
            FromAccountId = accountFromId,
            ToAccountId = accountToId,
            Amount = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Transaction_With_Valid_Model_Not_Enough_Balance_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.Account.AddAsync(new Account
        {
            Id = accountFromId,
            UserId = userId,
            AccountName = "test",
            Balance = 0
        });
        await FinanceDbContext.Account.AddAsync(new Account
        {
            Id = accountToId,
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.PostAsJsonAsync("api/Transaction/createTransaction", new TransactionDto
        {
            FromAccountId = accountFromId,
            ToAccountId = accountToId,
            Amount = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
}