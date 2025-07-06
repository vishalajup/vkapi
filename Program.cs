using Serilog;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Reflection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using vkapi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting VK API application");

    // Add services to the container
    builder.Services.AddControllers();
    
    // Configure CORS
    var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevelopmentPolicy", policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Configure Swagger/OpenAPI
    var apiSettings = builder.Configuration.GetSection("ApiSettings");
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = apiSettings["Title"] ?? "VK API",
            Description = apiSettings["Description"] ?? "Sample Web API for testing and development",
            Version = apiSettings["Version"] ?? "v1",
            Contact = new OpenApiContact
            {
                Name = apiSettings["ContactName"] ?? "Development Team",
                Email = apiSettings["ContactEmail"] ?? "dev@example.com"
            }
        });

        // Include XML comments for better documentation
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // Add annotations support
        c.EnableAnnotations();
        
        // Add JWT Bearer authentication (for future use)
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Add FluentValidation (SharpGrip + FluentValidation)
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateWeatherForecastRequestValidator>();

    // Add HTTP client factory
    builder.Services.AddHttpClient();

    // Add health checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "VK API v1");
            c.RoutePrefix = string.Empty; // Serve Swagger UI at root
            c.DocumentTitle = "VK API Documentation";
            c.DefaultModelsExpandDepth(-1); // Hide schemas section
        });

        // Enable CORS for development
        app.UseCors("DevelopmentPolicy");
    }
    else
    {
        // In production, you might want to configure CORS differently
        app.UseCors("DevelopmentPolicy");
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    // Map health checks
    app.MapHealthChecks("/health");

    // Map controllers
    app.MapControllers();

    // Sample endpoints for testing
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

    app.MapGet("/api/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
        .WithName("HealthCheck")
        .WithOpenApi();

    app.MapGet("/api/info", () => new 
    { 
        Application = "VK API",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow
    })
    .WithName("ApiInfo")
    .WithOpenApi();

    Log.Information("VK API application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Sample weather forecast record for testing purposes
/// </summary>
/// <param name="Date">The date of the forecast</param>
/// <param name="TemperatureC">Temperature in Celsius</param>
/// <param name="Summary">Weather summary</param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Temperature in Fahrenheit
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
