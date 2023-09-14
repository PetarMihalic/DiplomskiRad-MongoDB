using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Sneakers
{
	public class DetailsModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<DetailsModel> _logger;
        public string errorMessage = "";
        public string successMessage = "";

        public DetailsModel(SneakerShopService sneakerShopService, ILogger<DetailsModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }
        [BindProperty(SupportsGet = true)]
        public Sneaker Sneaker { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public List<Inventory> Inventories { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sneaker = await _sneakerShopService.GetSneakerAsync(id);
            var inventory = await _sneakerShopService.GetNotEmptyInventoryForSneakerAsync(id);
            if (sneaker == null && inventory == null)
            {
                return NotFound();
            }
            else
            {
                Inventories = inventory;
                Sneaker = sneaker;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Cart Cart = new Cart();

            string sneakerID = Request.Form["ID"];
            float size = float.Parse(Request.Form["size"]);
            int quantity = int.Parse(Request.Form["quantity"]);

            var sneakerOld = await _sneakerShopService.GetSneakerAsync(sneakerID);
            var inventoryOld = await _sneakerShopService.GetInventoryForSneakerAsync(sneakerID);
            if (sneakerOld == null && inventoryOld == null)
            {
                return NotFound();
            }
            else
            {
                Inventories = inventoryOld;
                Sneaker = sneakerOld;
            }

            var inventory = Inventories.Where(m => m.Size == size).FirstOrDefault();

            if (inventory.Quantity < quantity)
            {
                errorMessage = "Only " + inventory.Quantity + " available, lower quantity to order.";
                stopwatch.Stop();
                _logger.LogInformation("Cart Create (error) Time: {0}", stopwatch.ElapsedMilliseconds);
                return Page();
            }

            Cart.InventoryID = inventory.ID;
            Cart.Quantity = quantity;
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                Cart.SessionID = HttpContext.Session.Id;
            }
            else
            {
                Cart.UserID = HttpContext.Session.GetString("UserID");
            }

            await _sneakerShopService.CreateCartAsync(Cart);

            if (errorMessage == "")
            {
                int currentQuantity = int.Parse(HttpContext.Session.GetString("Cart"));
                int newQuantity = currentQuantity + quantity;
                HttpContext.Session.SetString("Cart", newQuantity.ToString());
                successMessage = "Added to cart";
            }
            stopwatch.Stop();
            _logger.LogInformation("Cart Create Time: {0}", stopwatch.ElapsedMilliseconds);
            return Page();
        }
    }
}
