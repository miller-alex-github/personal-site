using Hangfire;
using Ma.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Ma.Web.HangfireJobs
{
    /// <summary>
    /// The appointment notification will keep subscribers informed about their appointments.
    /// </summary> 
    public interface IAppointmentNotification
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        Task Check();
    }
       
    public class AppointmentNotification : IAppointmentNotification
    {
        private readonly IHostingEnvironment environment;
        private readonly ILogger<AppointmentNotification> logger;
        private readonly IEmailService email;

        public AppointmentNotification(IHostingEnvironment environment, ILogger<AppointmentNotification> logger, IEmailService email)
        {
            this.environment = environment;
            this.logger = logger;
            this.email = email;
        }

        public async Task Check()
        {
            var to      = "";
            var subject = "Appointment Notification";
            var message = DateTime.Now.ToString("g");

            // TODO: implement business logic here

            if (environment.IsDevelopment())
            {
                logger.LogInformation($"Send email to: {to}, subject: {subject}, message: {message}");
                return; 
            }

            try
            {                
                await email.SendEmail(to, subject, message);
            }
            catch (Exception exc)
            {                
                throw new Exception("Failed to send email notification.", exc);
            }            
        }
    }
}
