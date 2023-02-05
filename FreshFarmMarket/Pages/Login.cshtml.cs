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
                    ModelState.AddModelError("", "Your account is locked out.Kindly please wait for 5 minutes before attempting to login");
                }
				ModelState.AddModelError("", "Invalid Username or Password");
            }
			return Page();
		}

		[AllowAnonymous]
		public IActionResult GoogleLogin()
		{
			string redirectUrl = Url.Action("GoogleResponse", "Account");
			var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
			return new ChallengeResult("Google", properties);
		}

		[AllowAnonymous]
		public async Task<IActionResult> GoogleResponse()
		{
			ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
			if (info == null)
				return RedirectToAction(nameof(Login));

			var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
			string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
			if (result.Succeeded)
				return RedirectToPage("Index");
			else
			{
				ApplicationUser user = new ApplicationUser
				{
					Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
					UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
				};

				IdentityResult identResult = await userManager.CreateAsync(user);
				if (identResult.Succeeded)
				{
					identResult = await userManager.AddLoginAsync(user, info);
					if (identResult.Succeeded)
					{
						await signInManager.SignInAsync(user, false);
						return RedirectToPage("Index");
					}
				}
				return Page();
			}
		}
	}
}
