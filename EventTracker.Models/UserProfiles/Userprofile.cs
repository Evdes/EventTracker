using EventTracker.Models.Events;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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
        public ICollection<UserEvents> UserEvents { get; set; }
    }
}
