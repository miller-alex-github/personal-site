using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Ma.Web.Services
{
    public class EmailSender : IEmailSender
    {        
        private readonly ILogger<EmailSender> logger;
        private readonly IConfiguration configuration;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {           
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = configuration["Email:Email"],
                    Password = configuration["Email:Password"]
                };

                smtp.Credentials = credential;
                smtp.Host = configuration["Email:Host"];
                smtp.Port = int.Parse(configuration["Email:Port"]);
                smtp.EnableSsl = true;

                using (var mail = new MailMessage())
                {
                    mail.To.Add(new MailAddress(email));
                    mail.From = new MailAddress(configuration["Email:Email"], configuration["Email:DisplayName"]);
                    mail.Subject = subject;
                    mail.Body = htmlMessage;
                    mail.IsBodyHtml = true;

                    await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(2), (ex, ts) => { logger.LogError("Error sending mail. Retrying in 2 sec."); })
                        .Execute(() => smtp.SendMailAsync(mail))
                        .ContinueWith(_ => logger.LogInformation($"Notification mail sent to: {email}, subject: {subject}, message: {htmlMessage}"));
                }
            }
        }
    }
}
