using System.Net;
using System.Net.Http.Json;
using Lab4.Application.Utilities;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;

namespace Lab4IntegrationTests.Tests;

public class AuthTests : BaseIntegrationTest
{
    public AuthTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Login_With_Invalid_Credentials_Returns_BadRequest()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test",
            Password = "test"
        };
        
        // Act
        var request = await _client.PostAsJsonAsync("api/Auth/login", loginDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_Ok()
    {
        // Arrange
        var email = "test@mail.com";
        var password = "Test123_";
        var passwordSalt = PasswordUtility.CreatePasswordSalt();
        await FinanceDbContext.User.AddAsync(new User 
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "test",
            LastName = "test",
            PasswordHash = PasswordUtility.GetHashedPassword(password, passwordSalt),
            PasswordSalt = passwordSalt
        });
        await FinanceDbContext.SaveChangesAsync();
        var loginDto = new LoginDto
        {
            Email = "test@mail.com",
            Password = "Test123_"
        };
        
        // Act
        var request = await _client.PostAsJsonAsync("api/Auth/login", loginDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
    [Fact]
    public async Task Register_With_Invalid_Credentials_Returns_BadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test",
            Password = "test",
            FirstName = "test",
            LastName = "test"
        };
        
        // Act
        var request = await _client.PostAsJsonAsync("api/Auth/register", registerDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
    
    [Fact]
    public async Task Register_With_Valid_Credentials_Returns_Ok()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test1@mail.com",
            Password = "Test123_",
            FirstName = "test",
            LastName = "test"
        };
        
        // Act
        var request = await _client.PostAsJsonAsync("api/Auth/register", registerDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
    }
    
}