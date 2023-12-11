using System.Net;
using System.Net.Http.Json;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;

namespace Lab4IntegrationTests.Tests;

public class BudgetTests : BaseIntegrationTest
{
    public BudgetTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Get_Balance_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.GetAsync($"api/Budget/getBalance/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
    
    [Fact]
    public async Task Get_Balance_With_Invalid_Id_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Act
        var request = await _client.GetAsync($"api/Budget/getBalance/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Get_Balance_With_Valid_Id_Returns_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.GetAsync($"api/Budget/getBalance/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Add_CashFlow_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.PostAsJsonAsync($"api/Budget/addCashFlow/{userId}", new CashFlowDto());
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
    
    [Fact]
    public async Task Add_CashFlow_With_Invalid_Id_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Act
        var request = await _client.PostAsJsonAsync($"api/Budget/addCashFlow/{userId}", new CashFlowDto());
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Add_Invalid_CashFlow_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.PostAsJsonAsync($"api/Budget/addCashFlow/{userId}", new CashFlowDto());
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Add_Valid_CashFlow_Without_Account_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.PostAsJsonAsync($"api/Budget/addCashFlow/{userId}", new CashFlowDto
        {
            Source = "Test",
            Amount = 100,
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Add_Valid_CashFlow_Returns_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
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
            Id = accountId,
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.PostAsJsonAsync($"api/Budget/addCashFlow/{userId}", new CashFlowDto
        {
            AccountId = accountId,
            Source = "Test",
            Amount = 100,
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Get_Report_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.GetAsync($"api/Budget/getReport/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
    
    [Fact]
    public async Task Get_Report_With_Invalid_Id_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Act
        var request = await _client.GetAsync($"api/Budget/getReport/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Get_Report_With_Valid_Id_Returns_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = userId,
            Email = "test",
            FirstName = "test",
            LastName = "test",
            PasswordHash = "test",
            PasswordSalt = "test"
        });
        await FinanceDbContext.SaveChangesAsync();
        // Act
        var request = await _client.GetAsync($"api/Budget/getReport/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
}