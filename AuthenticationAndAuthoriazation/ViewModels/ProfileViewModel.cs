namespace AuthenticationAndAuthoriazation.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public IFormFile? UploadPhoto { get; set; }
    }
}
