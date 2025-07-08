<!-- markdownlint-disable MD029 -->

# Swashbuckle Configuration for Controller-Based APIs

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

## Swagger Configuration

### 1. Service Registration Configuration

- **Implement SwaggerServiceExtensions**: Create a Swagger service extension method:

  ```csharp
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
      }
  }
  ```

*A complete implementation example can be found in `MyApi/Extensions/SwaggerServiceExtensions.cs`*

- **Register the extensions**: Add the extension registration to `Program.cs`, within the service registration region:

  ```csharp
  builder.Services.AddCustomSwagger();
  ```

  *A complete implementation example can be found in `Program.cs`, in `Setup for API Specification` region*

### 2. Development Environment Configuration

Enable Swagger middleware in your `Program.cs` for development environments:

  ```csharp
  #region Middleware Configuration
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
  #endregion
  ```

This configuration dynamically discovers and configures all available API versions. The example above uses YAML format endpoints. To configure JSON format endpoints instead, use:  
  ```csharp
  options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"My API {description.ApiVersion}");
  ```

*A complete implementation example can be found in the `Middleware Configuration` region of `MyApi/Program.cs`*

### Configuration Regions

The `Program.cs` file should contain the following key regions:

- **`#region Setup for API Specification`**: Contains the `builder.Services.AddCustomSwagger()` call that registers all Swagger services
- **`#region Middleware Configuration`**: Contains the conditional Swagger middleware setup for development environments, including:
  - `app.UseSwagger()`: Enables API specification endpoint generation
  - `app.UseSwaggerUI()`: Configures the interactive Swagger UI with multiple API versions

*Reference implementation can be found in `MyApi/Program.cs`*

## XML Comments Configuration

To enable XML comments in the application, add the following configuration to the `PropertyGroup` section of the `<AppName>.csproj` file: 

  ```xml
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  ```

This configuration enables Swashbuckle to include XML documentation comments from controllers and models in the generated API documentation.

*Reference implementation can be found in `MyApi/MyApi.csproj`*

## Documentation Generation Verification

Upon completion of the configuration steps, the application should generate an API document following the OpenAPI specification, incorporating comprehensive information based on the implemented configuration.

After running the application, navigate to `http://localhost:5000/swagger/index.html` (adjust the port as required for the specific application), and review the Swagger UI to verify the content of the API specification file.

### Generated Documentation Structure

The implemented configuration generates multiple API documentation versions:

To review the content of the generated specification files:
- YAML format: `http://localhost:5000/swagger/v1/swagger.yaml` 
- JSON format: `http://localhost:5000/swagger/v1/swagger.json`

(Adjust the port and version as required based on the specific configuration)

The implementation of Swashbuckle using the demonstrated configuration enables the extension of the API specification to comply with established API guidelines, generating clear, interactive, and well-structured API documentation.

For additional guidance on implementing API guidelines in controller-based APIs, refer to the [API Guidelines in Controller-Based APIs](ControllerBasedApi/guidelinesInController.md#introduction) section.
