using FreshFarmMarket.ViewModels;
using FreshFarmMarket.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Sockets;

namespace FreshFarmMarket.Pages.Accounts
{
    public class LoginModel : PageModel
    {
		
        [BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
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
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
			   LModel.RememberMe, lockoutOnFailure: true);
				if (identityResult.Succeeded)
				{
                    //Create the security context
                    var claims = new List<Claim> {
						new Claim(ClaimTypes.Name, "c@c.com"),
						new Claim(ClaimTypes.Email, "c@c.com"),

                        new Claim("Department", "Admin")
                    };
                    var i = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

					return RedirectToPage("Index");
				}

				if(identityResult.IsLockedOut)
				{
                    ModelState.AddModelError("", "Your account is locked out.Kindly please wait for 2 minutes before attempting to login");
                }
				ModelState.AddModelError("", "Invalid Username or Password");
            }
			return Page();
		}
	}
}
