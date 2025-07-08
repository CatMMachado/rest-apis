using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;

namespace MyApi.Controllers;

/// <summary>
/// Controller for device operations.
/// </summary>
[Authorize(Policy = "ApiScope")]
[ApiController]
[Route("v{version:apiVersion}/devices")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class DeviceController : ControllerBase
{
    private static readonly string[] DeviceNames =
    [
        "Smart Sensor", "IoT Gateway", "Temperature Monitor", "Pressure Valve", "Control Panel",
        "Display Unit", "Communication Hub", "Power Module", "Safety Switch", "Data Logger"
    ];

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
    public IActionResult GetWithCustomHeader(
        [FromHeader(Name = "X-Custom-Header")]
        [SwaggerParameter("Custom header value to be passed in the request", Required = true)]
        string xCustomHeader
    )
    {
        return Ok($"Received header: {xCustomHeader}");
    }


    #region Service Usage Limits
    // --------------------------------------------------------------------
    // The following endpoint demonstrates rate limiting and quota headers.
    // --------------------------------------------------------------------

    /// <summary>
    /// Get devices with rate limiting.
    /// Test with:
    /// curl -X GET http://localhost:5000/devices/limited
    /// More then 5 requests per minute will return a 429 error (Too Many Requests).
    /// </summary>
    /// <remarks>
    /// This endpoint is protected by a rate limiting policy (e.g., fixed window, sliding window, etc.).
    /// Useful to demonstrate or enforce quota restrictions per client.
    /// Rate limits and quota are returned in headers:
    /// - X-RateLimit-Limit: Maximum requests allowed in the current window.
    /// - X-RateLimit-Remaining: Requests remaining in the current window.
    /// - X-Quota-Remaining: Requests remaining in your monthly quota.
    /// </remarks>
    /// <returns>A list of devices.</returns>
    [HttpGet("limited")]
    [EnableRateLimiting("fixed")] // Policy name defined in program.cs
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [SwaggerOperation(
        Summary = "Get devices with rate limiting",
        Description = "This endpoint demonstrates the use of rate limiting policies. If you exceed the quota, it returns status 429."
    )]
    [Produces("application/json")]
    public IActionResult GetWithRateLimit()
    {
        var devices = GenerateDevices();
        return Ok(new
        {
            message = "This endpoint is protected by rate limiting.",
            devices
        });
    }

    #endregion Service Usage Limits

    /// <summary>
    /// Get public devices.
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible without authentication.
    /// </remarks>
    /// <returns>A list of devices.</returns>
    /// <response code="200">Returns the list of devices.</response>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))]
    [Produces("application/json")]
    public IActionResult GetPublic()
    {
        var devices = GenerateDevices();
        return Ok(devices);
    }

    /// <summary>
    /// Get private devices.
    /// </summary>
    /// <remarks>
    /// This endpoint is protected and requires a valid token with the "api1" scope.
    /// </remarks>
    /// <returns>A list of devices along with user information.</returns>
    /// <response code="200">Returns the list of devices and user details.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    [HttpGet("private")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    public IActionResult GetPrivate()
    {
        var username = User.Identity?.Name ?? "No name provided";

        return Ok(new
        {
            message = "This endpoint is protected!",
            user = username,
            devices = GenerateDevices()
        });
    }

    #region Deprecation Notes
    // -------------------------------------------------------------
    // The following endpoint is deprecated.
    // -------------------------------------------------------------

    /// <summary>
    /// [Deprecated] Get devices (old version).
    /// </summary>
    /// <remarks>
    /// This endpoint is deprecated. Please use <c>/devices/public</c> instead.
    /// </remarks>
    /// <returns>A list of devices.</returns>
    /// <response code="410">Gone - This endpoint is deprecated.</response>
    [HttpGet("deprecated")]
    [Obsolete("This endpoint is deprecated. Please use /devices/public instead.")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [Produces("application/json")]
    public IActionResult GetDeprecated()
    {
        return StatusCode(StatusCodes.Status410Gone, new
        {
            message = "This endpoint is deprecated. Please use /devices/public instead."
        });
    }

    #endregion Deprecation Notes

    /// <summary>
    /// Generate a custom device.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom device by providing a name.
    /// </remarks>
    /// <param name="request">The request body containing the custom device name.</param>
    /// <returns>The generated device.</returns>
    /// <response code="200">Returns the generated device.</response>
    /// <response code="400">Bad request if the input is invalid.</response>
    [HttpPost("custom")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public IActionResult GenerateCustomDevice([FromBody] CustomDeviceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Name cannot be empty." });
        }

        var device = new Device(
            request.Name,
            new DeviceType(Random.Shared.Next(1, 6), "Custom Type"),
            new Dimension(
                Math.Round(Random.Shared.NextDouble() * 50 + 5, 2),
                Math.Round(Random.Shared.NextDouble() * 30 + 3, 2),
                Math.Round(Random.Shared.NextDouble() * 20 + 2, 2)
            )
        );

        return Ok(device);
    }

    #region Error Response Schema
    // -------------------------------------------------------------
    // The following class defines the error response schema. 
    // -------------------------------------------------------------
    
    /// <summary>
    /// Generate a custom device.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom device by providing the complete Device.
    /// </remarks>
    /// <param name="request">The request body containing the custom Device.</param>
    /// <returns>The generated device.</returns>
    [HttpPost("CompleteDevice")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))] // The "Type" parameter specifies the type of the response body.
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))] // The "Type" parameter specifies the type of the error response body.
    [Produces("application/json")]
    public IActionResult GenerateCustomDevice([FromBody] Device request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }
        var device = new Device(
            request.Name,
            request.DeviceType,
            request.Dimension
        );

        return Ok(device);
    }

    #endregion Error Response Schema

    /// <summary>
    /// Generate a device with additional parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom device by providing additional parameters such as location and status.
    /// </remarks>
    /// <param name="request">The request body containing the custom Device details.</param>
    /// <param name="location">The location for the device.</param>
    /// <param name="status">The status of the device (0=Offline, 1=Online).</param>
    /// <returns>The generated device with additional details.</returns>
    [HttpPost("custom-with-params")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public IActionResult GenerateDeviceWithParams(
        [FromBody] Device request,
        [FromQuery] string location,
        [FromQuery] int status)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }

        if (status < 0 || status > 1)
        {
            return BadRequest(new { message = "Status must be 0 (Offline) or 1 (Online)." });
        }

        var device = new
        {
            request.Name,
            request.DeviceType,
            request.Dimension,
            Location = location,
            Status = status == 1 ? "Online" : "Offline"
        };

        return Ok(device);
    }


    #region Parameter Restrictions and Defaults
    // -------------------------------------------------------------
    // The following endpoint demonstrates parameter restrictions and default values.
    // -------------------------------------------------------------

    /// <summary>
    /// Delete a device by ID and date.
    /// </summary>
    /// <remarks>
    /// This endpoint deletes a device based on the provided ID and date.
    /// The date must be in the format <c>yyyy-MM-dd</c>.
    /// Example: <c>/devices/1/2025-04-30</c>.
    /// </remarks>
    /// <param name="id">The ID of the device to delete.</param>
    /// <param name="date">The date of the device registration to delete.</param>
    /// <response code="204">No content - The device was successfully deleted.</response>
    /// <response code="400">Bad request - The date format is invalid.</response>
    /// <response code="404">Not found - The device with the specified ID and date does not exist.</response>
    [HttpDelete("{id:int}/{date:datetime}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete a device by ID and date",
        Description = "Deletes a device based on the provided ID and date. The date must be in the format yyyy-MM-dd."
    )]
    [Produces("application/json")]
    public IActionResult DeleteDevice(
        [FromRoute, SwaggerParameter(Description = "The ID of the device to delete (default: 1).")]
        [DefaultValue(2)] int id, // Default value for ID

        [FromRoute, SwaggerParameter(Description = "The date of the device registration to delete (default: 2025-01-01).")]
        [DefaultValue(typeof(DateTime), "2025-01-01")] DateTime date = default // Default value for date
    )
    {
        // Validate the date format
        if (date == default)
        {
            date = new DateTime(2025, 1, 1); // Set default date if not provided
        }

        // Simulate deletion logic
        bool deviceExists = id >= 1 && id <= 5; // Example: IDs 1-5 exist
        if (!deviceExists)
        {
            return NotFound(new { message = $"Device with ID {id} and date {date:yyyy-MM-dd} not found." });
        }

        // Simulate successful deletion
        return NoContent();
    }

    #endregion Parameter Restrictions and Defaults

    #region API Versioning
    // -------------------------------------------------------------
    // API Versioning Endpoints
    // -------------------------------------------------------------

    /// <summary>
    /// Get device data (V1 only endpoint).
    /// </summary>
    /// <remarks>
    /// This endpoint is only available in API version 1.0.
    /// It demonstrates a feature that exists only in the first version of the API.
    /// </remarks>
    /// <returns>A simple device response for V1.</returns>
    /// <response code="200">Returns the V1 device data.</response>
    [HttpGet("v1-only")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get devices (V1 only)",
        Description = "This endpoint is exclusive to API version 1.0 and returns basic device data."
    )]
    [Produces("application/json")]
    public IActionResult GetDevicesV1Only()
    {
        var devices = new
        {
            Version = "1.0",
            Message = "This is a V1-only endpoint",
            Data = GenerateDevices().Take(3), // V1 returns only 3 devices
            Features = new[] { "Basic device listing", "Simple data structure" }
        };

        return Ok(devices);
    }

    /// <summary>
    /// Get advanced device data (V2 only endpoint).
    /// </summary>
    /// <remarks>
    /// This endpoint is only available in API version 2.0.
    /// It demonstrates new features and enhanced data structure available in V2.
    /// </remarks>
    /// <returns>An advanced device response for V2.</returns>
    /// <response code="200">Returns the V2 advanced device data.</response>
    [HttpGet("v2-only")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get advanced devices (V2 only)",
        Description = "This endpoint is exclusive to API version 2.0 and returns enhanced device data with additional features."
    )]
    [Produces("application/json")]
    public IActionResult GetDevicesV2Only()
    {
        var devices = new
        {
            Version = "2.0",
            Message = "This is a V2-only endpoint with enhanced features",
            Data = GenerateDevices().Select(d => new
            {
                d.Name,
                d.DeviceType,
                d.Dimension,
                d.Dimension.Volume,
                // V2 additions
                Status = Random.Shared.Next(0, 2) == 1 ? "Online" : "Offline",
                BatteryLevel = Random.Shared.Next(10, 100),
                LastSeen = DateTime.UtcNow.AddMinutes(-Random.Shared.Next(1, 1440))
            }),
            Features = new[] { "Advanced device monitoring", "Extended data structure", "Additional device metrics" },
            Metadata = new
            {
                DataSource = "Advanced Device API v2",
                LastUpdated = DateTime.UtcNow,
                Accuracy = "High precision"
            }
        };

        return Ok(devices);
    }

    /// <summary>
    /// Get device summary (Available in both V1 and V2).
    /// </summary>
    /// <remarks>
    /// This endpoint is available in both API versions but returns different data structures.
    /// V1 returns basic summary, while V2 includes additional metadata.
    /// </remarks>
    /// <returns>Device summary data appropriate for the requested version.</returns>
    /// <response code="200">Returns version-specific device summary data.</response>
    [HttpGet("summary")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get device summary (V1 and V2)",
        Description = "This endpoint is available in both versions but returns different data structures based on the API version."
    )]
    [Produces("application/json")]
    public IActionResult GetDeviceSummary()
    {
        var requestedVersion = HttpContext.GetRequestedApiVersion();

        if (requestedVersion?.MajorVersion == 1)
        {
            // V1 response - simple structure
            var v1Response = new
            {
                Version = "1.0",
                Summary = "Basic device summary",
                TotalDevices = 5,
                AverageVolume = GenerateDevices().Average(d => d.Dimension.Volume),
                AvailableLocations = new[] { "Factory A", "Warehouse B", "Office C" }
            };

            return Ok(v1Response);
        }
        else
        {
            // V2 response - enhanced structure
            var devices = GenerateDevices().ToList();
            var v2Response = new
            {
                Version = "2.0",
                Summary = "Enhanced device summary with detailed analytics",
                Statistics = new
                {
                    TotalDevices = devices.Count,
                    AverageVolume = devices.Average(d => d.Dimension.Volume),
                    MinVolume = devices.Min(d => d.Dimension.Volume),
                    MaxVolume = devices.Max(d => d.Dimension.Volume),
                    VolumeRange = devices.Max(d => d.Dimension.Volume) - devices.Min(d => d.Dimension.Volume)
                },
                Locations = new[]
                {
                    new { Name = "Factory A", Region = "North", DeviceCount = 25 },
                    new { Name = "Warehouse B", Region = "South", DeviceCount = 18 },
                    new { Name = "Office C", Region = "East", DeviceCount = 12 }
                },
                Metadata = new
                {
                    AnalysisDate = DateTime.UtcNow,
                    DataQuality = "Premium",
                    MonitoringAccuracy = 98.5
                }
            };

            return Ok(v2Response);
        }
    }

    #endregion Versioning

    #region Internal and External APIs
    // -------------------------------------------------------------
    // Internal and External API Endpoints
    // These endpoints demonstrate different access levels using tags for documentation filtering
    // -------------------------------------------------------------

    /// <summary>
    /// Get internal device analytics data (Internal API only).
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to internal systems only and provides detailed analytics data
    /// that should not be exposed to external clients. Uses "Internal" tag for documentation filtering.
    /// </remarks>
    /// <returns>Detailed internal device analytics and system metrics.</returns>
    /// <response code="200">Returns internal analytics data.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    /// <response code="403">Forbidden if the client lacks appropriate access privileges.</response>
    [HttpGet("internal/analytics")]
    [Tags("Internal")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [SwaggerOperation(
        Summary = "Get internal device analytics (Internal API)",
        Description = "Restricted endpoint that provides detailed device analytics for internal systems only."
    )]
    [Produces("application/json")]
    public IActionResult GetInternalAnalytics()
    {
        var internalData = new
        {
            AccessLevel = "Internal",
            Message = "This is internal-only device analytics data",
            SystemMetrics = new
            {
                ServerLoad = Random.Shared.Next(10, 90),
                DatabaseConnections = Random.Shared.Next(50, 200),
                CacheHitRatio = Math.Round(Random.Shared.NextDouble() * 100, 2),
                LastUpdated = DateTime.UtcNow
            },
            DetailedDevices = GenerateDevices().Select(d => new
            {
                d.Name,
                d.DeviceType,
                d.Dimension,
                d.Dimension.Volume,
                // Internal-only data
                SerialNumber = $"SN{Random.Shared.Next(10000, 99999)}",
                FirmwareVersion = $"v{Random.Shared.Next(1, 5)}.{Random.Shared.Next(0, 9)}.{Random.Shared.Next(0, 9)}",
                ManufacturingDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365)),
                InternalStatus = Random.Shared.Next(0, 2) == 1 ? "Operational" : "Maintenance Required",
                ProcessingTime = Random.Shared.Next(10, 100) + "ms"
            }),
            InternalNotes = new[]
            {
                "All devices are within operational parameters",
                "Scheduled maintenance completed last week",
                "Monitoring system running optimally"
            }
        };

        return Ok(internalData);
    }

    /// <summary>
    /// Get public device summary (External API).
    /// </summary>
    /// <remarks>
    /// This endpoint is available to external clients and provides a clean, 
    /// public-facing device data without sensitive internal information.
    /// Uses "External" tag for documentation filtering.
    /// Available in both V1 and V2 for external client compatibility.
    /// </remarks>
    /// <returns>Public device data suitable for external consumption.</returns>
    /// <response code="200">Returns public device data.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    [HttpGet("external/devices")]
    [Tags("External")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Get public devices (External API)",
        Description = "Public endpoint that provides device data for external clients and third-party integrations."
    )]
    [Produces("application/json")]
    public IActionResult GetExternalDevices()
    {
        var externalData = new
        {
            AccessLevel = "External",
            Message = "Public device data for external clients",
            ApiVersion = "1.0",
            Devices = GenerateDevices().Select(d => new
            {
                Name = d.Name,
                Type = d.DeviceType.Name,
                Dimensions = new
                {
                    Width = d.Dimension.Width,
                    Height = d.Dimension.Height,
                    Depth = d.Dimension.Depth
                },
                Volume = d.Dimension.Volume,
                Status = GetDeviceStatus(d.Name)
            }),
            Metadata = new
            {
                Provider = "MyDevice API",
                UpdateFrequency = "Every 15 minutes",
                Coverage = "Global",
                Accuracy = "Standard precision"
            },
            Links = new
            {
                Documentation = "/swagger",
                Support = "https://api.mydevice.com/support"
            }
        };

        return Ok(externalData);
    }

    /// <summary>
    /// Helper method to get device status based on name.
    /// </summary>
    /// <param name="deviceName">Device name.</param>
    /// <returns>Status indicator for the device.</returns>
    private static string GetDeviceStatus(string? deviceName) => deviceName?.ToLower() switch
    {
        var n when n?.Contains("sensor") == true => "🟢 Online",
        var n when n?.Contains("gateway") == true => "🔵 Connected",
        var n when n?.Contains("monitor") == true => "🟡 Monitoring",
        var n when n?.Contains("control") == true => "🟠 Active",
        var n when n?.Contains("safety") == true => "🔴 Alert",
        _ => "⚪ Unknown"
    };

    #endregion Internal and External APIs

    /// <summary>
    /// Generate a list of sample devices.
    /// </summary>
    /// <remarks>
    /// This method generates a random list of sample devices.
    /// </remarks>
    /// <returns>A list of devices.</returns>
    private static IEnumerable<Device> GenerateDevices()
    {
        var deviceTypes = new[]
        {
            new DeviceType(1, "Sensor"),
            new DeviceType(2, "Controller"),
            new DeviceType(3, "Display"),
            new DeviceType(4, "Gateway"),
            new DeviceType(5, "Monitor")
        };

        return Enumerable.Range(1, 5).Select(index => new Device
        (
            DeviceNames[Random.Shared.Next(DeviceNames.Length)],
            deviceTypes[Random.Shared.Next(deviceTypes.Length)],
            new Dimension(
                Math.Round(Random.Shared.NextDouble() * 50 + 5, 2), // Width: 5-55 cm
                Math.Round(Random.Shared.NextDouble() * 30 + 3, 2), // Height: 3-33 cm
                Math.Round(Random.Shared.NextDouble() * 20 + 2, 2)  // Depth: 2-22 cm
            )
        ))
        .ToArray();
    }
}

/// <summary>
/// Represents a device.
/// </summary>
public record Device(
    string Name,
    [property: SwaggerSchema(Description = "The type of the device")]
    DeviceType DeviceType,
    [property: SwaggerSchema(Description = "The physical dimensions of the device")]
    Dimension Dimension);

/// <summary>
/// Represents a device type.
/// </summary>
public record DeviceType(
    [property: SwaggerSchema(Description = "The unique identifier of the device type")]
    int Id,
    [property: SwaggerSchema(Description = "The name of the device type")]
    string Name);

/// <summary>
/// Represents the physical dimensions of a device.
/// </summary>
public record Dimension(
    [property: SwaggerSchema(Description = "The width of the device in centimeters")]
    [System.ComponentModel.DataAnnotations.Range(0.1, 1000, ErrorMessage = "Width must be between 0.1 and 1000 cm.")]
    double Width,
    [property: SwaggerSchema(Description = "The height of the device in centimeters")]
    [System.ComponentModel.DataAnnotations.Range(0.1, 1000, ErrorMessage = "Height must be between 0.1 and 1000 cm.")]
    double Height,
    [property: SwaggerSchema(Description = "The depth of the device in centimeters")]
    [System.ComponentModel.DataAnnotations.Range(0.1, 1000, ErrorMessage = "Depth must be between 0.1 and 1000 cm.")]
    double Depth)
{
    /// <summary>
    /// Gets the volume of the device in cubic centimeters.
    /// </summary>
    public double Volume => Width * Height * Depth;
}

/// <summary>
/// Request model for generating a custom device.
/// </summary>
public class CustomDeviceRequest
{
    /// <summary>
    /// The custom name for the device.
    /// </summary>
    public string Name { get; set; } = string.Empty;
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

