using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreshFarmMarket.Pages.Accounts
{
    public class ForgotPasswordModel : PageModel
    {
        
        [BindProperty]
        public ResetPassword PModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        public ForgotPasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
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
                var user = await userManager.GetUserAsync(User);

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

                EmailHelper emailHelper = new EmailHelper();
                bool emailResponse = emailHelper.SendEmailPasswordReset(user.Email, link);

                if (emailResponse == true)
                {
                    return RedirectToPage("ForgotPasswordConfirmation");
                }
            }
            return Page();
        }
    }
}
