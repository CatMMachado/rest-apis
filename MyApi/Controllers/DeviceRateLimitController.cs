using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;
using MyApi.Services;

namespace MyApi.Controllers;

/// <summary>
/// Controller for device rate limiting operations.
/// </summary>
[Authorize(Policy = "ApiScope")]
[ApiController]
[Route("v{version:apiVersion}/devices")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class DeviceRateLimitController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    /// <summary>
    /// Initializes a new instance of the DeviceRateLimitController.
    /// </summary>
    /// <param name="deviceService">The device service for handling business logic.</param>
    public DeviceRateLimitController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
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
}
