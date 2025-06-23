# REST Controller-based APIs

1. [Introduction](#introduction)
2. [Setup and run Swashbuckle](#setup-and-run-swashbuckle)

**TO DECIDE:**
**- Confirm that the setup is the same for controller-based and minimal APIs, and extract the setting up section.**
**- Depending on how long this section ends up to be, create file for setting up and guideliens.**

## Introduction

Controller-based APIs follow the traditional ASP.NET Core MVC pattern, where routes and metadata are defined using attributes on controllers and actions. Swashbuckle integrates seamlessly with this style, offering robust support for:

- Automatic OpenAPI documentation generation
- API versioning support
- Enrichment through XML comments and annotations

This section demonstrates how to configure Swashbuckle in a project that uses controller-based APIs and versioning, and how to document endpoints using standard practices.

## Setup and Run Swashbuckle

### Install Swashbuckle and other required packages

In your API project, install the required packages via the CLI:

```bash
# Swashbuckle
dotnet add package Swashbuckle.AspNetCore

# OpenAPI/Swagger capabilities
dotnet add package Microsoft.AspNetCore.OpenApi

# Code annotations - required for controller-based APIs
dotnet add package Swashbuckle.AspNetCore.Annotations

# Versioning

```

### Configure Swagger

### Enable XML Comments for Endpoint Documentation

### Test Documentation Generation

### Integrate with Backstage ??

## Swashbuckle and the API Guidelines
