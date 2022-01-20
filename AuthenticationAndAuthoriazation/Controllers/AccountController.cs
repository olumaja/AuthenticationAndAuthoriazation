using AuthenticationAndAuthoriazation.Models;
using AuthenticationAndAuthoriazation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AuthenticationAndAuthoriazation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHost;

        public AccountController(
                UserManager<Employee> userManager, SignInManager<Employee> signInManager, RoleManager<IdentityRole> roleManager,
                IWebHostEnvironment webHost
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHost = webHost;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if(!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Account not confirmed yet!");
                return View(model);
            }

            var result =  await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(model);
            }

            if (returnUrl != "/") { return Redirect(returnUrl); }

            return RedirectToAction("Index", "Home");
        }

        //Remote validation
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<ActionResult> IsEmailExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if(user != null) { 
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var gender = new Dictionary<string, string> { { "Select Gender...", "" }, { "Male", "Male" }, { "Female", "Female" } };
            var model = new RegistrationViewModel { Options = gender.Select(x => new SelectListItem { Text = x.Key, Value = x.Value })};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var gender = new Dictionary<string, string> { { "Select Gender...", "" }, { "Male", "Male" }, { "Female", "Female" } };
                model.Options = gender.Select(x => new SelectListItem { Text = x.Key, Value = x.Value });
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                ModelState.AddModelError("", "Account already exist");
                return View(model);
            }

            var photos = model.Gender == "Male" ? "maleAvarter.jpg" : "femaleAvarter.jpg";

            var userToAdd = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Gender = model.Gender,
                UserName = model.Email,
                Photo = photos
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            //Junior staff by default
            await _userManager.AddToRoleAsync(userToAdd, "Junior Staff");
            //await _userManager.AddToRoleAsync(userToAdd, "Suppervisor");
            //await _userManager.AddClaimAsync(userToAdd, new Claim("CanEdit", "true"));

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(userToAdd);
            var emailConfirmationLink = Url.Action("AccountConfirmation", "Account", new { userId = userToAdd.Id, token = emailConfirmationToken }, Request.Scheme);
            //Email should be sent to user for account confirmation

            ViewBag.Message = "Account confirmation message has been sent to your mail.";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AccountConfirmation(string userId, string token)
        {
            if(!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(token))
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    ViewBag.Message = "Fail to confirm account.";
                    return View();
                }

                ViewBag.Message = "Account confirm successful, kindly login.";
                return View();
            }

            ViewBag.Message = "Invalid Credentials";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Account does not exist");
                return View(model);
            }

            var generatePasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { email = model.Email, token = generatePasswordToken });
            //Send email to user using link to reset password
            ViewBag.Message = "Reset password link has been sent to the registered email";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(token))
            {
                ViewBag.ErrorMessage = "Missing credentials";
            }

            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token))
            {
                ViewBag.ErrorMessage = "Missing credentials";
                return View(model);
            }
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if(user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            ViewBag.SuccessMessage("Password reset successful, kindly login again.");
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Manager")]
        [Authorize(Policy = "AdminOrManagerRoleAndCanEdit")]
        public async Task<IActionResult> Profile(string message)
        {
            ViewBag.ErrorMessage = message;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var model = new ProfileViewModel
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Photo = user.Photo,
                UploadPhoto = null
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfile(ProfileViewModel model)
        {
            var uploadMessage = "";

            if (model.UploadPhoto != null)
            {
                var allowedFileType = new List<string> { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".tif", ".tiff" };
                var typeArr = model.UploadPhoto.ContentType.Split('/');

                if (!allowedFileType.Contains("." + typeArr[1].ToLower()))
                {
                    uploadMessage = "Formats allow are " + String.Join(" ", allowedFileType);
                    return RedirectToAction(nameof(Profile), new { message = uploadMessage });
                }

                var folderPath = Path.Combine(_webHost.WebRootPath, "images");
                var photoName = Guid.NewGuid().ToString() + "_" + model.UploadPhoto.FileName;
                var fullPath = Path.Combine(folderPath, photoName);
                using(var fs = new FileStream(fullPath, FileMode.Create))
                {
                    model.UploadPhoto.CopyTo(fs);
                }

                var fileExist = new FileInfo(fullPath).Exists;

                if (fileExist)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    user.Photo = photoName;
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded) { ModelState.AddModelError("", "Fail to upload image"); }
                }
                else
                {
                    ViewBag.ErrorMessage = "Upload failed!";
                }
            }

            return RedirectToAction("Profile", new {message = uploadMessage});
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
