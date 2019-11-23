using System;

namespace Ma.Services.Appointments.DTO
{
    /// <summary>
    /// Represents an appointment DTO.
    /// </summary>
    [Serializable]
    public class Appointment
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