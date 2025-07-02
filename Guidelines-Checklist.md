# Common API Guidelines – Checklist

---

## Documentation Content Requirements

### Syntactic and Semantic Information

- [x] **API specifications MUST include or reference all required syntactic information for client developers.**  
  _Identified in: SwaggerServiceExtensions.cs, WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **A minimum level of semantic information SHOULD be included so that client developers with domain knowledge understand the API's purpose and usage.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

### Schema and Parameter Definitionsiew

This checklist serves as a practical guide for implementing comprehensive API documentation using Swashbuckle/OpenAPI in .NET projects. Each guideline represents industry best practices for creating well-documented, maintainable, and developer-friendly APIs.

Each item below includes:
- **Guideline statement** with priority level (MUST/SHOULD/MAY)
- **Checkbox** to track implementation status (`[x]` fulfilled, `[ ]` pending)
- **Implementation reference** pointing to specific code regions in the companion project

Use this checklist to ensure your API documentation meets professional standards and provides excellent developer experience.

---

## Table of Contents

1. [API Specification Foundation](#api-specification-foundation)
2. [Documentation Content Requirements](#documentation-content-requirements)
3. [API Design & Lifecycle Management](#api-design--lifecycle-management)
4. [Security & Authentication](#security--authentication)

---

## API Specification Foundation

### Specification Type & Format

- [x] **For synchronous HTTP-based APIs, OpenAPI specification version 3 or higher MUST be used.**  
  _Identified in: Program.cs, SwaggerServiceExtensions.cs (`#region API Specification Setup`)_

- [ ] **For asynchronous messaging APIs, AsyncAPI specification version 2 or higher MUST be used.**  
  _Not applicable (no async API in this project)_

- [x] **API specifications SHOULD be provided in YAML format for consistency reasons.**  
  _Identified in: Program.cs (`#region API Specification Setup` and YAML endpoint)_

### Basic Documentation Requirements

- [x] **API specifications MUST include or reference all required syntactic information for client developers.**  
  _Identified in: SwaggerServiceExtensions.cs, WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_
- [x] A minimum level of semantic information SHOULD be included so that client developers with domain knowledge understand the API’s purpose and usage.
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

## Required Content

- [x] **Fully defined request and response or message schemas MUST be provided.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on header, query, and path parameters MUST include allowed values and defaults.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on error response codes, schemas, types, and clear mapping of errors to operations MUST be documented.**  
  _Identified in: WeatherForecastController.cs (`#region Error Response Schema`)_

### Parameter and Property Constraints

- [x] **Restrictions on format, character count, or numeric range of parameters or properties MUST be specified.**  
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

- [x] **Restrictions on array item count or additional properties MUST be defined.**  
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

### Examples and Descriptions

- [x] **Examples for all parameters and request/response bodies MUST be provided.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Descriptions and summaries SHOULD provide meaningful extra information beyond names.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

### Service Usage Guidelines

- [x] **Limits based on service plan, quota, region, or environment MAY be mentioned, but actual values SHOULD NOT be included.**  
  _Identified in: Program.cs, WeatherForecastController.cs (`#region Service Usage Limits`)_

---

## API Design & Lifecycle Management

### Deprecation Management

- [x] **Deprecation notes MUST be added following the API Lifecycle Phases.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **OpenAPI/AsyncAPI description and deprecated properties SHOULD be used if applicable.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **Deprecation notes SHOULD indicate which alternative API or element should be used instead.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

### Deprecation Scope

- [x] **For HTTP APIs: endpoints, methods, parameters, supported media types, or parts of request/response schemas MAY be deprecated.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [ ] **For asynchronous messaging APIs: message topics, full messages, or parts of message schemas MAY be deprecated.**  
  _Not applicable (no async API in this project)_

### API Versioning

- [x] **API versioning SHOULD be implemented to enable backward compatibility and smooth transitions between different versions.**  
  _Identified in: Program.cs (`#region Versioning`)_

- [x] **Multiple versioning strategies SHOULD be supported (URL path, query string, headers).**  
  _Identified in: Program.cs (`#region Versioning` - UrlSegmentApiVersionReader, QueryStringApiVersionReader, HeaderApiVersionReader)_

- [x] **Version-specific endpoints SHOULD be clearly documented and demonstrated.**  
  _Identified in: WeatherForecastController.cs (`#region Versioning`)_

- [x] **Swagger documentation SHOULD support multiple API versions with separate documentation for each version.**  
  _Identified in: SwaggerServiceExtensions.cs, Program.cs (multiple SwaggerDoc configurations)_

---

## Security & Authentication

### OAuth2 and Scopes

- [ ] **The API MUST be secured using OAuth2, and the specification MUST document required scopes and authentication flows.**  
  _Implementation pending_
