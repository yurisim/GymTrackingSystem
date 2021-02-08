using System;

namespace GymTrackingSystem.Services.Database
{
    public class Visit
    {
        /// <summary>
        /// Do not make the Visit.ID property REQUIRED or KEY. It is autoincremented and therefore does not need such a property
        /// </summary>
        public int Id { get; set; }

        public User User { get; set; }

        public DateTime DateTimeIn { get; set; }

        public DateTime DateTimeOut { get; set; }

        public string BadgeColor { get; set; }

        public int BadgeNumber { get; set; }
    }
}
