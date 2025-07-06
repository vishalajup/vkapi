using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation;

namespace vkapi.Controllers;

/// <summary>
/// Weather forecast controller for testing and demonstration purposes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Weather forecast operations")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private static readonly string[] Summaries = new[]
    {
        "Freezing1", "Bracing1", "Chilly1", "Cool1", "Mild1", "Warm1", "Balmy1", "Hot1", "Sweltering1", "Scorching1"
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a weather forecast for the specified number of days
    /// </summary>
    /// <param name="days">Number of days to forecast (1-14)</param>
    /// <returns>Weather forecast data</returns>
    /// <response code="200">Returns the weather forecast</response>
    /// <response code="400">If the days parameter is invalid</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get weather forecast",
        Description = "Retrieves weather forecast data for the specified number of days",
        OperationId = "GetWeatherForecast",
        Tags = new[] { "Weather" }
    )]
    [SwaggerResponse(200, "Weather forecast retrieved successfully", typeof(IEnumerable<WeatherForecastDto>))]
    [SwaggerResponse(400, "Invalid request parameters")]
    public ActionResult<IEnumerable<WeatherForecastDto>> Get([FromQuery] int days = 5)
    {
        _logger.LogInformation("Getting weather forecast for {Days} days", days);

        if (days < 1 || days > 14)
        {
            return BadRequest(new { Error = "Days must be between 1 and 14" });
        }

        var forecast = Enumerable.Range(1, days).Select(index =>
            new WeatherForecastDto
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        return Ok(forecast);
    }

    /// <summary>
    /// Gets a specific weather forecast by date
    /// </summary>
    /// <param name="date">The date for the forecast (YYYY-MM-DD)</param>
    /// <returns>Weather forecast for the specified date</returns>
    /// <response code="200">Returns the weather forecast for the specified date</response>
    /// <response code="400">If the date format is invalid</response>
    /// <response code="404">If no forecast is available for the specified date</response>
    [HttpGet("{date:datetime}")]
    [SwaggerOperation(
        Summary = "Get weather forecast by date",
        Description = "Retrieves weather forecast data for a specific date",
        OperationId = "GetWeatherForecastByDate",
        Tags = new[] { "Weather" }
    )]
    [SwaggerResponse(200, "Weather forecast retrieved successfully", typeof(WeatherForecastDto))]
    [SwaggerResponse(400, "Invalid date format")]
    [SwaggerResponse(404, "Forecast not found for the specified date")]
    public ActionResult<WeatherForecastDto> GetByDate(DateOnly date)
    {
        _logger.LogInformation("Getting weather forecast for date: {Date}", date);

        // Simulate some business logic
        if (date < DateOnly.FromDateTime(DateTime.Now))
        {
            return NotFound(new { Error = "Cannot retrieve forecast for past dates" });
        }

        var forecast = new WeatherForecastDto
        {
            Date = date,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };

        return Ok(forecast);
    }

    /// <summary>
    /// Creates a new weather forecast entry
    /// </summary>
    /// <param name="request">Weather forecast data</param>
    /// <returns>Created weather forecast</returns>
    /// <response code="201">Weather forecast created successfully</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create weather forecast",
        Description = "Creates a new weather forecast entry",
        OperationId = "CreateWeatherForecast",
        Tags = new[] { "Weather" }
    )]
    [SwaggerResponse(201, "Weather forecast created successfully", typeof(WeatherForecastDto))]
    [SwaggerResponse(400, "Invalid request data")]
    public ActionResult<WeatherForecastDto> Create([FromBody] CreateWeatherForecastRequest request)
    {
        _logger.LogInformation("Creating weather forecast for date: {Date}", request.Date);

        var forecast = new WeatherForecastDto
        {
            Date = request.Date,
            TemperatureC = request.TemperatureC,
            Summary = request.Summary
        };

        return CreatedAtAction(nameof(GetByDate), new { date = request.Date }, forecast);
    }
}

/// <summary>
/// Weather forecast data transfer object
/// </summary>
public class WeatherForecastDto
{
    /// <summary>
    /// The date of the forecast
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Temperature in Celsius
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Temperature in Fahrenheit
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Weather summary
    /// </summary>
    public string? Summary { get; set; }
}

/// <summary>
/// Request model for creating a weather forecast
/// </summary>
public class CreateWeatherForecastRequest
{
    /// <summary>
    /// The date for the forecast
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Temperature in Celsius
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Weather summary
    /// </summary>
    public string? Summary { get; set; }
}

/// <summary>
/// Validator for CreateWeatherForecastRequest
/// </summary>
public class CreateWeatherForecastRequestValidator : AbstractValidator<CreateWeatherForecastRequest>
{
    public CreateWeatherForecastRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
            .WithMessage("Date must be today or in the future");

        RuleFor(x => x.TemperatureC)
            .InclusiveBetween(-50, 60)
            .WithMessage("Temperature must be between -50 and 60 degrees Celsius");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("Summary is required")
            .MaximumLength(100)
            .WithMessage("Summary cannot exceed 100 characters");
    }
} 