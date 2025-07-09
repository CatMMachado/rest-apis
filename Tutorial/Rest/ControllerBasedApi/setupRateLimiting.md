<!-- markdownlint-disable MD029 -->

# Rate Limiting Configuration for Controller-Based APIs

This guide provides step-by-step instructions for configuring rate limiting in a controller-based ASP.NET Core API project.

*For background information and architectural overview, see the [Controller-based REST API introduction](index.md).*

## Package Installation

1. Install the required package in the API project using the CLI:

    ```bash
    # Rate limiting
    dotnet add package AspNetCoreRateLimit
    ```

2. Verify that the package has been added to your `<AppName>.csproj` file.

    *For reference, a complete example can be found in `MyApi/MyApi.csproj` in the repository.*

## Configuration Setup

### Add Rate Limiting Configuration

Add rate limiting configuration to your `appsettings.json`:

```json
{
  "IpRateLimiting": {
    // Basic rate limiting settings
    "EnableEndpointRateLimiting": true,     // Enable per-endpoint rate limiting
    "StackBlockedRequests": false,          // Don't queue blocked requests
    "HttpStatusCode": 429,                  // HTTP status code for rate limit exceeded
    
    // IP addresses that bypass rate limiting (localhost for development)
    "IpWhitelist": ["127.0.0.1", "::1"],
    
    // Rate limiting rules
    "GeneralRules": [
      {
        "Endpoint": "*",        // Apply to all endpoints
        "Period": "1m",         // Time window: 1 minute
        "Limit": 100            // Maximum 100 requests per minute
      }
    ]
  }
}
```

*Reference implementation: `MyApi/appsettings.json`*

### Register Services

Add the following to your `Program.cs` in the service registration section:

```csharp
// Configure rate limiting options
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// Register rate limiting services
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Add memory cache for rate limiting
builder.Services.AddMemoryCache();
```

*Reference implementation: `MyApi/Program.cs` in the `Setup for Rate Limiting` region*

## Middleware Configuration

Add the following to your `Program.cs` in the middleware section:

```csharp
// Enable rate limiting (should be one of the first middleware)
app.UseIpRateLimiting();

// ...other middleware...
```

*Reference implementation: `MyApi/Program.cs` in the `Middleware Configuration` region*

## Controller Annotations

Add rate limiting attributes to your controllers or actions:

```csharp
using AspNetCoreRateLimit;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class ExampleController : ControllerBase
{
    [HttpGet]
    [EnableRateLimit(Policy = "CustomPolicy")]
    public IActionResult GetItems()
    {
        // Implementation
    }

    [HttpPost]
    [DisableRateLimit]
    public IActionResult CreateItem()
    {
        // Implementation - rate limiting disabled for this endpoint
    }
}
```

### Rate Limiting Response Headers

The middleware automatically adds response headers:

- `X-RateLimit-Limit`: Request limit per time period
- `X-RateLimit-Remaining`: Remaining requests in current period
- `X-RateLimit-Reset`: Time when the limit resets

## Verification

1. **Run the application**
2. **Make multiple requests** to an endpoint to trigger rate limiting
3. **Verify** that after exceeding the limit, you receive a `429 Too Many Requests` response
4. **Check response headers** for rate limiting information
