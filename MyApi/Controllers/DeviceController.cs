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
[Route("v{version:apiVersion}/device")]
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

    #region Header Details

    /// <summary>
    /// Retrieves a sample resource with custom headers.
    /// </summary>
    /// <remarks>
    /// This endpoint demonstrates how custom headers appear in Swagger documentation.
    /// </remarks>
    /// <param name="xCustomHeader">A custom header for demonstration purposes.</param>
    /// <returns>A sample response string.</returns>
    [HttpGet("custom-header")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get with custom header",
        Description = "Returns a message after receiving a custom header"
    )]
    public IActionResult GetWithCustomHeader(
        [FromHeader(Name = "X-Custom-Header")]
        [SwaggerParameter("Custom header value to be passed in the request", Required = true)]
        string xCustomHeader
    )
    {
        var result = _deviceService.ProcessCustomHeader(xCustomHeader);
        return Ok(new { message = result });
    }

    #endregion Header Details

    #region API Protection

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    public IActionResult GetPrivate()
    {
        var username = User.Identity?.Name ?? "No name provided";
        var response = _deviceService.GetPrivateDevices(username);
        return Ok(response);
    }
    
    #endregion API Protection

    #region Deprecation Notes

    /// <summary>
    /// Get devices.
    /// </summary>
    /// <remarks>
    /// This endpoint is deprecated. Please use <c>/devices/public</c> instead.
    /// </remarks>
    /// <returns>A list of devices.</returns>
    /// <response code="410">Gone - This endpoint is deprecated.</response>
    [HttpGet]
    [Obsolete("This endpoint is deprecated. Please use /devices/public instead.")]
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

    #region Request Body and Response Schemas
    
    /// <summary>
    /// Create a custom device.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to generate a custom device by providing the complete Device.
    /// </remarks>
    /// <param name="request">The request body containing the custom Device.</param>
    /// <returns>The generated device.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))] // The "Type" parameter specifies the type of the response body.
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))] // The "Type" parameter specifies the type of the error response body.
    [Produces("application/json")]
    public IActionResult CreateCustomDevice([FromBody] Device request)
    {
        var device = _deviceService.CreateCompleteDevice(request);
        if (device == null)
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }

        return Ok(device);
    }

    #endregion Request Body and Response Schemas

    #region Query Parameters with Default Value

    /// <summary>
    /// Create a device with additional parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to create a device by providing additional parameters such as location and status.
    /// </remarks>
    /// <param name="request">The request body containing the Device details.</param>
    /// <param name="location">The location for the device.</param>
    /// <param name="status">The status of the device (0=Offline, 1=Online).</param>
    /// <returns>The generated device with additional details.</returns>
    [HttpPost("with-params")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public IActionResult CreateDeviceWithParams(
        [FromBody] Device request,
        [FromQuery] string location,
        [FromQuery] [DefaultValue(0)] int status)
    {
        var device = _deviceService.CreateDeviceWithParameters(request, location, status);
        if (device == null)
        {
            return BadRequest(new { message = "Request object or Name cannot be null or empty." });
        }

        return Ok(device);
    }

    #endregion Query Parameters


    #region Path Parameters => ? Restrictions and Default Values

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
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Delete a device by ID and date",
        Description = "Deletes a device based on the provided ID and date. The date must be in the format yyyy-MM-dd."
    )]
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

    #endregion Path Parameter Restrictions and Default Values

    #region API Versioning

    /// <summary>
    /// Get device.
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
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get devices (V1 only)",
        Description = "This endpoint is exclusive to API version 1.0 and returns basic device data."
    )]
    public IActionResult GetDevicesV1Only()
    {
        var response = _deviceService.GetDevicesV1();
        return Ok(response);
    }

    /// <summary>
    /// Get advanced device data.
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
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get advanced devices (V2 only)",
        Description = "This endpoint is exclusive to API version 2.0 and returns enhanced device data with additional features."
    )]
    public IActionResult GetDevicesV2Only()
    {
        var response = _deviceService.GetDevicesV2();
        return Ok(response);
    }

    /// <summary>
    /// Get device summary (Available in both V1 and V2).
    /// </summary>
    /// <remarks>
    /// This endpoint is available in both API versions to exemplify adding an endpoint to multiple API versions.
    /// </remarks>
    /// <returns>Device summary data.</returns>
    /// <response code="200">Returns device summary data.</response>
    [HttpGet("summary")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get device summary (V1 and V2)",
        Description = "This endpoint is available in both V1 and V2."
    )]
    public IActionResult GetDeviceSummary()
    {
        return Ok(_deviceService.GetDeviceSummary());
    }

    #endregion API Versioning

    #region External and Internal APIs

    /// <summary>
    /// Get device analytics data (Internal API only).
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to internal systems and provides detailed analytics data
    /// that should not be exposed to external clients. Uses "Internal" tag for documentation filtering.
    /// </remarks>
    /// <returns>Detailed device analytics and system metrics.</returns>
    /// <response code="200">Returns analytics data.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    /// <response code="403">Forbidden if the client lacks appropriate access privileges.</response>
    [HttpGet("analytics")]
    [Tags("Internal")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get internal device analytics (Internal API)",
        Description = "Restricted endpoint that provides detailed device analytics for internal systems only."
    )]
    public IActionResult GetInternalAnalytics()
    {
        return Ok(null);
    }

    /// <summary>
    /// Get device summary (External API).
    /// </summary>
    /// <remarks>
    /// This endpoint is available to external clients and provides a clean, 
    /// public-facing device data without sensitive internal information.
    /// Uses "External" tag for documentation filtering.
    /// </remarks>
    /// <returns>Public device data suitable for external consumption.</returns>
    /// <response code="200">Returns public device data.</response>
    /// <response code="401">Unauthorized if the token is missing or invalid.</response>
    [HttpGet("devices-summary")]
    [Tags("External")]
    [Authorize(Policy = "ApiScope")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get public devices (External API)",
        Description = "Public endpoint that provides device data for external clients and third-party integrations."
    )]
    public IActionResult GetExternalDevices()
    {
        return Ok(null);
    }

    #endregion External and Internal APIs
}