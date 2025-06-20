using System.Text;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;

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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1 (JSON)");
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API v1 (YAML)");
        options.OAuthClientId("minimalrestapi-client");
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce();
    });
}

#endregion Configure Middleware

#region Configure Endpoints

app.MapControllers();

#endregion Configure Endpoints

app.Run();