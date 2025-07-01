using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyApi.Filters;

/// <summary>
/// Document filter to include only internal API endpoints in the documentation.
/// This filter generates comprehensive documentation for internal team use.
/// </summary>
public class InternalApiDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // For internal documentation, include all endpoints
        // This provides complete API coverage for internal team
        
        // Update document info for internal use
        swaggerDoc.Info.Title = "Internal Weather API";
        swaggerDoc.Info.Description = "Complete API documentation for internal team use. " +
                                     "Includes all endpoints, internal analytics, and administrative functions.";
        
        // Add internal-specific server information
        if (swaggerDoc.Servers == null)
            swaggerDoc.Servers = new List<OpenApiServer>();
            
        swaggerDoc.Servers.Add(new OpenApiServer
        {
            Url = "https://internal-api.weather.com",
            Description = "Internal API Server"
        });
    }
}

/// <summary>
/// Document filter to include only external API endpoints in the documentation.
/// This filter generates clean documentation for client delivery.
/// </summary>
public class ExternalApiDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // For external documentation, filter out internal endpoints
        var pathsToRemove = new List<string>();
        
        foreach (var path in swaggerDoc.Paths)
        {
            var operations = path.Value.Operations;
            var shouldRemovePath = true;
            
            foreach (var operation in operations)
            {
                // Keep endpoint if it has "External" tag or no specific internal tags
                if (operation.Value.Tags?.Any(tag => 
                    tag.Name.Equals("External", StringComparison.OrdinalIgnoreCase) ||
                    tag.Name.Equals("Weather", StringComparison.OrdinalIgnoreCase) ||
                    !tag.Name.Equals("Internal", StringComparison.OrdinalIgnoreCase)) == true)
                {
                    shouldRemovePath = false;
                }
                
                // Remove internal-specific tags from external documentation
                if (operation.Value.Tags != null)
                {
                    var tagsToRemove = operation.Value.Tags
                        .Where(tag => tag.Name.Equals("Internal", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    foreach (var tag in tagsToRemove)
                    {
                        operation.Value.Tags.Remove(tag);
                    }
                }
            }
            
            if (shouldRemovePath)
            {
                pathsToRemove.Add(path.Key);
            }
        }
        
        // Remove internal endpoints from external documentation
        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
        
        // Update document info for external use
        swaggerDoc.Info.Title = "Weather API";
        swaggerDoc.Info.Description = "Public Weather API for client integration. " +
                                     "Provides weather forecast data and related services for external consumption.";
        
        // Add external-specific server information
        if (swaggerDoc.Servers == null)
            swaggerDoc.Servers = new List<OpenApiServer>();
            
        swaggerDoc.Servers.Add(new OpenApiServer
        {
            Url = "https://api.weather.com",
            Description = "Production API Server"
        });
        
        // Remove internal tags from the document
        if (swaggerDoc.Tags != null)
        {
            var internalTags = swaggerDoc.Tags
                .Where(tag => tag.Name.Equals("Internal", StringComparison.OrdinalIgnoreCase))
                .ToList();
                
            foreach (var tag in internalTags)
            {
                swaggerDoc.Tags.Remove(tag);
            }
        }
    }
}
