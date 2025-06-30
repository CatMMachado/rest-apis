using AspNetCoreRateLimit;
using Asp.Versioning;
using MyApi.Filters;

var builder = WebApplication.CreateBuilder(args);

#region Service Registration

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

#region Setup for Rate Limiting
// ----------------------------------------------------------------------------
// Setup for Rate Limiting
// ----------------------------------------------------------------------------

builder.Services.AddInMemoryRateLimiting();

#endregion Setup for Rate Limiting

#region Setup for Authentication, Authorization, and IdentityServer
// ----------------------------------------------------------------------------
// Setup for Authentication, Authorization, and IdentityServer
// ----------------------------------------------------------------------------

builder.Services.AddCustomIdentityServer();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();

#endregion Setup for Authentication, Authorization, and IdentityServer

#region API Specification Setup
// ----------------------------------------------------------------------------
// API Specification Setup
// ----------------------------------------------------------------------------

// Swagger configuration is handled in the "Internal and External APIs" section
// to support separate documentation for internal and external consumers

#endregion API Specification Setup

#region Versioning
// ----------------------------------------------------------------------------
// API Versioning Setup
// This configuration supports three types of versioning:
// URL path (/v1/weather), query string (?version=1.0), and header (X-Version).
// ----------------------------------------------------------------------------

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(), // Support /v1/weather, /v2/weather
        new QueryStringApiVersionReader("version"), // Support ?version=1.0
        new HeaderApiVersionReader("X-Version") // Support X-Version header
    );
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});

#endregion Versioning

#region Internal and External APIs
// ----------------------------------------------------------------------------
// Internal and External APIs Configuration
// This section configures different access levels for API consumers using tags:
// - Internal APIs: Tagged as "Internal" for internal team documentation
// - External APIs: Tagged as "External" for client delivery documentation
// Tags enable automatic filtering of endpoints in generated documentation
// ----------------------------------------------------------------------------

// Configure Swagger to generate separate documentation for internal and external APIs
builder.Services.AddSwaggerGen(options =>
{
    // Configure multiple swagger documents for internal and external APIs
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "My API", 
        Version = "v1",
        Description = "Complete API documentation for internal team use"
    });
    
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "My API", 
        Version = "v2",
        Description = "Complete API documentation for internal team use"
    });
    
    options.SwaggerDoc("v1-external", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "External API", 
        Version = "v1",
        Description = "Public API documentation for external clients"
    });
    
    options.SwaggerDoc("v2-external", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "External API", 
        Version = "v2",
        Description = "Public API documentation for external clients"
    });

    // Configure API explorer to include actions in appropriate documents
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Get the API version from the action descriptor
        var versionMetadata = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<Asp.Versioning.ApiVersionAttribute>()
            .FirstOrDefault();
        
        var mappedVersions = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<Asp.Versioning.MapToApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .ToList();
        
        // Determine if this is a V1 or V2 endpoint
        bool isV1 = mappedVersions.Any(v => v.MajorVersion == 1) || 
                   (!mappedVersions.Any() && (versionMetadata?.Versions.Any(v => v.MajorVersion == 1) ?? true));
        bool isV2 = mappedVersions.Any(v => v.MajorVersion == 2) || 
                   (versionMetadata?.Versions.Any(v => v.MajorVersion == 2) ?? false);
        
        // Check for tags in the route template or action name
        var routeTemplate = apiDesc.RelativePath?.ToLower() ?? "";
        var hasExternalTag = routeTemplate.Contains("external");
        var hasInternalTag = routeTemplate.Contains("internal");

        return docName switch
        {
            "v1" => isV1, // Include all V1 endpoints for internal docs
            "v2" => isV2, // Include all V2 endpoints for internal docs
            "v1-external" => isV1 && (hasExternalTag || (!hasInternalTag && !hasExternalTag)), // V1 external + general endpoints
            "v2-external" => isV2 && (hasExternalTag || (!hasInternalTag && !hasExternalTag)), // V2 external + general endpoints
            _ => false
        };
    });
});

#endregion Internal and External APIs

#endregion Service Registration

var app = builder.Build();

#region Configure Middleware

app.UseIpRateLimiting();
app.UseAuthentication(); // Middleware for authentication
app.UseAuthorization(); // Middleware for authorization
app.UseIdentityServer(); // Middleware for IdentityServer

if (app.Environment.IsDevelopment()) // Enable Swagger UI in development environment
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Internal API Documentation (Complete - for internal team)
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1 - Complete (JSON)");
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API V1 - Complete (YAML)");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2 - Complete (JSON)");
        options.SwaggerEndpoint("/swagger/v2/swagger.yaml", "My API V2 - Complete (YAML)");
        
        // External API Documentation (Filtered - for client delivery)
        options.SwaggerEndpoint("/swagger/v1-external/swagger.json", "External API V1 - Client (JSON)");
        options.SwaggerEndpoint("/swagger/v1-external/swagger.yaml", "External API V1 - Client (YAML)");
        options.SwaggerEndpoint("/swagger/v2-external/swagger.json", "External API V2 - Client (JSON)");
        options.SwaggerEndpoint("/swagger/v2-external/swagger.yaml", "External API V2 - Client (YAML)");
        
        options.OAuthClientId("auth-client-id");
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce();
    });
}

#endregion Configure Middleware

#region Configure Endpoints

app.MapControllers();

#endregion Configure Endpoints

app.Run();