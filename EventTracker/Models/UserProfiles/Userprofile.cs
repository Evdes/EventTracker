using System.ComponentModel.DataAnnotations;

namespace EventTracker.BLL.Models.UserProfiles
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public UserRole UserRole { get; set; }
    }
}
