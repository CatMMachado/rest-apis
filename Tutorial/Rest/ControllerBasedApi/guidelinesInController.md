# The API Guidelines in a controller-based API

## Introduction

The goal of this section is to assist you in using Swashbuckle in a controller-based API to generate an API specification as complete as possible, given your company's recommendations at [API guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines).

An important distinction made in that repository is that an "API Documentation" refers to the full documentation of a service, which must contain "API specifications" for each major API version offered by that service [1], in addition to other parts.

Since the focus of this tutorial is on the "API specification", in this section we will focus on the components referenced in the API Guidelines that MUST and SHOULD be added to your [API specification](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-specification.md).

However, and for the sake of completion, we have included components referred in the guidelines in other sections as well, namely:

- [Common API Guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/nav.md) (inside which are included the API Specification guidelines)
- [REST API Guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/rest/nav.md)

We recommend that you read the guidelines before moving on.

The intended use of this section is as a quick guide on how to use Swashbuckle to assist in the implementation of the API Guidelines.
Please note that not all of the guidelines will be reflected here: some are more related to the API design and have no direct visibility on the API specification, and others can be included by adding some approach already explained.

Before proceeding, Swashbuckle should already be setup in your repository. If haven't done it yet, please follow the instructions in the section [Setup and Run Swashbuckle](./setupSwashbuckleInController.md).

As before, we continue to use the repository [ControllerBasedRestApi](**ADD LINK**) as the main source of code for this part of the tutorial.

## Common API Guidelines

### API Versioning

To be able to generate an API document for a specific version, you need to have the API versioning setup in your repository.

In the auxiliary repository [ControllerBasedRestApi](**ADD LINK**) there is an example of setup, explained [here](**ADD LINK**). This setup allows the management of multiple versions in the API, which means there can exist controllers and methods visble in different versions of the API, representing the evolution of the repository.
Consider the scenario that best resembles your reality.

To guarantee that you generate a document for your version, you need to:

- Have the attribute `ApiVersion` in the controllers. Example:

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class DeviceController : ControllerBase
```

In the example above, the `DeviceController` supports both v1 and v2, and the applicable endpoints will be visible in both documents.

- Have the actions annotated with the attribute `MapToApiVersion`. Example:

```csharp
[HttpGet("v2-only")]
[MapToApiVersion("2.0")]
public IActionResult GetDeviceV2Only()
```

In the example above, the endpoint `GetDeviceV2Only` will just be visible in the API specification of v2. The document for v1 will not contain it.

If all the endpoints are part of a single version, e.g., v1, you just need to annotate the controller with that version, and all the endpoints are assumed to be part of that version.

If you follwed the instructions in the section [Setup and Run Swashbuckle](./setupSwashbuckleInController.md), you should be using the versions captured by the API Provider, and thus have Swashbuckle prepared to:

- Generate the required API documents automatically from the version attributes defined in the previous points
- Expose the created file(s) in the Swagger UI

The relevant sections in `Setup and Run Swashbuckle` are:

- **Ponto do setup do generate document**
- **Development Environment Configuration**

For a working example in the [ControllerBasedRestApi](**ADD LINK**) repository check the following:

- `DeviceController`: the attributes with each it is annotated
- In the same controller:
  - Region `API Versioning` for examples of endpoints belonging to different API versions.   Please note that the remainder endpoints, with no version attribute, will be present in all the API documents generated.

### API Specification

#### OpenAPI specification version 3

The [API Specification guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-specification.md) state the following:

- "For synchronous HTTP-based APIs, OpenAPI specifications of at least version 3 MUST be used".

OpenAPI version 3 is provided by default by Swashbuckle, so there is no need for additional steps to comply with this request.

#### Specification file in YAML format

- "The API specifications SHOULD be provided in YAML format for consistency reasons."

Swahbuckle creates both YAML and JSON files under the hood, and you can control the visibility of these files in the local environment, as shown in the [ControllerBasedRestApi](**ADD LINK**), in the following regions:

- `Middleware configuration`, specifically under `API Specification`
  - The line `app.UseSwagger` enables the visualization of API documentation in endpoints such as `/swagger/v1/swagger.yaml`, from where it can be downloaded
  - The line `options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml", $"My API {description.ApiVersion}");` enables the visualization of the documentation in Swagger UI

- `With the download of the file`
  - (...)

The examples provided work both for YAML and JSON, it is just a matter of replacing `.yaml` with `.json` in the case you want to control the visibility of the JSON document.

#### External and internal APIs

#### API protection

#### Request and Response schemas

In terms of API specification content, the following list shows the required elements:

- fully defined request and response
- header, query, and path parameter details including allowed values and default values
- details on error response status codes, error schemas, error types, and a clear association of errors to operations
- restrictions with respect to format, character or number range of parameters and properties
- restrictions with respect to number of array items or additional properties
- examples for all parameters and request/response bodies
- descriptions and summaries SHOULD contain meaningful additional information to already existing information such as the names of properties, parameters, or operations
- limits which are dependent on the used service plan, acquired quota, the service region or deployment environment (MAY be mentioned in the API specification)
- deprecation notes

For each of these elements, we show below how to add them to your repository.

**External & internal is here**

#### Request and response definition

(...)

---
#region External and Internal APIs
    // -------------------------------------------------------------
    // These endpoints demonstrate different access levels using tags for documentation filtering
    // -------------------------------------------------------------

## API Specification Foundation

### Basic Documentation Requirements

- [x] **API specifications MUST include or reference all required syntactic information for client developers.**  
  _Located in: SwaggerServiceExtensions.cs, WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **A minimum level of semantic information SHOULD be included so that client developers with domain knowledge understand the API's purpose and usage.**  
  _Located in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

---

## Documentation Content Requirements

### Schema Definitions & Parameter Documentation

- [x] **Fully defined request and response schemas MUST be provided.**  
  _Located in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on header, query, and path parameters, including allowed values and defaults, MUST be documented.**  
  _Located in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on error response codes, schemas, types, and a clear mapping of errors to operations MUST be provided.**  
  _Located in: WeatherForecastController.cs (`#region Error Response Schema`)_

### Data Validation & Constraints

- [x] **Restrictions on format, character count, or numeric range of parameters or properties SHOULD be specified.**  
  _Located in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

- [x] **Restrictions on array item counts or additional properties SHOULD be defined.**  
  _Located in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

### Examples & Clarity

- [x] **Examples for all parameters and request/response bodies SHOULD be provided.**  
  _Located in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Descriptions and summaries SHOULD provide meaningful information beyond names.**  
  _Located in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

### Service Information

- [x] **Limits based on service plan, quota, region, or environment MAY be mentioned, but actual values SHOULD NOT be included.**  
  _Located in: Program.cs, WeatherForecastController.cs (`#region Service Usage Limits`)_

### Deprecation Management

- [x] **Deprecation notes MUST be added according to API lifecycle phases.**  
  _Located in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **OpenAPI/AsyncAPI description and deprecated properties SHOULD be used if applicable.**  
  _Located in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **Deprecation notes SHOULD indicate which alternative API or element should be used instead.**  
  _Located in: WeatherForecastController.cs (`#region Deprecation Notes`)_

### What Can Be Deprecated

- [x] **For HTTP APIs: endpoints, methods, parameters, supported media types, or parts of request/response schemas MAY be deprecated.**  
  _Located in: WeatherForecastController.cs (`#region Deprecation Notes`)_

---

## Performance & Rate Limiting

### API Rate Limiting & Throttling

- [x] **Rate limiting policies SHOULD be implemented to protect API resources and ensure fair usage.**  
  _Located in: Program.cs (AddInMemoryRateLimiting), appsettings.json (IpRateLimiting configuration)_

- [x] **Rate limiting information SHOULD be exposed through response headers.**  
  _Located in: WeatherForecastController.cs (X-RateLimit-Limit, X-RateLimit-Remaining headers documentation)_

- [x] **Different rate limiting strategies SHOULD be available for different endpoint types.**  
  _Located in: WeatherForecastController.cs ([EnableRateLimiting("fixed")] attribute)_

- [x] **Rate limiting errors (429 Too Many Requests) MUST be properly documented.**  
  _Located in: WeatherForecastController.cs (ProducesResponseType(StatusCodes.Status429TooManyRequests))_

---

## Technical Implementation Standards

### Content Type & Media Type Support

- [x] **Supported content types MUST be explicitly defined for each endpoint.**  
  _Located in: WeatherForecastController.cs ([Produces], [Consumes] attributes)_

- [x] **Default content types SHOULD be specified when multiple formats are supported.**  
  _Located in: WeatherForecastController.cs ([Produces("application/json")] attributes)_

### XML Documentation Integration

- [x] **XML documentation generation SHOULD be enabled for comprehensive API documentation.**  
  _Located in: MyApi.csproj (GenerateDocumentationFile=true), SwaggerServiceExtensions.cs (XML comments integration)_

- [x] **XML comments SHOULD be used to provide detailed method and parameter descriptions.**  
  _Located in: WeatherForecastController.cs (/// XML comments), SwaggerServiceExtensions.cs_

---

## Security & Authentication

### OAuth2 & Security Implementation

- [x] **The API MUST be secured using OAuth2 with proper authentication flows documented.**  
  _Located in: SwaggerServiceExtensions.cs (`#region OAuth2 Security Configuration`), IdentityServerConfig.cs_

- [x] **Required scopes for each endpoint MUST be clearly documented in the API specification.**  
  _Located in: WeatherForecastController.cs ([Authorize(Policy = "ApiScope")]), SwaggerServiceExtensions.cs (security requirements)_

- [x] **Authentication flows (Authorization Code, Client Credentials, etc.) SHOULD be documented with examples.**  
  _Located in: SwaggerServiceExtensions.cs (ClientCredentials flow configuration), Program.cs (OAuth configuration)_

### Security Documentation

- [x] **Security schemes MUST be defined in the OpenAPI specification.**  
  _Located in: SwaggerServiceExtensions.cs (AddSecurityDefinition "oauth2")_

- [x] **Protected endpoints MUST indicate required authentication and authorization levels.**  
  _Located in: WeatherForecastController.cs ([Authorize] attributes and AddSecurityRequirement in Swagger)_

### Public Access & Anonymous Endpoints

- [x] **Public endpoints that do not require authentication SHOULD be clearly marked and documented.**  
  _Located in: WeatherForecastController.cs ([AllowAnonymous] attributes)_

---

## Implementation Highlights

- **Complete OAuth2 security integration** with Client Credentials flow  
- **Comprehensive rate limiting** with configurable policies and informative headers  
- **Multi-strategy API versioning** supporting URL path, query string, and header-based versioning  
- **Full OpenAPI 3.0+ specification** with YAML format support  
- **XML documentation integration** for enhanced API descriptions  
- **Content type specification** for proper media type handling  
- **Public endpoint support** with clear anonymous access documentation  

--
References:

[1] [API Documentation](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-documentation.md)
