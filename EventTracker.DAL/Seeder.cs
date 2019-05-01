using EventTracker.DAL.SqlData;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventTracker.DAL
{
    public class Seeder
    {
        private readonly EventTrackerDbContext _context;
        private readonly UserManager<UserProfile> _userManager;

        public Seeder(EventTrackerDbContext context, UserManager<UserProfile> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                await _userManager.AddToRoleAsync(currentUser, user1.UserRole.ToString());
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
                await _userManager.AddToRoleAsync(currentUser, user2.UserRole.ToString());
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
    }
}
