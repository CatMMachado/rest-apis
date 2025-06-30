using System.Reflection;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

/// <summary>
/// Extension methods for configuring Swagger in the ASP.NET Core application.
/// This includes setting up API documentation, OAuth2 security, and XML comments for better API visibility
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Configures Swagger for the API.
    /// This includes setting up the Swagger document, enabling annotations, and configuring OAuth2 security.
    /// </summary>
    /// <param name="services"> The service collection to add services to.</param>
    /// <returns> The updated service collection with Swagger configured.</returns>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Configure multiple API versions
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "My API",
                    Version = description.ApiVersion.ToString(),
                    Description = $"Example API using local IdentityServer for authentication - Version {description.ApiVersion}"
                });
            }
            
            options.EnableAnnotations();

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

            // Enable XML comments for API documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        
        return services;
    }
}