using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;

namespace MyApi.Controllers;

/// <summary>
/// Controller for weather forecast operations.
/// </summary>
[Authorize(Policy = "ApiScope")]
[ApiController]
[Route("v{version:apiVersion}/weather")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Retrieves a sample resource with custom headers.
    /// </summary>
    /// <remarks>
    /// This endpoint demonstrates how custom headers appear in Swagger documentation.
    /// </remarks>
    /// <param name="xCustomHeader">A custom header for demonstration purposes.</param>
    /// <returns>A sample response string.</returns>
    [HttpGet("custom-header")]
    [SwaggerOperation(
        Summary = "Get with custom header",
        Description = "Returns a message after receiving a custom header"
    )]
    [ProducesResponseType(typeof(string), 200)]
    [Produces("application/json")]
    public IActionResult GetWithCustomHeader(
        [FromHeader(Name = "X-Custom-Header")]
        [SwaggerParameter("Custom header value to be passed in the request", Required = true)]
        string xCustomHeader
    )
    {
        return Ok($"Received header: {xCustomHeader}");
    }


    #region Service Usage Limits
    // -------------------------------------------------------------
    // The following endpoint demonstrates rate limiting and quota headers.
    // Guideline: "limits which are dependent on the used service plan, acquired quota, 
    // the service region or deployment environment MAY be mentioned in the API specification, 
    // but their concrete values SHOULD NOT be added to an API specification 
    // (see service usage limits in [/devsecops/api-governance/api-guidelines/-/blob/review/common/api-documentation.md]API Documentation)."
    // -------------------------------------------------------------

    /// <summary>
    /// Get a weather forecast with rate limiting.
    /// Test with:
    /// curl -X GET http://localhost:5000/weather/limited
    /// More then 5 requests for minut will return a 429 error (Too Many Requests).
    /// </summary>
    /// <remarks>
    /// This endpoint is protected by a rate limiting policy (e.g., fixed window, sliding window, etc.).
    /// Useful to demonstrate or enforce quota restrictions per client.
    /// Rate limits and quota are returned in headers:
    /// - X-RateLimit-Limit: Maximum requests allowed in the current window.
    /// - X-RateLimit-Remaining: Requests remaining in the current window.
    /// - X-Quota-Remaining: Requests remaining in your monthly quota.
    /// </remarks>
    /// <returns>A list of weather forecasts.</returns>
    [HttpGet("limited")]
    [EnableRateLimiting("fixed")] // Policy name defined in program.cs
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [SwaggerOperation(
        Summary = "Get weather forecast with rate limiting",
        Description = "This endpoint demonstrates the use of rate limiting policies. If you exceed the quota, it returns status 429."
    )]
    public IActionResult GetWithRateLimit()
    {
        var forecast = GenerateForecast();
        return Ok(new
        {
            message = "This endpoint is protected by rate limiting.",
            forecast
        });
    }

    #endregion Service Usage Limits

    /// <summary>
    /// Get a public weather forecast.
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible without authentication.
    /// </remarks>
    /// <returns>A list of weather forecasts.</returns>
    /// <response code="200">Returns the list of weather forecasts.</response>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPublic()
    {
        var forecast = GenerateForecast();
        return Ok(forecast);
    }

    /// <summary>
    /// Get a private weather forecast.
    /// </summary>
    /// <remarks>
    /// This endpoint is protected and requires a valid token with the "api1" scope.
    /// </remarks>
    /// <returns>A list of weather forecasts along with user information.</returns>
    /// <response code="200">Returns the list of weather forecasts and user details.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    [HttpGet("private")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetPrivate()
    {
        var username = User.Identity?.Name ?? "No name provided";

        return Ok(new
        {
            message = "This endpoint is protected!",
            user = username,
            forecast = GenerateForecast()
        });
    }

    #region Deprecation Notes
    // -------------------------------------------------------------
    // The following endpoint is deprecated.
    // Guideline: "Deprecation notes. Deprecation notes MUST be added to an API specification 
    // according to [/devsecops/api-governance/api-guidelines/-/blob/review/common/api-lifecycle-phases.md#deprecation]Guideline Lifecycle Phases. 
    // OpenAPI or AsyncAPI properties description and deprecated SHOULD be used, if applicable. 
    // Deprecation notes SHOULD mention which other API or part of an API should be used instead of the deprecated one.
    // -------------------------------------------------------------

    /// <summary>
    /// [Deprecated] Get a weather forecast (old version).
    /// </summary>
    /// <remarks>
    /// This endpoint is deprecated. Please use <c>/weather/public</c> instead.
    /// </remarks>
    /// <returns>A list of weather forecasts.</returns>
    /// <response code="410">Gone - This endpoint is deprecated.</response>
    [HttpGet("deprecated")]
    [Obsolete("This endpoint is deprecated. Please use /weather/public instead.")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public IActionResult GetDeprecated()
    {
        return StatusCode(StatusCodes.Status410Gone, new
        {
            message = "This endpoint is deprecated. Please use /weather/public instead."
        });
    }

    #endregion Deprecation Notes

    /// <summary>
    /// Generate a custom weather forecast.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom weather forecast by providing a summary.
    /// </remarks>
    /// <param name="request">The request body containing the custom summary.</param>
    /// <returns>The generated weather forecast.</returns>
    /// <response code="200">Returns the generated weather forecast.</response>
    /// <response code="400">Bad request if the input is invalid.</response>
    [HttpPost("custom")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GenerateCustomForecast([FromBody] CustomForecastRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            return BadRequest(new { message = "Summary cannot be empty." });
        }

        var forecast = new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now),
            Random.Shared.Next(-20, 55),
            request.Summary
        );

        return Ok(forecast);
    }

    #region Error Response Schema
    // -------------------------------------------------------------
    // The following class defines the error response schema. 
    // Guideline: "details on error response status codes, error schemas, 
    // error types, and a clear association of errors to operations"
    // -------------------------------------------------------------
    
    /// <summary>
    /// Generate a custom weather forecast.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom weather forecast by providing the complete WeatherForecast.
    /// </remarks>
    /// <param name="request">The request body containing the custom WeatherForecast.</param>
    /// <returns>The generated weather forecast.</returns>
    [HttpPost("CompleteForecast")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherForecast))] // The "Type" parameter specifies the type of the response body.
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))] // The "Type" parameter specifies the type of the error response body.

    public IActionResult GenerateCustomForecast([FromBody] WeatherForecast request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Summary))
        {
            return BadRequest(new { message = "Request object or Summary cannot be null or empty." });
        }
        var forecast = new WeatherForecast(
            request.Date,
            request.TemperatureC,
            request.Summary
        );

        return Ok(forecast);
    }

    #endregion Error Response Schema

    /// <summary>
    /// Generate a weather forecast with additional parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom weather forecast by providing additional parameters such as location and humidity.
    /// </remarks>
    /// <param name="request">The request body containing the custom WeatherForecast details.</param>
    /// <param name="location">The location for the weather forecast.</param>
    /// <param name="humidity">The humidity level for the weather forecast.</param>
    /// <returns>The generated weather forecast with additional details.</returns>
    [HttpPost("custom-with-params")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GenerateForecastWithParams(
        [FromBody] WeatherForecast request,
        [FromQuery] string location,
        [FromQuery] int humidity)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Summary))
        {
            return BadRequest(new { message = "Request object or Summary cannot be null or empty." });
        }

        if (humidity < 0 || humidity > 100)
        {
            return BadRequest(new { message = "Humidity must be between 0 and 100." });
        }

        var forecast = new
        {
            request.Date,
            request.TemperatureC,
            request.Summary,
            Location = location,
            Humidity = humidity
        };

        return Ok(forecast);
    }


    #region Parameter Restrictions and Defaults
    // -------------------------------------------------------------
    // The following endpoint demonstrates parameter restrictions and default values.
    // Guideline: "restrictions with respect to format, character or number range of parameters and properties
    // restrictions with respect to number of array items or additional properties"
    // -------------------------------------------------------------

    /// <summary>
    /// Delete a weather forecast by ID and date.
    /// </summary>
    /// <remarks>
    /// This endpoint deletes a weather forecast based on the provided ID and date.
    /// The date must be in the format <c>yyyy-MM-dd</c>.
    /// Example: <c>/weather/1/2025-04-30</c>.
    /// </remarks>
    /// <param name="id">The ID of the weather forecast to delete.</param>
    /// <param name="date">The date of the weather forecast to delete.</param>
    /// <response code="204">No content - The weather forecast was successfully deleted.</response>
    /// <response code="400">Bad request - The date format is invalid.</response>
    /// <response code="404">Not found - The weather forecast with the specified ID and date does not exist.</response>
    [HttpDelete("{id:int}/{date:datetime}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete a weather forecast by ID and date",
        Description = "Deletes a weather forecast based on the provided ID and date. The date must be in the format yyyy-MM-dd."
    )]
    public IActionResult DeleteForecast(
        [FromRoute, SwaggerParameter(Description = "The ID of the weather forecast to delete (default: 1).")]
        [DefaultValue(2)] int id, // Default value for ID

        [FromRoute, SwaggerParameter(Description = "The date of the weather forecast to delete (default: 2025-01-01).")]
        [DefaultValue(typeof(DateTime), "2025-01-01")] DateTime date = default // Default value for date
    )
    {
        // Validate the date format
        if (date == default)
        {
            date = new DateTime(2025, 1, 1); // Set default date if not provided
        }

        // Simulate deletion logic
        bool forecastExists = id >= 1 && id <= 5; // Example: IDs 1-5 exist
        if (!forecastExists)
        {
            return NotFound(new { message = $"Weather forecast with ID {id} and date {date:yyyy-MM-dd} not found." });
        }

        // Simulate successful deletion
        return NoContent();
    }

    #endregion Parameter Restrictions and Defaults

    #region Versioning
    // -------------------------------------------------------------
    // API Versioning Endpoints
    // Guidelines: "API versioning enables backward compatibility and allows 
    // different versions of endpoints to coexist, providing smooth transitions 
    // between API versions."
    // -------------------------------------------------------------

    /// <summary>
    /// Get weather forecast data (V1 only endpoint).
    /// </summary>
    /// <remarks>
    /// This endpoint is only available in API version 1.0.
    /// It demonstrates a feature that exists only in the first version of the API.
    /// </remarks>
    /// <returns>A simple weather forecast response for V1.</returns>
    /// <response code="200">Returns the V1 weather forecast data.</response>
    [HttpGet("v1-only")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get weather forecast (V1 only)",
        Description = "This endpoint is exclusive to API version 1.0 and returns basic weather data."
    )]
    public IActionResult GetWeatherV1Only()
    {
        var forecast = new
        {
            Version = "1.0",
            Message = "This is a V1-only endpoint",
            Data = GenerateForecast().Take(3), // V1 returns only 3 forecasts
            Features = new[] { "Basic forecasting", "Simple data structure" }
        };

        return Ok(forecast);
    }

    /// <summary>
    /// Get advanced weather forecast data (V2 only endpoint).
    /// </summary>
    /// <remarks>
    /// This endpoint is only available in API version 2.0.
    /// It demonstrates new features and enhanced data structure available in V2.
    /// </remarks>
    /// <returns>An advanced weather forecast response for V2.</returns>
    /// <response code="200">Returns the V2 advanced weather forecast data.</response>
    [HttpGet("v2-only")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get advanced weather forecast (V2 only)",
        Description = "This endpoint is exclusive to API version 2.0 and returns enhanced weather data with additional features."
    )]
    public IActionResult GetWeatherV2Only()
    {
        var forecast = new
        {
            Version = "2.0",
            Message = "This is a V2-only endpoint with enhanced features",
            Data = GenerateForecast().Select(f => new
            {
                f.Date,
                f.TemperatureC,
                f.TemperatureF,
                f.Summary,
                // V2 additions
                Humidity = Random.Shared.Next(30, 90),
                WindSpeed = Random.Shared.Next(0, 25),
                Pressure = Random.Shared.Next(995, 1025)
            }),
            Features = new[] { "Advanced forecasting", "Extended data structure", "Additional weather metrics" },
            Metadata = new
            {
                DataSource = "Advanced Weather API v2",
                LastUpdated = DateTime.UtcNow,
                Accuracy = "High precision"
            }
        };

        return Ok(forecast);
    }

    /// <summary>
    /// Get weather summary (Available in both V1 and V2).
    /// </summary>
    /// <remarks>
    /// This endpoint is available in both API versions but returns different data structures.
    /// V1 returns basic summary, while V2 includes additional metadata.
    /// </remarks>
    /// <returns>Weather summary data appropriate for the requested version.</returns>
    /// <response code="200">Returns version-specific weather summary data.</response>
    [HttpGet("summary")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get weather summary (V1 and V2)",
        Description = "This endpoint is available in both versions but returns different data structures based on the API version."
    )]
    public IActionResult GetWeatherSummary()
    {
        var requestedVersion = HttpContext.GetRequestedApiVersion();
        
        if (requestedVersion?.MajorVersion == 1)
        {
            // V1 response - simple structure
            var v1Response = new
            {
                Version = "1.0",
                Summary = "Basic weather summary",
                TotalForecasts = 5,
                AverageTemperature = GenerateForecast().Average(f => f.TemperatureC),
                AvailableLocations = new[] { "City A", "City B", "City C" }
            };

            return Ok(v1Response);
        }
        else
        {
            // V2 response - enhanced structure
            var forecasts = GenerateForecast().ToList();
            var v2Response = new
            {
                Version = "2.0",
                Summary = "Enhanced weather summary with detailed analytics",
                Statistics = new
                {
                    TotalForecasts = forecasts.Count,
                    AverageTemperature = forecasts.Average(f => f.TemperatureC),
                    MinTemperature = forecasts.Min(f => f.TemperatureC),
                    MaxTemperature = forecasts.Max(f => f.TemperatureC),
                    TemperatureRange = forecasts.Max(f => f.TemperatureC) - forecasts.Min(f => f.TemperatureC)
                },
                Locations = new[]
                {
                    new { Name = "City A", Region = "North", Population = 150000 },
                    new { Name = "City B", Region = "South", Population = 200000 },
                    new { Name = "City C", Region = "East", Population = 175000 }
                },
                Metadata = new
                {
                    AnalysisDate = DateTime.UtcNow,
                    DataQuality = "Premium",
                    PredictionAccuracy = 95.5
                }
            };

            return Ok(v2Response);
        }
    }

    #endregion Versioning

    /// <summary>
    /// Generate a weather forecast for the next 5 days.
    /// </summary>
    /// <remarks>
    /// This method generates a random weather forecast for the next 5 days.
    /// </remarks>
    /// <returns>A list of weather forecasts.</returns>
    private static IEnumerable<WeatherForecast> GenerateForecast()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ))
        .ToArray();
    }
}

/// <summary>
/// Represents a weather forecast.
/// </summary>
public record WeatherForecast(
    DateOnly Date,
    [property: SwaggerSchema(
    Description = "The temperature in Celsius. Acceptable values range between -20 and 55.")]
    [System.ComponentModel.DataAnnotations.Range(-20, 55, ErrorMessage = "TemperatureC must be between -20 and 55.")]
    int TemperatureC, string? Summary)
{
    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/// <summary>
/// Request model for generating a custom weather forecast.
/// </summary>
public class CustomForecastRequest
{
    /// <summary>
    /// The custom summary for the weather forecast.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Represents an error response.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public string? Details { get; set; }
}