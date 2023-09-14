using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Inventories
{
	public class IndexModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(SneakerShopService sneakerShopService, ILogger<IndexModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public IList<Inventory> Inventory { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Inventory = await _sneakerShopService.GetInventoryAsync();
            stopwatch.Stop();
            _logger.LogInformation("Inventory Index Time: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
