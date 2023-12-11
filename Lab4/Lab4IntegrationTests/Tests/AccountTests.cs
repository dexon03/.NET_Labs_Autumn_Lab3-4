using System.Net;
using System.Net.Http.Json;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;

namespace Lab4IntegrationTests.Tests;

public class AccountTests : BaseIntegrationTest
{
    public AccountTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task Get_Account_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.GetAsync($"api/Account/getAccounts/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }

    [Fact]
    public async Task Get_Not_Existing_Account_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
       

        // Act
        var request = await _client.GetAsync($"api/Account/getAccounts/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }

    [Fact]
    public async Task Get_Existing_Account_Returns_Ok()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
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
        var request = await _client.GetAsync($"api/Account/getAccounts/{userId}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Account_Without_Auth_Returns_Unauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = null;        
        // Act
        var request = await _client.PostAsJsonAsync($"api/Account/createAccount", new AccountDto
        {
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Account_With_Not_Existing_User_Returns_BadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Act
        var request = await _client.PostAsJsonAsync($"api/Account/createAccount", new AccountDto
        {
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Valid_Account_With_Existing_User_Returns_Ok()
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
        var request = await _client.PostAsJsonAsync($"api/Account/createAccount", new AccountDto
        {
            UserId = userId,
            AccountName = "test",
            Balance = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Create_Not_Valid_Account_With_Existing_User_Returns_BadRequest()
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
        var request = await _client.PostAsJsonAsync($"api/Account/createAccount", new AccountDto
        {
            UserId = userId,
            AccountName = "",
            Balance = 100
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
}