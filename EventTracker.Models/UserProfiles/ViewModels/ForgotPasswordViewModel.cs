using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.UserProfiles.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
