using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;
using MyApi.Services;
using MyApi.Models;

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
    private readonly IDeviceService _deviceService;

    /// <summary>
    /// Initializes a new instance of the DeviceController.
    /// </summary>
    /// <param name="deviceService">The device service for handling business logic.</param>
    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

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
        var message = _deviceService.ProcessCustomHeader(xCustomHeader);
        return Ok(message);
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
        var response = _deviceService.GetDevicesWithRateLimit();
        return Ok(response);
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
        var devices = _deviceService.GetDevices();
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
        var response = _deviceService.GetPrivateDevices(username);
        return Ok(response);
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
        var device = _deviceService.GenerateCustomDevice(request);
        if (device == null)
        {
            return BadRequest(new { message = "Name cannot be empty." });
        }

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
        var device = _deviceService.CreateCompleteDevice(request);
        if (device == null)
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }

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
        var device = _deviceService.GenerateDeviceWithParameters(request, location, status);
        if (device == null)
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }

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

        bool deviceExists = _deviceService.ValidateDeviceForDeletion(id, date);
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
        var response = _deviceService.GetDevicesV1();
        return Ok(response);
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
        var response = _deviceService.GetDevicesV2();
        return Ok(response);
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
        var majorVersion = requestedVersion?.MajorVersion ?? 1;
        
        var response = _deviceService.GetDeviceSummary(majorVersion);
        return Ok(response);
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
        var response = _deviceService.GetInternalAnalytics();
        return Ok(response);
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
        var response = _deviceService.GetExternalDevices();
        return Ok(response);
    }

    #endregion Internal and External APIs
}