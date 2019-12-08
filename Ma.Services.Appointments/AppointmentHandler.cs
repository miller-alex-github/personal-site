using System;
using System.Collections.Generic;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents a handler of appointments.
    /// </summary>
    public class AppointmentHandler
    {
        /// <summary>
        /// Parse a file to appointments.
        /// </summary>
        /// <param name="userId">User ID who creates the appointments.</param>
        /// <param name="text">A text of appointments.</param>
        internal static Appointment[] ParseFile(Guid userId, string text)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId), "User Id is not available!");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text), "Text is not available!");

            var lines = text.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<Appointment>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Length < 10)
                    continue;

                var textDate = line.Substring(0, 10);                
                var textTitle = line.Substring(10).Trim();
                
                if (DateTimeOffset.TryParse(textDate, out DateTimeOffset date))
                {
                    result.Add(new Appointment
                    {
                        Id     = Guid.NewGuid(),
                        Title  = textTitle,
                        Date   = date,
                        UserId = userId
                    });
                }                
            }

            return result.ToArray();
        }
    }
}
