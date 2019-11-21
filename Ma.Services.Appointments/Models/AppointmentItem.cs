using System;
using System.ComponentModel.DataAnnotations;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents an appointment item.
    /// </summary>
    public class AppointmentItem
    {
        /// <summary>
        /// Gets or sets the unique id of this appointment.
        /// </summary>          
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user id who has created this appointment.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the short title of the appointment.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
                
        /// <summary>
        /// Specifies how often the appointment should be re-evaluated.
        /// </summary>
        public AppointmentRepetitions Repetitions { get; set; }

        /// <summary>
        /// Gets or sets the notes to the appointment.
        /// </summary>        
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the reference date used for appointment. 
        /// It can be fixed date or the date of birth.        
        /// </summary>
        public DateTimeOffset ReferenceDate { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n day(s) before the appointment happens. 
        /// </summary>       
        public int? RemindeBeforeDays { get; set; }

        /// <summary>
        /// Gets or sets the value which will be used to remind 
        /// in n week(s) before the appointment happens. 
        /// </summary>
        public int? RemindeBeforeWeeks { get; set; }
                
        /// <summary>
        /// Get the numbers of days left until the appointment.
        /// </summary>
        public double DaysLeft => (DateTime.UtcNow - ReferenceDate).TotalDays;

        /// <summary>
        /// Get the numbers of week left until the appointment.
        /// </summary>
        public double WeeksLeft => DaysLeft / 7 /* days a week */;

        /// <summary>
        /// Returns true if the appointment is today.
        /// </summary>
        public bool IsToday => DaysLeft < 0;

        /// <summary>
        /// Returns true if the appointment need to be reminded now.
        /// </summary>
        public bool IsNeedToRemindNow => IsToday || DaysLeft < RemindeBeforeDays || WeeksLeft < RemindeBeforeWeeks; 
    }
}