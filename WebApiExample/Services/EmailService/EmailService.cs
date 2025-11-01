using IDGF;
using System.Net.Mail;
using System.Net;

namespace IDGFAuth.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var smtpHost = _config["SmtpSettings:Host"];
            var smtpPort = _config.GetValue<int>("SmtpSettings:Port");
            var fromAddress = _config["SmtpSettings:FromAddress"];
            var fromPassword = _config["SmtpSettings:FromPassword"];

            var mailMessage = new MailMessage(fromAddress, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation("Password reset email sent to {To}", to);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to {To}", to);
                }
            }
        }
    }
}
