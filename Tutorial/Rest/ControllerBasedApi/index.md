# Controller-based REST API

## Introduction

Controller-based APIs follow the traditional ASP.NET Core MVC pattern, where routes and metadata are defined using attributes on controllers and actions. Swashbuckle integrates seamlessly with this style, offering robust support for:

- Automatic OpenAPI documentation generation
- API versioning support
- Enrichment through XML comments and annotations
- Interactive documentation and testing through Swagger UI

This section demonstrates how to configure Swashbuckle in a project that uses a controller-based API, and how to document endpoints using the [API guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines).

We will beggin by giving an overview of the repository that accompanies this section, [rest-apis](**ADD LINK**).
We recommend that you read these sections, as they will help you better understand what we ask of you later on, when we refer you to the repository get the code to add to your own repository.
However, if you wish to proceed immediately to [setting up Swashbuckle](ControllerBasedApi/setupSwashbuckleInController.md#setup-and-run-swashbuckle) in your repository, you can do so, and later come back here for more detailed explanations.

## Code Regions and Guideline Mapping

The code in the repository is organized using #region blocks to facilitate a clear connection between specific API guidelines, present in this tutorial, and their implementation in the codebase. Each region is labeled according to a relevant topic or guideline area. This structure enables straightforward navigation and searching within the code, allowing readers to quickly identify where and how each guideline is addressed. The region names correspond
directly to guideline topics, making it easier to verify compliance and understand the rationale behind each code section.

For example:

- #region `Error Response Schema` documents the structure of error responses.
- #region Deprecation Notes provides information about deprecated endpoints or features.
- #region Service Usage Limits includes logic and documentation for rate limiting.
- #region API Versioning shows how versioning is configured and applied.

In addition, within files such as Program.cs, regions are further distinguished by their purpose:

- **Infrastructure code**: These regions contain the minimal setup and configuration required to make the example project functional. This code is included solely to support the demonstration of API documentation features and <u>should not be considered a template or best practice for building production projects</u>.
- **API specification code**: These regions focus specifically on the configuration and integration of API documentation tools, such as Swashbuckle and Swagger UI. They demonstrate how to set up and expose API documentation in alignment with the guidelines.

### Purpose of Installing Swagger UI

Providing a visual and interactive representation of each API specification is an essential aspect.
Swagger UI offers an interactive, browser-based interface for API documentation.
Enabling Swagger UI in a project improves visibility, collaboration, and maintainability of APIs by:

- Providing a clear overview of all available API endpoints, including parameters, request and response formats, and authentication requirements.
- Allowing endpoints to be tested directly in the browser, streamlining development workflows and supporting faster debugging and onboarding.
- Ensuring that documentation remains aligned with the implementation, as Swagger UI renders the OpenAPI specification directly from the codebase.
- Centralizing API information in a single, accessible source, reducing ambiguity and improving communication across teams.

Swagger UI transforms static API definitions into an interactive and easily navigable interface, improving both usability and long-term maintainability.

### The *rest-apis* repository

The repository [rest-apis](**ADD LINK**) is an integral part of this tutorial. In it you can find a working example of a C# repository taking advantage of Swashbuckle to generate API documentation.
It is a very simple example, with the single purpose of exemplifying how to use the documentation generation tool.
In it you will find the following components relevant for the API documentation:

- `Controllers/ForecastController`: contains the endpoints that compose the API. It is sectioned in regions, where each regions represents a topic relevant for the API documentation, and present in the [API Guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines).
- `Extensions/AuthorizationServiceExtensions`, `Extensions/IdentityServerServiceExtensions` and `IdentityServerConfig`: contain authorization-related information, relevant for the definition of the API, and some of which will be present in the API documentation.
- `Extensions/SwaggerServiceExtensions`: contains configurations relevant for Swashbuckle and Swagger UI. It includes adding the support for the coexistence of multiple API versions, and enabling the annotations provided by the package `Swashbuckle.AspNetCore.Annotations`.
- `Program.cs`: relevant for the documentation, there are the following regions:
  - `Setup for Rate Limiting`: this is necessary to setup rate limiting in the repository, and it is relevant for the documentation in the sense that the rate limitings you configure in your code, will be reflected in the documentation and Swagger UI.
  - `Setup for Authentication, Authorization, and IdentityServer`: necessary to setup authentication and authorization in the repository, which will be reflected in the documentation and when interacting with Swagger UI.
  - `Setup for API Specification`: this adds to the application the extension methods defined at `Extensions/SwaggerServiceExtensions`, with the first part of the setup for Swashhbuckle and Swagger UI.
  - `API Versioning`: this region adds the necessary configurations to work with different versions of an API. In particular, it adds:
    - Support to 3 different approaches in terms of how the version number is represented: in the API URL, as a query parameter, ans as a header. In your repository, you only have to follow one of these approaches. **ToDo: Do the guidelines refer which approach to follow? Say it here, or just refer that each approach has advantages and disadvantages, but that that is outside of the scope of the tutorial.**
    - A default version, meaning that, unless you indicate otherwise in your endpoints, all of them will be considered as part of this version.
    - The format of the version name, e.g., `v3`.
  - `Middleware Configuration`: this region includes the final steps in setting up Swashbuckle and Swagger UI for the development environment. These steps enabled the visualization of the API documentation files (yaml/json) in the browser, in the URL defined for Swagger, as well as interacting with the documentation in the browser, thus being able to test the endpoints during the development phase.
  - `keys/is-signing-key-<GUID>.json`: stores the cryptographic signing key used by IdentityServer for issuing secure tokens. This file is automatically generated and managed by IdentityServer, and is essential for enabling secure authentication and authorization flows in the API. (It should be protected and not shared outside trusted environments)
  - `appsettings.json`: holds the main configuration settings for the API, including logging levels, allowed hosts, and rate limiting rules. This file centralizes application settings, making it easy to adjust operational parameters such as rate limits, authentication options, and other environment-specific values without changing the code.
  - `MyApi.csproj`: defines the project structure and dependencies for the API. This file lists all NuGet packages required for features such as API versioning, rate limiting, authentication, and OpenAPI/Swagger documentation. It also includes build settings like the target .NET version and enabling XML documentation, which is important for generating enriched API docs with Swashbuckle.

  **ToDo:Is the auth part really working in the UI? From what I recall, no. Consider removing this from Program.cs?**
  **ToDo:Confirm that all this is applicable also in Backstage.**
  
  **Note:**
The setup of Swashbuckle and Swagger UI is distributed across several files in this repository, each with a specific role in the documentation process:

  - The main configuration for Swagger and API documentation generation is located in `Extensions/SwaggerServiceExtensions`. This section explains how to enable support for multiple API versions, add annotations, and configure security settings.
  - The integration of these configurations into the application takes place in `Program.cs`, specifically within the regions labeled `Setup for API Specification` and `Middleware Configuration`. These sections enable Swagger and Swagger UI at runtime, making the documentation accessible through the browser.
  - Additional documentation enrichment is configured in the project file `<AppName>.csproj`. This includes enabling XML comments for controllers and models to enhance the generated documentation.
