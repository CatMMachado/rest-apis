# Common API Guidelines – Checklist

## Overview

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
4. [Performance & Rate Limiting](#performance--rate-limiting)
5. [Technical Implementation Standards](#technical-implementation-standards)
6. [Security & Authentication](#security--authentication)

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

- [x] **A minimum level of semantic information SHOULD be included so that client developers with domain knowledge understand the API's purpose and usage.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

---

## Documentation Content Requirements

### Schema Definitions & Parameter Documentation

- [x] **Fully defined request and response or message schemas MUST be provided.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on header, query, and path parameters, including allowed values and defaults MUST be documented.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Details on error response codes, schemas, types, and a clear mapping of errors to operations MUST be provided.**  
  _Identified in: WeatherForecastController.cs (`#region Error Response Schema`)_

### Data Validation & Constraints

- [x] **Restrictions on format, character count, or numeric range of parameters or properties SHOULD be specified.**  
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

- [x] **Restrictions on array item count or additional properties SHOULD be defined.**  
  _Identified in: WeatherForecastController.cs (`#region Parameter Restrictions and Defaults`)_

### Examples & Clarity

- [x] **Examples for all parameters and request/response bodies SHOULD be provided.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

- [x] **Descriptions and summaries SHOULD provide meaningful extra information beyond names.**  
  _Identified in: WeatherForecastController.cs (`#region Request/Response Schemas, Parameters, and Examples`)_

### Service Information

- [x] **Limits based on service plan, quota, region, or environment MAY be mentioned, but actual values SHOULD NOT be included.**  
  _Identified in: Program.cs, WeatherForecastController.cs (`#region Service Usage Limits`)_

---

## API Design & Lifecycle Management

### API Versioning Strategy

- [x] **API versioning SHOULD be implemented to enable backward compatibility and smooth transitions between different versions.**  
  _Identified in: Program.cs (`#region Versioning`)_

- [x] **Multiple versioning strategies SHOULD be supported (URL path, query string, headers).**  
  _Identified in: Program.cs (`#region Versioning` - UrlSegmentApiVersionReader, QueryStringApiVersionReader, HeaderApiVersionReader)_

- [x] **Version-specific endpoints SHOULD be clearly documented and demonstrated.**  
  _Identified in: WeatherForecastController.cs (`#region Versioning`)_

- [x] **Swagger documentation SHOULD support multiple API versions with separate documentation for each version.**  
  _Identified in: SwaggerServiceExtensions.cs, Program.cs (multiple SwaggerDoc configurations)_

### Deprecation Management

- [x] **Deprecation notes MUST be added following the API Lifecycle Phases.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **OpenAPI/AsyncAPI description and deprecated properties SHOULD be used if applicable.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [x] **Deprecation notes SHOULD indicate which alternative API or element should be used instead.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

### What Can Be Deprecated

- [x] **For HTTP APIs: endpoints, methods, parameters, supported media types, or parts of request/response schemas MAY be deprecated.**  
  _Identified in: WeatherForecastController.cs (`#region Deprecation Notes`)_

- [ ] **For asynchronous messaging APIs: message topics, full messages, or parts of message schemas MAY be deprecated.**  
  _Not applicable (no async API in this project)_

---

## Performance & Rate Limiting

### API Rate Limiting & Throttling

- [x] **Rate limiting policies SHOULD be implemented to protect API resources and ensure fair usage.**  
  _Identified in: Program.cs (AddInMemoryRateLimiting), appsettings.json (IpRateLimiting configuration)_

- [x] **Rate limiting information SHOULD be exposed through response headers.**  
  _Identified in: WeatherForecastController.cs (X-RateLimit-Limit, X-RateLimit-Remaining headers documentation)_

- [x] **Different rate limiting strategies SHOULD be available for different endpoint types.**  
  _Identified in: WeatherForecastController.cs ([EnableRateLimiting("fixed")] attribute)_

- [x] **Rate limiting errors (429 Too Many Requests) MUST be properly documented.**  
  _Identified in: WeatherForecastController.cs (ProducesResponseType(StatusCodes.Status429TooManyRequests))_

---

## Technical Implementation Standards

### Content Type & Media Type Support

- [x] **Supported content types MUST be explicitly defined for each endpoint.**  
  _Identified in: WeatherForecastController.cs ([Produces], [Consumes] attributes)_

- [x] **Default content types SHOULD be specified when multiple formats are supported.**  
  _Identified in: WeatherForecastController.cs ([Produces("application/json")] attributes)_

### XML Documentation Integration

- [x] **XML documentation generation SHOULD be enabled for comprehensive API documentation.**  
  _Identified in: MyApi.csproj (GenerateDocumentationFile=true), SwaggerServiceExtensions.cs (XML comments integration)_

- [x] **XML comments SHOULD be used to provide detailed method and parameter descriptions.**  
  _Identified in: WeatherForecastController.cs (/// XML comments), SwaggerServiceExtensions.cs_

---

## Security & Authentication

### OAuth2 & Security Implementation

- [x] **The API MUST be secured using OAuth2 with proper authentication flows documented.**  
  _Identified in: SwaggerServiceExtensions.cs (`#region OAuth2 Security Configuration`), IdentityServerConfig.cs_

- [x] **Required scopes for each endpoint MUST be clearly documented in the API specification.**  
  _Identified in: WeatherForecastController.cs ([Authorize(Policy = "ApiScope")]), SwaggerServiceExtensions.cs (security requirements)_

- [x] **Authentication flows (Authorization Code, Client Credentials, etc.) SHOULD be documented with examples.**  
  _Identified in: SwaggerServiceExtensions.cs (ClientCredentials flow configuration), Program.cs (OAuth configuration)_

### Security Documentation

- [x] **Security schemes MUST be defined in the OpenAPI specification.**  
  _Identified in: SwaggerServiceExtensions.cs (AddSecurityDefinition "oauth2")_

- [x] **Protected endpoints MUST indicate required authentication and authorization levels.**  
  _Identified in: WeatherForecastController.cs ([Authorize] attributes and AddSecurityRequirement in Swagger)_

### Public Access & Anonymous Endpoints

- [x] **Public endpoints that do not require authentication SHOULD be clearly marked and documented.**  
  _Identified in: WeatherForecastController.cs ([AllowAnonymous] attributes)_

---

## Implementation Status Summary

**✅ Completed Guidelines:** 28/30 (93%)  
**⏳ Pending Guidelines:** 0/30 (0%)  
**🚫 Not Applicable:** 2/30 (7%) - AsyncAPI guidelines (project uses only HTTP APIs)

### Implementation Highlights
- **Complete OAuth2 security integration** with Client Credentials flow
- **Comprehensive rate limiting** with configurable policies and informative headers  
- **Multi-strategy API versioning** supporting URL path, query string, and header-based versioning
- **Full OpenAPI 3.0+ specification** with YAML format support
- **XML documentation integration** for enhanced API descriptions
- **Content type specification** for proper media type handling
- **Public endpoint support** with clear anonymous access documentation

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
