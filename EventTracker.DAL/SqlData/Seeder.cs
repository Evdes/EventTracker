using EventTracker.Models.Enums;
using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventTracker.DAL.SqlData
{
    public class Seeder
    {
        public static void Seed(EventTrackerDbContext _context, UserManager<UserProfile> _userManager)
        {
            _context.Database.EnsureCreated();

            if (!_context.Roles.Any())
            {
                SeedRoles(_context);
            }

            if (!_context.Users.Any())
            {
                SeedUsers(_context, _userManager);
            }

            if (!_context.Events.Any())
            {
                SeedEvents(_context);
            }
        }

        private static void SeedRoles(EventTrackerDbContext _context)
        {
            _context.Roles.Add(new IdentityRole()
            {
                Name = "Basic",
                NormalizedName = "BASIC"
            });
            _context.Roles.Add(new IdentityRole()
            {
                Name = "Super",
                NormalizedName = "SUPER"
            });
            _context.Roles.Add(new IdentityRole()
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            _context.SaveChanges();
        }
        private static void SeedUsers(EventTrackerDbContext _context, UserManager<UserProfile> _usermanager)
        {
            //user 1
            var user = new UserProfile
            {
                FirstName = "Jack",
                LastName = "O'Neill",
                UserName = "Jack.ONeill@SG1.com",
                NormalizedUserName = "JACK.ONEILL@SG1.COM",
                Email = "Jack.ONeill@SG1.com",
                NormalizedEmail = "JACK.ONEILL@SG1.COM",
                UserRole = UserRole.Admin,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var password = new PasswordHasher<UserProfile>();
            var hashed = password.HashPassword(user, "Jack-sg1");
            user.PasswordHash = hashed;

            var userStore = new UserStore<UserProfile>(_context);
            userStore.CreateAsync(user).Wait();

            var currentUser =  userStore.FindByIdAsync(user.Id);
            currentUser.Wait();
            _usermanager.AddToRolesAsync(currentUser.Result, new string[] { "ADMIN", "SUPER", "BASIC" }).Wait();

            //user2
            user = new UserProfile
            {
                FirstName = "Samantha",
                LastName = "Carter",
                UserName = "Samantha.Carter@SG1.com",
                NormalizedUserName = "SAMANTHA.CARTER@SG1.COM",
                Email = "Samantha.Carter@SG1.com",
                NormalizedEmail = "SAMANTHA.CARTER@SG1.COM",
                UserRole = UserRole.Super,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            password = new PasswordHasher<UserProfile>();
            hashed = password.HashPassword(user, "Samantha-sg1");
            user.PasswordHash = hashed;

            userStore = new UserStore<UserProfile>(_context);
            userStore.CreateAsync(user).Wait();

            currentUser = userStore.FindByIdAsync(user.Id);
            currentUser.Wait();
            _usermanager.AddToRolesAsync(currentUser.Result, new string[] { "SUPER", "BASIC" }).Wait();

            //user3
            user = new UserProfile
            {
                FirstName = "Daniel",
                LastName = "Jackson",
                UserName = "Daniel.Jackson@SG1.com",
                NormalizedUserName = "DANIEL.JACKSON@SG1.COM",
                Email = "Daniel.Jackson@SG1.com",
                NormalizedEmail = "DANIEL.JACKSON@SG1.COM",
                UserRole = UserRole.Basic,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            password = new PasswordHasher<UserProfile>();
            hashed = password.HashPassword(user, "Daniel-sg1");
            user.PasswordHash = hashed;

            userStore = new UserStore<UserProfile>(_context);
            userStore.CreateAsync(user).Wait();

            currentUser = userStore.FindByIdAsync(user.Id);
            currentUser.Wait();
            _usermanager.AddToRolesAsync(currentUser.Result, new string[] { "BASIC" }).Wait();
        }

        private static void SeedEvents(EventTrackerDbContext _context)
        {
            var events = new List<Event> {
                new Event {
                    Name = "Pukkelpop 2019",
                    Description = "Merch stand op Pukkelpop 2019",
                    WantedAmountOfParticipants = 1,
                    Timeframes =
                    {
                        new Timeframe { EventDate = new DateTime(2051, 1, 1), Starttime = 10, Endtime = 17 },
                        new Timeframe { EventDate = new DateTime(2051, 1, 2), Starttime = 10, Endtime = 17 }
                    },
                    Location = new Location {City="Kiewit", Province="Limburg"}
                },
                new Event {
                    Name = "F.A.C.T.S. 2019",
                    Description = "Merch stand op F.A.C.T.S. 2019",
                    WantedAmountOfParticipants = 2,
                    Timeframes = {
                        new Timeframe { EventDate = new DateTime(2052, 2, 1), Starttime = 9, Endtime = 18 },
                        new Timeframe { EventDate = new DateTime(2052, 2, 2), Starttime = 9, Endtime = 18 }
                    },
                    Location = new Location {City="Gent", Province="Oost-Vlaanderen"},
                },
                new Event {
                    Name = "Ieperfest 2019",
                    Description = "Merch stand op Ieperfest 2019",
                    WantedAmountOfParticipants = 3,
                    Timeframes =
                        { new Timeframe { EventDate = new DateTime(2053, 3, 1), Starttime = 9, Endtime = 15}
                    },
                    Location = new Location {City="Ieper", Province="West-Vlaanderen"},
                    IsCancelled = true
                }
            };
            _context.Events.AddRange(events);
            _context.SaveChanges();
        }
    }
}
