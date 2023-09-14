using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
	public class LoginModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<LoginModel> _logger;
        public string errorMessage = "";

        public LoginModel(SneakerShopService sneakerShopService, ILogger<LoginModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (User == null)
            {
                return Page();
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var user = await _sneakerShopService.GetUserByEmailAsync(User.Email);
            if (user == null)
            {
                return NotFound();
            }
            if (!PasswordHasher.Verify(User.Password, user.Password))
            {
                errorMessage = "Wrong password";
                return Page();
            }
            else
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
                {
                    HttpContext.Session.SetString("UserID", user.ID.ToString());
                    HttpContext.Session.SetString("Name", user.FirstName + " " + user.LastName);
                    HttpContext.Session.SetString("Email", user.Email);
                }
                var cart = await _sneakerShopService.GetCartByUserIdAsync(user.ID);
                HttpContext.Session.Remove("Cart");
                int cartQuantity = 0;
                foreach (var cartItem in cart)
                {
                    cartQuantity += cartItem.Quantity;
                }
                HttpContext.Session.SetString("Cart", cartQuantity.ToString());

                stopwatch.Stop();
                _logger.LogInformation("User Login Time: {0}", stopwatch.ElapsedMilliseconds);

                return RedirectToPage("/Index");
            }
        }
    }
}
