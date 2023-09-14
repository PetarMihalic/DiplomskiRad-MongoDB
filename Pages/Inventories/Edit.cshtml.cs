using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Inventories
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
            Inventory = inventory;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await _sneakerShopService.UpdateInventoryAsync(Inventory.ID, Inventory);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(Inventory.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            stopwatch.Stop();
            _logger.LogInformation("Inventory Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("./Index");
        }

        private bool InventoryExists(string? id)
        {
            return _sneakerShopService.GetInventoryAsync(id).IsCompletedSuccessfully;
        }
    }
}
