# WorldCup API - Web Interface

This document describes the web interface implementation in the WorldCup.API layer.

## Architecture Decision

The web interface (views, controllers for UI) has been moved from `WorldCup.Identity` to `WorldCup.API` layer. This follows best practices:

- **WorldCup.Identity**: Focuses on identity services, data models, and authentication logic
- **WorldCup.API**: Public-facing layer that includes both API endpoints and web UI

## Structure

```
WorldCup.API/
├── Controllers/
│   ├── HealthController.cs          # API health check endpoint
│   ├── HomeController.cs            # Public web pages (Home, Docs, Plans)
│   └── AccountController.cs         # User account management (Register, Login, Profile, API Keys)
├── Models/
│   ├── HealthCheckResponse.cs       # API response models
│   └── ViewModels.cs                # View models for web pages
├── Views/
│   ├── _ViewStart.cshtml            # View configuration
│   ├── _ViewImports.cshtml          # Global imports
│   ├── Shared/
│   │   ├── _Layout.cshtml           # Main layout with Bootstrap 5
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/
│   │   ├── Index.cshtml             # Landing page
│   │   ├── Documentation.cshtml     # API documentation
│   │   └── Plans.cshtml             # Pricing plans
│   └── Account/
│       ├── Register.cshtml          # User registration
│       ├── Login.cshtml             # User login
│       ├── Profile.cshtml           # User profile & statistics
│       └── ApiKeys.cshtml           # API key management
└── wwwroot/
    ├── css/
    │   └── site.css                 # Custom styles
    └── js/
        └── site.js                  # Custom JavaScript
```

## Pages

### Public Pages

1. **Home (`/`)**: Landing page showcasing API features, endpoints, and quick start guide
2. **Documentation (`/docs`)**: Complete API documentation with code examples
3. **Plans (`/plans`)**: Pricing plans comparison (Free, Premium, Pro)

### Account Pages (Requires Authentication)

1. **Register (`/account/register`)**: New user registration
2. **Login (`/account/login`)**: User authentication
3. **Profile (`/account/profile`)**: User profile with usage statistics
4. **API Keys (`/account/apikeys`)**: Create and manage API keys

## Features

### Design
- Modern, clean interface using Bootstrap 5
- Responsive design (mobile-friendly)
- Consistent branding with logo and color scheme
- Custom CSS animations and transitions

### Functionality
- User registration and authentication
- API key generation and management
- Usage statistics tracking
- Plan-based limits enforcement
- Secure password handling
- Remember me functionality

### Security
- CSRF protection with anti-forgery tokens
- Encrypted API keys
- Secure cookie configuration
- Input validation
- SQL injection protection (via Entity Framework)

## Configuration

### Required Services (Program.cs)

```csharp
// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { ... })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// MVC with Views
builder.Services.AddControllersWithViews();
```

### Connection String (appsettings.Development.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorldCupIdentity;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Running the Application

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Apply migrations** (from WorldCup.Identity project):
   ```bash
   cd src/WorldCup.Identity
   dotnet ef database update
   ```

3. **Run the API**:
   ```bash
   cd src/WorldCup.API
   dotnet run
   ```

4. **Access the application**:
   - Web Interface: https://localhost:7001
   - Swagger API Docs: https://localhost:7001/swagger

## API Key Usage

After registering and creating an API key:

```bash
curl -X GET "https://localhost:7001/api/v1/health" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

## Plan Limits

| Plan | Price | Daily Requests | Per Minute | API Keys |
|------|-------|----------------|------------|----------|
| Free | $0 | 500 | 10 | 1 |
| Premium | $9.99 CAD | 25,000 | 100 | 3 |
| Pro | $49.99 CAD | 250,000 | 1,000 | 10 |

## Development Notes

- All code, comments, and UI text are in English
- ViewModels are defined in `Models/ViewModels.cs`
- Custom styles in `wwwroot/css/site.css`
- JavaScript utilities in `wwwroot/js/site.js`
- Bootstrap 5.3.2 loaded via CDN
- Bootstrap Icons for UI elements

## Future Enhancements

- Email verification
- Password reset functionality
- OAuth integration (Google, GitHub)
- Usage analytics dashboard
- Rate limiting middleware
- API key rotation
- Webhook notifications
