namespace Ma.Web.Models
{
    /// <summary>
    /// Represents a ViewModel for appointments to store and manage UI-related data.
    /// </summary>
    public class AppointmentViewModel
    {
        /// <summary>
        /// Gets or sets the list of appointments. 
        /// </summary>
        public AppointmentItem[] Items { get; set; }
    }
}
