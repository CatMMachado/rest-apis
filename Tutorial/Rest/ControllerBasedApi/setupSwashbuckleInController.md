<!-- markdownlint-disable MD029 -->

# Swashbuckle Configuration for Controller-Based APIs

## Overview

This document provides comprehensive guidance for configuring Swashbuckle in controller-based ASP.NET Core API projects. Controller-based APIs utilize a structured approach with service extensions and region-based code organization, differing from minimal APIs where Swagger configuration is typically embedded directly in `Program.cs`.

**Note**: All code examples and implementations referenced in this document are available in the repository under the `MyApi` project. Implementation should follow the patterns demonstrated in the repository.

## Repository Structure

The repository implements a structured approach utilizing the following key files:

- **`Program.cs`**: Main application configuration file organized into clearly defined regions
- **`Extensions/SwaggerServiceExtensions.cs`**: Contains the `AddCustomSwagger()` extension method for Swagger configuration
- **`Controllers/WeatherForecastController.cs`**: Example controller demonstrating Swashbuckle annotations
- **`MyApi.csproj`**: Project file containing package definitions

### Code Organization with Regions

The codebase employs `#region` blocks to organize functionality:

- **Service Registration**: Contains all service configurations including Swagger setup
- **Setup for API Specification**: Dedicated region for API documentation configuration
- **Middleware Configuration**: Contains both infrastructure and API specification middleware
- **Configure Endpoints**: Endpoint mapping configuration

This organization facilitates the location and understanding of different aspects of the application setup.

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

2. Verify that the packages have been added to the `<AppName>.csproj` file by referencing the repository's `MyApi.csproj` file.

## Architecture: Centralized Configuration Approach

Controller-based APIs do not contain explicit Swashbuckle configuration lines scattered throughout the controller files. This architectural decision provides several benefits:

- **Centralized Configuration**: All Swashbuckle setup is centralized in the `SwaggerServiceExtensions.cs` file and registered in `Program.cs`
- **Attribute-Based Documentation**: Controllers utilize declarative attributes (such as `[SwaggerOperation]`, `[SwaggerResponse]`) rather than imperative configuration
- **Clean Separation**: This approach separates API documentation concerns from business logic, improving code maintainability

Controller files focus on API logic while utilizing Swashbuckle annotations to provide metadata for documentation generation.

## Swagger Configuration

1. Service registration configuration:

- **Implement SwaggerServiceExtensions**: Copy the contents of `SwaggerServiceExtensions.cs` from the repository's `MyApi/Extensions/` folder to the target project
- **Register the extensions**: Add the `AddCustomSwagger()` call to the `Program.cs` file as demonstrated in the `Setup for API Specification` region of the repository's `Program.cs`

2. Development environment configuration:

- **Implement middleware configuration**: Copy the Swagger middleware setup from the `Middleware Configuration` region in the repository's `Program.cs`, adapting the API names as required for the specific application
- **Configuration details**: This section defines URLs for API specification files in both YAML and JSON formats, and configures multiple API versions with internal/external documentation variants

### Configuration Regions

The repository's `Program.cs` contains the following key regions:

- **`#region Setup for API Specification`**: Contains the `builder.Services.AddCustomSwagger()` call that registers all Swagger services
- **`#region Middleware Configuration`**: Contains the conditional Swagger middleware setup for development environments, including:
  - `app.UseSwagger()`: Enables API specification endpoint generation
  - `app.UseSwaggerUI()`: Configures the interactive Swagger UI with multiple API versions

## XML Comments Configuration

To enable XML comments in the application, add the following configuration to the `PropertyGroup` section of the `<AppName>.csproj` file (this configuration is demonstrated in the repository's `MyApi.csproj`): 

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

This configuration enables Swashbuckle to include XML documentation comments from controllers and models in the generated API documentation.

## Documentation Generation Verification

Upon completion of the configuration steps, the application should generate an API document following the OpenAPI specification, incorporating comprehensive information based on the implemented configuration.

After running the application, navigate to `http://localhost:5000/swagger/index.html` (adjust the port as required for the specific application), and review the Swagger UI to verify the content of the API specification file.

### Generated Documentation Structure

The implemented configuration generates multiple API documentation versions:

- **Internal API versions**: Complete documentation with all endpoints for internal team use
- **External API versions**: Filtered documentation for client delivery and external consumers

To review the content of the generated specification files:
- YAML format: `http://localhost:5000/swagger/v1/swagger.yaml` 
- JSON format: `http://localhost:5000/swagger/v1/swagger.json`

(Adjust the port and version as required based on the specific configuration)

The implementation of Swashbuckle using the demonstrated configuration enables the extension of the API specification to comply with established API guidelines, generating clear, interactive, and well-structured API documentation.

For additional guidance on implementing API guidelines in controller-based APIs, refer to the [API Guidelines in Controller-Based APIs](ControllerBasedApi/guidelinesInController.md#introduction) section.
