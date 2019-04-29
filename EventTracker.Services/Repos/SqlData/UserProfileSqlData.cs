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
        private EventTrackerDbContext _context;

        public UserProfileSqlData(EventTrackerDbContext context)
        {
            _context = context;
        }

        public UserProfile AddUserProfile(UserProfile userProfileToAdd)
        {
            _context.UserProfiles.Add(userProfileToAdd);
            _context.SaveChanges();
            return userProfileToAdd;
        }

        public void DeleteUserProfile(UserProfile userProfileToDelete)
        {
            _context.UserProfiles.Remove(userProfileToDelete);
            _context.SaveChanges();
        }

        public IEnumerable<UserProfile> GetAllUserProfiles()
        {
            return _context.UserProfiles;
        }

        public UserProfile GetUserProfile(int? id)
        {
            return _context.UserProfiles.FirstOrDefault(u => u.Id == id.Value);
        }

        public UserProfile UpdateUserProfile(UserProfile userToUpdate)
        {
            _context.Update(userToUpdate);
            _context.SaveChanges();
            return userToUpdate;
        }
    }
}
