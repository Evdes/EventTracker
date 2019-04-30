using EventTracker.DAL.SqlData;
using EventTracker.Models.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventTracker.Services.Repos.SqlData
{
    public class UserProfileSqlData : IUserProfileRepo
    {
        private readonly EventTrackerDbContext _context;

        public UserProfileSqlData(EventTrackerDbContext context)
        {
            _context = context;
        }

        public UserProfile AddUserProfile(UserProfile userProfileToAdd)
        {
            _context.Users.Add(userProfileToAdd);
            _context.SaveChanges();
            return userProfileToAdd;
        }

        public void DeleteUserProfile(UserProfile userProfileToDelete)
        {
            _context.Users.Remove(userProfileToDelete);
            _context.SaveChanges();
        }

        public IEnumerable<UserProfile> GetAllUserProfiles()
        {
            return _context.Users;
        }

        public UserProfile GetUserProfile(int? id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id.Value.ToString());
        }

        public UserProfile UpdateUserProfile(UserProfile userToUpdate)
        {
            _context.Update(userToUpdate);
            _context.SaveChanges();
            return userToUpdate;
        }
    }
}
