using AspNetCoreRateLimit;
using Asp.Versioning;
using MyApi.Services;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;
using Asp.Versioning.ApiExplorer;

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

#endregion Setup for API Specification

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

    // This configuration allows the API to assume a default version when no version is specified in incoming requests.
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

     // Include API version in response headers
    options.ReportApiVersions = true;
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
        // Get API version provider to dynamically configure endpoints
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // Configure endpoints for each API version dynamically
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}-internal/swagger.yaml", $"Internal API {description.ApiVersion} - Complete");
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml", $"My API {description.ApiVersion} - Client");
        }

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

#region Specification Export
// Export V1 API specification to YAML file during startup

if (app.Environment.IsDevelopment())
{
    try
    {
        // Get the Swagger document for V1
        var swaggerProvider = app.Services.GetRequiredService<ISwaggerProvider>();
        var swagger = swaggerProvider.GetSwagger("v1");
        
        // Serialize to YAML format using OpenAPI writer
        var outputString = new StringWriter();
        var writer = new OpenApiYamlWriter(outputString);
        swagger.SerializeAsV3(writer);
        var yamlContent = outputString.ToString();
        
        // Write to file
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "api-spec.yaml");
        await File.WriteAllTextAsync(outputPath, yamlContent);
        
        Console.WriteLine($"✅ API specification exported to: {outputPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to export API specification: {ex.Message}");
    }
}

#endregion Specification Export

app.Run();