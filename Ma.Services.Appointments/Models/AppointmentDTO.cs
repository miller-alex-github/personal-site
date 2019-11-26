using System;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents an appointment DTO.
    /// </summary>
    [Serializable]
    public class AppointmentDTO
    {        
        /// <summary>
        /// Gets or sets the short title of the appointment.
        /// </summary>    
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the reference date used for appointment.
        /// </summary>
        public DateTimeOffset Date { get; set; }
    }
}