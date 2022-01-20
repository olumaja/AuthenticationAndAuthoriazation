using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthoriazation.Models
{
    public class Employee : IdentityUser
    {
        [Required]
        [StringLength(50, ErrorMessage ="Maximum of 50 characters")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Maximum of 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Photo { get; set; }
    }
}
