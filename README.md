Invoice Microservice

A foundational microservice built with ASP.NET Core Web API that handles invoice management within a distributed event-driven system. This project demonstrates layered architecture, API design, integration with Entity Framework Core, reliable messaging, dynamic PDF generation, and role-based authorization.

Overview

This project showcases several key backend development techniques, including:

Layered Architecture: Separating concerns into Data, Business, and API layers for maintainability and testability.

Dependency Injection: Leveraging ASP.NET Core’s built-in DI container to manage service lifetimes and configurations.

Entity Framework Core Integration: Using EF Core for database modeling, migrations, and data access.

RESTful API Design: Implementing clear, resource-oriented endpoints following HTTP conventions.

Azure Service Bus Integration: Utilizing Azure Service Bus queues and topics to publish and subscribe to invoice events reliably.

PDF Generation with QuestPDF: Dynamically creating and styling invoice PDFs through the QuestPDF library.

Role-Based Authorization: Enforcing access control using API key validation for users and administrators.

DTOs and Mapper: Mapping between domain models and API contracts to decouple internal data structures.

Validation and Error Handling: Centralizing input validation and consistent error responses.

Key Techniques and Features

Layered Architecture

Data Layer:

Defines EF Core DbContext and invoice entity models.

Manages database migrations and schema updates.

Business Layer:

Contains service interfaces and implementations for invoice operations (Create, Read, Update, Delete).

Encapsulates business rules, validation logic, and transaction management.

API Layer:

Exposes HTTP endpoints via ASP.NET Core Web API controllers.

Routes requests to the Business layer and returns standardized API responses.

Dependency Injection

Services (repositories, business services, Azure Service Bus clients, authorization handlers) are registered in Startup.cs or Program.cs.

Scoped lifetimes ensure a new DbContext per request and singleton lifetimes for Service Bus and authorization clients.

Entity Framework Core

Model classes represent invoice data (e.g., Invoice, InvoiceItem).

Migrations track schema changes, enabling versioned database updates.

LINQ queries are used for efficient data retrieval and manipulation.

Azure Service Bus Integration

Configures Azure Service Bus client using connection strings and settings in appsettings.json.

Publishes domain events (e.g., InvoiceCreated, InvoiceUpdated) to queues or topics after successful operations.

Implements background listeners to process incoming messages and trigger downstream workflows.

PDF Generation with QuestPDF

Defines reusable PDF document templates with branding, headers, and footers.

Uses QuestPDF’s fluent API to layout invoice details, table of items, and totals.

Streams generated PDFs directly from controller endpoints for user download or storage.

Role-Based Authorization

API Key Authentication: Validates incoming requests using predefined API keys stored securely in configuration.

User and Admin Keys: Enforces different access levels by assigning separate API keys for regular users and administrators.

Middleware Enforcement: Custom middleware inspects the X-API-KEY header and rejects unauthorized requests with 401 Unauthorized or 403 Forbidden.

Access Control: Admin endpoints (e.g., bulk invoice deletion) require the admin API key, while standard invoice operations accept the user API key.
Conclusion

This Invoice Microservice demonstrates proficiency in building robust, maintainable backend systems using ASP.NET Core, EF Core, Azure Service Bus, QuestPDF, and role-based authorization. With a clear layered architecture, reliable messaging, dynamic PDF creation, and fine-grained access control, this project exemplifies modern microservice development best practices.
