# Email Verification Configuration Guide

## Overview

The World Cup API includes a complete email verification system to ensure user email addresses are valid and secure. This guide explains how to configure and use the email verification feature.

## Features

### Email Sender Service
- `IEmailSender` interface for email operations
- `AzureEmailSender` implementation using Azure Communication Services
- `SmtpEmailSender` implementation using SMTP (for development)
- Professional HTML email templates
- Error handling and logging

### Email Verification Flow
1. User registers account
2. Email confirmation token generated
3. Professional email sent with confirmation link
4. User clicks link to verify email
5. Account marked as verified

### User Experience
- Profile page shows verification status badge
- "Resend Verification Email" button for unverified accounts
- Professional email templates with branding
- Success/error messages

## Configuration

The application supports two email providers: **Azure Communication Services** (recommended) and **SMTP** (development only).

### Choosing Email Provider

Set the provider in `appsettings.json`:

```json
{
  "Email": {
    "Provider": "Azure",  // or "Smtp"
    ...
  }
}
```

### Option 1: Azure Communication Services (Recommended)

**Benefits:**
- High deliverability rates
- Built-in monitoring and analytics
- Managed infrastructure
- Scalable and reliable

**Setup Steps:**

1. **Create Azure Communication Services resource**:
   ```bash
   az communication create \
     --name "your-communication-service" \
     --resource-group "your-rg" \
     --data-location "unitedstates" \
     --location "canadacentral"
   ```

2. **Get your connection string** from Azure Portal:
   - Go to your Communication Services resource
   - Navigate to "Keys"
   - Copy the connection string

3. **Configure in User Secrets** (recommended for development):
   ```bash
   cd src/WorldCup.API
   dotnet user-secrets set "Email:Provider" "Azure"
   dotnet user-secrets set "Email:Azure:ConnectionString" "endpoint=https://...;accesskey=..."
   dotnet user-secrets set "Email:Azure:FromEmail" "DoNotReply@your-domain.azurecomm.net"
   ```

4. **Or configure in appsettings.json** (use environment variables for production):
   ```json
   {
     "Email": {
       "Provider": "Azure",
       "Azure": {
         "ConnectionString": "",
         "FromEmail": "DoNotReply@your-domain.azurecomm.net"
       }
     }
   }
   ```

**Using Custom Domain:**

After registering your domain, configure it in Azure Communication Services:

1. Add custom domain in Azure Portal (Email Communication Service)
2. Configure DNS records (SPF, DKIM, DMARC)
3. Update sender email to your domain (e.g., `noreply@yourdomain.com`)

### Option 2: SMTP (Development Only)

**Gmail Setup:**

1. Create Gmail App Password:
   - Enable 2FA on Google Account
   - Generate App Password in Security settings

2. Configure in User Secrets:
   ```bash
   cd src/WorldCup.API
   dotnet user-secrets set "Email:Provider" "Smtp"
   dotnet user-secrets set "Email:Smtp:Username" "your-email@gmail.com"
   dotnet user-secrets set "Email:Smtp:Password" "your-app-password"
   ```

**Configuration:**
```json
{
  "Email": {
    "Provider": "Smtp",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "",
      "Password": ""
    }
  }
}
```

## Email Templates

The system includes professional HTML email templates with:
- Gradient header with branding
- Clear call-to-action button
- Fallback text link
- Responsive design
- Footer with copyright information

Templates available:
- **Email Confirmation** - Sent on user registration
- **Password Reset** - Ready for future implementation

## API Endpoints

### Confirm Email
```
GET /account/confirm-email?userId={userId}&token={token}
```
Validates email confirmation token and updates account status.

### Resend Confirmation Email
```
POST /account/resend-confirmation
[Authorize]
```
Generates new token and resends confirmation email.

## User Registration Flow

1. User completes registration form
2. Account created in database
3. Email confirmation token generated
4. Professional email sent with confirmation link
5. User signed in (email not yet confirmed)
6. User sees "Not Verified" badge
7. User clicks link in email
8. Email confirmed → "Verified" badge
9. Full account access granted

## Development Testing

### Without Email Configuration

The system gracefully handles missing configuration:
- Logs warning message
- Skips email sending
- Continues normal operation
- Users can still register and login

### Testing Email Verification

1. Configure email provider (Azure or SMTP)
2. Register with a real email you control
3. Check inbox for verification email
4. Click confirmation link
5. Verify badge changes to "Verified"

## Production Deployment

### Recommended Setup

1. **Use Azure Communication Services**
2. **Store secrets in Azure Key Vault**:
   ```bash
   az keyvault secret set \
     --vault-name "your-keyvault" \
     --name "EmailConnectionString" \
     --value "endpoint=https://...;accesskey=..."
   ```

3. **Configure App Service**:
   ```bash
   az webapp config appsettings set \
     --name "your-app" \
     --resource-group "your-rg" \
     --settings \
       Email__Provider="Azure" \
       Email__Azure__ConnectionString="@Microsoft.KeyVault(SecretUri=https://...)" \
       Email__Azure__FromEmail="noreply@yourdomain.com"
   ```

4. **Enable Managed Identity** for Key Vault access

## Monitoring

### Application Logs

The system logs email-related events:

```
[INF] Email sent successfully to user@example.com
[INF] Email confirmed for user user@example.com
[ERR] Failed to send email to user@example.com
```

### Azure Portal (for Azure Communication Services)

Monitor email delivery:
1. Go to your Email Communication Service
2. Click "Metrics"
3. View: Emails sent, Delivery status, Failures

## Troubleshooting

### Emails Not Sending

1. Check email configuration in User Secrets/appsettings
2. Verify connection string/credentials are correct
3. Check application logs for error messages
4. For Azure: Check service status in Azure Portal
5. For SMTP: Verify port 587 is not blocked

### Confirmation Link Not Working

1. Check token encoding/decoding
2. Verify URL is complete and not truncated
3. Ensure token hasn't expired
4. Review error logs

### Gmail Authentication Errors

1. Verify 2FA is enabled
2. Use App Password, not account password
3. Generate new App Password if needed
4. Check Gmail security settings

## Security Considerations

- Tokens use ASP.NET Core Identity secure token generation
- Tokens expire after reasonable time
- All confirmation links use HTTPS
- Tokens validated server-side
- Failed confirmation attempts logged

## Related Documentation

- [ASP.NET Core Identity Email Confirmation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm)
- [Azure Communication Services](https://docs.microsoft.com/en-us/azure/communication-services/)
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
