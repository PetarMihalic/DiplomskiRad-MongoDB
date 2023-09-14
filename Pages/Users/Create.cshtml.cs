using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<CreateModel> _logger;
        public CreateModel(SneakerShopService sneakerShopService, ILogger<CreateModel> logger)
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
            User.Password = PasswordHasher.Hash(User.Password);
            await _sneakerShopService.CreateUserAsync(User);
            stopwatch.Stop();
            _logger.LogInformation("User Create Time: {0}", stopwatch.ElapsedMilliseconds);
            if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com")
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("./Login");
            }
        }
    }
}
