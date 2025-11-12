# Microsoft Entra External ID Configuration Guide

> **Note**: As of May 1, 2025, Azure AD B2C is no longer available for new sales. This guide uses Microsoft Entra External ID, the official replacement.

## 1. Create Microsoft Entra External ID Tenant

1. Go to Azure Portal
2. Navigate to **Microsoft Entra External ID**
3. Create a new External ID tenant
4. Note down the tenant name (e.g., `your-tenant.ciamlogin.com`)

## 2. Register Application

1. In your External ID tenant, go to "App registrations"
2. Click "New registration"
3. Set name: `WorldCup API Identity`
4. Set supported account types: "Accounts in any identity provider or organizational directory"
5. Set redirect URI: `https://localhost:7001/signin-oidc`
6. Note down the Application (client) ID

## 3. Configure External Identity Providers

1. Go to "Identity providers" in External ID
2. Add identity providers:
   - **Google**: Configure Google OAuth 2.0
   - **Microsoft**: Configure Microsoft Account
   - **GitHub**: Configure GitHub OAuth
3. Go to "User flows"
4. Create "Sign up and sign in" flow
5. Configure user attributes:
   - Collect: Email Address, Display Name
   - Return: Email Addresses, Display Name, Object ID

## 4. Configure Application Settings

Update `appsettings.json` in WorldCup.Identity project:

```json
{
  "EntraExternalId": {
    "Instance": "https://your-tenant.ciamlogin.com/",
    "Domain": "your-tenant.onmicrosoft.com", 
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "SignUpSignInPolicyId": "B2C_1_signupsignin"
  }
}
```

## 5. Configure User Secrets (Development)

```bash
dotnet user-secrets init --project src/WorldCup.Identity
dotnet user-secrets set "EntraExternalId:ClientSecret" "your-client-secret" --project src/WorldCup.Identity
```

## 6. Test Authentication Flow

1. Run the Identity project: `dotnet run --project src/WorldCup.Identity`
2. Navigate to `https://localhost:7001/graphql`
3. The GraphQL playground should require authentication
4. Users can authenticate via Google, Microsoft, or GitHub
5. After authentication, users can register (starts with Free plan) and generate API keys

## GraphQL Endpoints

### Mutations
- `registerUser` - Register authenticated user in the system (starts with Free plan)
- `generateApiKey(name: String!)` - Generate new API key for authenticated user (respects plan limits)

### Queries  
- `me` - Get current authenticated user information (includes plan type)
- `plans` - Get all available subscription plans

## Subscription Plans

| Feature | Free | Premium | Pro |
|---------|------|---------|-----|
| 💲 **Price/month** | $0 | $9.99 CAD | $49.99 CAD |
| ⚙️ **Requests/day** | 500 | 25,000 | 250,000 |
| ⚡ **Requests/minute** | 10 | 100 | 1,000 |
| 📄 **Endpoints** | REST (basic) | All REST | REST + GraphQL |
| 🔑 **API Keys** | 1 | 3 | 10 |

## API Key Usage

Generated API keys should be used in REST API calls:

```http
Authorization: Bearer <API_KEY>
```

**Note**: API key generation respects plan limits. Users cannot exceed their plan's maximum API key count.