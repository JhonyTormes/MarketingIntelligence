# MarketingIntelligence

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Angular](https://img.shields.io/badge/Angular-18-red.svg)](https://angular.io/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](#)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](#)
[![MassTransit+RabbitMQ](https://img.shields.io/badge/MassTransit+RabbitMQ-Enabled-orange.svg)](#)

**Note:** The overengineering here is intentional for practicing purposes.

Solução modular de análise e automação de processos de marketing desenvolvida em **.NET 9** e **Angular**. O projeto utiliza princípios de **Clean Architecture**, **SaaS Multi-tenant** e comunicação baseada em eventos.

## 🏛️ Arquitetura e Tech Stack

O ecossistema foi projetado para ser escalável e desacoplado, utilizando:

*   **Back-end:** ASP.NET Core API (.NET 9).
*   **Front-end:** Angular 18 (PWA) com build via Vite.
*   **Mensageria:** RabbitMQ com MassTransit para integração entre módulos.
*   **Persistência:** PostgreSQL com Entity Framework Core.
*   **Inteligência:** Configurações de MCP (Model Context Protocol) e regras de design para desenvolvimento assistido por IA (Cursor/Gemini).

## 📂 Estrutura do Repositório

```bash
├── src/
│   ├── MarketingIntelligence.Api/       # Entry-point da API e Host da aplicação
│   ├── MarketingIntelligence.Web/       # SPA Angular (Front-end)
│   ├── MarketingIntelligence.Shared/    # Core Shared (Result Pattern, Entities, Kernel)
│   └── MarketingIntelligence.Modules/   # Módulos de Domínio (ex: LinkShortener)
├── tests/                               # Testes unitários e de integração
├── docs/                                # Documentação técnica e de banco de dados
└── .cursor/rules/                       # Regras de arquitetura para assistentes de IA
🚀 Como Começar
Pré-requisitos
.NET 9 SDK

Node.js (LTS)

Docker Desktop

Visual Studio 2022 ou VS Code

Configuração do Ambiente
Clone o repositório:

Bash
git clone [https://github.com/seu-usuario/MarketingIntelligence.git](https://github.com/seu-usuario/MarketingIntelligence.git)
cd MarketingIntelligence


2.  **Suba a infraestrutura (PostgreSQL/RabbitMQ):**
    ```bash
    docker-compose up -d
    
Execute a API:

Bash
dotnet run --project src/MarketingIntelligence.Api


4.  **Execute o Front-end:**
    ```bash
    cd src/MarketingIntelligence.Web
    npm install
    npm start
    
🛠️ Padrões de Desenvolvimento
Result Pattern: Fluxos de negócio utilizam a classe Result para evitar exceções de controle.

Clean Architecture: Separação rigorosa entre as camadas de Domínio, Aplicação e Infraestrutura.

Migrations: Utilize dotnet ef migrations add dentro do diretório do módulo correspondente para gerenciar o esquema do banco.

🧪 Testes
Para garantir a integridade dos módulos e das regras de negócio:

Bash
# Executar todos os testes da solução
dotnet test
Este projeto é parte da suíte de inteligência para automação de workflows de marketing.
