using System.Collections.Generic;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.Repos;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.BLL.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly IUserProfileRepo _userProfiles;

        public UserProfilesController(IUserProfileRepo repo)
        {
            _userProfiles = repo;
        }

        public IActionResult AllUserProfiles()
        {
            var allUserProfiles = _userProfiles.GetAllUserProfiles();
            return View(allUserProfiles);
        }

        [HttpGet]
        public IActionResult AddUserProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUserProfile(UserProfile userProfileToAdd)
        {
            if(ModelState.IsValid)
            {
                _userProfiles.AddUserProfile(userProfileToAdd);
                return RedirectToAction(nameof(AllUserProfiles));
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        public IActionResult DeleteUserProfile(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var userToDelete = _userProfiles.GetUserProfile(id);
                return View(userToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteUserProfile")]
        public IActionResult DeleteUserProfilePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                _userProfiles.DeleteUserProfile(_userProfiles.GetUserProfile(id.Value));
                return RedirectToAction("AllUserProfiles");
            }
        }

        [HttpGet]
        public IActionResult EditUserProfile(int? id){
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var userToUpdate = _userProfiles.GetUserProfile(id.Value);
                return View(userToUpdate);
            }
        }

        [HttpPost]
        public IActionResult EditUserProfile(int? id, UserProfile postedUserProfile)
        {
            if (id == null || postedUserProfile == null)
            {
                return NotFound();
            }
            else
            {
                var userToUpdate = _userProfiles.GetUserProfile(id.Value);
                userToUpdate.FirstName = postedUserProfile.FirstName;
                userToUpdate.LastName = postedUserProfile.LastName;
                userToUpdate.Email = postedUserProfile.Email;
                userToUpdate.UserRole = postedUserProfile.UserRole;

                _userProfiles.UpdateUserProfile(userToUpdate);
                return RedirectToAction(nameof(AllUserProfiles));
            }
        }
    }
}