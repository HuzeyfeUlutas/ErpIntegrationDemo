using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Domain.Events;
using PersonnelAccessManagement.Infrastructure.Auth;
using PersonnelAccessManagement.Infrastructure.EventHandlers;
using PersonnelAccessManagement.Infrastructure.Jobs;
using PersonnelAccessManagement.Infrastructure.Kafka;
using PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;
using PersonnelAccessManagement.Infrastructure.Services;

namespace PersonnelAccessManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
                               ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");

        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                });
        });

        services.AddHangfireServer();

        // Job + processor
        services.AddScoped<IScheduledActionJob, DailyScheduledActionJob>();
        services.AddScoped<IScheduledActionProcessor, ScheduledActionProcessor>();
        
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
        
        services.AddScoped<IPersonnelRoleBatchProcessor, PersonnelRoleBatchProcessor>();
        services.AddScoped<IPersonnelRoleService, PersonnelRoleService>();
        services.AddScoped<IRoleReconciliationEngine, RoleReconciliationEngine>();
        services.AddTransient<RuleCreatedEventHandler>();
        services.AddTransient<RuleDeletedEventHandler>();
        services.AddTransient<RuleUpdatedEventHandler>();
        
        services.AddOptions<KafkaConsumerOptions>()
            .Bind(configuration.GetSection(KafkaConsumerOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<PersonnelRoleOptions>()
            .Bind(configuration.GetSection(PersonnelRoleOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<AdminSettings>()
            .Bind(configuration.GetSection(AdminSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton<IKafkaMessageDeserializer<PersonnelLifecycleIntegrationEvent>, KafkaMessageDeserializer>();
        services.AddSingleton<IKafkaEventLogger, KafkaEventLogger>();
        services.AddSingleton<IKafkaCapBridge, KafkaCapBridge>();
        
        services.AddHostedService<KafkaPersonnelConsumer>();
        
        
        services.AddTransient<HiredEventHandler>();
        services.AddTransient<TerminatedEventHandler>();
        services.AddTransient<PositionChangedEventHandler>();
        
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordVerifier, SimplePasswordVerifier>();

        return services;
    }
}