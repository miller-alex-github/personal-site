using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;

namespace Ma.Web.Services
{
    /// <summary>
    /// The appointment notification will keep subscribers informed about their appointments.
    /// </summary> 
    public interface IAppointmentEmailNotification
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        Task CheckAppointment();
    }
       
    public class AppointmentEmailNotification : IAppointmentEmailNotification
    {
        private readonly IEmailSender emailSender;

        public AppointmentEmailNotification(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task CheckAppointment()
        {          
            // TODO: implement here the business logic.

            try
            {
                await emailSender.SendEmailAsync("info@miller.alex.de", "Appointment", "<h1>Test</h1>");
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to send email notification.", exc);
            }            
        }
    }
}
