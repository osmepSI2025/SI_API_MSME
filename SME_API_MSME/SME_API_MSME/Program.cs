using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using Serilog;
using Serilog.Events;
using SME_API_MSME.Entities;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;

try
{
    Log.Logger = new LoggerConfiguration()
               .WriteTo.File(
                   path: "Logs\\log-.txt",
                   outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:1j}{NewLine}{Exception}",
                   rollingInterval: RollingInterval.Day,
                   restrictedToMinimumLevel: LogEventLevel.Information
               ).CreateLogger();

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<MSMEDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
    // Add services to the container.
    builder.Services.AddControllers();

    // ✅ Register NSwag (Swagger 2.0)
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = "API_SME_MSME_v1";
        config.Title = "API SME MSME V1.0002";
        config.Version = "v1";
        config.Description = "API documentation using Swagger 2.0";
        config.SchemaType = NJsonSchema.SchemaType.Swagger2; // This makes it Swagger 2.0
    });
    builder.Services.AddScoped<ProjectRepository>();
    builder.Services.AddScoped<ProjectService>();
    builder.Services.AddScoped<ProjectAreaRepository>();
    builder.Services.AddScoped<ProjectAreaService>();
    builder.Services.AddScoped<ProjectProductRepository>();
    builder.Services.AddScoped<ProjectProductService>();
    builder.Services.AddScoped<ProjectOutcomeRepository>();
    builder.Services.AddScoped<ProjectOutcomeService>();
    builder.Services.AddScoped<ExpectedOutcomeRepository>();
    builder.Services.AddScoped<ExpectedOutcomeService>();
    builder.Services.AddScoped<ProjectActivityRepository>();
    builder.Services.AddScoped<ProjectActivityService>();
    builder.Services.AddScoped<BudgetPlanRepository>();
    builder.Services.AddScoped<BudgetPlanService>();
    builder.Services.AddScoped<ProductResultRepository>();
    builder.Services.AddScoped<ProductResultService>();
    builder.Services.AddScoped<OutcomeResultRepository>();
    builder.Services.AddScoped<OutcomeResultService>();
    builder.Services.AddScoped<PerformanceResultRepository>();
    builder.Services.AddScoped<PerformanceResultService>();
    builder.Services.AddScoped<DisbursementResultRepository>();
    builder.Services.AddScoped<DisbursementResultService>();
    builder.Services.AddScoped<ExpectedOutcomeResultRepository>();
    builder.Services.AddScoped<ExpectedOutcomeResultService>();
    builder.Services.AddScoped<EconomicValueRepository>();
    builder.Services.AddScoped<EconomicValueService>();


    builder.Services.AddScoped<IApiInformationRepository, ApiInformationRepository>();

    builder.Services.AddScoped<ICallAPIService, CallAPIService>(); // Register ICallAPIService with CallAPIService
    builder.Services.AddHttpClient<CallAPIService>();



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseOpenApi();     // Serve the Swagger JSON
        app.UseSwaggerUi3();  // Use Swagger UI v3
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

catch (Exception ex)
{
    Log.Fatal(ex.Message, "Application terminated unexpectedly");
    Console.WriteLine($"An error occurred: {ex.Message}");
    System.IO.File.WriteAllText("startup-error.log", ex.ToString());
}
