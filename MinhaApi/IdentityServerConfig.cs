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
            new Client
            {
                ClientId = "minimalrestapi-client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("your-client-secret".Sha256())
                },
                AllowedScopes = { "api1" }
            }
        };

    /// <summary>
    /// Gets the list of API scopes that define the resources available to clients.
    /// </summary>
    /// <returns>A collection of configured API scopes.</returns>
    public static IEnumerable<ApiScope> GetApiScopes() =>
        new List<ApiScope>
        {
            new ApiScope("api1", "MinimalRestAPI")
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
}

#endregion