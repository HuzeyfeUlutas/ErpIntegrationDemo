using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Persistence.DbContexts;
using PersonnelAccessManagement.Persistence.Repositories;

namespace PersonnelAccessManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration
            .GetSection(RabbitMQOptions.SectionName)
            .Get<RabbitMQOptions>()!;

        services.AddCap(cap =>
        {
            // PostgreSQL - outbox pattern için aynı DB'yi kullanır
            cap.UsePostgreSql(opt =>
            {
                opt.ConnectionString = configuration.GetConnectionString("Default")!;
                opt.Schema = "cap";  // ayrı schema ile izole et
            });

            // RabbitMQ transport
            cap.UseRabbitMQ(rabbit =>
            {
                rabbit.HostName = rabbitMqOptions.Host;
                rabbit.Port = rabbitMqOptions.Port;
                rabbit.UserName = rabbitMqOptions.UserName;
                rabbit.Password = rabbitMqOptions.Password;
                rabbit.VirtualHost = rabbitMqOptions.VirtualHost;
                rabbit.ExchangeName = rabbitMqOptions.ExchangeName;
            });

            // Dashboard (dev ortamı için)
            cap.UseDashboard(d =>
            {
                d.PathMatch = "/cap-dashboard";
            });

            // Genel ayarlar
            cap.DefaultGroupName = configuration["Cap:DefaultGroupName"] ?? "pam.api";
            cap.FailedRetryCount = int.Parse(configuration["Cap:FailedRetryCount"] ?? "5");
            cap.FailedRetryInterval = int.Parse(configuration["Cap:FailedRetryInterval"] ?? "60");
            
            cap.ConsumerThreadCount = 1;
        });
        
        services.AddScoped<IPersonnelRoleService, PersonnelRoleService>();
        services.AddTransient<RuleCreatedEventHandler>();

        return services;
    }
}