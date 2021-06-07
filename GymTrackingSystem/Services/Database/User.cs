namespace GymTrackingSystem.Services.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        /// <summary>
        ///     This the primary Key for the User class. It is NOT an int because DoDIDs
        ///     are 10 digits long and ints won't cover DoDIDs larger than 2.1 bil.
        /// </summary>
        [Key]
        public long Id { get; set; }

        public string LastName { get; set; }

        public string Unit { get; set; }

        public string Phone { get; set; }

        //
        /// <summary>
        /// Creates a 1 to many relationship with Visits
        /// </summary>
        public ICollection<Visit> Visits { get; set; }
    }
}