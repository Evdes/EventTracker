using EventTracker.BLL.Models.UserProfiles;
using System.Collections.Generic;

namespace EventTracker.BLL.Services.Repos
{
    public interface IUserProfileRepo
    {
        IEnumerable<UserProfile> GetAllUserProfiles();
        UserProfile GetUserProfile(int? id);
        void DeleteUserProfile(UserProfile userProfileToDelete);
        UserProfile UpdateUserProfile(UserProfile userToUpdate, UserProfile postedUserProfile);
    }
}
