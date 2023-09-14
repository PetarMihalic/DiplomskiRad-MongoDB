using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Carts
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

        public List<CartPreview> listCart { get; set; } = default!;
        public float TotalCost = 0;
        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                listCart = await _sneakerShopService.GetCartDetailsGuestAsync(HttpContext.Session.Id);
            }
            else
            {
                listCart = await _sneakerShopService.GetCartDetailsUserAsync(HttpContext.Session.GetString("UserID"));
            }
            foreach (var item in listCart)
            {
                TotalCost += item.total;
            }
            TotalCost = (float)Math.Round(TotalCost, 2);
            stopwatch.Stop();
            _logger.LogInformation("Cart Index Time: {0}", stopwatch.ElapsedMilliseconds);
            
        }

        public class CartPreview
        {
            public string? cartID { get; set; }
            public string picture1 { get; set; }
            public string name { get; set; }
            public float size { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
            public float total { get; set; }
            public string? inventoryID { get; set; }
        }
    }
}
