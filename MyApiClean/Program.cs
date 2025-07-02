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

#region API Versioning
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

#region Middleware Configuration

app.UseIpRateLimiting();
app.UseAuthentication(); // Middleware for authentication
app.UseAuthorization(); // Middleware for authorization
app.UseIdentityServer(); // Middleware for IdentityServer

#endregion Middleware Configuration

#region Configure Endpoints

app.MapControllers();

#endregion Configure Endpoints

app.Run();