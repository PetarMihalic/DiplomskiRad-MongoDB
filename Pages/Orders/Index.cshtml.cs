using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Orders
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

        public IList<Order> Order { get; set; } = default!;

        [BindProperty]
        public string? OrderID { get; set; }
        [BindProperty]
        public string Status { get; set; }

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Order = await _sneakerShopService.GetOrderWithUserAsync();
            stopwatch.Stop();
            _logger.LogInformation("Order Index Time: {0}", stopwatch.ElapsedMilliseconds);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Order order = await _sneakerShopService.GetOrderAsync(OrderID);
            order.Status = Status;

            try
            {
                await _sneakerShopService.UpdateOrderAsync(OrderID, order);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            stopwatch.Stop();
            _logger.LogInformation("Order Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Orders/Index");
        }
    }
}
