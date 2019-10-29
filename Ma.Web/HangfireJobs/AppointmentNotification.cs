using Hangfire;
using Ma.Web.Services;
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
        private readonly IEmailService email;

        public AppointmentNotification(IEmailService email)
        {
            this.email = email;
        }

        public async Task Check()
        {
            try
            {
                // TODO: implement business logic here
                await email.SendEmail("to", "subject", "message");
            }
            catch (Exception exc)
            {                
                throw new Exception("Failed to send email notification.", exc);
            }            
        }
    }
}
