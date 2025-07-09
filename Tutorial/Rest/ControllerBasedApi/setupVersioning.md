# Setup Guide: API Versioning in Controller-Based API

This guide provides step-by-step instructions to configure API versioning in your controller-based ASP.NET Core API.

For theoretical background and concepts, see [index.md](./index.md). For a complete implementation example, refer to the [example repository](https://github.com/your-repo/ConstructorBasedRestAPI).

## Prerequisites

- ASP.NET Core 8.0 project with controllers
- Basic understanding of RESTful APIs

## Step 1: Install Required Package

Add the API versioning package to your project:

```bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

## Step 2: Configure Versioning Services

In `Program.cs`, add versioning configuration to the service collection:

```csharp
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add controllers first
builder.Services.AddControllers();

// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    // Support multiple versioning strategies
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),           // /v1/products
        new QueryStringApiVersionReader("version"), // ?version=1.0
        new HeaderApiVersionReader("X-Version")     // X-Version: 1.0
    );

    // Default version when none specified
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Include version info in response headers
    options.ReportApiVersions = true;

}).AddApiExplorer(options =>
{
    // Group name format for Swagger
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();
```

## Step 3: Apply Versioning to Controllers

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new { version = "1.0", message = "Products from V1" });
    }
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new { version = "2.0", message = "Products from V2" });
    }
}
```

## Step 4: Configure Swagger for Multiple Versions

Ensure your Swagger configuration supports multiple API versions. If you haven't set up Swagger yet, follow the [Swagger Setup Guide](./setupSwashbuckleInController.md) which includes proper versioning support.

### Register Services

In `Program.cs`, add Swagger services (if not already configured):

```csharp
// In Program.cs - Service registration
builder.Services.AddCustomSwagger();
```

*For the complete Swagger extension method implementation, see [setupSwashbuckleInController.md](./setupSwashbuckleInController.md#swagger-configuration)*

## Step 5: Verify Configuration

1. **Check Swagger UI**: Multiple version dropdowns should appear
2. **Test API responses**: Should include version headers:
   ```
   api-supported-versions: 1.0, 2.0
   ```
3. **Validate routing**: All versioning strategies should work