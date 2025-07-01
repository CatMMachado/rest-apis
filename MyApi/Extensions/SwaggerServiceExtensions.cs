using System.Reflection;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

/// <summary>
/// Extension methods for configuring Swashbuckle in the ASP.NET Core application.
/// This includes generating API documents for the existing API versions; 
/// setting up OAuth2 security, Swashbuckle annotations, and XML comments for improved API documentation.
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Configures Swashbuckle API document generation and Swagger UI.
    /// This includes setting up the OpenAPI document, enabling annotations, and configuring OAuth2 security.
    /// </summary>
    /// <param name="services"> The service collection to add services to.</param>
    /// <returns> The updated service collection with Swashbuckle and Swagger UI configured.</returns>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        // Add support for multiple API versions
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

        // Generate API documents for each API version, all of them visible in Swagger UI       
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc($"{description.GroupName}-internal", new OpenApiInfo
            {
                Title = "My API (Internal)", // <-- Internal in the title
                Version = description.ApiVersion.ToString(),
                Description = $"Internal API - Version {description.ApiVersion}"
            });

            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "My API (External)", // <-- External in the title
                Version = description.ApiVersion.ToString(),
                Description = $"External API - Version {description.ApiVersion}"
            });
        }

        // Predicate to control which endpoints appear in which docs
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            // Determine version group
            var isInternalDoc = docName.EndsWith("-internal");
            var version = isInternalDoc ? docName.Replace("-internal", "") : docName;

            // Only include endpoints for the correct version
            var versions = apiDesc.GroupName != null ? new[] { apiDesc.GroupName } : Array.Empty<string>();
            var matchesVersion = versions.Contains(version);

            // Get tags from [Tags] attribute
            var tags = apiDesc.ActionDescriptor.EndpointMetadata
                .OfType<TagsAttribute>()
                .SelectMany(attr => attr.Tags)
                .Select(t => t.ToString().ToLower())
                .ToList();

            bool isInternal = tags.Contains("internal");
            bool isExternal = tags.Contains("external");
            bool isGeneral = !isInternal && !isExternal;

            if (isInternalDoc)
            {
                // Internal doc: endpoints tagged "internal" or general
                return matchesVersion && (isInternal || isGeneral);
            }
            else
            {
                // External doc: endpoints tagged "external" or general
                return matchesVersion && (isExternal || isGeneral);
            }
        });

        // Configure OAuth2 security for the API document and Swagger UI
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("http://localhost:5001/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "api1", "Access to My API" }
                    }
                }
            }
        });
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
                new[] { "api1" }
            }
        });

        // Enable annotations provided by Swashbuckle.AspNetCore.Annotations
        options.EnableAnnotations();

        // Enable XML comments in Swashbuckle's generated API documentation
        // This requires that the XML documentation is enable in the project file (usually by setting 
        // <GenerateDocumentationFile>true</GenerateDocumentationFile> in the .csproj file)
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    return services;
}
}