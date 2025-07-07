<!-- markdownlint-disable MD029 -->

# Setup and Run Swashbuckle

## Install Swashbuckle and other required packages

1. In your API project, install the required packages via the CLI:

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

2. Confirm that the packages were added to your `<AppName>.csproj`.

## Configure Swagger

1. To configure and register Swagger in your application:

- Add the contents of `SwaggerServiceExtension.AddCustomSwagger()` to your repository
- Integrate these extensions into your application as shown in section `API Specification Setup` of `Program.cs`

2. To develop and test the API documentation, enable Swagger and he Swagger UI in the development environment, as shown in section `Middleware Configuration` of `Program.cs`.
In this secttion you can see that an URL is being defined to show the API specification files, both in yaml and json format.

## Enable XML Comments for Endpoint Documentation

To enable XML comments in your application, add the following line to the `PropertyGroup` section of your `<AppName>.csproj`: `<GenerateDocumentationFile>true</GenerateDocumentationFile>`.

## Test Documentation Generation

At this point, you should be able to run your application and generate an API document following the OpenAPI specification, with a minimum of information, based on the code in your repository.

After running your application, go to `http://localhost:5000/swagger/index.html` (if needed, adjust to the port selected for the application), and check in the Swagger UI the content currently in yout API specification file.

To check the content of you actual specification files, go to `http://localhost:5000/swagger/v1/swagger.yaml` for the yaml file and to `http://localhost:5000/swagger/v1/swagger.json` for the json file (as before, adjust the port if needed).

Now that you have Swashbuckle up and running in your repository, you can start extending your current API specification, so that it complies with your company's [API guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines), generating clear, interactive, and well-structured API documentation.

Follow to the section [The API Guidelines in a controller-based API](ControllerBasedApi/guidelinesInController.md#introduction) to learn how.
