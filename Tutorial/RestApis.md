# REST APIs

## Introduction

REST APIs are one of the most common architectures in .NET backend development. ASP.NET Core offers a powerful and flexible way to expose HTTP endpoints, and documenting these APIs is essential for both internal collaboration and external consumption.

In this section, we focus on generating and customizing API documentation for REST services using **Swashbuckle**, a popular library that integrates Swagger/OpenAPI tooling into ASP.NET Core applications.

### Swashbuckle

Swashbuckle uses runtime metadata to automatically discover and describe endpoints, generating OpenAPI documentation that's always up-to-date with your code. The generated documentation can then be consumed by tools such as Backstage to enable discoverability and API governance within your organization.

Swashbuckle generates documentation using:

- Reflection: Scans your application to extract metadata about controllers, parameters, and return types.
- API Explorer: A built-in ASP.NET Core feature that helps map endpoint routes and behaviors.

This ensures that your API documentation remains synchronized with your application's implementation.

It supports three main types of annotations, through the following plugins:

- Microsoft.AspNetCore.Mvc: [HttpGet], [Route], [Produces], [Consumes]
- Swashbuckle.AspNetCore.Annotations: [SwaggerOperation], [SwaggerResponse], [SwaggerParameter]
- System.ComponentModel.DataAnnotations: [Required], [StringLength], [Range], [Display]

These annotations enhance the generated OpenAPI spec and Swagger UI documentation, helping clarify endpoint usage, expected input/output, and behavioral semantics.

#### Supported Development Approaches

Swashbuckle fully supports both Controller-based and Minimal APIs, but the way documentation is authored differs:

##### Controller-based APIs

This is the traditional ASP.NET Core MVC style, which relies on attributes to define routes, HTTP verbs, and documentation metadata. It’s ideal for structured, layered applications where consistency and extensibility are important.

Example:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        return Ok(new[] { "Product A", "Product B" });
    }
}
```

##### Minimal APIs

Minimal APIs offer a concise, function-based syntax, typically using method chaining instead of attributes. While annotations like [HttpGet] aren't used, metadata can still be enriched via chained configuration methods like .WithName(), .Produces(), .WithTags(), and XML comments.

Example:

```csharp
app.MapGet("/products", () =>
{
    return Results.Ok(new[] { "Product A", "Product B" });
})
.WithName("GetProducts")
.Produces<string[]>(StatusCodes.Status200OK)
.WithTags("Product Endpoints");
```

##### Key Differences for Documentation

| Feature | Controller-based API | Minimal API |
| ----------- | ----------- | ----------- |
| Routing | [Route("...")] | .MapGet("...") / .MapPost("...") / ... |
| Endpoint summary | [SwaggerOperation(Summary = "...")] | .WithName("") |
| Response type | [ProducesResponseType(typeof(...), StatusCodes.Status200OK)] | .Produces<...>(StatusCodes.Status200OK)  |
| Error response type | [ProducesResponseType(StatusCodes.Status400BadRequest)] | .Produces(StatusCodes.Status400BadRequest)  |
| Request content | [Consumes("application/json")] | .Accepts<...>("application/json") |
| Response content | [Produces("application/json")] | .Produces<...>("application/json") |
| Authorization Metadata | [Authorize] | .RequireAuthorization() |

### Next steps

The next sections will guide you through the concrete setup of Swashbuckle in your .NET project and show you how to document your endpoints depending on the approach you’ve adopted.

A demo repository is available to illustrate how Swashbuckle is applied to both controller-based APIs and Minimal APIs, respectively called "ControllerBasedRestApi" and "MinimalRestApi".
This tutorial will reference those repositories in the subsequent examples.
