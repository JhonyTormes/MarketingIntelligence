# MarketingIntelligence

🇧🇷 [Clique aqui para a versão em Português](README.pt-BR.md)

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Angular](https://img.shields.io/badge/Angular-21-red.svg)](https://angular.io/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](#)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](#)
[![MassTransit+RabbitMQ](https://img.shields.io/badge/MassTransit+8.x+RabbitMQ-Enabled-orange.svg)](#)
[![Redis](https://img.shields.io/badge/Redis-Cache_Aside-dc382d.svg)](#)
[![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Aspire_Dashboard-7B42BC.svg)](#)
[![JWT](https://img.shields.io/badge/Auth-JWT_Bearer-ff69b4.svg)](#)

**Note:** The overengineering here is intentional for practicing purposes.

A modular solution for marketing process analysis and automation developed in **.NET 9** and **Angular 21**. The project leverages **Clean Architecture**, **Multi-tenant SaaS** principles, and event-driven communication via MassTransit + RabbitMQ.

## 🎯 The Business

The main goal of **MarketingIntelligence** is to provide a centralized hub for marketing teams to manage, track, and optimize their campaigns. As a SaaS platform, it is designed to serve both agencies and companies needing autonomy over their marketing data, ensuring isolation (multi-tenancy) and scalability.

### Key Capabilities (Business Modules)

* 🔗 **Link Tracking:** Parametrized URL creation with advanced metrics capture (geolocation, devices, referrers, UTMs) to measure campaign effectiveness. Features a high-performance redirect system using Redis with the Cache-Aside pattern. Supports campaign names and per-user link scoping.
* 📊 **Analysis and Insights:** Real-time dashboard showing each link's click count and campaign metadata. Per-link statistics endpoint for deeper analysis.
* 🔐 **Identity & Access:** User registration with BCrypt password hashing, JWT Bearer authentication, and secure login flow. User-scoped data isolation.
* 📧 **Notifications:** Transactional email notifications triggered by domain events — welcome emails on registration, login alerts on account access.
* 👥 **Customer Management:** Full CRUD for customers (PF/PJ via TPH inheritance), addresses, and contacts. `BrandIdentity` value object stored as JSONB. Customer activation/deactivation workflow. Multi-tenant, user-scoped.
* 📱 **Social Media Scheduling:** Post entity and scheduling commands (in development) for automated social media publishing.
* 🏢 **Multi-tenant Workspaces:** Isolated environments for different organizations or clients to manage their assets, campaigns, and users with full data security and privacy.
* ⚙️ **Event-Driven Automation:** Workflows that react in real-time to user behavior — updating click metrics, sending notifications, and integrating with external systems.

## 🏛️ Architecture and Tech Stack

The ecosystem is designed to be scalable and decoupled, utilizing:

* **Back-end:** ASP.NET Core API (.NET 9) with Clean Architecture (Domain → Application → Infrastructure).
* **Front-end:** The primary UI is built with vanilla HTML/CSS/JS pages for core features (Login, Register, Link Shortener). An Angular 21 project (Vite build, Vitest tests) also exists in the repository, serving as scaffolding for future migration.
* **Authentication:** JWT Bearer tokens with configurable issuer, audience, and signing key.
* **Messaging:** RabbitMQ with MassTransit 8.x for asynchronous inter-module integration (9.x requires paid license).
* **Persistence:** PostgreSQL with Entity Framework Core — each module owns its schema (`link_shortener`, `customers`, `socialmedia`, `finance`).
* **Caching:** Redis with Cache-Aside pattern (24h TTL) for high-performance link redirects.
* **Email:** SMTP client for transactional notifications (welcome emails, login alerts).
* **Containers:** Docker Compose for local development — PostgreSQL 18, RabbitMQ 3, Redis 7, Aspire Dashboard.
* **Observability:** OpenTelemetry SDK (traces, metrics, logs) exporting OTLP to the Aspire Dashboard — logs via `ILogger`, traces via ASP.NET Core + EF Core + MassTransit instrumentation, metrics via ASP.NET Core + MassTransit meters.
* **Intelligence:** MCP (Model Context Protocol) configurations and design rules for AI-assisted development.

## 📂 Repository Structure

```bash
├── src/
│   ├── MarketingIntelligence.Api/              # API entry point and application Host
│   ├── MarketingIntelligence.Web/              # Front-end (Vanilla HTML/JS + Angular)
│   │   ├── src/app/                          # Angular 21 SPA Scaffolding
│   │   └── src/Pages/                        # Vanilla HTML/CSS/JS pages (Login, Register, etc)
│   ├── MarketingIntelligence.Shared/           # Shared Kernel (Result Pattern, Base Entity, Contracts)
│   └── Modules/
│       ├── LinkShortener/                      # Core + Infrastructure + Tests
│       ├── Identity/                           # Core + Infrastructure
│       ├── Notification/                       # Core + Infrastructure
│       ├── Customers/                          # Core + Infrastructure + Tests
│       ├── SocialMedia/                        # Core + Infrastructure + Tests
│       └── Finance/                            # Core + Infrastructure (scaffold)
├── docs/                                       # Technical and database architecture docs
├── .github/workflows/                          # CI/CD pipelines
├── AGENTS.md                                   # Development agent guide
└── docker-compose.yml                          # PostgreSQL, RabbitMQ, Redis, Aspire Dashboard
```

## 📋 Modules

| Module | Status | Schema | Packages | Description |
|--------|--------|--------|----------|-------------|
| **LinkShortener** | ✅ Active | `link_shortener` | Core, Infrastructure, Tests | URL shortening, redirect with Redis cache, click tracking, per-link stats, campaign metadata |
| **Identity** | ✅ Active | — | Core, Infrastructure | User registration (BCrypt), JWT login, user query |
| **Notification** | ✅ Active | — | Core, Infrastructure | SMTP transactional emails — welcome, login alert — via MassTransit consumers |
| **Customers** | ✅ Active | `customers` | Core, Infrastructure, Tests | PF/PJ customer CRUD with TPH, addresses, contacts, `BrandIdentity` (JSONB), activate/deactivate, user-scoped controller |
| **SocialMedia** | 🟡 In dev | — | Core, Infrastructure, Tests | `Post` entity, `SchedulePostCommand`/handler, `IPostRepository` |
| **Finance** | ⬜ Scaffold | — | Core, Infrastructure | Empty projects ready for future billing/payment features |

## 🔐 Authentication

The API uses **JWT Bearer** token authentication. Three Identity endpoints are available:

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `/api/identity/createUser` | ❌ | Register a new user (returns 201 Created) |
| `POST` | `/api/identity/login` | ❌ | Authenticate and receive a JWT token |
| `GET` | `/api/identity/{userId}` | 🔒 JWT | Get user by ID |

The JWT token must be sent as an `Authorization: Bearer <token>` header on protected endpoints. Token configuration (Issuer, Audience, Secret) is stored in **User Secrets**.

## ⚡ Event-Driven Architecture

Events are published via **MassTransit** to **RabbitMQ** and consumed asynchronously by module consumers:

| Event | Publisher | Consumer | Effect |
|-------|-----------|----------|--------|
| `LinkShortenerClickedEvent` | Redirect endpoint | `LinkShortenerClickedConsumer` | Saves click metadata (IP, User-Agent, timestamp) to database |
| `UserRegisteredEvent` | Registration service | `UserRegisteredConsumer` | Sends welcome email via SMTP |
| `UserLogedInEvent` | Login service | `UserLogedInConsumer` | Sends login alert email via SMTP |

## 📡 API Endpoints

| Method | Route | Auth | Module | Description |
|--------|-------|------|--------|-------------|
| `POST` | `/api/links` | 🔒 JWT | LinkShortener | Shorten a URL (optionally with campaign name) |
| `GET` | `/~{shortCode}` | ❌ | LinkShortener | Redirect to original URL (Redis-cached) |
| `GET` | `/api/links/{shortCode}/stats` | 🔒 JWT | LinkShortener | Get statistics for a shortened link |
| `GET` | `/api/links` | 🔒 JWT | LinkShortener | List current user's links with click counts |
| `POST` | `/api/identity/createUser` | ❌ | Identity | Register a new user |
| `POST` | `/api/identity/login` | ❌ | Identity | Login — returns JWT token |
| `GET` | `/api/identity/{userId}` | 🔒 JWT | Identity | Get user details |
| `POST` | `/api/customers` | 🔒 JWT | Customers | Create a customer (PF or PJ) |
| `GET` | `/api/customers` | 🔒 JWT | Customers | List user's customers |
| `GET` | `/api/customers/{id}` | 🔒 JWT | Customers | Get customer by ID |
| `PUT` | `/api/customers/{id}` | 🔒 JWT | Customers | Update customer |
| `DELETE` | `/api/customers/{id}` | 🔒 JWT | Customers | Delete customer |
| `PATCH` | `/api/customers/{id}/activate` | 🔒 JWT | Customers | Activate customer |
| `PATCH` | `/api/customers/{id}/deactivate` | 🔒 JWT | Customers | Deactivate customer |

## 📡 Observability

The API is instrumented with **OpenTelemetry** and exports telemetry to the **Aspire Dashboard** via the OTLP protocol.

### Infrastructure

The `docker-compose.yml` runs the Aspire Dashboard as a standalone container (`mi-aspire-dashboard`):

| Container port | Description | Host mapping |
|----------------|-------------|-------------|
| `18888` | Dashboard UI | `18888` |
| `18889` | OTLP/gRPC endpoint | `4317` |
| `18890` | OTLP/HTTP endpoint | `4318` |

The internal container ports (`18889`/`18890`) differ from the host-facing ports (`4317`/`4318`). The API sends telemetry to `http://127.0.0.1:4317` (gRPC).

### What's monitored

| Signal | Source | Data |
|--------|--------|------|
| **Logs** | `ILogger` (all categories) | Structured log output — startup info, DB commands, MassTransit bus events, request processing |
| **Traces** | ASP.NET Core (`Microsoft.AspNetCore`) | HTTP requests — method, route, status code, duration |
| **Traces** | EF Core (`Microsoft.EntityFrameworkCore`) | Database commands — SQL, parameters, duration |
| **Traces** | MassTransit | Message handling — consumer processing, bus activity |
| **Metrics** | ASP.NET Core (`Microsoft.AspNetCore`) | HTTP request rate, duration, active connections |
| **Metrics** | MassTransit | Message delivery rate, consumer execution, bus status |

### Configuration

The OTLP endpoint is read from the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable, falling back to `http://127.0.0.1:4317`:

```bash
# Custom endpoint (optional, default from launchSettings.json)
export OTEL_EXPORTER_OTLP_ENDPOINT=http://127.0.0.1:4317
```

The resource is named `MarketingIntelligence.Api`. The service name is set via `ResourceBuilder.CreateDefault().AddService("MarketingIntelligence.Api")`.

### Accessing telemetry

Open the Aspire Dashboard at [http://localhost:18888](http://localhost:18888):

- **Structured Logs** → `http://localhost:18888/structuredlogs`
- **Traces** → `http://localhost:18888/traces`
- **Metrics** → `http://localhost:18888/metrics`

### NuGet packages

```xml
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.16.0" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.16.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.15.2" />
<PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.15.1-beta.1" />
```

## 🗄️ Secrets Configuration

Sensitive settings are stored in **dotnet User Secrets** (never in `appsettings.json`):

> ⚠️ All values below are **placeholders**. Never store real secrets in version-controlled files.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=marketing_intelligence;Username=postgres;Password=your-password"
dotnet user-secrets set "Jwt:Secret" "your-256-bit-secret-key-here"
dotnet user-secrets set "Jwt:Issuer" "MarketingIntelligence"
dotnet user-secrets set "Jwt:Audience" "MarketingIntelligence"
dotnet user-secrets set "Smtp:Host" "smtp.example.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:Username" "your-email@example.com"
dotnet user-secrets set "Smtp:Password" "your-smtp-password"
```

## 🚀 Getting Started

### Prerequisites

* .NET 9 SDK
* Node.js (LTS)
* Docker Desktop
* Visual Studio 2022 or VS Code

### Environment Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/MarketingIntelligence.git
    cd MarketingIntelligence
    ```

2.  **Spin up the infrastructure (PostgreSQL, RabbitMQ, Redis, Aspire Dashboard):**
    ```bash
    docker-compose up -d
    ```

3.  **Configure secrets:**
    ```bash
    cd src/MarketingIntelligence.Api
    dotnet user-secrets set "Jwt:Secret" "your-256-bit-secret-key-here"
    # Set other secrets as needed (see Secrets Configuration section above)
    cd ../..
    ```

4.  **Run the API:**
    ```bash
    dotnet run --project src/MarketingIntelligence.Api
    ```

5.  **Run the Front-end:**
    The active front-end consists of vanilla HTML pages. You can serve them using a simple HTTP server or a VS Code extension like Live Server from the `src/MarketingIntelligence.Web/src/Pages/` directory.

    Alternatively, to work on the Angular application scaffolding:
    ```bash
    cd src/MarketingIntelligence.Web
    npm install
    ng serve --proxy-config proxy.conf.json
    ```

The API will be available at `http://localhost:5278` and Swagger UI at `http://localhost:5278/swagger`. The Angular front-end runs on `http://localhost:4200`.



## 🛠️ Development Standards

* **Result Pattern:** Business workflows use the `Result` class to avoid control flow exceptions.
* **Clean Architecture:** Strict separation between the Domain, Application, and Infrastructure layers.
* **Migrations:** Use `dotnet ef migrations add` within the corresponding module directory to manage the database schema. Migrations are never auto-applied in production.
* **Agent Guide:** See [`AGENTS.md`](AGENTS.md) for the complete development guide, known gotchas, and test patterns.

## 🧪 Tests

To ensure the integrity of the modules and business rules:

```bash
# Run all tests in the solution
dotnet test
```

Tests use xUnit + FluentAssertions + Moq (back-end) and Vitest + jsdom (front-end). Each module has its own `Tests/` directory.

---
*This project is part of the intelligence suite for marketing workflow automation.*
