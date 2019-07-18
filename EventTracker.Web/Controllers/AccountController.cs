using System;
using System.IO;
using System.Threading.Tasks;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.EmailSender;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using EventTracker.Models.UserProfiles.ViewModels;
using EventTracker.Services.Alerts;
using EventTracker.Web.Extensions;

namespace EventTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UserProfile> _signInManager;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<UserProfile> signInManager,
                                    UserManager<UserProfile> userManager,
                                    IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                RedirectToAction(nameof(EventsController.UpcomingEvents), "Events");
            }
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(UserProfileLoginViewModel postedUserProfile)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(postedUserProfile.Email,
                                                                                 postedUserProfile.Password,
                                                                                 postedUserProfile.RememberMe,
                                                                                 false);
                if (result.Succeeded)
                {
                    var loggedInUser = await _userManager.FindByEmailAsync(postedUserProfile.Email);
                    if (loggedInUser.IsFirstLogin)
                    {
                        loggedInUser.IsFirstLogin = false;
                        await _userManager.UpdateAsync(loggedInUser);
                        return RedirectToAction("ChangePassword").WithSuccess("Success", "Login successful. Since this is your first login, please provide a new password");
                    }
                    return RedirectToAction(nameof(EventsController.UpcomingEvents), "Events").WithSuccess("Success", "Login successful");
                }
                else
                {
                    var attemptedLoginUserProfile = await _userManager.FindByEmailAsync(postedUserProfile.Email);
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
                    return View().WithDanger("Failed", "Unable to log in");
                }
            }
            return View();
        }

        [HttpGet]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login)).WithSuccess("Success", "Logout succesful");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(EventsController.UpcomingEvents), "Events");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Unknown email.");
                    return View(nameof(ForgotPassword), model).WithDanger("Failed", "Request for reset link denied");
                }
                else
                {
                    if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        ModelState.AddModelError(string.Empty, 
                            "Email has not yet been confirmed. " +
                            "Please confirm the email by clicking the link in the confirmation mail which was sent to you.");
                    }
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id,
                                                                code,
                                                                Request.Scheme);

                var email = model.Email;
                var subject = "EventTracker - Reset Password";

                string pathToFile = Directory.GetParent(Environment.CurrentDirectory).FullName
                    + Path.DirectorySeparatorChar.ToString()
                    + "wwwroot"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailSender"
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "_EmailResetTemplate.html";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }

                string message = string.Format(builder.HtmlBody, user.FirstName, callbackUrl);
                await _emailSender.SendEmailAsync(email, subject, message);
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "A link to reset your password has been sent.");
            }
            return View(nameof(ForgotPassword), model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null, string userId = null)
        {
            if (code == null || userId == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(ResetPassword), model);
            }

            var user = await _userManager.FindByIdAsync(model.userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "Password has been reset");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "Password has been reset");
            }

            return View(nameof(ResetPassword));
        }

        [HttpGet]
        [ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(EventsController.UpcomingEvents), "Events").WithSuccess("Success", "Password was changed");
        }
    }
}