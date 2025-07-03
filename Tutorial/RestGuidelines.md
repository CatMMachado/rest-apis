For each guideline, you’ll find a short explanation and a reference to the file and region in this repository where you can see how it’s implemented.

---

1. **OpenAPI Specification (version 3 or higher)**  
   Configure Swashbuckle in your service registration to generate an OpenAPI v3 document. Swashbuckle automatically generates the OpenAPI document in version 3.x format. You do not need to explicitly set "OpenAPI: 3.0.1" in code—Swashbuckle does this for you. 

   Where to check:
    The OpenAPI version will appear at the top of your generated swagger.json or swagger.yaml file as "openapi": "3.0.1" (or similar).

   *See*: `Program.cs` and `SwaggerServiceExtensions.cs` (`#region API Specification Setup`)

2. **YAML Format**  
   Swashbuckle generates JSON by default. To provide YAML, add a custom endpoint that serializes the OpenAPI document to YAML and maps it to a route.  
   *See*: `Program.cs` (`#region API Specification Setup` and YAML endpoint)

3. **Distinction between internal and external APIs**  
   To distinguish internal from external APIs, use tags or `[ApiExplorerSettings(IgnoreApi = true)]` for internal endpoints.  **Não temos ApiExplorerSettings no repo, não estamos a explicar isto.**
   *See*: `WeatherForecastController.cs` (`#region Internal vs. External API Distinction`)

4. **Syntactic and Semantic Information**  
   Document all endpoints, parameters, and models with XML comments and Swashbuckle annotations. This ensures both the structure and the purpose of the API are clear to consumers.  
   *See*: `SwaggerServiceExtensions.cs` and `WeatherForecastController.cs` (`#region Request/Response Schemas, Parameters, and Examples`)

5. **Request/Response Schemas and Parameters**  
   Define request and response models. Use attributes to specify parameter details, allowed values, and defaults.  
   *See*: `WeatherForecastController.cs` (`#region Request/Response Schemas, Parameters, and Examples`)

6. **Error Handling**  
   Define error response models and document error codes and types for each endpoint.  
   *See*: `WeatherForecastController.cs` (`#region Error Response Schema`)

7. **Parameter and Property Restrictions**  
   Use data annotations (like `[Range]`, `[StringLength]`) and document restrictions in XML comments.  
   *See*: `WeatherForecastController.cs` (`#region Parameter Restrictions and Defaults`)

8. **Service Limits**  
   Mention service plan, quota, or environment-based limits in your documentation, but do not include concrete values.  **Reescrever para clarificar a diferença entre "documentation" and "specification" que é indicada nas guidelines. Isto já terá de vir de trás.**
   *See*: `Program.cs` and `WeatherForecastController.cs` (`#region Service Usage Limits`)

9. **Deprecation Notes**  
   Mark deprecated endpoints or properties with `[Obsolete]` and provide notes on alternatives in the documentation.  
   *See*: `WeatherForecastController.cs` (`#region Deprecation Notes`)

10. **API Versioning**  
    Implement API versioning using versioning libraries and document version-specific endpoints and strategies.  
    *See*: `Program.cs` (`#region Versioning`), `WeatherForecastController.cs` (`#region Versioning`), and `SwaggerServiceExtensions.cs` (multiple SwaggerDoc configurations) **Rever a refª a SwaggerDoc para OpenAPI/Swashbuckle.**

11. **Security (OAuth2, Scopes)**  
    Configure OAuth2 authentication and document required scopes in the OpenAPI specification.  
    *See*: `Program.cs`, `AuthorizationServiceExtensions.cs`, and `IdentityServerServiceExtensions.cs` (`#region Security (OAuth2, Scopes)`)

12. **Service Introduction**  
    Provide a brief description of your service, its main features, and intended audience.  
    *See*: Top of your Swagger/OpenAPI documentation (`SwaggerServiceExtensions.cs`, OpenApiInfo section)

13. **Access Control**  
    Document who can access each API, required roles, and scopes.  
    *See*: `AuthorizationServiceExtensions.cs`, `IdentityServerServiceExtensions.cs` (`#region Security (OAuth2, Scopes)`), and security definitions in `SwaggerServiceExtensions.cs`

14. **Additional Service Documentation**  
    Include or reference any other relevant documentation, such as onboarding guides, usage examples, or support contacts.  
    *See*: [Add a link or section as appropriate]

---

**Tip:**  
If you are new to any of these guidelines, open the referenced file and search for the region name to see exactly how it’s implemented in code.
