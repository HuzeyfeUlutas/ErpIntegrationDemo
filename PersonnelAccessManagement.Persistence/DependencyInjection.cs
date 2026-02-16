using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Persistence.DbContexts;
using PersonnelAccessManagement.Persistence.Repositories;

namespace PersonnelAccessManagement.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PersonnelAccessManagementDbContext>(options =>
        {
            var cs = configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("ConnectionStrings:Default missing.");

            options.UseNpgsql(cs, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(PersonnelAccessManagementDbContext).Assembly.FullName);
            });

            // Debug için (prod’da kapatılır)
            // options.EnableSensitiveDataLogging();
            // options.EnableDetailedErrors();
        });
        
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork.EfUnitOfWork>();

        return services;
    }
}