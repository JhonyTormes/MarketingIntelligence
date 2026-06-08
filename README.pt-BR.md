# MarketingIntelligence

🇺🇸 [Click here for the English version](README.md)

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Angular](https://img.shields.io/badge/Angular-21-red.svg)](https://angular.io/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](#)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](#)
[![MassTransit+RabbitMQ](https://img.shields.io/badge/MassTransit+RabbitMQ-Enabled-orange.svg)](#)
[![Redis](https://img.shields.io/badge/Redis-Cache_Aside-dc382d.svg)](#)
[![JWT](https://img.shields.io/badge/Auth-JWT_Bearer-ff69b4.svg)](#)

**Nota:** O overengineering aqui é intencional para fins de prática.

Solução modular de análise e automação de processos de marketing desenvolvida em **.NET 9** e **Angular 21**. O projeto utiliza princípios de **Clean Architecture**, **SaaS Multi-tenant** e comunicação baseada em eventos via MassTransit + RabbitMQ.

## 🎯 O Negócio

O principal objetivo do **MarketingIntelligence** é fornecer um hub centralizado para equipes de marketing gerenciarem, rastrearem e otimizarem suas campanhas. Como uma plataforma SaaS, ela foi idealizada para atender desde agências até empresas que precisam de autonomia sobre seus dados de marketing, garantindo isolamento (multi-tenancy) e escalabilidade.

### Principais Capacidades (Módulos de Negócio)

* 🔗 **Rastreamento de Links:** Criação de URLs parametrizadas com captura de métricas avançadas (geolocalização, dispositivos, referenciadores, UTMs) para medir a eficácia das campanhas. Inclui sistema de redirecionamento de alto desempenho com Redis (Cache-Aside). Suporte a nomes de campanha e escopo por usuário.
* 📊 **Análise e Insights:** Dashboard em tempo real exibindo contagem de cliques e metadados de campanha para cada link. Endpoint de estatísticas por link para análise aprofundada.
* 🔐 **Identidade e Acesso:** Cadastro de usuários com hash BCrypt, autenticação via JWT Bearer e fluxo seguro de login. Isolamento de dados por usuário.
* 📧 **Notificações:** Emails transacionais disparados por eventos de domínio — email de boas-vindas no registro, alerta de login no acesso à conta.
* 👥 **Gestão de Clientes:** Entidade `Customer` com value object `BrandIdentity` (JSONB), construída para gestão multi-tenant do ciclo de vida do cliente.
* 📱 **Agendamento para Redes Sociais:** Entidade `Post` e comandos de agendamento (em desenvolvimento) para publicação automatizada em redes sociais.
* 🏢 **Workspaces Multi-tenant:** Ambientes isolados para diferentes organizações ou clientes gerenciarem seus próprios ativos, campanhas e usuários com total segurança e privacidade de dados.
* ⚙️ **Automação Baseada em Eventos:** Fluxos de trabalho que reagem em tempo real ao comportamento do usuário — atualização de métricas de clique, envio de notificações e integração com sistemas externos.

## 🏛️ Arquitetura e Tech Stack

O ecossistema foi projetado para ser escalável e desacoplado, utilizando:

* **Back-end:** ASP.NET Core API (.NET 9) com Clean Architecture (Domínio → Aplicação → Infraestrutura).
* **Front-end:** Angular 21 PWA (componentes standalone, build via Vite, Service Worker) com testes Vitest.
* **Autenticação:** Tokens JWT Bearer com issuer, audience e chave de assinatura configuráveis.
* **Mensageria:** RabbitMQ com MassTransit para integração assíncrona entre módulos.
* **Persistência:** PostgreSQL com Entity Framework Core — cada módulo possui seu próprio schema (`link_shortener`, `customers`, `socialmedia`, `finance`).
* **Cache:** Redis com padrão Cache-Aside (TTL de 24h) para redirecionamentos de alto desempenho.
* **Email:** Cliente SMTP para notificações transacionais (boas-vindas, alerta de login).
* **Containers:** Docker Compose para desenvolvimento local — PostgreSQL 18, RabbitMQ 3, Redis 7.
* **Inteligência:** Configurações de MCP (Model Context Protocol) e regras de design para desenvolvimento assistido por IA.

## 📂 Estrutura do Repositório

```bash
├── src/
│   ├── MarketingIntelligence.Api/              # Entry-point da API e Host da aplicação
│   ├── MarketingIntelligence.Web/              # PWA Angular 21 (Front-end)
│   ├── MarketingIntelligence.Shared/           # Shared Kernel (Result Pattern, Entidade Base, Contratos)
│   └── Modules/
│       ├── LinkShortener/                      # Core + Infrastructure + Tests
│       ├── Identity/                           # Core + Infrastructure
│       ├── Notification/                       # Core + Infrastructure
│       ├── Customers/                          # Core + Infrastructure + Tests
│       ├── SocialMedia/                        # Core + Infrastructure + Tests
│       └── Finance/                            # Core + Infrastructure (scaffold)
├── docs/                                       # Documentação técnica e de banco de dados
├── .github/workflows/                          # Pipelines CI/CD
├── AGENTS.md                                   # Guia do agente de desenvolvimento
└── docker-compose.yml                          # PostgreSQL, RabbitMQ, Redis
```

## 📋 Módulos

| Módulo | Status | Schema | Pacotes | Descrição |
|--------|--------|--------|---------|-----------|
| **LinkShortener** | ✅ Ativo | `link_shortener` | Core, Infrastructure, Tests | Encurtamento de URLs, redirecionamento com cache Redis, rastreio de cliques, estatísticas por link, campanhas |
| **Identity** | ✅ Ativo | — | Core, Infrastructure | Cadastro de usuários (BCrypt), login JWT, consulta de usuários |
| **Notification** | ✅ Ativo | — | Core, Infrastructure | Emails transacionais SMTP — boas-vindas, alerta de login — via consumers MassTransit |
| **Customers** | 🟡 Parcial | `customers` | Core, Infrastructure, Tests | Entidade `Customer`, `BrandIdentity` (JSONB), repositório |
| **SocialMedia** | 🟡 Em dev | — | Core, Infrastructure, Tests | Entidade `Post`, `SchedulePostCommand`/handler, `IPostRepository` |
| **Finance** | ⬜ Scaffold | — | Core, Infrastructure | Projetos vazios prontos para futuras funcionalidades de faturamento |

## 🔐 Autenticação

A API utiliza autenticação via **JWT Bearer**. Três endpoints de Identity estão disponíveis:

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| `POST` | `/api/identity/createUser` | ❌ | Registrar novo usuário (retorna 201 Created) |
| `POST` | `/api/identity/login` | ❌ | Autenticar e receber token JWT |
| `GET` | `/api/identity/{userId}` | ❌ | Obter usuário por ID |

O token JWT deve ser enviado no header `Authorization: Bearer <token>` nos endpoints protegidos. A configuração do token (Issuer, Audience, Secret) é armazenada em **User Secrets**.

## ⚡ Arquitetura Orientada a Eventos

Eventos são publicados via **MassTransit** no **RabbitMQ** e consumidos assincronamente pelos consumers dos módulos:

| Evento | Publisher | Consumer | Efeito |
|--------|-----------|----------|--------|
| `LinkShortenerClickedEvent` | Endpoint de redirecionamento | `LinkShortenerClickedConsumer` | Salva metadados do clique (IP, User-Agent, timestamp) no banco |
| `UserRegisteredEvent` | Serviço de registro | `UserRegisteredConsumer` | Envia email de boas-vindas via SMTP |
| `UserLogedInEvent` | Serviço de login | `UserLogedInConsumer` | Envia alerta de login via SMTP |

## 📡 Endpoints da API

| Método | Rota | Auth | Módulo | Descrição |
|--------|------|------|--------|-----------|
| `POST` | `/api/links` | 🔒 JWT | LinkShortener | Encurtar URL (opcionalmente com nome de campanha) |
| `GET` | `/~{shortCode}` | ❌ | LinkShortener | Redirecionar para URL original (com cache Redis) |
| `GET` | `/api/links/{shortCode}/stats` | 🔒 JWT | LinkShortener | Obter contagem de cliques de um link curto |
| `GET` | `/api/links` | 🔒 JWT | LinkShortener | Listar links do usuário atual com contagens de clique |
| `POST` | `/api/identity/createUser` | ❌ | Identity | Registrar novo usuário |
| `POST` | `/api/identity/login` | ❌ | Identity | Login — retorna token JWT |
| `GET` | `/api/identity/{userId}` | ❌ | Identity | Obter detalhes do usuário |

## 🗄️ Configuração de Secrets

Configurações sensíveis são armazenadas em **dotnet User Secrets** (nunca em `appsettings.json`):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=marketing_intelligence;Username=postgres;Password=password123"
dotnet user-secrets set "Jwt:Secret" "sua-chave-secreta-de-256-bit"
dotnet user-secrets set "Jwt:Issuer" "MarketingIntelligence"
dotnet user-secrets set "Jwt:Audience" "MarketingIntelligence"
dotnet user-secrets set "Smtp:Host" "smtp.example.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:Username" "user@example.com"
dotnet user-secrets set "Smtp:Password" "sua-senha-smtp"
```

## 🚀 Como Começar

### Pré-requisitos

* .NET 9 SDK
* Node.js (LTS)
* Docker Desktop
* Visual Studio 2022 ou VS Code

### Configuração do Ambiente

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/seu-usuario/MarketingIntelligence.git
    cd MarketingIntelligence
    ```

2.  **Suba a infraestrutura (PostgreSQL, RabbitMQ, Redis):**
    ```bash
    docker-compose up -d
    ```

3.  **Configure os secrets:**
    ```bash
    cd src/MarketingIntelligence.Api
    dotnet user-secrets set "Jwt:Secret" "sua-chave-secreta-de-256-bit"
    # Configure outros secrets conforme necessário (veja seção Configuração de Secrets acima)
    cd ../..
    ```

4.  **Execute a API:**
    ```bash
    dotnet run --project src/MarketingIntelligence.Api
    ```

5.  **Execute o Front-end:**
    ```bash
    cd src/MarketingIntelligence.Web
    npm install
    ng serve --proxy-config proxy.conf.json
    ```

A API estará disponível em `http://localhost:5278` e o Swagger UI em `http://localhost:5278/swagger`. O front-end roda em `http://localhost:4200`.

## 📦 PWA

O front-end é um **Progressive Web App** construído com o Service Worker do Angular (`ngsw-config.json`). Ele faz cache de assets estáticos (JS, CSS, fontes) para suporte offline e visitas repetidas mais rápidas.

## 🛠️ Padrões de Desenvolvimento

* **Result Pattern:** Fluxos de negócio utilizam a classe `Result` para evitar exceções de controle.
* **Clean Architecture:** Separação rigorosa entre as camadas de Domínio, Aplicação e Infraestrutura.
* **Migrations:** Utilize `dotnet ef migrations add` dentro do diretório do módulo correspondente para gerenciar o esquema do banco. Migrations nunca são aplicadas automaticamente em produção.
* **Guia do Agente:** Consulte o [`AGENTS.md`](AGENTS.md) para o guia completo de desenvolvimento, problemas conhecidos e padrões de teste.

## 🧪 Testes

Para garantir a integridade dos módulos e das regras de negócio:

```bash
# Executar todos os testes da solução
dotnet test
```

Os testes usam xUnit + FluentAssertions + Moq (back-end) e Vitest + jsdom (front-end). Cada módulo possui seu próprio diretório `Tests/`.

---
*Este projeto é parte da suíte de inteligência para automação de workflows de marketing.*
