# API Documentation in .NET: A Practical Guide

## Introduction

In modern software development, keeping API documentation up-to-date and accessible is a crucial part of maintaining a high-quality developer experience.

This tutorial is designed for .NET developers, with a specific focus on C# projects. It walks through how to document APIs across different architectures — REST, gRPC, and asynchronous messaging using brokers — with a consistent and maintainable approach.

The overall goal of this tutorial is to assist in the generation of standardized API documentation that can be published to a centralized developer portal, Backstage, streamlining internal discovery and consumption of APIs.

As part of this process, we will follow a set of standardized [API development guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines), which all teams are expected to adopt. This tutorial not only explains the tooling but also demonstrates how to apply these guidelines to your repositories effectively, ensuring alignment across teams and services.

The API guidelines make an important distinction between API documentation and the API specification. The API specification is a core component of a service’s documentation, providing the detailed technical contract necessary for client developers—whether internal or external—to successfully consume the API.

This tutorial focuses specifically on the generation of API specifications, guiding you in how to produce tool-supported specifications as defined by the API guidelines. As such, any content that falls outside the scope of an API specification is intentionally excluded from this tutorial.

Please not that we use both terms API Documentation and API Specification interchangeably throughout the tutorial, but are always referring to the above mentioned API Specification context, unless otherwise stated.

### What you will learn

*By folowing this tutorial, you will learn how to use tooling to generate API specification files from you code base.*

Depending on the API architecture, some tools provide more out of the box functionalities, and others require more programming to generate the specification with the necessary information.

The specification formats produced by these tools are standards, and have been proposed in the API Guidelines.

These are the API architectures considered in this tutorial, and what is detailed for each:

- For REST APIs, how to generate OpenAPI specifications with Swashbuckle
- For asynchronous APIs using message brokers (e.g., RabbitMQ, Kafka), how to generate AsyncAPI specifications with Neuroglia
- For gRPC APIs, (**TBD**)

Additionally, you will learn how to publish your documents in Backstage, enabling discoverability across your organization.

## Next steps

The next sections of this guide can be used independently, based on your needs: if you are working with REST APIs, just go to [REST APIs](RestApis.md#introduction), and when you are ready to share your API documentation, go to [Integration with Backstage](TBD). The same applies to message brokers and gRPC APIs.

For each architecture and tool pairing you have access to a repository with a working example. *This repository is an integral part of this guide*, since the actual code you need to add to your repository exists **only/mostly** in the repository. The tutorial walks you through the steps, provides explanations, and guides you on how to comply with the API guidelines, but directs you to the sections in the repository that you need to integrate in your code. The repository itself is standalone in the sense that it doesn't depend on the content provided by the tutorial, but is nonetheless enriched by it, since the more detailed explanations can be found in the tutorial.

In the sections regarding the architecture styles, after a more theoretical introduction, you will find 2 sections:

- Tool setup. This first section shows how to setup the tool to generate the API specification. It is intended as a simple demonstration of the tool, since it just entails adding the necessary to run the tool with the current state of your repository (in respect to API documentation). By following these steps, you can rapidly start using the tool, without worrying too much with the API specification details.
- Enriching the respository with the guidelines. This second section guides you through the steps to add to your repository the elements present in the [API guidelines](https://gitlab.prod.sgre.one/devsecops/api-governance/api-guidelines). The guidelines will be briefly referenced, and you will be referred to the example repository of that section to see the actual implementation.

A final section is provided in this tutorial that shows you how to make your repository visible to Backstage, so that your API specification is easily shared and discoverable by other teams.

Depending on your next goal, go to one of these sections:

- [REST APIs](Rest/RestApis.md#introduction)
- [Asynchronous APIs and message brokers](Async/messageBrokers.md#introduction)
- [gRPC APIs](TBD)
- [Integration with Backstage](TBD)
