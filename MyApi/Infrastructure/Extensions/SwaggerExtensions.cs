using Microsoft.OpenApi.Models;
using MyApi.Infrastructure.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MyApi.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds custom Swagger configuration with support for internal/external API specifications.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Configure multiple swagger documents
            ConfigureSwaggerDocuments(options);
            
            // Add XML documentation
            AddXmlDocumentation(options);
            
            // Configure security definitions
            ConfigureSecurity(options);
            
            // Add custom operation filters
            options.OperationFilter<AutoTagOperationFilter>();
            
            // Enable annotations
            options.EnableAnnotations();
            
            // Custom schema IDs to avoid conflicts
            options.CustomSchemaIds(type => type.FullName);
        });

        return services;
    }

    private static void ConfigureSwaggerDocuments(SwaggerGenOptions options)
    {
        // Complete API specification (internal + external)
        options.SwaggerDoc("v1-complete", new OpenApiInfo
        {
            Version = "v1.0",
            Title = "Weather API - Complete",
            Description = "Complete API specification including internal and external endpoints",
            Contact = new OpenApiContact
            {
                Name = "API Team",
                Email = "api-team@company.com"
            }
        });

        // External-only API specification
        options.SwaggerDoc("v1-external", new OpenApiInfo
        {
            Version = "v1.0",
            Title = "Weather API - External",
            Description = "Public API specification for external clients and third-party integrations",
            Contact = new OpenApiContact
            {
                Name = "API Support",
                Email = "api-support@company.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // Add document filters for each specification
        options.DocumentFilter<InternalExternalDocumentFilter>(true); // Complete version
        options.DocumentFilterForDocument<InternalExternalDocumentFilter>("v1-external", false); // External-only version
    }

    private static void AddXmlDocumentation(SwaggerGenOptions options)
    {
        // Include XML documentation if available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }

    private static void ConfigureSecurity(SwaggerGenOptions options)
    {
        // Add OAuth2 security definition
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                    TokenUrl = new Uri("https://localhost:5001/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "api1.external", "External API access" },
                        { "api1.internal", "Internal API access" }
                    }
                }
            }
        });

        // Add security requirement
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new[] { "api1.external", "api1.internal" }
            }
        });
    }
}

/// <summary>
/// Extension methods for document filters.
/// </summary>
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Adds a document filter for a specific document.
    /// </summary>
    public static void DocumentFilterForDocument<T>(this SwaggerGenOptions options, string documentName, params object[] parameters)
        where T : class, IDocumentFilter
    {
        options.DocumentFilter<DocumentFilterWrapper<T>>(documentName, parameters);
    }
}

/// <summary>
/// Wrapper to apply document filter only to specific documents.
/// </summary>
public class DocumentFilterWrapper<T> : IDocumentFilter where T : class, IDocumentFilter
{
    private readonly string _targetDocumentName;
    private readonly T _innerFilter;

    public DocumentFilterWrapper(string targetDocumentName, params object[] parameters)
    {
        _targetDocumentName = targetDocumentName;
        _innerFilter = (T)Activator.CreateInstance(typeof(T), parameters)!;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Only apply filter if this is the target document
        if (context.DocumentName == _targetDocumentName)
        {
            _innerFilter.Apply(swaggerDoc, context);
        }
    }
}
