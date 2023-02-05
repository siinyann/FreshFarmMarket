using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreshFarmMarket.Pages
{
    public class ChangePasswordModel : PageModel
    {
        
        [BindProperty]
        public ChangePassword CModel { get; set; }

        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                var user = await userManager.GetUserAsync(User);
                //if (user == null)
                //{
                //    return RedirectToPage("Index");
                //}

                // ChangePasswordAsync changes the user password
                var result = await userManager.ChangePasswordAsync(user,
                    CModel.CurrentPassword, CModel.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (result.Succeeded)
                {
                    // Upon successfully changing the password refresh sign-in cookie
                    await signInManager.RefreshSignInAsync(user);
                    return RedirectToPage("Index");
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
