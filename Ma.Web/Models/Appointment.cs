using System;
using System.ComponentModel.DataAnnotations;

namespace Ma.Web
{
    public class Appointment
    {
        /// <summary>
        /// Gets or sets the unique id of this appointment.
        /// </summary>          
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user id who has created this appointment.
        /// </summary>          
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the short title of the appointment.
        /// </summary>    
        [Required]
        public string Title { get; set; }
        
        /// <summary>
        /// Specifies how often the appointment should be re-evaluated.
        /// </summary>
        [Required]        
        public AppointmentRepetitions Repetitions { get; set; }

        /// <summary>
        /// Gets or sets the notes to the appointment.
        /// </summary>        
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the reference date used for appointment. 
        /// It can be fixed date or the date of birth.        
        /// </summary>
        [Required]
        public DateTimeOffset ReferenceDate { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n day(s) before the appointment happens. 
        /// </summary>   
        [Required]
        public int? RemindeBeforeDays { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n week(s) before the appointment happens. 
        /// </summary>
        [Required]
        public int? RemindeBeforeWeeks { get; set; }
    }
}
