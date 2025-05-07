using System.Reflection;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add support for memory and rate limit configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Add controllers to the container
builder.Services.AddControllers();

// Configure IdentityServer with in-memory resources for clients, scopes, and identity info
builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfig.GetClients())
    .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
    .AddDeveloperSigningCredential(); // Developer signing key (not for production use)

// Configure JWT Bearer authentication to validate tokens issued by IdentityServer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:5001"; // Must be the address where IdentityServer is running
        options.Audience = "api1"; // Must match the API scope defined in IdentityServerConfig
        options.RequireHttpsMetadata = false; // For local development use
    });

// ✅ Add authorization policy requiring 'api1' scope to access protected endpoints
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});

// Configure Swagger for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Example API using local IdentityServer for authentication"
    });

    // Enable annotations for Swagger
    options.EnableAnnotations();

    // Define the OAuth2 Client Credentials flow in Swagger
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

    // Apply the security requirement globally in Swagger
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

    // Include XML comments in Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath); // Enables XML comments in Swagger
});

var app = builder.Build();

// Middleware for rate limit
app.UseIpRateLimiting();

// ✅ First: authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Then: IdentityServer middleware
app.UseIdentityServer();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // JSON specification endpoint
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1 (JSON)");

        // YAML specification endpoint
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API v1 (YAML)");

        options.OAuthClientId("minimalrestapi-client");
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce(); // Optional
    });
}

// Map controllers
app.MapControllers();

// Middleware to debug received claims
app.Use(async (context, next) =>
{
    var user = context.User;
    if (user?.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine("User claims:");
        foreach (var claim in user.Claims)
        {
            Console.WriteLine($" - {claim.Type}: {claim.Value}");
        }
    }
    await next();
});

// Start the application
app.Run();