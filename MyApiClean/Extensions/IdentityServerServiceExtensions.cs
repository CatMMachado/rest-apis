#region Security (OAuth2, Scopes)

/// <summary>
/// Extension methods for configuring IdentityServer in the ASP.NET Core application.
/// This includes registering in-memory clients, API scopes, and identity resources.
/// </summary>
public static class IdentityServerServiceExtensions
{
    /// <summary>
    /// Configures IdentityServer with in-memory clients, API scopes, and identity resources.
    /// This is used to secure the API with OAuth2 Client Credentials flow.
    /// </summary>
    /// <param name="services"> The service collection to add services to.</param>
    /// <returns> The updated service collection with IdentityServer configured.</returns>
    public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services)
    {
        services.AddIdentityServer()
            .AddInMemoryClients(IdentityServerConfig.GetClients())
            .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
            .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
            .AddDeveloperSigningCredential();
        return services;
    }
}

#endregion
