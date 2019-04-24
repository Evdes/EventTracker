using System.Collections.Generic;
using System.Linq;
using EventTracker.BLL.Models.UserProfiles;

namespace EventTracker.BLL.Services.Repos
{
    public class UserProfileMockRepo : IUserProfileRepo
    {
        private List<UserProfile> _userProfiles;

        public UserProfileMockRepo()
        {
            PopulateMock();
        }

        private void PopulateMock()
        {
            _userProfiles = new List<UserProfile> {
                                        new UserProfile {
                                            Id = 1,
                                            FirstName ="Jack",
                                            LastName = "O'Neill",
                                            Email ="Jack.Oneill@SG1.com",
                                            UserRole = UserRole.Admin
                                        },
                                        new UserProfile {
                                            Id = 2,
                                            FirstName ="Sam",
                                            LastName = "Carter",
                                            Email ="Sam.Carter@SG1.com",
                                            UserRole = UserRole.Super
                                        },
                                        new UserProfile {
                                            Id = 3,
                                            FirstName ="Daniel",
                                            LastName = "Jackson",
                                            Email ="Daniel.Jackson@SG1.com",
                                            UserRole = UserRole.Basic
                                        }
            };
        }

        public IEnumerable<UserProfile> GetAllUserProfiles()
        {
            return _userProfiles;
        }

        public UserProfile GetUserProfile(int? id)
        {
            return _userProfiles.FirstOrDefault(u => u.Id == id.Value);
        }

        public void DeleteUserProfile(UserProfile userProfileToDelete)
        {
            _userProfiles.Remove(userProfileToDelete);
        }

        public UserProfile UpdateUserProfile(UserProfile userToUpdate, UserProfile postedUserProfile)
        {
            userToUpdate.FirstName = postedUserProfile.FirstName;
            userToUpdate.LastName = postedUserProfile.LastName;
            userToUpdate.Email = postedUserProfile.Email;
            userToUpdate.UserRole = postedUserProfile.UserRole;
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
