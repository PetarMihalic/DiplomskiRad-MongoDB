using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Orders
{
	public class MyOrdersModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<MyOrdersModel> _logger;
        public MyOrdersModel(SneakerShopService sneakerShopService, ILogger<MyOrdersModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public IList<Order> Order { get; set; }

        public IList<OrderDetails> Details { get; set; }

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string UserID = HttpContext.Session.GetString("UserID");

            Order = await _sneakerShopService.GetAllOrdersForUserAsync(UserID);
            foreach (var order in Order)
            {
                foreach (var orderDetail in order.OrderDetails) 
                {
                    orderDetail.Inventory = await _sneakerShopService.GetInventoryAsync(orderDetail.InventoryID);
                    orderDetail.Inventory.Sneaker = await _sneakerShopService.GetSneakerAsync(orderDetail.Inventory.SneakerID);
                }
            }
            
            stopwatch.Stop();
            _logger.LogInformation("My Orders Time: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
