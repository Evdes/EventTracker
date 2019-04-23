using System.Collections.Generic;
using EventTracker.BLL.Models.UserProfiles;
using EventTracker.BLL.Services.Repos;
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
                _userProfiles.UpdateUserProfile(userToUpdate, postedUserProfile);
                return RedirectToAction(nameof(AllUserProfiles));
            }
        }
    }
}