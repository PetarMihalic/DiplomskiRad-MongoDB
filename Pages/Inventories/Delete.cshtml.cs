using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Inventories
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
        public Inventory Inventory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _sneakerShopService.GetInventoryAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }
            else
            {
                Inventory = inventory;
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

            var inventory = await _sneakerShopService.GetInventoryAsync(id);

            if (inventory != null)
            {
                Inventory = inventory;
                await _sneakerShopService.RemoveInventoryAsync(id);
            }
            stopwatch.Stop();
            _logger.LogInformation("Inventory Delete Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("./Index");
        }
    }
}
