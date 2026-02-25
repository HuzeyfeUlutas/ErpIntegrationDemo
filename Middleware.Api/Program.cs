using Middleware.Infrastructure.Kafka;
using Middleware.Infrastructure.Persistence;
using Middleware.Infrastructure.Security;
using MiddlewareApplication.Abstractions;
using MiddlewareApplication.Ingestion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var middlewareCs = builder.Configuration.GetConnectionString("Default")
                  ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

var pamCs = builder.Configuration.GetConnectionString("PamDb")
            ?? throw new InvalidOperationException("ConnectionStrings:PamDb is missing.");

var hmacOptions = builder.Configuration.GetSection("Security").Get<HmacOptions>()
                  ?? throw new InvalidOperationException("Security section is missing.");

builder.Services.AddSingleton(hmacOptions);
builder.Services.AddSingleton<HmacSignatureVerifier>();

builder.Services.AddSingleton(new DbOptions { ConnectionString = middlewareCs });
builder.Services.AddScoped<IProcessedRequestsStore, ProcessedRequestsRepository>();
builder.Services.AddScoped<DbInitializer>();

var pamDbOptions = new PamDbOptions { ConnectionString = pamCs };
builder.Services.AddSingleton(pamDbOptions);
builder.Services.AddScoped<IPersonnelDbUpdater, PersonnelDbUpdater>();

var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>()
                   ?? throw new InvalidOperationException("Kafka section is missing.");

builder.Services.AddSingleton(kafkaOptions);
builder.Services.AddSingleton<IEventPublisher, KafkaProducer>();

builder.Services.AddScoped<SapEventIngestionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var init = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await init.InitializeAsync();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();