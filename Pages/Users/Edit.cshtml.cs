using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
	public class EditModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<EditModel> _logger;
        public EditModel(SneakerShopService sneakerShopService, ILogger<EditModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _sneakerShopService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            User.Password = PasswordHasher.Hash(User.Password);

            try
            {
                await _sneakerShopService.UpdateUserAsync(User.ID, User);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            stopwatch.Stop();
            _logger.LogInformation("User Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com") return RedirectToPage("./Index");
            else return Page();
        }

        private bool UserExists(string id)
        {
            return _sneakerShopService.GetUserAsync(id).IsCompleted;
        }
    }
}
