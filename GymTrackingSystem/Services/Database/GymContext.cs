using Microsoft.EntityFrameworkCore;

namespace GymTrackingSystem.Services.Database
{
    public class GymContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Visit> Visits { get; set; }

        private const string DBName = "GymDatabase.db";

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
