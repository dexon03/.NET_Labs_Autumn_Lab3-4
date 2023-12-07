using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using FluentValidation;
using Lab4.Application.Services;
using Lab4.Domain.Contracts;
using Lab4.Infrastructure.Database;
using Lab4.Infrastructure.Database.AutoUpdateDatabase;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Lab4.Setup;

public static class DependencyInjection
{
    private static Assembly _applicationAssembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection RegisterDependencies(this IServiceCollection services,
        IConfiguration appConfiguration)
    {
        services.AddDbContext<FinanceDbContext>(opt =>
        {
            opt.UseNpgsql(appConfiguration.GetConnectionString("DefaultConnection"));
        });
        services.AddValidatorsFromAssembly(_applicationAssembly);
        services.AddFluentValidationAutoValidation();
        services.AddScoped<UserManager>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMigrationsManager, MigrationsManager>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddAutoMapper(_applicationAssembly);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appConfiguration["Jwt:Issuer"],
                    ValidAudience = appConfiguration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfiguration["Jwt:Key"]))
                };
            });
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        return services;
    }
}