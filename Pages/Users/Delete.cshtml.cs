using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
	public class DeleteModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<DeleteModel> _logger;
        public DeleteModel(SneakerShopService sneakerShopService, ILogger<DeleteModel> logger)
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
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var user = await _sneakerShopService.GetUserAsync(id);

            if (user != null)
            {
                User = user;
                await _sneakerShopService.RemoveUserAsync(id);
            }
            stopwatch.Stop();
            _logger.LogInformation("User Delete Time: {0}", stopwatch.ElapsedMilliseconds);
            if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com") return RedirectToPage("./Index");
            else
            {
                try
                {
                    HttpContext.Session.Clear();
                    return RedirectToPage("../Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Page();
                }
            }
        }
    }
}
