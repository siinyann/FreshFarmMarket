using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using FreshFarmMarket.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authorization;
using Google.Authenticator;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Google.Authenticator;


namespace FreshFarmMarket.Pages
{
    public class RegisterModel : PageModel
    {

		[BindProperty]
		public IFormFile? Upload { get; set; }

		private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
		private IWebHostEnvironment _environment;


		[BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole>roleManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this._environment = environment;
        }

        public void OnGet()
        {


		}

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("MySecretKey");

				if (Upload != null)
				{
					if (Upload.Length > 2 * 1024 * 1024)
					{
						ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
						return Page();
					}

					var uploadsFolder = "uploads";
                    var profileFolder = "profile";
					var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
					var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
					using var fileStream = new FileStream(imagePath, FileMode.Create);
					await Upload.CopyToAsync(fileStream);
					RModel.Photo = string.Format("/{0}/{1}/{2}", uploadsFolder, profileFolder, imageFile);
				}

                var user = new ApplicationUser()
                {
                    FullName = RModel.FullName,
                    CreditCardNo = protector.Protect(RModel.CreditCardNo),
                    Gender = RModel.Gender,
                    MobileNo = RModel.MobileNo,
                    DeliveryAddress = RModel.DeliveryAddress,
                    Photo = RModel.Photo,
                    AboutMe = RModel.AboutMe,
                    UserName = RModel.Email,
                    Email = RModel.Email,
                };

				//Create the Admin role if NOT exist
				IdentityRole role = await roleManager.FindByIdAsync("Admin");
				if (role == null)
				{
					IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
					if (!result2.Succeeded)
					{
						ModelState.AddModelError("", "Create role admin failed");
					}
				}

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
					//Add users to Admin Role
					result = await userManager.AddToRoleAsync(user, "Admin");

                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}

