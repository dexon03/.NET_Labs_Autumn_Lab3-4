using System.Net;

namespace Lab4IntegrationTests;

public class AccountTests : BaseIntegrationTest
{
    public AccountTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task Get_Accounts_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        
        // Act
        var request = await _client.GetAsync($"api/Account/getAccounts/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
    }
}