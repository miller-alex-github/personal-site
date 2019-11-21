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
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the short title of the appointment.
        /// </summary>    
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment is a birthday.
        /// </summary>
        [Required]
        [Display(Name = "It is birthday")]
        public bool IsBirthday { get; set; }

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
        [Display(Name = "Date")]
        public DateTimeOffset ReferenceDate { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n day(s) before the appointment happens. 
        /// </summary>   
        [Required]
        [Display(Name = "Days before reminding")]
        public int? RemindeBeforeDays { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n week(s) before the appointment happens. 
        /// </summary>
        [Required]
        [Display(Name = "Weeks before reminding")]
        public int? RemindeBeforeWeeks { get; set; }
    }
}
