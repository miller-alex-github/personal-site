using System;

namespace Ma.Web
{
    /// <summary>
    /// Values for appointment evaluation frequency.
    /// </summary>
    [Serializable]
    public enum AppointmentRepetitions : byte
    {        
        Never   = 0,
        Daily   = 1,
        Weekly  = 2,
        Monthly = 3,
        Yearly  = 4
    }
}