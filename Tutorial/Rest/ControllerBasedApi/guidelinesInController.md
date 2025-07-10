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

Additionaly, bear in mind that Swashbuckle provides many automatic behaviors on top of ASP.NET Core — such as generating schemas from controllers, inferring parameter types, and documenting basic routes — but other aspects, such as security requirements, request validation metadata, or custom headers, require explicit configuration or custom filters to be correctly included in the generated OpenAPI specification.

Before proceeding, Swashbuckle should already be setup in your repository. If haven't done it yet, please follow the instructions in the section [Setup and Run Swashbuckle](./setupSwashbuckleInController.md).

As before, we continue to use the repository [rest-apis](**ADD LINK**) as a working example for this part of the tutorial.

**Add somewhere: it is important to bear in mind that, although Swash

## Common API Guidelines

### API Versioning

To be able to generate an API document for a specific version, you need to have the API versioning setup in your repository.

In the auxiliary repository [rest-apis](**ADD LINK**) there is an example of setup, explained [here](**ADD LINK**). This setup allows the management of multiple versions in the API, which means there can exist controllers and methods visble in different versions of the API, representing the evolution of the API.
Consider the scenario that best resembles your reality.

To guarantee that you generate a document for your version, you need to:

- Have the attribute `ApiVersion` in the controllers.

In the example below, the `DeviceController` supports both v1 and v2, and the applicable endpoints will be visible in both documents.

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class DeviceController : ControllerBase
```

- Have the actions annotated with the attribute `MapToApiVersion`.

In the example below, the endpoint `GetDevicesV2Only` will just be visible in the API specification of v2. The v1 document will not contain it.

```csharp
[HttpGet("v2-only")]
[MapToApiVersion("2.0")]
public IActionResult GetDevicesV2Only()
```

Endpoints not annotated with a version will be treated as part of all API versions.

If all the endpoints are part of a single version, e.g., v1, you just need to annotate the controller with that version, and all the endpoints are assumed to be part of that version.

If you follwed the instructions in section [Setup and Run Swashbuckle](./setupSwashbuckleInController.md), you should be using the versions automatically captured by the *API Provider*, and thus have Swashbuckle prepared to:

- Generate the API documents automatically for all the versions you annotated your controllers and actions with
- Expose the created file(s) in the Swagger UI

For a working example in the [rest-apis](**ADD LINK**) repository check the following:

- The `DeviceController` for it's version attributes
- In the same controller:
  - Region `API Versioning` for examples of endpoints belonging to different API versions
- To check the dynamic creation of the API documents and their exposure in Swagger UI, check:
  - The method `GenerateApiDocument` of the `SwaggerServiceExternsions` and
  - The `Middleware Configuration` region of `Program.cs`, specifically the "API Specification` part.

### API Specification

In this section you can find how to implement the diverse elements referred in the [API Specification guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-specification.md).

For convenience, some have been aglomerated together.

#### OpenAPI specification version 3

For synchronous HTTP-based APIs, it is required that an OpenAPI specification be used, of at least version 3.

OpenAPI version 3 is provided by default by Swashbuckle, so there is no need for additional steps to comply with this request.

#### Specification file in YAML format

Swahbuckle creates both YAML and JSON files at run time, and you can control the visibility of these files in your local environment, as shown in the [rest-apis](**ADD LINK**) repository, in the following regions:

- `Middleware configuration`, specifically under `API Specification`
  - The line `app.UseSwagger` enables the visualization of API documents in endpoints such as `/swagger/v1/swagger.yaml`, from where they can be downloaded
  - The line `options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml", $"My API {description.ApiVersion}");` enables the visualization of the documentation in Swagger UI
  - Both examples work for YAML and JSON, it is just a matter of replacing `.yaml` with `.json` in the case you want to control the visibility of the JSON document

- Additionally, region `API Specification Export to YAML` contains an example on how to export your API specification to file, so that it is more easily accessible.

#### API protection

The setup of authentication and authorization in your repository is outside of the scope of this guide. Here the focus is on the information that can be added to the specification.

For security information to be added to the API specification, it needs to be configured in Swashbuckle. In the [rest-apis](**ADD LINK**) repository, class `SwaggerServiceExtensions`, the method `ConfigureOAuth2Security` configures the Oauth2 security definition and requirements that will appear in the API documentation, and that will be applied to the endpoints when the API is tested in Swagger UI.

In the API specification file, these are the sections added:

```yaml
components:
  securitySchemes:
    oauth2:
      type: oauth2
      flows:
        clientCredentials:
          tokenUrl: http://localhost:5001/connect/token
          scopes:
            api1: Access to My API
            api1.internal: Internal API Access
            api1.external: External API Access
security:
  - oauth2:
      - api1
      - api1.internal
      - api1.external  
```

Bear in mind that this information needs to match your security implementation, and that it is required that you configure Swagger with your implementation details. That is the only way for Swashbuckle to know what the implementation is, to add it to the specification.

This configuration results in the presence of an `Authorize` button, at the top of the page, which will contain the authorization information, including the available scopes for the users to interact with the API. This information is applicable to all endpoints.

At the controller and endpoints level, you will use the attribute `Authorize` to identify that the specified authorization is required. At the writing of this tutorial, Swahsbuckle doesn't map this attribute to the OpenAPI specification, and for the security information to be added for each endpoint, an operation filer needs to be added. However, when testing the API in Swagger UI, the authorization required in the endpoint is still enforced (as this is a runtime behavior).

In the region `API Protection` of the `DeviceController` there is an example of a `public` endpoint, identified by the `AllowAnonymous` attribute that allows the endpoint to be accessed without proper authorization, even being the API protected, and an example of a `private` endpoint, requiring a specific policy to be respected.
As refered above, this information will not be reflected in the specification with the current implementation.

#### External and internal APIs

The management of what components of your API are visible to external clients of solely to internal clients, is done through the use of tags in your endpoints.

In the example below it's visible the use of the tags `Internal` and `External` to clearly mark these endpoints:

```csharp
[HttpGet("analytics")]
[Tags("Internal")]
public IActionResult GetInternalAnalytics() {}

[HttpGet("devices-summary")]
[Tags("External")]
public IActionResult GetExternalDevices() {}
```

Those tags are then used to filter the endpoints that will be added to the different API specifications, since the goal here is to be able to provide specifications tailored to the content your different clients have access to.
This is shown in the [rest-apis](**ADD LINK**) repository, class `SwaggerServiceExtensions`, in the method `ConfigureApiDocInclusion`, where a predicate is set up to filter the endpoints to include in each API document, base on the tag they are annotated with.

The tag is also considered when guaranteeing that all documentation files are visible on the Swagger UI, as is shown in `Program.cs`, in the `API Specification` part, where 2 files per version are made visible, one of them for the `internal` API.

"These endpoints demonstrate different access levels using tags for documentation filtering"

#### Request and response schemas

- restrictions with respect to format, character or number range of parameters and properties
- examples for all parameters and request/response bodies

#### Error responses

- details on error response status codes, error schemas, error types, and a clear association of errors to operations

#### Header details

- header, query, and path parameter details including allowed values and default values

#### Query parameters

- header, query, and path parameter details including allowed values and default values

#### Path parameters

- header, query, and path parameter details including allowed values and default values

#### Deprecation notes

Explain that by using the attribute `Obsolete`, the endpoint is marked as `deprecated`.
The information on alternative endpoit to use needs to be added as a commment, or in the summary field.

#### Service plans and quotas

#### Descriptions and summaries

--
References:

[1] [API Documentation](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-documentation.md)
