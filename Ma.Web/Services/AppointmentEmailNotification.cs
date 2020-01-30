using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Text;
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
        private readonly IAppointmentsAPI appointmentsAPI;

        public AppointmentEmailNotification(IEmailSender emailSender, IAppointmentsAPI appointmentsAPI)
        {
            this.emailSender = emailSender;
            this.appointmentsAPI = appointmentsAPI;
        }

        public async Task CheckAppointment()
        {   
            try
            {
                var upcomingAppointments = await appointmentsAPI.FindUpcomingAppointmentsAsync(period: 5, pageSize: 30);
                if (upcomingAppointments == null || upcomingAppointments.TotalCount == 0)
                    return;

                var sb = new StringBuilder();
                sb.AppendLine("<h2>Upcoming appointments:</h2>");
                sb.AppendLine("<ul>");
                foreach (var appointment in upcomingAppointments.Data)
                {
                    sb.Append("<li>")
                      .Append(appointment.Title)
                      .Append(" <strong> ")
                      .Append(appointment.Date.LocalDateTime.ToShortDateString())
                      .Append(" </strong> ")
                      .AppendLine("</li>");                    
                }
                sb.AppendLine("</ul>");

                await emailSender.SendEmailAsync("info@miller-alex.de", "Upcoming appointments", sb.ToString());
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to send email notification.", exc);
            }            
        }
    }
}
