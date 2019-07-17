using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.Models.Enums;
using EventTracker.Models.UserProfiles;
using EventTracker.Models.UserProfiles.ViewModels;
using EventTracker.Services.Alerts;
using EventTracker.Services.EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EventTracker.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProfilesController : Controller
    {

        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailSender _emailSender;

        public UserProfilesController(UserManager<UserProfile> userManager,
                                        IEmailSender emailSender)
        {
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
                    UserRole = userProfileToAdd.UserRole,
                    IsFirstLogin = true
                };

                var result = await _userManager.CreateAsync(user, userProfileToAdd.Password);
                if (result.Succeeded)
                {
                    switch (user.UserRole)
                    {
                        case UserRole.Basic:
                            await _userManager.AddToRoleAsync(user, UserRole.Basic.ToString());
                            break;
                        case UserRole.Super:
                            await _userManager.AddToRolesAsync(user, new string[] {UserRole.Super.ToString(),
                                                                                    UserRole.Basic.ToString()});
                            break;
                        case UserRole.Admin:
                            await _userManager.AddToRolesAsync(user, new string[] {UserRole.Super.ToString(),
                                                                                    UserRole.Basic.ToString(),
                                                                                    UserRole.Admin.ToString()});
                            break;
                        default:
                            break;
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action(
                        action: nameof(AccountController.ConfirmEmailAsync),
                        controller: "account",
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    var email = userProfileToAdd.Email;
                    var subject = "Welcome to EventTracker! Please confirm your email.";

                    string pathToFile = Directory.GetParent(Environment.CurrentDirectory).FullName
                    + Path.DirectorySeparatorChar.ToString()
                    + "EventTracker.Services"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailSender"
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "_EmailConfirmTemplate.html";

                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string message = string.Format(builder.HtmlBody, user.FirstName, callbackUrl);

                    await _emailSender.SendEmailAsync(email, subject, message);
                    return RedirectToAction(nameof(AllUserProfiles)).WithSuccess("Success", "User account added");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View().WithDanger("Failed", "Unable to add user account");
        }

        [HttpGet]
        public IActionResult AllUserProfiles()
        {
            return View(_userManager.Users.OrderBy(u => u.FirstName).ToList());
        }

        [HttpPost]
        [ActionName("DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfilePostAsync(string id)
        {
            if (ModelState.IsValid)
            {
                var UserProfileToDelete = await _userManager.FindByIdAsync(id);
                if (UserProfileToDelete == null)
                {
                    return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
                }
                else
                {
                    await _userManager.DeleteAsync(UserProfileToDelete);
                    return RedirectToAction(nameof(AllUserProfiles)).WithSuccess("Success", "User account deleted");
                }
            }
            else
            {
                return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
            }
        }

        [HttpGet]
        [ActionName("EditUserProfile")]
        public async Task<IActionResult> EditUserProfileAsync(string id)
        {
            if (ModelState.IsValid)
            {
                var userProfileToEdit = await _userManager.FindByIdAsync(id);
                if(userProfileToEdit == null)
                {
                    return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
                }
                else
                {
                    var userProfileToEditViewModel = new UserProfileViewModel
                    {
                        Id = userProfileToEdit.Id,
                        FirstName = userProfileToEdit.FirstName,
                        LastName = userProfileToEdit.LastName,
                        Email = userProfileToEdit.Email,
                        UserRole = userProfileToEdit.UserRole
                    };
                    return View(userProfileToEditViewModel);
                }
            }
            else
            {
                return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
            }
        }

        [HttpPost]
        [ActionName("EditUserProfile")]
        public async Task<IActionResult> EditUserProfileAsync(UserProfileViewModel postedUserProfileViewModel)
        {
            if (ModelState.IsValid)
            {
                var userProfileToEdit = await _userManager.FindByIdAsync(postedUserProfileViewModel.Id);
                if(userProfileToEdit == null)
                {
                    return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
                }
                else
                {
                    userProfileToEdit.FirstName = postedUserProfileViewModel.FirstName;
                    userProfileToEdit.LastName = postedUserProfileViewModel.LastName;

                    if(postedUserProfileViewModel.Email != userProfileToEdit.Email 
                        && _userManager.Users.Any(u => u.Email == postedUserProfileViewModel.Email))
                    {
                        ModelState.AddModelError("InvalidEmail", "The email is already assigned to another user");
                        return View().WithDanger("Failed.", "User not updated");
                    }
                    else
                    {
                        userProfileToEdit.UserName = postedUserProfileViewModel.Email;
                        userProfileToEdit.Email = postedUserProfileViewModel.Email;
                    }
                   
                    switch (userProfileToEdit.UserRole)
                    {
                        case UserRole.Basic:
                            if (postedUserProfileViewModel.UserRole == UserRole.Admin)
                            {
                                await _userManager.AddToRolesAsync(userProfileToEdit, new string[] { UserRole.Admin.ToString(),
                                                                                UserRole.Super.ToString()});

                            }
                            else
                            {
                                if (postedUserProfileViewModel.UserRole == UserRole.Super)
                                {
                                    await _userManager.AddToRoleAsync(userProfileToEdit, UserRole.Super.ToString());
                                }
                            }
                            //posted userole must be basic.No change
                            break;
                        case UserRole.Super:
                            if (postedUserProfileViewModel.UserRole == UserRole.Admin)
                            {
                                await _userManager.AddToRoleAsync(userProfileToEdit, UserRole.Admin.ToString());
                            }
                            else
                            {
                                if (postedUserProfileViewModel.UserRole == UserRole.Basic)
                                {
                                    await _userManager.RemoveFromRoleAsync(userProfileToEdit, UserRole.Super.ToString());
                                }
                            }
                            //posted role must be super. No change
                            break;
                        case UserRole.Admin:
                            if (postedUserProfileViewModel.UserRole == UserRole.Super)
                            {
                                await _userManager.RemoveFromRoleAsync(userProfileToEdit, UserRole.Admin.ToString());
                            }
                            else
                            {
                                if (postedUserProfileViewModel.UserRole == UserRole.Basic)
                                {
                                    await _userManager.RemoveFromRolesAsync(userProfileToEdit, new string[] { UserRole.Admin.ToString(),
                                                                                        UserRole.Super.ToString() });
                                }
                            }
                            //posted role must be admin. No change
                            break;
                        default:
                            break;
                    }
                    userProfileToEdit.UserRole = postedUserProfileViewModel.UserRole;
                    await _userManager.UpdateAsync(userProfileToEdit);

                    return RedirectToAction(nameof(AllUserProfiles)).WithSuccess("Success", "User account updated");
                }
                
            }
            else
            {
                return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
            }

        }
    }
}