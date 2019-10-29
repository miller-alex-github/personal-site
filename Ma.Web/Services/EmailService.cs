using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Ma.Web.Services
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly IConfiguration configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task SendEmail(string to, string subject, string message)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = configuration["Email:Email"],
                    Password = configuration["Email:Password"]
                };

                smtp.Credentials = credential;
                smtp.Host        = configuration["Email:Host"];
                smtp.Port        = int.Parse(configuration["Email:Port"]);
                smtp.EnableSsl   = true;

                using (var mail = new MailMessage())
                {
                    mail.To.Add(new MailAddress(to));
                    mail.From    = new MailAddress(configuration["Email:Email"]);
                    mail.Subject = subject;
                    mail.Body    = message;
                    smtp.Send(mail);
                }
            }

            await Task.CompletedTask;
        }
    }
}
