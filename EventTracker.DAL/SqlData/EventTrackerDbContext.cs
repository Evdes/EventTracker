using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
            modelBuilder.Entity<Event>().HasMany(e => e.Participants);
            modelBuilder.Entity<Event>().HasMany(e => e.Timeframes);
            base.OnModelCreating(modelBuilder);

        }
    }

}
