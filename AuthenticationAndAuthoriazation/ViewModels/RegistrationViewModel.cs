using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthoriazation.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailExist", controller: "Account")]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        public IEnumerable<SelectListItem>? Options { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,Display(Name ="Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password doesn't match")]
        public string ConfirmPassword { get; set; }
    }
}
