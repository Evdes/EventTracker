using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.BLL.Extensions;
using EventTracker.BLL.Extensions.Alerts;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EventTracker.BLL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UserProfile> _signInManager;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _env;

        public AccountController(SignInManager<UserProfile> signInManager,
                                    UserManager<UserProfile> userManager,
                                    IEmailSender emailSender,
                                    IHostingEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _env = env;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                RedirectToAction(nameof(EventsController.UpcomingEvents), "Events");
            }
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(UserProfileLoginViewModel userProfile)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(userProfile.Email,
                                                                                 userProfile.Password,
                                                                                 userProfile.RememberMe,
                                                                                 false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(EventsController.UpcomingEvents), "Events").WithSuccess("Success", "Login succesful");
                }
                else
                {
                    var attemptedLoginUserProfile = await _userManager.FindByEmailAsync(userProfile.Email);
                    if (attemptedLoginUserProfile == null)
                    {
                        ModelState.AddModelError(String.Empty, "Unknown Email");
                    }
                    else
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(attemptedLoginUserProfile))
                        {
                            ModelState.AddModelError(String.Empty,
                                "Account has not been activated. " +
                                "Please activated your account by following the instructions in the activation mail");
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Login failed");
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
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Unknown email.");
                    return View(model).WithDanger("Failed", "Request for reset link denied");
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
                //var message = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";
                var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "_EmailResetTemplate.html";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {

                    builder.HtmlBody = SourceReader.ReadToEnd();

                }

                string message = string.Format(builder.HtmlBody, user.FirstName, callbackUrl);
                await _emailSender.SendEmailAsync(email, subject, message);
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "A link to reset your password has been sent. " +
                    "Please keep in mind that this link is only valid for 24 hours");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "Password has been reset");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login)).WithSuccess("Success", "Password has been reset");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
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
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
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