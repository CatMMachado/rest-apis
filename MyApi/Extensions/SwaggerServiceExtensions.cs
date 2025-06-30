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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Add support for multiple API versions
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            
            // Generate API documents for each API version, all of them visible in Swagger UI       
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "My API",
                    Version = description.ApiVersion.ToString(),
                    Description = $"Example API using local IdentityServer for authentication - Version {description.ApiVersion}"
                });
            }
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