# Common API Guidelines – Checklist

Each item below represents a guideline. At the end of each item, there's a checkbox to mark whether it is fulfilled (`[x]`) or not (`[ ]`).

## API Specification Type

- [x] For synchronous HTTP-based APIs, OpenAPI specification version 3 or higher MUST be used.
  _Identified in: Program.cs, SwaggerServiceExtensions.cs (`#region Setup for API Specification`)_
- [ ] For asynchronous messaging APIs, AsyncAPI specification version 2 or higher MUST be used.
   _Not applicable (no async API in this project)_

## Format

- [x] API specifications SHOULD be provided in YAML format for consistency reasons.
  _Identified in: Program.cs (`#region Setup for API Specification` and YAML endpoint)_

## API Specification Content

- [x] API specifications MUST include or reference all required syntactic information for client developers.
  _Identified in: SwaggerServiceExtensions.cs, WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_
- [x] A minimum level of semantic information SHOULD be included so that client developers with domain knowledge understand the API’s purpose and usage.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

## Required Content

- [x] Fully defined request and response or message schemas.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_
- [x] Details on header, query, and path parameters, including allowed values and defaults.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_
- [x] Details on error response codes, schemas, types, and a clear mapping of errors to operations.
  _Identified in: WeatherForecastController.cs (`#region Error Response Schema`)_
- [x] Restrictions on format, character count, or numeric range of parameters or properties.
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_
- [x] Restrictions on array item count or additional properties.
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_
- [x] Examples for all parameters and request/response bodies.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_
- [x] Descriptions and summaries SHOULD provide meaningful extra information beyond names.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

## Service Limits

- [x] Limits based on service plan, quota, region, or environment MAY be mentioned, but actual values SHOULD NOT be included.
  _Identified in: Program.cs, WeatherForecastController.cs (`#region Service Usage Limits`)_

## Deprecation Notes

- [x] Deprecation notes MUST be added following the API Lifecycle Phases.
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_
- [x] OpenAPI/AsyncAPI description and deprecated properties SHOULD be used if applicable.
   _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_
- [x] Deprecation notes SHOULD indicate which alternative API or element should be used instead.
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

## What Can Be Deprecated

- [x] For HTTP APIs: endpoints, methods, parameters, supported media types, or parts of request/response schemas MAY be deprecated.
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_
- [ ] For asynchronous messaging APIs: message topics, full messages, or parts of message schemas MAY be deprecated.
  _Not applicable (no async API in this project)_

## API Versioning

- [x] API versioning SHOULD be implemented to enable backward compatibility and smooth transitions between different versions.
  _Identified in: Program.cs (`#region API Versioning`)_
- [x] Multiple versioning strategies SHOULD be supported (URL path, query string, headers).
  _Identified in: Program.cs (`#region API Versioning` - UrlSegmentApiVersionReader, QueryStringApiVersionReader, HeaderApiVersionReader)_
- [x] Version-specific endpoints SHOULD be clearly documented and demonstrated.
  _Identified in: WeatherForecastController.cs (`#region API Versioning`)_
- [x] Swagger documentation SHOULD support multiple API versions with separate documentation for each version.
  _Identified in: SwaggerServiceExtensions.cs, Program.cs (multiple SwaggerDoc configurations)_

**Add Authenticattion**
// Security (OAuth2, Scopes): The API must be secured using OAuth2,
// and the specification must document required scopes and authentication flows.
