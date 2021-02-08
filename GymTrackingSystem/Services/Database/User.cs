using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymTrackingSystem.Services.Database
{
    public class User
    {
        /// <summary>
        ///     This the primary Key for the User class. It is NOT an int because DoDIDs are 10 digits long and ints won't cover
        ///     DoDIDs larger than 2.1 mil. It is not a string because it's faster to check for equality comparisons for numbers
        ///     than it is for strings.
        /// </summary>
        [Key]
        public long Id { get; set; }
        
        public string LastName { get; set; }

        public string Unit { get; set; }

        public string Phone { get; set; }

        public ICollection<Visit> Visits { get; set; }
    }
}