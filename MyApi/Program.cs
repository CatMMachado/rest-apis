using AspNetCoreRateLimit;
using Asp.Versioning;
using MyApi.Services;

var builder = WebApplication.CreateBuilder(args);

#region Service Registration
// Infrastructure code

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Register application services
builder.Services.AddScoped<IDeviceService, DeviceService>();

#region Setup for Rate Limiting
// Infrastructure code

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

#endregion Setup for Rate Limiting

#region Setup for Authentication, Authorization, and IdentityServer
// Infrastructure code

builder.Services.AddCustomIdentityServer();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();

#endregion Setup for Authentication, Authorization, and IdentityServer

#region Setup for API Specification
// API specification code

builder.Services.AddCustomSwagger();

#endregion API Specification Setup

#region API Versioning
// Infrastructure code

// This configuration supports three types of versioning:
// URL path (/v1/device), query string (?version=1.0), and header (X-Version).
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(), // Support /v1/device, /v2/device
        new QueryStringApiVersionReader("version"), // Support ?version=1.0
        new HeaderApiVersionReader("X-Version") // Support X-Version header
    );
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#endregion Versioning

#endregion Service Registration
// Infrastructure code

var app = builder.Build();

#region Middleware Configuration
// Infrastructure code and API specification code

// Infrastructure code
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();

// API Specification
// Enables functionalities related with the visualization of the API documentation in the browser, for development environments
if (app.Environment.IsDevelopment())
{
    // Enables the visualization of API documentation in endpoints such as "/swagger/v1/swagger.yaml"
    app.UseSwagger();

    // Enables the interactive Swagger UI, for each version specified
    app.UseSwaggerUI(options =>
    {
        // Internal API documentation versions (Complete - for internal team)
        options.SwaggerEndpoint("/swagger/v1-internal/swagger.yaml", "Internal API V1 - Complete");
        options.SwaggerEndpoint("/swagger/v2-internal/swagger.yaml", "Internal API V2 - Complete");

        // External API documentation versions (Filtered - for client delivery and external consumers)
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API V1 - Client");
        options.SwaggerEndpoint("/swagger/v2/swagger.yaml", "My API V2 - Client");

        // OAuth2 configuration for Swagger UI, allowing users to authenticate in the UI
        options.OAuthClientId("auth-client-id");
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce();
    });
}

#endregion Middleware Configuration

#region Configure Endpoints
// Infrastructure code

app.MapControllers();

#endregion Configure Endpoints

app.Run();