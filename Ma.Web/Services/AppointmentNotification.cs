using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
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
        private readonly ILogger<AppointmentNotification> logger;
        private readonly IEmailSender email;
        private readonly IConfiguration configuration;

        public AppointmentNotification(ILogger<AppointmentNotification> logger, IEmailSender email, IConfiguration configuration)
        {            
            this.logger = logger;
            this.email = email;
            this.configuration = configuration;
        }

        public async Task Check()
        {
            var to      = configuration["Admin:Email"];
            var subject = "Appointment Notification";
            var message = DateTime.Now.ToString("g");

            // TODO: implement business logic here

            try
            {                
                await email.SendEmailAsync(to, subject, message);
            }
            catch (Exception exc)
            {                
                throw new Exception("Failed to send email notification.", exc);
            }            
        }
    }
}
