using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.UserProfiles.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string userId { get; set;}
    }
}
