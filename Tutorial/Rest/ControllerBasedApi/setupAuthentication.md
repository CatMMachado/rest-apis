<!-- markdownlint-disable MD029 -->

# Setup Guide: Authentication

> **⚠️ DEMONSTRATION PURPOSES ONLY**
> 
> **The Duende IdentityServer implementation used in this guide is for DEMONSTRATION PURPOSES ONLY. This is NOT a mandatory requirement for controller-based APIs.**
> 
> **You can choose any authentication solution that fits your needs.**

## Install Packages

## Step 1: Install Required Packages

Install Duende IdentityServer and JWT Bearer authentication packages.

```bash
dotnet add package Duende.IdentityServer
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

*Reference: Use `dotnet add package` commands for required packages*

## Create Configuration

Create `IdentityServerConfig.cs` with client credentials configuration, API scopes, and client definitions.

*Reference implementation: `MyApi/IdentityServerConfig.cs`*

## Create Extension Methods

Create extension methods in `Extensions/AuthorizationServiceExtensions.cs` for authentication, authorization, and IdentityServer configuration.

*Reference implementation: `MyApi/Extensions/AuthorizationServiceExtensions.cs`*

## Configure Services

Register IdentityServer, authentication, and authorization services in `Program.cs`.

*Reference implementation: `MyApi/Program.cs`*

## Configure Middleware

Add authentication, authorization, and IdentityServer middleware to the pipeline in `Program.cs`.

*Reference implementation: `MyApi/Program.cs`*

## Protect Controllers

Add `[Authorize]` attributes to controllers that require authentication.

*Reference implementation: `MyApi/Controllers/WeatherForecastController.cs`*

## Add to Swagger

Update Swagger configuration to include OAuth2 security definitions for API testing.

*Reference implementation: `MyApi/Extensions/SwaggerServiceExtensions.cs`*

*Complete example: `MyApi/Extensions/AuthorizationServiceExtensions.cs` in the repository.*