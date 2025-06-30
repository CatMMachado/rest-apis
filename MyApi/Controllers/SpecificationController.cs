using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi;

namespace MyApi.Controllers;

/// <summary>
/// Controller for API specification downloads.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SpecificationController : ControllerBase
{
    private readonly ISwaggerProvider _swaggerProvider;

    /// <summary>
    /// Initializes a new instance of the SpecificationController.
    /// </summary>
    /// <param name="swaggerProvider">The Swagger provider for generating API specifications.</param>
    public SpecificationController(ISwaggerProvider swaggerProvider)
    {
        _swaggerProvider = swaggerProvider;
    }

    /// <summary>
    /// Download the complete API specification (includes internal endpoints).
    /// </summary>
    /// <param name="format">The format of the specification (json or yaml).</param>
    /// <returns>The complete API specification file.</returns>
    [HttpGet("complete")]
    [SwaggerOperation(
        Summary = "Download complete API specification",
        Description = "Downloads the complete API specification including internal endpoints. Available in JSON or YAML format.",
        Tags = new[] { "Specification", "Internal" }
    )]
    public IActionResult GetCompleteSpecification([FromQuery] string format = "json")
    {
        return GetSpecification("v1-complete", format, "weather-api-complete");
    }

    /// <summary>
    /// Download the external API specification (excludes internal endpoints).
    /// </summary>
    /// <param name="format">The format of the specification (json or yaml).</param>
    /// <returns>The external API specification file.</returns>
    [HttpGet("external")]
    [SwaggerOperation(
        Summary = "Download external API specification",
        Description = "Downloads the external API specification excluding internal endpoints. Available in JSON or YAML format.",
        Tags = new[] { "Specification", "External" }
    )]
    public IActionResult GetExternalSpecification([FromQuery] string format = "json")
    {
        return GetSpecification("v1-external", format, "weather-api-external");
    }

    private IActionResult GetSpecification(string documentName, string format, string fileName)
    {
        try
        {
            var swagger = _swaggerProvider.GetSwagger(documentName);
            
            if (format.ToLower() == "yaml")
            {
                using var stringWriter = new StringWriter();
                var yamlWriter = new OpenApiYamlWriter(stringWriter);
                swagger.SerializeAsV3(yamlWriter);
                
                var yamlContent = stringWriter.ToString();
                return File(System.Text.Encoding.UTF8.GetBytes(yamlContent), 
                    "application/x-yaml", 
                    $"{fileName}-v{swagger.Info.Version}.yaml");
            }
            else
            {
                using var stringWriter = new StringWriter();
                var jsonWriter = new OpenApiJsonWriter(stringWriter);
                swagger.SerializeAsV3(jsonWriter);
                
                var jsonContent = stringWriter.ToString();
                return File(System.Text.Encoding.UTF8.GetBytes(jsonContent), 
                    "application/json", 
                    $"{fileName}-v{swagger.Info.Version}.json");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "Error generating specification", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Get available API specifications.
    /// </summary>
    /// <returns>List of available specifications with download links.</returns>
    [HttpGet("available")]
    [SwaggerOperation(
        Summary = "Get available API specifications",
        Description = "Returns a list of available API specifications with download links.",
        Tags = new[] { "Specification" }
    )]
    public IActionResult GetAvailableSpecifications()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        
        return Ok(new
        {
            specifications = new[]
            {
                new
                {
                    name = "Complete API",
                    description = "Includes all endpoints (internal + external)",
                    downloads = new
                    {
                        json = $"{baseUrl}/api/specification/complete?format=json",
                        yaml = $"{baseUrl}/api/specification/complete?format=yaml"
                    },
                    swagger_ui = $"{baseUrl}/swagger/index.html"
                },
                new
                {
                    name = "External API",
                    description = "Public endpoints only (for external clients)",
                    downloads = new
                    {
                        json = $"{baseUrl}/api/specification/external?format=json",
                        yaml = $"{baseUrl}/api/specification/external?format=yaml"
                    },
                    swagger_ui = $"{baseUrl}/swagger/index.html"
                }
            }
        });
    }
}
