# Controller-based REST APIs

**TO DECIDE:**
**- Confirm that the setup is the same for controller-based and minimal APIs, and extract the setting up section.**
**- Depending on how long this section ends up to be, create file for setting up and guideliens.**

## Introduction

Controller-based APIs follow the traditional ASP.NET Core MVC pattern, where routes and metadata are defined using attributes on controllers and actions. Swashbuckle integrates seamlessly with this style, offering robust support for:

- Automatic OpenAPI documentation generation
- API versioning support
- Enrichment through XML comments and annotations

This section demonstrates how to configure Swashbuckle in a project that uses controller-based APIs and versioning, and how to document endpoints using standard practices.

### The *ControllerBasedRestApi* repository

The repository [ControllerBasedRestApi](**ADD LINK**) is an integral part of this tutorial. In it you can find a working example of a C# repository taking advantage of Swashbuckle to generate API documentation.
It is a very simple example, with the single purpose of exemplifying how to use the documentation generation tool.
In it you will find the following components relevant for the API documentation:

- Controllers/ForecastController: contains the endpoints that compose the API. It is sectioned in regions, where each regions represents a topic relevant for the API documentation, and present in the [API Guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines).
- Extensions/AuthorizationServiceExtensions and /IdentityServerServiceExtensions: contain authorization-related information, relevant for the definition of the API, and some of which will be present in the API documentation.
- Extensions/SwaggerServiceExtensions: contains configurations relevant for Swashbuckle, namely the API name (...)



## Setup and Run Swashbuckle

### Install Swashbuckle and other required packages

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

**TO DO:**
**Reorder csproj to be according to this list of packages.**

### Configure Swagger

1. To configure and register Swagger in your application:

- Add the contents of `SwaggerServiceExtension.AddCustomSwagger()` to your repository
- Integrate these extensions into your application as shown in section `API Specification Setup` of `Program.cs`

2. To develop and test the API documentation, enable Swagger and he Swagger UI in the development environment, as shown in section `Configure Middleware` of `Program.cs`.
In this secttion you can see that an URL is being defined to show the API specification files, both in yaml and json format.

### Enable XML Comments for Endpoint Documentation

To enable XML comments in your application, add the following line to the `PropertyGroup` section of your `<AppName>.csproj`: `<GenerateDocumentationFile>true</GenerateDocumentationFile>`.

### Test Documentation Generation

At this point, you should be able to run your application and generate an API document following the OpenAPI specification, with a minimum of information, based on the code in your repository.

After running your application, go to `http://localhost:5000/swagger/index.html` (if needed, adjust to the port selected for the application), and check in the Swagger UI the content currently in yout API specification file.

To check the content of you actual specification files, go to `http://localhost:5000/swagger/v1/swagger.yaml` for the yaml file and to `http://localhost:5000/swagger/v1/swagger.json` for the json file (as before, adjust the port if needed).

### Integrate with Backstage

To see your new API documentation in Backstage, follow the instructions in section [Integration with Backstage](Backstage.md#backstage).

## Swashbuckle and the API Guidelines

This section describes how to use Swashbuckle to extend your current API documentation, so that it complies with your company's [API guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines), generating clear, interactive, and well-structured API documentation.

To keep the explanation simple and focused, we will cover each guideline, highlighting how Swashbuckle (always referring to the accompanying repository), can help.