using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Persistence.DbContexts;
using PersonnelAccessManagement.Persistence.Interceptors;
using PersonnelAccessManagement.Persistence.Repositories;

namespace PersonnelAccessManagement.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddScoped<AuditableEntityInterceptor>();
        
        services.AddDbContext<PersonnelAccessManagementDbContext>((sp, options) =>  // ← sp eklendi
        {
            var cs = configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("ConnectionStrings:Default missing.");

            options.UseNpgsql(cs, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(PersonnelAccessManagementDbContext).Assembly.FullName);
            });

            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());  // ← interceptor
        });
        
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork.EfUnitOfWork>();
        
        return services;
    }
}