using System.Collections.Generic;
using System.Linq;
using EventTracker.DAL.Mocks;
using EventTracker.Models.UserProfiles;

namespace EventTracker.Services.Repos.Mocks
{
    public class UserProfileMock : IUserProfileRepo
    {
        private readonly List<UserProfile> _userProfiles;

        public UserProfileMock(UserProfileInMemoryMock userProfileMock)
        {
            _userProfiles = userProfileMock.GetUserProfiles();
        }

        public IEnumerable<UserProfile> GetAllUserProfiles()
        {
            return _userProfiles;
        }

        public UserProfile GetUserProfile(int? id)
        {
            return _userProfiles.FirstOrDefault(u => u.Id == id.Value.ToString());
        }

        public void DeleteUserProfile(UserProfile userProfileToDelete)
        {
            _userProfiles.Remove(userProfileToDelete);
        }

        public UserProfile UpdateUserProfile(UserProfile userToUpdate)
        {

            return userToUpdate;
        }

        public UserProfile AddUserProfile(UserProfile userProfileToAdd)
        {
            userProfileToAdd.Id = _userProfiles.Max(u => u.Id) + 1;
            _userProfiles.Add(userProfileToAdd);
            return userProfileToAdd;
        }
    }
}
