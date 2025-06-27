using AspNetCoreRateLimit;
using Asp.Versioning;

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

builder.Services.AddCustomSwagger();

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
// This section configures different access levels for API consumers:
// - Internal APIs: Restricted to internal systems and services only
// - External APIs: Available to external clients and third-party consumers
// ----------------------------------------------------------------------------

// Configure policies for internal and external API access
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InternalApiAccess", policy =>
        policy.RequireClaim("scope", "api1.internal")
              .RequireClaim("client_type", "internal"));
    
    options.AddPolicy("ExternalApiAccess", policy =>
        policy.RequireClaim("scope", "api1.external"));
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
        var provider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();
        
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"My API {description.GroupName.ToUpper()} (JSON)"
            );
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.yaml",
                $"My API {description.GroupName.ToUpper()} (YAML)"
            );
        }
        
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