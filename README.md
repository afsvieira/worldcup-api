# WorldCup API

> An open data API for World Cup history, built with .NET 8 and Azure

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/afsvieira/worldcup-api/deploy.yml?style=flat-square)](https://github.com/afsvieira/worldcup-api/actions)
[![Azure](https://img.shields.io/badge/Deployed%20on-Azure-0078D4?style=flat-square&logo=microsoft-azure)](https://azure.microsoft.com/)

## 🧭 Project Overview

WorldCup API is a modern, open-source REST + GraphQL backend built with **.NET 8** and designed to demonstrate clean architecture, cloud deployment, and modern authentication practices.

It provides structured, read-only access to historical data from all FIFA World Cup tournaments (both Men's and Women's), powered by a local SQLite database.

Although the dataset is static and not included in the public repository, the source code and architecture are fully open for educational and portfolio purposes.

## ⚙️ Technology Stack

| Category | Technologies |
|----------|-------------|
| **Backend** | .NET 8 (C#), Clean Architecture |
| **Database** | Dapper + SQLite (read-only) |
| **APIs** | REST API + GraphQL (HotChocolate) |
| **Authentication** | Azure AD B2C (Google, Microsoft, GitHub) |
| **Documentation** | Swagger / OpenAPI 3 |
| **Cloud & DevOps** | Azure App Service, GitHub Actions |
| **Monitoring** | Azure Key Vault, Application Insights |

## 🧱 Project Architecture

```
WorldCup.sln
│
├── src/
│   ├── WorldCup.API/           → REST endpoints + Swagger + HealthCheck + API Key middleware
│   ├── WorldCup.Identity/      → GraphQL endpoint + Azure AD B2C integration + API key service
│   ├── WorldCup.Application/   → Use cases + DTOs + business logic
│   ├── WorldCup.Domain/        → Entities + enums + validation
│   └── WorldCup.Infrastructure/ → Dapper repositories + database context
│
└── data/
    └── worldcup_clean.db       → local SQLite dataset (not included in repo)
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/afsvieira/worldcup-api.git
   cd worldcup-api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure user secrets** (for local development)
   ```bash
   dotnet user-secrets init --project src/WorldCup.API
   dotnet user-secrets set "AzureAdB2C:ClientSecret" "your-client-secret" --project src/WorldCup.API
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/WorldCup.API
   ```

5. **Access the APIs**
   - REST API: `https://localhost:7001/swagger`
   - GraphQL: `https://localhost:7001/graphql`
   - Health Check: `https://localhost:7001/api/v1/health`

### Azure Deployment

The application is configured for automated deployment to Azure App Service using GitHub Actions. See the [deployment workflow](.github/workflows/deploy.yml) for details.

## 🔐 Authentication & Security

- Users log in via **Azure AD B2C**, with social providers: **Google**, **Microsoft**, and **GitHub**
- After authentication, users interact with the **GraphQL** endpoint to:
  - Register their developer account
  - Generate and manage API Keys
- All REST endpoints require the header:
  ```http
  Authorization: Bearer <API_KEY>
  ```
- API Keys are encrypted and stored securely in **Azure SQL** and **Key Vault**

## 📊 API Endpoints

### REST API
- `GET /api/v1/tournaments` - List all World Cup tournaments
- `GET /api/v1/tournaments/{id}` - Get tournament details
- `GET /api/v1/teams` - List all national teams
- `GET /api/v1/matches` - List matches with filtering options
- `GET /api/v1/players` - List players with search capabilities

### GraphQL
- User registration and authentication
- API key management
- Developer account operations

## 🩺 Health & Versioning

- `/api/v1/health` → returns service status and environment info
- Supports **API Versioning** (`/api/v{version}/controller`) via Microsoft.AspNetCore.Mvc.Versioning

## ☁️ Deployment & CI/CD

- Hosted on **Azure App Service** (Canada region)
- Automated deployment using **GitHub Actions**
- **Application Insights** enabled for telemetry and performance monitoring
- Configuration and secrets managed through **Azure Key Vault**

## 🧩 Future Enhancements

- [ ] Add user dashboard (Next.js + Azure Static Web Apps) for API key management
- [ ] Add rate limiting per API key
- [ ] Add usage analytics (queries per endpoint, top countries)
- [ ] Publish public GraphQL schema documentation
- [ ] Expand dataset with images (players, stadiums, flags)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🧑‍💻 About the Author

This project is part of the personal portfolio of **Antonio Felipe Souza Vieira**, a **Full-Stack Developer** based in Canada, showcasing proficiency in modern .NET 8 architecture, GraphQL, Azure integration, and CI/CD automation.

**GitHub Profile**: [https://github.com/afsvieira](https://github.com/afsvieira)

---

⭐ **If you find this project helpful, please consider giving it a star!**