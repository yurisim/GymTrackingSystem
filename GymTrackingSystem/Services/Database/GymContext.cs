namespace GymTrackingSystem.Services.Database
{
    using Microsoft.EntityFrameworkCore;

    public class GymContext : DbContext
    {
        /// <summary>
        ///
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DbSet<Visit> Visits { get; set; }

        /// <summary>
        ///
        /// </summary>
        private const string DBName = "GymDatabase.db";

        /// <summary>
        ///
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DBName}");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Visit>().ToTable("Visits");

            // This makes the primary key of Visit be auto generated
            modelBuilder.Entity<Visit>()
                        .Property(visit => visit.Id)
                        .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
}
