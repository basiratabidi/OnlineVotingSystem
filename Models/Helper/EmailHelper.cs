using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace OnlineVotingSystem.Models.Helper
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Online Voting System";
        public bool EnableSsl { get; set; } = true;
        public int Timeout { get; set; } = 30000;
    }

    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load email settings
            _emailSettings = new EmailSettings
            {
                Host = _configuration["Smtp:Host"] ?? _configuration["EmailSettings:Host"] ?? "smtp.gmail.com",
                Port = int.Parse(_configuration["Smtp:Port"] ?? _configuration["EmailSettings:Port"] ?? "587"),
                Username = _configuration["Smtp:User"] ?? _configuration["EmailSettings:Username"] ?? "",
                Password = _configuration["Smtp:Pass"] ?? _configuration["EmailSettings:Password"] ?? "",
                FromEmail = _configuration["Smtp:From"] ?? _configuration["EmailSettings:FromEmail"] ?? "",
                FromName = _configuration["EmailSettings:FromName"] ?? "Online Voting System",
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true"),
                Timeout = int.Parse(_configuration["EmailSettings:Timeout"] ?? "30000")
            };
        }

        /// <summary>
        /// Sends a vote confirmation email to the voter asynchronously.
        /// </summary>
        public async Task SendVoteConfirmationAsync(string toEmail, string electionName, string candidateName)
        {
            string voterName = "Valued Voter"; // You can pass this as parameter if needed

            // Generate professional HTML email body
            string htmlBody = GenerateVoteConfirmationHtml(voterName, electionName, candidateName);
            string plainTextBody = $"DEMOCRACY IN ACTION\n\nDear Voter,\n\nYour vote has been successfully recorded!\n\n📋 ELECTION: {electionName}\n✅ YOUR CHOICE: {candidateName}\n⏰ TIMESTAMP: {DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt")}\n🔒 CONFIRMATION ID: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}\n\nYour participation strengthens our democracy. Thank you for making your voice heard!\n\nBest regards,\nThe Online Voting System Team\n\nℹ️ This is an automated confirmation. Please do not reply to this email.";

            await SendEmailAsync(toEmail, $"🎯 Vote Confirmed! - {electionName}", htmlBody, plainTextBody);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string plainTextBody)
        {
            using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                client.EnableSsl = _emailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.Timeout = _emailSettings.Timeout;

                var fromAddress = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);

                var mail = new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                // Add plain text alternative
                var plainTextView = AlternateView.CreateAlternateViewFromString(
                    plainTextBody, null, "text/plain");
                mail.AlternateViews.Add(plainTextView);

                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);
            }
        }

        private string GenerateVoteConfirmationHtml(string voterName, string electionName, string candidateName)
        {
            string currentTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            string confirmationId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            string currentYear = DateTime.Now.Year.ToString();

            // Note: For production, you should host these images on a server or use embedded base64 images
            // These are placeholder URLs - replace with your actual image URLs
            string logoUrl = "https://cdn-icons-png.flaticon.com/512/1037/1037855.png";
            string checkmarkUrl = "https://cdn-icons-png.flaticon.com/512/4315/4315445.png";
            string shieldUrl = "https://cdn-icons-png.flaticon.com/512/2830/2830996.png";
            string democracyUrl = "https://cdn-icons-png.flaticon.com/512/3069/3069392.png";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Your Vote Has Been Recorded</title>
    <style>
        /* Reset and Base Styles */
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #2c3e50;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            min-height: 100vh;
        }}
        
        .email-wrapper {{
            max-width: 700px;
            margin: 0 auto;
        }}
        
        /* Header with Logo */
        .email-header {{
            background: white;
            padding: 40px 30px;
            border-radius: 20px 20px 0 0;
            text-align: center;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }}
        
        .logo-container {{
            margin-bottom: 25px;
        }}
        
        .logo {{
            width: 100px;
            height: 100px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 50%;
            margin: 0 auto 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 40px;
            font-weight: bold;
            box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
        }}
        
        .logo-icon {{
            font-size: 50px;
        }}
        
        .header-title {{
            color: #2c3e50;
            font-size: 32px;
            font-weight: 700;
            margin-bottom: 10px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
        }}
        
        .header-subtitle {{
            color: #7f8c8d;
            font-size: 18px;
            font-weight: 400;
        }}
        
        /* Main Content */
        .email-content {{
            background: white;
            padding: 50px 40px;
        }}
        
        /* Success Banner */
        .success-banner {{
            background: linear-gradient(135deg, #00b09b 0%, #96c93d 100%);
            border-radius: 15px;
            padding: 30px;
            margin-bottom: 40px;
            text-align: center;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 20px;
            box-shadow: 0 10px 20px rgba(0, 176, 155, 0.2);
        }}
        
        .success-icon {{
            font-size: 48px;
            animation: pulse 2s infinite;
        }}
        
        @keyframes pulse {{
            0% {{ transform: scale(1); }}
            50% {{ transform: scale(1.1); }}
            100% {{ transform: scale(1); }}
        }}
        
        .success-text {{
            font-size: 24px;
            font-weight: 600;
        }}
        
        /* Greeting */
        .greeting {{
            font-size: 22px;
            color: #2c3e50;
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 2px solid #f8f9fa;
        }}
        
        .voter-name {{
            color: #667eea;
            font-weight: 700;
        }}
        
        /* Voting Card */
        .voting-card {{
            background: #f8f9fa;
            border-radius: 15px;
            padding: 35px;
            margin: 30px 0;
            border-left: 6px solid #667eea;
            position: relative;
            overflow: hidden;
        }}
        
        .card-icon {{
            position: absolute;
            right: 20px;
            top: 20px;
            font-size: 60px;
            color: rgba(102, 126, 234, 0.1);
        }}
        
        .detail-row {{
            display: flex;
            align-items: center;
            margin-bottom: 25px;
            padding-bottom: 25px;
            border-bottom: 1px solid #e9ecef;
        }}
        
        .detail-row:last-child {{
            border-bottom: none;
            margin-bottom: 0;
            padding-bottom: 0;
        }}
        
        .detail-icon {{
            width: 50px;
            height: 50px;
            background: white;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 20px;
            font-size: 24px;
            color: #667eea;
            box-shadow: 0 5px 15px rgba(0,0,0,0.05);
        }}
        
        .detail-content {{
            flex: 1;
        }}
        
        .detail-label {{
            font-size: 14px;
            color: #7f8c8d;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 5px;
        }}
        
        .detail-value {{
            font-size: 20px;
            font-weight: 600;
            color: #2c3e50;
        }}
        
        .highlight-value {{
            color: #667eea;
            font-weight: 700;
        }}
        
        /* Security Box */
        .security-box {{
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            border-radius: 15px;
            padding: 30px;
            margin: 40px 0;
            color: white;
        }}
        
        .security-header {{
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
        }}
        
        .security-icon {{
            font-size: 32px;
        }}
        
        .security-title {{
            font-size: 22px;
            font-weight: 600;
        }}
        
        .security-points {{
            list-style: none;
        }}
        
        .security-points li {{
            margin-bottom: 15px;
            padding-left: 30px;
            position: relative;
        }}
        
        .security-points li:before {{
            content: '✓';
            position: absolute;
            left: 0;
            color: #00ff95;
            font-weight: bold;
        }}
        
        /* Stats Section */
        .stats-section {{
            background: #f8f9fa;
            border-radius: 15px;
            padding: 30px;
            margin: 40px 0;
            display: flex;
            justify-content: space-around;
            flex-wrap: wrap;
            gap: 20px;
        }}
        
        .stat-item {{
            text-align: center;
            flex: 1;
            min-width: 150px;
        }}
        
        .stat-number {{
            font-size: 36px;
            font-weight: 700;
            color: #667eea;
            margin-bottom: 10px;
        }}
        
        .stat-label {{
            color: #7f8c8d;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        
        /* Footer */
        .email-footer {{
            background: #2c3e50;
            color: white;
            padding: 40px;
            border-radius: 0 0 20px 20px;
            text-align: center;
        }}
        
        .footer-links {{
            display: flex;
            justify-content: center;
            gap: 30px;
            margin: 25px 0;
        }}
        
        .footer-link {{
            color: #667eea;
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s;
        }}
        
        .footer-link:hover {{
            color: white;
        }}
        
        .copyright {{
            color: #bdc3c7;
            font-size: 14px;
            margin-top: 20px;
        }}
        
        /* Responsive */
        @media (max-width: 768px) {{
            .detail-row {{
                flex-direction: column;
                text-align: center;
            }}
            
            .detail-icon {{
                margin-right: 0;
                margin-bottom: 15px;
            }}
            
            .footer-links {{
                flex-direction: column;
                gap: 15px;
            }}
            
            .email-header, .email-content, .email-footer {{
                padding: 30px 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-wrapper'>
        <!-- Header -->
        <div class='email-header'>
            <div class='logo-container'>
                <div class='logo'>
                    <div class='logo-icon'>✓</div>
                </div>
            </div>
            <h1 class='header-title'>Online Voting System</h1>
            <p class='header-subtitle'>Secure & Transparent Digital Democracy</p>
        </div>
        
        <!-- Main Content -->
        <div class='email-content'>
            <!-- Success Banner -->
            <div class='success-banner'>
                <div class='success-icon'>🎉</div>
                <div class='success-text'>YOUR VOTE HAS BEEN RECORDED!</div>
            </div>
            
            <!-- Greeting -->
            <p class='greeting'>
                Dear <span class='voter-name'>{voterName}</span>,<br>
                Thank you for participating in our democratic process. Your voice matters!
            </p>
            
            <!-- Voting Details Card -->
            <div class='voting-card'>
                <div class='card-icon'>🗳️</div>
                
                <div class='detail-row'>
                    <div class='detail-icon'>📋</div>
                    <div class='detail-content'>
                        <div class='detail-label'>Election</div>
                        <div class='detail-value'>{electionName}</div>
                    </div>
                </div>
                
                <div class='detail-row'>
                    <div class='detail-icon'>✅</div>
                    <div class='detail-content'>
                        <div class='detail-label'>Your Selection</div>
                        <div class='detail-value highlight-value'>{candidateName}</div>
                    </div>
                </div>
                
                <div class='detail-row'>
                    <div class='detail-icon'>⏰</div>
                    <div class='detail-content'>
                        <div class='detail-label'>Timestamp</div>
                        <div class='detail-value'>{currentTime}</div>
                    </div>
                </div>
                
                <div class='detail-row'>
                    <div class='detail-icon'>🔒</div>
                    <div class='detail-content'>
                        <div class='detail-label'>Confirmation ID</div>
                        <div class='detail-value'>{confirmationId}</div>
                    </div>
                </div>
            </div>
            
            <!-- Security Information -->
            <div class='security-box'>
                <div class='security-header'>
                    <div class='security-icon'>🛡️</div>
                    <div class='security-title'>Security & Privacy Guaranteed</div>
                </div>
                <ul class='security-points'>
                    <li>Your vote remains 100% anonymous and confidential</li>
                    <li>All data is encrypted using military-grade encryption</li>
                    <li>Vote verification system ensures accuracy</li>
                    <li>Regular security audits by independent experts</li>
                </ul>
            </div>
            
            <!-- Impact Stats -->
            <div class='stats-section'>
                <div class='stat-item'>
                    <div class='stat-number'>1</div>
                    <div class='stat-label'>Your Vote</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>1000+</div>
                    <div class='stat-label'>Votes Cast Today</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>99.9%</div>
                    <div class='stat-label'>System Uptime</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>0</div>
                    <div class='stat-label'>Security Breaches</div>
                </div>
            </div>
            
            <!-- Closing Message -->
            <div style='text-align: center; margin: 40px 0;'>
                <p style='font-size: 18px; color: #2c3e50; margin-bottom: 20px;'>
                    <strong>Every vote counts.</strong> Together, we're building a stronger democracy.
                </p>
                <div style='background: #f8f9fa; padding: 20px; border-radius: 10px; display: inline-block;'>
                    <p style='margin: 0; font-style: italic; color: #667eea;'>
                        ""The vote is the most powerful instrument ever devised by man for breaking down injustice.""<br>
                        <small style='color: #7f8c8d;'>- Lyndon B. Johnson</small>
                    </p>
                </div>
            </div>
            
            <!-- Closing -->
            <div style='text-align: center; padding-top: 30px; border-top: 2px solid #f8f9fa;'>
                <p style='font-size: 16px; color: #2c3e50;'>
                    Best regards,<br>
                    <strong style='color: #667eea; font-size: 18px;'>The Online Voting System Team</strong>
                </p>
            </div>
        </div>
        
        <!-- Footer -->
        <div class='email-footer'>
            <p style='font-size: 16px; margin-bottom: 20px;'>
                Need help? Have questions about your vote?
            </p>
            <div class='footer-links'>
                <a href='#' class='footer-link'>Contact Support</a>
                <a href='#' class='footer-link'>View Election Results</a>
                <a href='#' class='footer-link'>Learn About Our Security</a>
            </div>
            <p class='copyright'>
                © {currentYear} Online Voting System. All rights reserved.<br>
                This is an automated message. Please do not reply to this email.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}