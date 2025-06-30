using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyApi.Infrastructure.Swagger;

/// <summary>
/// Document filter to exclude endpoints based on Swagger tags.
/// </summary>
public class TagBasedDocumentFilter : IDocumentFilter
{
    private readonly string[] _excludedTags;

    public TagBasedDocumentFilter(params string[] excludedTags)
    {
        _excludedTags = excludedTags ?? Array.Empty<string>();
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new List<string>();

        foreach (var pathItem in swaggerDoc.Paths)
        {
            var operationsToRemove = new List<OperationType>();

            foreach (var operation in pathItem.Value.Operations)
            {
                // Check if operation has any excluded tags
                if (operation.Value.Tags?.Any(tag => _excludedTags.Contains(tag.Name, StringComparer.OrdinalIgnoreCase)) == true)
                {
                    operationsToRemove.Add(operation.Key);
                }
            }

            // Remove operations with excluded tags
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
    }
}

/// <summary>
/// Operation filter to automatically assign tags based on endpoint characteristics.
/// </summary>
public class AutoTagOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Initialize tags list if not exists
        operation.Tags ??= new List<OpenApiTag>();

        // Get authorization attributes from the action
        var authorizeAttributes = context.MethodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .ToList();

        // Check controller-level authorize attributes as well
        var controllerAuthorizeAttributes = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .ToList() ?? new List<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

        authorizeAttributes.AddRange(controllerAuthorizeAttributes);

        // Add tags based on authorization policies
        if (authorizeAttributes.Any(attr => attr.Policy == "InternalApiAccess"))
        {
            operation.Tags.Add(new OpenApiTag { Name = "Internal" });
        }

        if (authorizeAttributes.Any(attr => attr.Policy == "ExternalApiAccess"))
        {
            operation.Tags.Add(new OpenApiTag { Name = "External" });
        }

        // Add tags based on endpoint path patterns
        var relativePath = context.ApiDescription.RelativePath?.ToLower() ?? "";
        
        if (relativePath.Contains("/internal/"))
        {
            if (!operation.Tags.Any(t => t.Name == "Internal"))
                operation.Tags.Add(new OpenApiTag { Name = "Internal" });
        }
        
        if (relativePath.Contains("/external/"))
        {
            if (!operation.Tags.Any(t => t.Name == "External"))
                operation.Tags.Add(new OpenApiTag { Name = "External" });
        }

        // Add general categorization tags
        if (relativePath.Contains("/weather"))
        {
            operation.Tags.Add(new OpenApiTag { Name = "Weather" });
        }

        // Check for AllowAnonymous attribute
        var allowAnonymousAttributes = context.MethodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), true);
        if (allowAnonymousAttributes.Any())
        {
            operation.Tags.Add(new OpenApiTag { Name = "Public" });
        }
    }
}
