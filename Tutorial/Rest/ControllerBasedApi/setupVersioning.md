<!-- markdownlint-disable MD029 -->

# Setup Guide: API Versioning

> **⚠️ DEMONSTRATION PURPOSES ONLY**
> 
> **The Asp.Versioning packages used in this guide are for DEMONSTRATION PURPOSES ONLY. This is NOT a mandatory requirement for controller-based APIs.**
> 
> **This example shows how API versioning can be integrated and documented in OpenAPI specifications. You can choose any versioning strategy that fits your needs, use custom URL patterns, header-based versioning, or skip versioning entirely if not required for your use case.**

This guide provides step-by-step instructions to configure API versioning in your controller-based ASP.NET Core API.

For theoretical background and concepts, see [index.md](./index.md). For a complete implementation example, refer to the [example repository](https://github.com/your-repo/ConstructorBasedRestAPI).

## Step 1: Install Required Package

Install the API versioning packages for ASP.NET Core.

```bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

*Reference: Use `dotnet add package` commands for required packages*

## Configure Services

Configure API versioning with multiple reader strategies (URL segment, query string, headers) and API explorer settings in `Program.cs`.

*Reference implementation: `MyApi/Program.cs`*

## Version Controllers

Add version attributes and routing to your controllers to support multiple API versions.

*Reference implementation: `MyApi/Controllers/WeatherForecastController.cs`*

## Swagger Integration

Ensure Swagger configuration supports multiple API versions for documentation.

*Reference: Follow the [Swagger Setup Guide](./setupSwashbuckleInController.md) which includes versioning support*

*Complete example: `MyApi/Program.cs` in the repository.*