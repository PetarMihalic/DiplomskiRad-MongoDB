using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;

namespace SneakerShopMongoDB.Pages.Carts
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
        public Cart Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id, int? quantity)
        {
            if (id == null)
            {
                return NotFound();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var cart = await _sneakerShopService.GetCartAsync(id);

            if (cart != null)
            {
                Cart = cart;
                await _sneakerShopService.RemoveFromCartAsync(id);

                int previousQuantity = int.Parse(HttpContext.Session.GetString("Cart"));
                int newQuantity = (int)(previousQuantity - quantity);
                HttpContext.Session.SetString("Cart", newQuantity.ToString());
            }
            stopwatch.Stop();
            _logger.LogInformation("Cart Delete Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("./Index");
        }
    }
}
