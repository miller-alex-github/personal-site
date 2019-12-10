using System;
using System.ComponentModel.DataAnnotations;

namespace Ma.Web
{
    public class Appointment
    {
        /// <summary>
        /// Gets or sets the unique id of this appointment.
        /// </summary>   
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user id who has created this appointment.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the short title of the appointment.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the reference date used for appointment  
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = @"{0:yyyy\/MM\/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Date { get; set; } = DateTime.UtcNow;
    }
}
