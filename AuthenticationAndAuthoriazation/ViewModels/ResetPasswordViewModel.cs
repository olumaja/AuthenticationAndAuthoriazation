using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthoriazation.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Confirm password doesn't match password")]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassowrd { get; set; }
    }
}
