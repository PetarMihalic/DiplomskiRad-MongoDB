using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Inventories
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

        public async Task<IActionResult> OnGet()
        {
            var sneakers = await _sneakerShopService.GetSneakerAsync();
            ViewData["Name"] = new SelectList(sneakers, "ID", "Name");
            return Page();
        }

        [BindProperty]
        public Inventory Inventory { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (Inventory == null)
            {
                return Page();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Inventory.Sneaker = await _sneakerShopService.GetSneakerAsync(Inventory.SneakerID);
            await _sneakerShopService.CreateInventoryAsync(Inventory);
            stopwatch.Stop();
            _logger.LogInformation("Inventory Create Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("./Index");
        }
    }
}
