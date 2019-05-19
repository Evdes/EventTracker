using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventTracker.DAL.SqlData
{
    public class EventTrackerDbContext : IdentityDbContext<UserProfile>
    {
        public EventTrackerDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().HasOne(e => e.Location);
            modelBuilder.Entity<Event>().HasMany(e => e.Timeframes);

            modelBuilder.Entity<UserEvents>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            modelBuilder.Entity<UserEvents>()
                .HasOne(ue => ue.UserProfile)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<UserEvents>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
