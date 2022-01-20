using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthoriazation.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
