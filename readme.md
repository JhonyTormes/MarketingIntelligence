# MarketingIntelligence

[![EN](https://img.shields.io/badge/lang-EN-blue.svg)](README.md)
[![PT-BR](https://img.shields.io/badge/lang-PT--BR-green.svg)](README.pt-BR.md)

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Angular](https://img.shields.io/badge/Angular-18-red.svg)](https://angular.io/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](#)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](#)
[![MassTransit+RabbitMQ](https://img.shields.io/badge/MassTransit+RabbitMQ-Enabled-orange.svg)](#)

**Note:** The overengineering here is intentional for practicing purposes.

A modular solution for marketing process analysis and automation developed in **.NET 9** and **Angular**. The project leverages **Clean Architecture**, **Multi-tenant SaaS** principles, and event-driven communication.

## 🎯 The Business

The main goal of **MarketingIntelligence** is to provide a centralized hub for marketing teams to manage, track, and optimize their campaigns. As a SaaS platform, it is designed to serve both agencies and companies needing autonomy over their marketing data, ensuring isolation (multi-tenancy) and scalability.

### Key Capabilities (Business Modules)

* 🔗 **Link Tracking:** Parametrized URL creation with advanced metrics capture (geolocation, devices, referrers, UTMs) to measure campaign effectiveness.
* 📊 **Analysis and Insights:** Real-time monitoring of marketing performance and engagement, consolidating data for better decision-making.
* 🏢 **Multi-tenant Workspaces:** Isolated environments for different organizations or clients to manage their assets, campaigns, and users with full data security and privacy.
* ⚙️ **Event-Driven Automation:** Workflows that react in real-time to user behavior (e.g., updating lead scores or notifying external systems when a strategic link is clicked).

## 🏛️ Architecture and Tech Stack

The ecosystem is designed to be scalable and decoupled, utilizing:

* **Back-end:** ASP.NET Core API (.NET 9).
* **Front-end:** Angular 18 (PWA) built with Vite.
* **Messaging:** RabbitMQ with MassTransit for inter-module integration.
* **Persistence:** PostgreSQL with Entity Framework Core.
* **Intelligence:** MCP (Model Context Protocol) configurations and design rules for AI-assisted development (Cursor/Gemini).

## 📂 Repository Structure

```bash
├── src/
│   ├── MarketingIntelligence.Api/       # API entry point and application Host
│   ├── MarketingIntelligence.Web/       # Angular SPA (Front-end)
│   ├── MarketingIntelligence.Shared/    # Shared Core (Result Pattern, Entities, Kernel)
│   └── MarketingIntelligence.Modules/   # Domain Modules (e.g., LinkShortener)
├── tests/                               # Unit and integration tests
├── docs/                                # Technical and database documentation
└── .cursor/rules/                       # Architecture rules for AI assistants
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
    git clone [https://github.com/your-username/MarketingIntelligence.git](https://github.com/your-username/MarketingIntelligence.git)
    cd MarketingIntelligence
    ```

2.  **Spin up the infrastructure (PostgreSQL/RabbitMQ):**
    ```bash
    docker-compose up -d
    ```

3.  **Run the API:**
    ```bash
    dotnet run --project src/MarketingIntelligence.Api
    ```

4.  **Run the Front-end:**
    ```bash
    cd src/MarketingIntelligence.Web
    npm install
    npm start
    ```

## 🛠️ Development Standards

* **Result Pattern:** Business workflows use the `Result` class to avoid control flow exceptions.
* **Clean Architecture:** Strict separation between the Domain, Application, and Infrastructure layers.
* **Migrations:** Use `dotnet ef migrations add` within the corresponding module directory to manage the database schema.

## 🧪 Tests

To ensure the integrity of the modules and business rules:

```bash
# Run all tests in the solution
dotnet test
```

---
*This project is part of the intelligence suite for marketing workflow automation.*