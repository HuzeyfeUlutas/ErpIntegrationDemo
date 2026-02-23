using System.Text;
using System.Text.Json;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using PersonnelAccessManagement.Api.Middlewares;
using PersonnelAccessManagement.Api.Observability;
using PersonnelAccessManagement.Api.Services;
using PersonnelAccessManagement.Application;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Infrastructure;
using PersonnelAccessManagement.Infrastructure.Jobs;
using PersonnelAccessManagement.Infrastructure.Seeders;
using PersonnelAccessManagement.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",    // Vite default port
                "https://localhost:7051"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ---------- Elastic options (fail-fast) ----------
var elastic = builder.Configuration.GetRequiredSection("Elastic").Get<ElasticOptions>()
              ?? throw new InvalidOperationException("Elastic section is missing.");

if (elastic.Uris is null || elastic.Uris.Length == 0)
    throw new InvalidOperationException("Elastic:Uris is missing or empty.");

Uri[] nodes;
try
{
    nodes = elastic.Uris.Select(u => new Uri(u)).ToArray();
}
catch (Exception ex)
{
    throw new InvalidOperationException("Elastic:Uris contains invalid URI.", ex);
}

// ---------- Serilog ----------
builder.Host.UseSerilog((context, services, lc) =>
{
    lc.ReadFrom.Configuration(context.Configuration)
      .Enrich.FromLogContext()
      .Enrich.WithMachineName()
      .Enrich.WithThreadId()
      .WriteTo.Console()
      .WriteTo.Elasticsearch(
          nodes,
          opts =>
          {
              opts.DataStream = new DataStreamName(
                  elastic.DataStream.Type,
                  elastic.DataStream.Dataset,
                  elastic.DataStream.Namespace);

              opts.BootstrapMethod = BootstrapMethod.Silent; // dev için
          });
});

// ---------- Services ----------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationIdAccessor, HttpCorrelationIdAccessor>();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["Secret"]
             ?? throw new InvalidOperationException("Jwt:Secret is required.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


// ---------- Health Checks ----------
var defaultCs = builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
var rabbitHost = rabbitMqConfig["Host"] ?? throw new InvalidOperationException("RabbitMQ:Host is required.");
var rabbitPort = rabbitMqConfig["Port"] ?? "5672";
var rabbitUser = rabbitMqConfig["UserName"] ?? throw new InvalidOperationException("RabbitMQ:UserName is required.");
var rabbitPass = rabbitMqConfig["Password"] ?? throw new InvalidOperationException("RabbitMQ:Password is required.");
var rabbitVHost = rabbitMqConfig["VirtualHost"] ?? "/";

var elasticUri = builder.Configuration["Elastic:Uris:0"]
                 ?? throw new InvalidOperationException("Elastic:Uris:0 is required.");

var kafkaBootstrap = builder.Configuration["Kafka:BootstrapServers"]
                     ?? throw new InvalidOperationException("Kafka:BootstrapServers is required.");

builder.Services.AddHealthChecks()
    .AddNpgSql(
        defaultCs,
        name: "postgresql",
        tags: new[] { "ready" })
    .AddRabbitMQ(
        sp => new RabbitMQ.Client.ConnectionFactory
        {
            HostName = rabbitHost,
            Port = int.Parse(rabbitPort),
            UserName = rabbitUser,
            Password = rabbitPass,
            VirtualHost = rabbitVHost
        }.CreateConnectionAsync().GetAwaiter().GetResult(),
        name: "rabbitmq",
        tags: new[] { "ready" })
    .AddUrlGroup(
        new Uri($"{elasticUri}/_cluster/health"),
        name: "elasticsearch",
        tags: new[] { "ready" })
    .AddKafka(
        setup => { setup.BootstrapServers = kafkaBootstrap; },
        name: "kafka",
        tags: new[] { "ready" });

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);



// Data Seed
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

app.UseCors("AllowFrontend");

// ---------- Middleware pipeline (order matters) ----------
app.UseMiddleware<CorrelationIdMiddleware>();      // trace.id set
app.UseMiddleware<ExceptionHandlingMiddleware>();  // validation/app/500 mapping
app.UseSerilogRequestLogging();                    // request logs include trace.id

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();  
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthFilter() },
    DashboardTitle = "Personnel Access Management — Jobs"
});

RecurringJob.AddOrUpdate<IScheduledActionJob>(
    recurringJobId: "daily-scheduled-action",
    methodCall: job => job.ExecuteAsync(CancellationToken.None),
    cronExpression: "0 9 * * *",  // Her gün 09:00
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
    });

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});

// ---------- Endpoints ----------
app.MapControllers();

app.Run();
