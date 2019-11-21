using System;
using System.ComponentModel.DataAnnotations;

namespace Ma.Web
{
    /// <summary>
    /// Values for appointment evaluation frequency.
    /// </summary>
    [Serializable]
    public enum AppointmentRepetitions : byte
    {
        Never = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
        /// <summary>
        /// Indicating whether the appointment is a birthday
        /// </summary>
        [Display(Name = "Birthday")]
        YearlyAsBirthday = 5
    }
}