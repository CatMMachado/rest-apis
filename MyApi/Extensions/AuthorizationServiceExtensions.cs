using Microsoft.AspNetCore.Authentication.JwtBearer;

#region Security (OAuth2, Scopes)
// This class configures JWT Bearer authentication and authorization policies.


/// <summary>
/// Extension methods for configuring authentication and authorization in the ASP.NET Core application.
/// This includes JWT Bearer authentication for securing API endpoints and defining authorization policies.
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Configures JWT Bearer authentication for the API.
    /// This is used to validate tokens issued by IdentityServer and secure the API endpoints.
    /// </summary>
    /// <param name="services"> The service collection to add services to.</param>
    /// <returns> The updated service collection with authentication configured.</returns>
    /// <remarks>
    /// This method sets up JWT Bearer authentication using the IdentityServer authority and audience.
    /// It is essential for protecting API endpoints and ensuring that only authenticated users can access them.
    /// </remarks>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:5001";
                options.Audience = "api1";
                options.RequireHttpsMetadata = false;
            });
        return services;
    }

    /// <summary>
    /// Configures authorization policies for the API.
    /// This includes requiring authenticated users with specific claims to access protected endpoints.
    /// </summary>
    /// <param name="services"> The service collection to add services to.</param>
    /// <returns> The updated service collection with authorization policies configured.</returns>
    /// <remarks>
    /// This method adds a policy named "ApiScope" that requires users to be authenticated
    /// and have a claim with the scope "api1". This is used to protect API
    /// endpoints that require access to the API resources.
    /// </remarks>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                context.User.HasClaim("scope", "api1") ||
                context.User.HasClaim("scope", "api1.internal") ||
                context.User.HasClaim("scope", "api1.external"));
            });
        });
        return services;
    }
}

#endregion