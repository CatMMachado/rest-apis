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
        options.SwaggerEndpoint("/swagger/v1-internal/swagger.json", "Internal API V1 - Complete (JSON)");
        options.SwaggerEndpoint("/swagger/v1-internal/swagger.yaml", "Internal API V1 - Complete (YAML)");
        options.SwaggerEndpoint("/swagger/v2-internal/swagger.json", "Internal API V2 - Complete (JSON)");
        options.SwaggerEndpoint("/swagger/v2-internal/swagger.yaml", "Internal API V2 - Complete (YAML)");
        
        // External API Documentation (Filtered - for client delivery and external consumers)
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1 - Client (JSON)");
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API V1 - Client (YAML)");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2 - Client (JSON)");
        options.SwaggerEndpoint("/swagger/v2/swagger.yaml", "My API V2 - Client (YAML)");
        
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