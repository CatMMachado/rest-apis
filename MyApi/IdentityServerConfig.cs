using Duende.IdentityServer.Models;

#region Security (OAuth2, Scopes)
// This class configures OAuth2 clients, scopes, and identity resources for securing the API.

/// <summary>
/// Configuration for IdentityServer with in-memory clients, scopes, and identity resources.
/// This is used to secure the API with OAuth2 Client Credentials flow.
/// </summary>
public static class IdentityServerConfig
{
    /// <summary>
    /// Gets the list of clients that can access the API.
    /// </summary>
    /// <returns>A collection of configured clients.</returns>
    public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
            // Swagger UI testing client with access to all scopes for development
            new Client
            {
                ClientId = "swagger-ui-client",
                ClientName = "Swagger UI Test Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("swagger-ui-secret".Sha256())
                },
                AllowedScopes = { "devices.read", "devices.write", "devices.internal", "devices.external" },
                Claims = new List<ClientClaim>
                {
                    new ClientClaim("client_type", "swagger_test")
                }
            }
        };

    /// <summary>
    /// Gets the list of API scopes that define the resources available to clients.
    /// </summary>
    /// <returns>A collection of configured API scopes.</returns>
    public static IEnumerable<ApiScope> GetApiScopes() =>
        new List<ApiScope>
        {
            new ApiScope("devices.read", "Read access to the Device Management API"),
            new ApiScope("devices.write", "Write access to the Device Management API"),
            new ApiScope("devices.internal", "Internal API Access"),
            new ApiScope("devices.external", "External API Access")
        };

    /// <summary>
    /// Gets the list of identity resources that define user information available to clients.
    /// </summary>
    /// <returns>A collection of configured identity resources.</returns>
    public static IEnumerable<IdentityResource> GetIdentityResources() =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    /// <summary>
    /// Gets the list of API resources that define the APIs available to clients.
    /// </summary> 
    public static IEnumerable<ApiResource> GetApiResources() =>
        new List<ApiResource>
        {
            new ApiResource("device-management-api", "The device management API")
            {
                Scopes = { "devices.read", "devices.write", "devices.internal", "devices.external" }
            }
        };
}

#endregion