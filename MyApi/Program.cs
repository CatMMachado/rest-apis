using System.Text;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

#region Service Registration

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

#region Setup for Rate Limiting
// -------------------------------------------------------------
// Setup for Rate Limiting
// Guidelines: "limits which are dependent on the used service plan,
// acquired quota, the service region or deployment environment MAY be mentioned in the API specification, 
// but their concrete values SHOULD NOT be added to an API specification."
// -------------------------------------------------------------

builder.Services.AddInMemoryRateLimiting();

#endregion Setup for Rate Limiting

#region Setup for Authentication, Authorization, and IdentityServer
// -------------------------------------------------------------
// Setup for Authentication, Authorization, and IdentityServer
// Guidelines: "Security (OAuth2, Scopes): The API must be secured using OAuth2, 
// and the specification must document required scopes and authentication flows."
// -------------------------------------------------------------

builder.Services.AddCustomIdentityServer();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();

#endregion Setup for Authentication, Authorization, and IdentityServer

#region API Specification Setup
// -------------------------------------------------------------
// API Specification Setup
// Guidelines: "For synchronous HTTP-based APIs, OpenAPI specifications of at least version 3 MUST be used.
// The API specifications SHOULD be provided in YAML format for consistency reasons."
// -------------------------------------------------------------

builder.Services.AddCustomSwagger();

#endregion API Specification Setup

#region Versioning
// -------------------------------------------------------------
// API Versioning Setup
// Guidelines: "API versioning enables backward compatibility and smooth transitions 
// between different versions of the API. This configuration supports URL path versioning."
// -------------------------------------------------------------

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(), // Support /v1/weather, /v2/weather
        new QueryStringApiVersionReader("version"), // Support ?version=1.0
        new HeaderApiVersionReader("X-Version") // Support X-Version header
    );
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
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
        
        options.OAuthClientId("minimalrestapi-client"); // TODO: replace this reference to the minimal api
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce();
    });
}

#endregion Configure Middleware

#region Configure Endpoints

app.MapControllers();

#endregion Configure Endpoints

app.Run();