namespace WorldCup.Shared.Services;

/// <summary>
/// Helper class to generate email templates
/// </summary>
public static class EmailTemplates
{
    /// <summary>
    /// Generates the email confirmation HTML template
    /// </summary>
    public static string GenerateEmailConfirmationTemplate(string userName, string confirmationLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirm Your Email</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚽ World Cup API</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Welcome, {userName}!</h2>
                            <p style='color: #666666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                Thank you for registering with World Cup API. To complete your registration and start using our services, 
                                please verify your email address by clicking the button below.
                            </p>
                            
                            <!-- Button -->
                            <table width='100%' cellpadding='0' cellspacing='0' style='margin: 30px 0;'>
                                <tr>
                                    <td align='center'>
                                        <a href='{confirmationLink}' 
                                           style='display: inline-block; padding: 14px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                                  color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 16px;'>
                                            Verify Email Address
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='color: #666666; font-size: 14px; line-height: 1.6; margin: 20px 0 0 0;'>
                                If the button doesn't work, copy and paste this link into your browser:
                            </p>
                            <p style='color: #667eea; font-size: 14px; word-break: break-all; margin: 10px 0 0 0;'>
                                {confirmationLink}
                            </p>
                            
                            <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
                            
                            <p style='color: #999999; font-size: 12px; line-height: 1.6; margin: 0;'>
                                If you didn't create an account with World Cup API, please ignore this email.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #999999; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.UtcNow.Year} World Cup API. All rights reserved.
                            </p>
                            <p style='color: #999999; font-size: 12px; margin: 10px 0 0 0;'>
                                Powered by .NET 9 & Azure
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    /// <summary>
    /// Generates a password reset email template
    /// </summary>
    public static string GeneratePasswordResetTemplate(string userName, string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚽ World Cup API</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Password Reset Request</h2>
                            <p style='color: #666666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                Hi {userName},
                            </p>
                            <p style='color: #666666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                We received a request to reset your password. Click the button below to create a new password:
                            </p>
                            
                            <!-- Button -->
                            <table width='100%' cellpadding='0' cellspacing='0' style='margin: 30px 0;'>
                                <tr>
                                    <td align='center'>
                                        <a href='{resetLink}' 
                                           style='display: inline-block; padding: 14px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                                  color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 16px;'>
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='color: #666666; font-size: 14px; line-height: 1.6; margin: 20px 0 0 0;'>
                                If the button doesn't work, copy and paste this link into your browser:
                            </p>
                            <p style='color: #667eea; font-size: 14px; word-break: break-all; margin: 10px 0 0 0;'>
                                {resetLink}
                            </p>
                            
                            <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
                            
                            <p style='color: #999999; font-size: 12px; line-height: 1.6; margin: 0;'>
                                If you didn't request a password reset, please ignore this email. Your password will remain unchanged.
                            </p>
                            <p style='color: #999999; font-size: 12px; line-height: 1.6; margin: 10px 0 0 0;'>
                                This link will expire in 24 hours.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #999999; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.UtcNow.Year} World Cup API. All rights reserved.
                            </p>
                            <p style='color: #999999; font-size: 12px; margin: 10px 0 0 0;'>
                                Powered by .NET 9 & Azure
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
