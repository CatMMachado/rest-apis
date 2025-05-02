using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApi.Controllers;

/// <summary>
/// Controller for weather forecast operations.
/// </summary>
[Authorize(Policy = "ApiScope")]
[ApiController]
[Route("weather")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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


    /// <summary>
    /// Delete a weather forecast by ID and date.
    /// </summary>
    /// <remarks>
    /// This endpoint deletes a weather forecast based on the provided ID and date.
    /// The date must be in the format <c>yyyy-MM-dd</c>.
    /// Example: <c>/weather/1/2025-04-30</c>.
    /// </remarks>
    /// <param name="id">The ID of the weather forecast to delete (default: 1).</param>
    /// <param name="date">The date of the weather forecast to delete (default: 2025-01-01).</param>
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