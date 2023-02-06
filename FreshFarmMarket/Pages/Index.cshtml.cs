using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreshFarmMarket.Pages
{

    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IFormFile? Upload { get; set; }

        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        [BindProperty]
        public Register RModel { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

		//private readonly ILogger<IndexModel> _logger;

  //      public IndexModel(ILogger<IndexModel> logger)
  //      {
  //          _logger = logger;
  //      }
        public async Task OnGet()
        {
            //var user = await userManager.GetUserAsync(User);
            //RModel.FullName = user?.FullName;
            //RModel.CreditCardNo = user?.CreditCardNo;
            //RModel.Gender = user?.Gender;
            //RModel.MobileNo = user?.MobileNo;
            //RModel.DeliveryAddress = user?.DeliveryAddress;
            //RModel.AboutMe = user?.AboutMe;
            //RModel.Email = user?.Email;
        }

    }
}