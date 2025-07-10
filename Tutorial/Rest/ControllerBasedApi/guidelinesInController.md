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

--
Swashbuckle doesn’t map [Authorize] → OpenAPI security automatically. In order for the security information to be added to the spec, an operation filter needs to be added.
However, when testing the API in Swagger UI, the authorization required in the endpoint will still be enforced.

**Falar dos campos que se adicionam e do que aparece na spec.**

--
For the purpose of showing SWashbuckle's and OpenAPI's capabilities, (...) => útil para a secção do Pedro?

#### External and internal APIs

"These endpoints demonstrate different access levels using tags for documentation filtering"

#### Request and response schemas

- restrictions with respect to format, character or number range of parameters and properties ?? **add it here?**
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
The information on alternative endpoit to use needs tto be added as a commment, or in the summary field.

#### Service plans and quotas

#### Descriptions and summaries

--
References:

[1] [API Documentation](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines/-/blob/review/common/api-documentation.md)
