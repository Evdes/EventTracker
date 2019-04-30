using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.UserProfiles
{
    public class UserProfile : IdentityUser
    {

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public UserRole UserRole { get; set; }
    }
}
