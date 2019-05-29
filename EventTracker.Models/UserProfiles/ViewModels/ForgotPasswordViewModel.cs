using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.UserProfiles.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
