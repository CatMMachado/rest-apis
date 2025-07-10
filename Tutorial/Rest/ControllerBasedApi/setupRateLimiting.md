<!-- markdownlint-disable MD029 -->

# # Setup Guide: Rate Limiting

> **⚠️ DEMONSTRATION PURPOSES ONLY**
>
> **The AspNetCoreRateLimit package used in this guide is for DEMONSTRATION PURPOSES ONLY. This is NOT a mandatory requirement for controller-based APIs.**
>
> **This example shows how rate limiting can be integrated and documented in OpenAPI specifications. You can choose any rate limiting solution that fits your needs.**

This guide provides step-by-step instructions for configuring rate limiting in a controller-based ASP.NET Core API project.

*For background information and architectural overview, see the [Controller-based REST API introduction](index.md).*

## Install Package

Install the AspNetCoreRateLimit package.

*Reference: Use `dotnet add package` commands for required packages*

## Configuration

Add rate limiting configuration to your `appsettings.json` file with IP whitelisting and general rules.

*Reference implementation: `MyApi/appsettings.json`*

## Configure Services

Configure rate limiting options, register rate limiting services, and add memory cache in your `Program.cs` service registration section.

*Reference implementation: `MyApi/Program.cs` in the `Setup for Rate Limiting` region*

## Configure Middleware

Add the rate limiting middleware early in your `Program.cs` middleware pipeline.

```csharp
app.UseIpRateLimiting(); // Add early in the pipeline
```

*Complete example: `MyApi/Program.cs` in the repository.*