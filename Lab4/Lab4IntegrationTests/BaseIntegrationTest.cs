using System.Net.Http.Headers;
using Lab4.Infrastructure.Database;
using Lab4IntegrationTests.AuthMock;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Lab4IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IntegrationTestWebAppFactory _factory;
    private readonly IServiceScope _scope;
    protected readonly FinanceDbContext FinanceDbContext;
    protected HttpClient _client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _factory = factory;
        FinanceDbContext = _scope.ServiceProvider
            .GetRequiredService<FinanceDbContext>();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtMock.GenerateJwtToken());
    }
}