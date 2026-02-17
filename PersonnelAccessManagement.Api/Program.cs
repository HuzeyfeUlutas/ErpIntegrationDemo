using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using PersonnelAccessManagement.Api.Middlewares;
using PersonnelAccessManagement.Api.Observability;
using PersonnelAccessManagement.Api.Services;
using PersonnelAccessManagement.Application;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Infrastructure;
using PersonnelAccessManagement.Infrastructure.Seeders;
using PersonnelAccessManagement.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Vite dev port
            .AllowAnyHeader()
            .AllowAnyMethod();
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

// If you don't have HTTPS configured locally, you can comment this out during dev.
app.UseHttpsRedirection();

// ---------- Endpoints ----------
app.MapControllers();

app.Run();
