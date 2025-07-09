<!-- markdownlint-disable MD029 -->

# Swashbuckle Configuration for Controller-Based APIs

This guide provides step-by-step instructions for configuring Swashbuckle in a controller-based ASP.NET Core API project.

*For background information and architectural overview, see the [Controller-based REST API introduction](index.md).*

## Install Swashbuckle and other required packages

1. Install the required packages in the API project using the CLI:

    ```bash
    # Swashbuckle
    dotnet add package Swashbuckle.AspNetCore

    # Code annotations - required for controller-based APIs
    dotnet add package Swashbuckle.AspNetCore.Annotations

    # Versioning
    # Provides libraries and middleware for API versioning
    dotnet add package Asp.Versioning.Http
    # Extends API versioning support and integrates with API documentation tools like Swagger
    dotnet add package Asp.Versioning.Mvc.ApiExplorer

    # OpenAPI/Swagger capabilities
    dotnet add package Microsoft.AspNetCore.OpenApi
    ```

2. Verify that the packages have been added to your `<AppName>.csproj` file. 

    *For reference, a complete example can be found in `MyApi/MyApi.csproj` in the repository.*

## Swagger Configuration

### Create Swagger Extension Method

Implement Swagger service extension method:

```csharp
using System.Reflection;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Add support for multiple API versions
            using var serviceProvider = services.BuildServiceProvider();
            var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "My API",
                    Version = description.ApiVersion.ToString(),
                    Description = $"API - Version {description.ApiVersion}"
                });
            }
        });

        return services;
    }
}
```

*A complete implementation example can be found in `MyApi/Extensions/SwaggerServiceExtensions.cs`*

### Register Services

Add the following to your `Program.cs` in the service registration section:

```csharp
builder.Services.AddCustomSwagger();
```

*A complete implementation example can be found in `MyApi/Program.cs`, in the `Setup for API Specification` region*

### Swagger and Swagger UI Middleware Configuration

Enable Swagger and Swagger UI middleware in your `Program.cs` for development environments:

```csharp
using Asp.Versioning.ApiExplorer;

...

if (app.Environment.IsDevelopment())
{
    // Enable API specification endpoint generation
    app.UseSwagger();

    // Enable interactive Swagger UI with multiple API versions
    app.UseSwaggerUI(options =>
    {
        // Get API version provider to dynamically configure endpoints
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // Configure endpoints for each API version dynamically
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml", $"My API {description.ApiVersion}");
        }
    });
  }
  ```

**Alternative endpoints:** To use JSON format instead of YAML, replace `.yaml` with `.json` in the endpoint URLs.

*A complete implementation example can be found in `MyApi/Program.cs`, in the `Middleware Configuration` region*

## XML Comments Configuration

To enable XML comments in the application, add the following configuration to the `PropertyGroup` section of the `<AppName>.csproj` file: 

```xml
<PropertyGroup>
    ...
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

This configuration enables Swashbuckle to include XML documentation comments from controllers and models in the generated API documentation.

*A complete implementation example can be found in `MyApi/MyApi.csproj`*

## Verification

1. **Run the application**
2. **Navigate to** `http://localhost:5000/swagger/index.html` (adjust port as needed)
3. **Verify** that the Swagger UI displays your API documentation with multiple versions

### Available Endpoints

- **Swagger UI**: `http://localhost:5000/swagger/index.html`
- **API Specification (YAML)**: `http://localhost:5000/swagger/v1/swagger.yaml`
- **API Specification (JSON)**: `http://localhost:5000/swagger/v1/swagger.json`

*(Adjust the port and version as required based on the specific configuration)*

## Next Steps

For guidance on implementing specific API guidelines using Swashbuckle annotations, see [API Guidelines in Controller-Based APIs](guidelinesInController.md).
