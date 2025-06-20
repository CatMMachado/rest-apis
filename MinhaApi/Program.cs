using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

#region Service Registration
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddCustomIdentityServer();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();
builder.Services.AddCustomSwagger();

#endregion

var app = builder.Build();

#region Configure Middleware

app.UseIpRateLimiting(); // Middleware for rate limiting
app.UseAuthentication(); // Middleware for authentication
app.UseAuthorization(); // Middleware for authorization
app.UseIdentityServer(); // Middleware for IdentityServer

if (app.Environment.IsDevelopment()) // Enable Swagger UI in development environment
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1 (JSON)"); // JSON specification endpoint
        options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My API v1 (YAML)"); // YAML specification endpoint
        options.OAuthClientId("minimalrestapi-client");
        options.OAuthClientSecret("your-client-secret");
        options.OAuthUsePkce(); // Optional
    });
}

#endregion

#region Configure Endpoints

app.MapControllers();

#endregion

app.Run();