using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.BLL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProfilesController : Controller
    {
        private readonly SignInManager<UserProfile> _signInManager;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailSender _emailSender;

        public UserProfilesController(SignInManager<UserProfile> signInManager, 
                                        UserManager<UserProfile> userManager,
                                        IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult AddUserProfile()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddUserProfile")]
        public async Task<IActionResult> AddUserProfileAsync(AddUserProfileViewModel userProfileToAdd)
        {
            if (ModelState.IsValid)
            {
                var user = new UserProfile
                {
                    FirstName = userProfileToAdd.FirstName,
                    LastName = userProfileToAdd.LastName,
                    UserName = userProfileToAdd.Email,
                    Email = userProfileToAdd.Email,
                    UserRole = userProfileToAdd.UserRole
                };

                var result = await _userManager.CreateAsync(user, userProfileToAdd.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(userProfileToAdd.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("AllUpcomingEvents", "Events");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
    }
}