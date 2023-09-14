using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
	public class DetailsModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<DetailsModel> _logger;
        public DetailsModel(SneakerShopService sneakerShopService, ILogger<DetailsModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var user = await _sneakerShopService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            stopwatch.Stop();
            _logger.LogInformation("User Details Time: {0}", stopwatch.ElapsedMilliseconds);
            return Page();
        }

        public void OnPost()
        {
            try
            {
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Response.Redirect("/Index");
        }
    }
}
