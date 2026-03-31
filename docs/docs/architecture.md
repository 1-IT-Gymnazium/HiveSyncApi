# Architecture Overview

HiveSync uses a **single-tenant, API-driven architecture** with clean separation of concerns.

## Core Principles

- **Repository Pattern with Specifications**  
  - All data access goes through generic repositories (`IRepository<T>` and `IReadOnlyRepository<T>`).  
  - Supports soft-deletes, multi-entity querying, and encapsulates database logic.
  
- **CQRS with MediatR**  
  - Commands handle state changes (create/update/delete).  
  - Queries handle read operations.  
  - Keeps business logic decoupled and maintainable.

- **API-first Design**  
  - Backend exposes a REST API consumed by the React frontend.  
  - All operations, including authentication and email sending, are API-driven.

## Core Features

- Manage **Todos**, **Projects**, and **Sections** inside projects.  
- **User Authentication** with JWT and IdentityServer.  
- **Email Notifications** via SMTP.  

## Simplicity and Extensibility

- Designed to be easy to deploy and extend.  
- Clear separation of concerns allows adding new modules without affecting existing code.  
- Supports a structured approach for future enhancements such as multi-tenancy, microservices, or additional frontend frameworks.