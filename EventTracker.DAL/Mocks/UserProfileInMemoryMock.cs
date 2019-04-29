using EventTracker.Models.UserProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventTracker.DAL.Mocks
{
    public class UserProfileInMemoryMock
    {
        private List<UserProfile> _userProfiles;

        public UserProfileInMemoryMock()
        {
            Populate(); 
        }

        private void Populate()
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

        public List<UserProfile> GetUserProfiles()
        {
            return _userProfiles;
        }

    }
}
