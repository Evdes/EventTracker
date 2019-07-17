using EventTracker.Models.UserProfiles.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EventTracker.BLL
{
    class AccountManager
    {
        public async Task FailedSignin(UserProfileLoginViewModel userProfile)
        {
            var attemptedLoginUserProfile = await _userManager.FindByEmailAsync(userProfile.Email);
            if (attemptedLoginUserProfile == null)
            {
                ModelState.AddModelError(string.Empty, "Unknown Email");
            }
            else
            {
                if (!await _userManager.IsEmailConfirmedAsync(attemptedLoginUserProfile))
                {
                    ModelState.AddModelError(string.Empty,
                        "Account has not been activated. " +
                        "Please activated your account by following the instructions in the activation mail");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed");
                }
            }
        }
    }
}
