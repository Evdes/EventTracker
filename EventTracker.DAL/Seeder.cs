using EventTracker.DAL.SqlData;
using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventTracker.DAL
{

    //**Seeder can be used for TEST PURPOSES ONLY!**
    //**Seed needs to be instantiated & its methods need to called from Configure method in startup class (Project EventTracker.BLL)**
    //**Seeder must only be used in dev env**
    //**For safety reasons, code must be deleted from startup class after seeder has populated Db**
    public class Seeder
    {
        private readonly EventTrackerDbContext _context;
        private readonly UserManager<UserProfile> _userManager;

        public Seeder(EventTrackerDbContext context, UserManager<UserProfile> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async void SeedAll()
        {
            SeedRoles();
            await SeedUsers();
            SeedEvents();
        }
    
        public void SeedRoles()
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
        public async Task SeedUsers()
        {
            //start user1
            var user1 = new UserProfile
            {
                FirstName = "Jack",
                LastName = "O'Neill",
                UserName = "Jack.ONeill@SG1.com",
                Email = "Jack.ONeill@SG1.com",
                UserRole = UserRole.Admin
            };

            var result = await _userManager.CreateAsync(user1, "Jack-sg1");
            if (result.Succeeded)
            {
                var currentUser = _userManager.FindByIdAsync(user1.Id).Result;
                currentUser.EmailConfirmed = true;
                await _userManager.AddToRolesAsync(currentUser, new string[] { "ADMIN", "SUPER", "BASIC" });
            }

            //start user2
            var user2 = new UserProfile
            {
                FirstName = "Samantha",
                LastName = "Carter",
                UserName = "Samantha.Carter@SG1.com",
                Email = "Samantha.Carter@SG1.com",
                UserRole = UserRole.Super
            };

            result = await _userManager.CreateAsync(user2, "Samantha-sg1");
            if (result.Succeeded)
            {
                var currentUser = _userManager.FindByIdAsync(user2.Id).Result;
                currentUser.EmailConfirmed = true;
                await _userManager.AddToRolesAsync(currentUser, new string[] { "ADMIN", "SUPER" });
            }

            //start user3
            var user3 = new UserProfile
            {
                FirstName = "Daniel",
                LastName = "Jackson",
                UserName = "Daniel.Jackson@SG1.com",
                Email = "Daniel.Jackson@SG1.com",
                UserRole = UserRole.Basic
            };

            result = await _userManager.CreateAsync(user3, "Daniel-sg1");
            if (result.Succeeded)
            {
                var currentUser = _userManager.FindByIdAsync(user3.Id).Result;
                currentUser.EmailConfirmed = true;
                await _userManager.AddToRoleAsync(currentUser, user3.UserRole.ToString());
            }
        }
        public void SeedEvents()
        {
            new List<Event> {
                new Event { Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    WantedAmountOfParticipants = 1,
                    Timeframes = new List<Timeframe>
                    {
                        new Timeframe { EventDate = new DateTime(2051, 1, 1), Starttime = 10, Endtime = 17 },
                        new Timeframe { EventDate = new DateTime(2051, 1, 2), Starttime = 10, Endtime = 17 }
                    },
                    Location = new Location {City="City1", Province="Province1"},
                    IsCancelled=true
                },
                new Event { Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    WantedAmountOfParticipants = 2,
                    Timeframes = new List<Timeframe> {
                        new Timeframe { EventDate = new DateTime(2052, 2, 1), Starttime = 9, Endtime = 18 },
                        new Timeframe { EventDate = new DateTime(2052, 2, 2), Starttime = 9, Endtime = 18 }
                    },
                    Location = new Location {City="City2", Province="Province2"},
                },
                new Event { Id = 3,
                    Name = "Event3",
                    Description = "Description3",
                    WantedAmountOfParticipants = 3,
                    Timeframes = new List<Timeframe>
                        { new Timeframe { EventDate = new DateTime(2053, 3, 1), Starttime = 9, Endtime = 15}
                    },
                    Location = new Location {City="City3", Province="Province3"},
                }
            };
        }
    }
}
