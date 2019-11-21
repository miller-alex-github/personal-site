using System;

namespace Ma.Services.Appointments
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
        YearlyAsBirthday = 5
    }
}