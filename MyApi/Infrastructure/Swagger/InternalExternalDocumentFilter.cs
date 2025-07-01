using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace MyApi.Infrastructure.Swagger;

/// <summary>
/// Document filter to separate internal and external API specifications.
/// </summary>
public class InternalExternalDocumentFilter : IDocumentFilter
{
    private readonly bool _includeInternalApis;

    public InternalExternalDocumentFilter(bool includeInternalApis = true)
    {
        _includeInternalApis = includeInternalApis;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new List<string>();

        foreach (var pathItem in swaggerDoc.Paths)
        {
            var operationsToRemove = new List<OperationType>();

            foreach (var operation in pathItem.Value.Operations)
            {
                var apiDescription = context.ApiDescriptions
                    .FirstOrDefault(x => x.RelativePath?.TrimStart('/') == pathItem.Key.TrimStart('/') 
                                        && x.HttpMethod?.Equals(operation.Key.ToString(), StringComparison.OrdinalIgnoreCase) == true);

                if (apiDescription?.ActionDescriptor?.EndpointMetadata != null)
                {
                    var isInternalApi = IsInternalApi(apiDescription.ActionDescriptor.EndpointMetadata);
                    var isExternalApi = IsExternalApi(apiDescription.ActionDescriptor.EndpointMetadata);

                    // Remove based on filter configuration
                    if (!_includeInternalApis && isInternalApi)
                    {
                        // External specification - remove internal APIs
                        operationsToRemove.Add(operation.Key);
                    }
                    else if (_includeInternalApis && !isInternalApi && !isExternalApi)
                    {
                        // For internal specification, we can include everything
                        // Or add logic here to exclude certain endpoints if needed
                    }
                }
            }

            // Remove operations that shouldn't be included
            foreach (var operationType in operationsToRemove)
            {
                pathItem.Value.Operations.Remove(operationType);
            }

            // If all operations are removed, mark path for removal
            if (!pathItem.Value.Operations.Any())
            {
                pathsToRemove.Add(pathItem.Key);
            }
        }

        // Remove paths that have no operations
        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }

        // Update document info based on specification type
        if (!_includeInternalApis)
        {
            swaggerDoc.Info.Title = "Weather API - External";
            swaggerDoc.Info.Description = "Public API specification for external clients and third-party integrations.";
        }
        else
        {
            swaggerDoc.Info.Title = "Weather API - Complete";
            swaggerDoc.Info.Description = "Complete API specification including internal and external endpoints.";
        }
    }

    private static bool IsInternalApi(IList<object> endpointMetadata)
    {
        // Check for InternalApiAccess policy
        var authorizeAttributes = endpointMetadata.OfType<AuthorizeAttribute>();
        return authorizeAttributes.Any(attr => attr.Policy == "InternalApiAccess");
    }

    private static bool IsExternalApi(IList<object> endpointMetadata)
    {
        // Check for ExternalApiAccess policy
        var authorizeAttributes = endpointMetadata.OfType<AuthorizeAttribute>();
        return authorizeAttributes.Any(attr => attr.Policy == "ExternalApiAccess");
    }
}
