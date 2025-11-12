# Azure AD B2C Configuration Guide

## 1. Create Azure AD B2C Tenant

1. Go to Azure Portal
2. Create a new Azure AD B2C tenant
3. Note down the tenant name (e.g., `your-tenant.onmicrosoft.com`)

## 2. Register Application

1. In your B2C tenant, go to "App registrations"
2. Click "New registration"
3. Set name: `WorldCup API Identity`
4. Set redirect URI: `https://localhost:7001/signin-oidc`
5. Note down the Application (client) ID

## 3. Create User Flow

1. Go to "User flows" in B2C
2. Click "New user flow"
3. Select "Sign up and sign in"
4. Choose "Recommended" version
5. Name: `B2C_1_signupsignin`
6. Configure identity providers:
   - **Google**: Add Google as identity provider
   - **Microsoft**: Add Microsoft Account as identity provider  
   - **GitHub**: Add GitHub as identity provider
7. Configure user attributes and claims:
   - Collect: Email Address, Display Name
   - Return: Email Addresses, Display Name, Object ID

## 4. Configure Application Settings

Update `appsettings.json` in WorldCup.Identity project:

```json
{
  "AzureAdB2C": {
    "Instance": "https://your-tenant.b2clogin.com/",
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
dotnet user-secrets set "AzureAdB2C:ClientSecret" "your-client-secret" --project src/WorldCup.Identity
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