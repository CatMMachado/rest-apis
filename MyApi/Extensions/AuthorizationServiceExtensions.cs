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
                options.Audience = "device-management-api";
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
    /// This method defines several authorization policies:
    /// - "ApiScope": Requires authenticated users with the "devices.read", "devices.write", "devices.internal", or "devices.external" scopes.
    /// - "InternalOnly": Requires authenticated users with the "devices.internal" scope.
    /// - "ExternalOnly": Requires authenticated users with the "devices.external" scope.
    /// - "ReadAccess": Requires authenticated users with the "devices.read" scope.
    /// - "WriteAccess": Requires authenticated users with the "devices.write" scope.
   /// These policies are used to secure API endpoints and ensure that only authorized users can access them.
   /// </remarks>   
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // General API scope policy
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "devices.read") ||
                    context.User.HasClaim("scope", "devices.write") ||
                    context.User.HasClaim("scope", "devices.internal") ||
                    context.User.HasClaim("scope", "devices.external"));
            });

            // Internal-only endpoints policy
            options.AddPolicy("InternalOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "devices.internal"));
            });

            // External-only endpoints policy
            options.AddPolicy("ExternalOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "devices.external"));
            });

            // Read access policy
            options.AddPolicy("ReadAccess", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "devices.read"));
            });

            // Write access policy
            options.AddPolicy("WriteAccess", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "devices.write"));
            });
        });
        return services;
    }
}

#endregion