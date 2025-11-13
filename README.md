# WorldCup API

> An open data API for World Cup history, built with .NET 9 and Azure

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/afsvieira/worldcup-api/deploy.yml?style=flat-square)](https://github.com/afsvieira/worldcup-api/actions)
[![Azure](https://img.shields.io/badge/Deployed%20on-Azure-0078D4?style=flat-square&logo=microsoft-azure)](https://azure.microsoft.com/)

## 🧭 Project Overview

WorldCup API is a modern, open-source REST API backend built with **.NET 9** and designed to demonstrate clean architecture, cloud deployment, and modern authentication practices.

It provides structured, read-only access to historical data from all FIFA World Cup tournaments (both Men's and Women's), powered by a local SQLite database.

Although the dataset is static and not included in the public repository, the source code and architecture are fully open for educational and portfolio purposes.

## ⚙️ Technology Stack

| Category | Technologies |
|----------|-------------|
| **Backend** | .NET 9 (C#), Clean Architecture |
| **Database** | Entity Framework Core + Azure SQL Database |
| **Caching** | Redis (Azure Cache for Redis) |
| **APIs** | REST API |
| **Authentication** | ASP.NET Core Identity |
| **Documentation** | Swagger / OpenAPI 3 |
| **Cloud & DevOps** | Azure App Service, Azure SQL Database, GitHub Actions |
| **Monitoring** | Azure Key Vault, Application Insights |

## 🧱 Project Architecture

```
WorldCup.sln
│
├── src/
│   ├── WorldCup.API/           → REST endpoints + Swagger + HealthCheck + API Key middleware
│   ├── WorldCup.Identity/      → ASP.NET Core Identity + User management + API key service
│   ├── WorldCup.Application/   → Use cases + DTOs + business logic
│   ├── WorldCup.Domain/        → Entities + enums + validation
│   └── WorldCup.Infrastructure/ → EF Core DbContext + repositories + Redis cache
│
├── docs/                       → Documentation files
└── data/
    └── README.md               → Data source documentation
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
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

3. **Run the application**
   ```bash
   dotnet run --project src/WorldCup.API
   ```

4. **Create and apply database migrations**
   ```bash
   cd src/WorldCup.Identity
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Access the APIs**
   - REST API: `https://localhost:7001/swagger`
   - Identity Management: `https://localhost:7001`
   - Health Check: `https://localhost:7001/api/v1/health`

### Azure Deployment

The application is configured for automated deployment to Azure App Service using GitHub Actions. See the [deployment workflow](.github/workflows/deploy.yml) for details.

## 🔐 Authentication & Security

- Users register and log in via **ASP.NET Core Identity**
- **Email verification** required for full account access (optional SMTP configuration)
- After authentication, users can:
  - Manage their developer account (starts with **Free** plan)
  - Generate and manage API Keys (respects plan limits)
  - View available subscription plans
- All REST endpoints require the header:
  ```http
  Authorization: Bearer <API_KEY>
  ```
- API Keys are encrypted and stored securely in Azure SQL Database, with generation limits based on subscription plan
- Email verification system with professional HTML templates (see [Email Verification Guide](docs/EMAIL-VERIFICATION.md))

## 📧 Email Configuration

The application includes a complete email verification system with support for **Azure Communication Services** (recommended) or **SMTP**.

### Option 1: Azure Communication Services (Recommended)

1. **Use your existing Azure Communication Services** or create a new one
2. **Get your connection string** from Azure Portal
3. **Configure in User Secrets**:
   ```bash
   cd src/WorldCup.API
   dotnet user-secrets set "Email:Provider" "Azure"
   dotnet user-secrets set "Email:Azure:ConnectionString" "endpoint=https://...;accesskey=..."
   dotnet user-secrets set "Email:Azure:FromEmail" "DoNotReply@xxx.azurecomm.net"
   ```

4. **When you register your domain**, configure custom domain in Azure for professional emails

### Option 2: SMTP (Gmail - Development Only)

1. **Create Gmail App Password**: [Instructions](https://support.google.com/accounts/answer/185833)
2. **Configure in User Secrets**:
   ```bash
   cd src/WorldCup.API
   dotnet user-secrets set "Email:Provider" "Smtp"
   dotnet user-secrets set "Email:Smtp:Username" "your-email@gmail.com"
   dotnet user-secrets set "Email:Smtp:Password" "your-app-password"
   ```

### Documentation

- **Full Guide**: [Email Verification Documentation](docs/EMAIL-VERIFICATION.md)
- **Note**: Email configuration is optional for development. The system works without it configured, but email verification features will be disabled.

## 📊 API Endpoints

### REST API
- `GET /api/v1/tournaments` - List all World Cup tournaments
- `GET /api/v1/tournaments/{id}` - Get tournament details
- `GET /api/v1/teams` - List all national teams
- `GET /api/v1/matches` - List matches with filtering options
- `GET /api/v1/players` - List players with search capabilities

### Identity Management
- User registration and authentication (ASP.NET Core Identity)
- API key management (with plan validation)
- Developer account operations
- Subscription plan information

## 💳 Subscription Plans

| Feature | Free | Premium | Pro |
|---------|------|---------|-----|
| 💲 **Price/month** | $0 | $9.99 CAD | $49.99 CAD |
| ⚙️ **Requests/day** | 500 | 25,000 | 250,000 |
| ⚡ **Requests/minute** | 10 | 100 | 1,000 |
| 📄 **Endpoints** | REST (basic) | All REST | All REST + Premium Support |
| 🔑 **API Keys** | 1 | 3 | 10 |

- **Free Plan**: Perfect for testing and small projects
- **Premium Plan**: Ideal for production applications with moderate traffic
- **Pro Plan**: Enterprise-grade access with premium support

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
- [ ] Add rate limiting per API key using Redis
- [ ] Add usage analytics with Redis (queries per endpoint, top countries)
- [ ] Implement distributed caching for frequently accessed data
- [ ] Expand dataset with images (players, stadiums, flags)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🧑‍💻 About the Author

This project is part of the personal portfolio of **Antonio Felipe Souza Vieira**, a **Full-Stack Developer** based in Canada, showcasing proficiency in modern .NET 9 architecture, GraphQL, Azure integration, and CI/CD automation.

**GitHub Profile**: [https://github.com/afsvieira](https://github.com/afsvieira)

---

⭐ **If you find this project helpful, please consider giving it a star!**