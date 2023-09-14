using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Sneakers
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
        public Sneaker Sneaker { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sneaker = await _sneakerShopService.GetSneakerAsync(id);

            if (sneaker == null)
            {
                return NotFound();
            }
            else
            {
                Sneaker = sneaker;
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
            var sneaker = await _sneakerShopService.GetSneakerAsync(id);

            if (sneaker != null)
            {
                Sneaker = sneaker;
                await _sneakerShopService.RemoveSneakerAsync(id);
            }
            stopwatch.Stop();
            _logger.LogInformation("Sneaker Delete Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Index");
        }
    }
}
