using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EventTracker.BLL.Extensions.Alerts;
using EventTracker.DAL.SqlData;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EventTracker.BLL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProfilesController : Controller
    {

        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _env;

        public UserProfilesController(UserManager<UserProfile> userManager,
                                        IEmailSender emailSender, IHostingEnvironment env)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _env = env;
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
                        action: nameof(AccountController.ConfirmEmail),
                        controller: "account",
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    var email = userProfileToAdd.Email;
                    var subject = "Welcome to EventTracker! Please confirm your email.";
                    //var message = $"Welome {userProfileToAdd.FirstName}! <br> <br> Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                    var pathToFile = _env.WebRootPath
                           + Path.DirectorySeparatorChar.ToString()
                           + "templates"
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

        [HttpGet]
        [ActionName("DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfileAsync(string id)
        {
            if (ModelState.IsValid)
            {
                var userProfileToDelete = await _userManager.FindByIdAsync(id);
                if (userProfileToDelete == null)
                {
                    return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
                }
                else
                {
                    var userProfileToDeleteViewModel = new UserProfileViewModel
                    {
                        Id = userProfileToDelete.Id,
                        FirstName = userProfileToDelete.FirstName,
                        LastName = userProfileToDelete.LastName,
                        Email = userProfileToDelete.Email,
                        UserRole = userProfileToDelete.UserRole
                    };

                    return View(userProfileToDeleteViewModel);
                }
            }
            else
            {
                return RedirectToAction(nameof(ErrorController.UserProfileNotFound), "error");
            }
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
                    userProfileToEdit.UserName = postedUserProfileViewModel.Email;
                    userProfileToEdit.Email = postedUserProfileViewModel.Email;

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