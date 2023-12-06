using System.Reflection;
using FluentValidation;
using Lab4.Infrastructure.Database;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;
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
        services.AddScoped<IRepository, Repository>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}