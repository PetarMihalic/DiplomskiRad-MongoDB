using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;
using static SneakerShopMongoDB.Pages.Carts.IndexModel;

namespace SneakerShopMongoDB.Pages.Orders
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

        [BindProperty(SupportsGet = true)]
        public User User { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string PaymentType { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public Order Order { get; set; } = default!;

        public List<CartPreview> listCart { get; set; } = default!;
        public float TotalCost = 0;
        public async Task OnGetAsync()
        {            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                listCart = await _sneakerShopService.GetCartDetailsGuestAsync(HttpContext.Session.Id);
            }
            else
            {
                User = await _sneakerShopService.GetUserAsync(HttpContext.Session.GetString("UserID"));
                listCart = await _sneakerShopService.GetCartDetailsUserAsync(User.ID);
            }
            foreach (var item in listCart)
            {
                TotalCost += item.total;
            }
            TotalCost = (float)Math.Round(TotalCost, 2);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Order Order = new Order();
            if (User.ID != "") Order.UserID = User.ID;
            Random random = new Random();
            Order.Name = "ORDER-" + DateTime.Now.ToString() + "-" + random.Next(100, 1000);
            Order.PaymentType = PaymentType;
            Order.CreatedDate = DateTime.Now;
            Order.OrderDetails = new List<OrderDetails>();

            if (User.ID == "")
            {
                listCart = await _sneakerShopService.GetCartDetailsGuestAsync(HttpContext.Session.Id);
            }
            else
            {
                listCart = await _sneakerShopService.GetCartDetailsUserAsync(HttpContext.Session.GetString("UserID"));
            }

            foreach (CartPreview cartPreview in listCart)
            {
                OrderDetails OrderDetails = new OrderDetails();
                OrderDetails.InventoryID = cartPreview.inventoryID;
                OrderDetails.Quantity = cartPreview.quantity;
                OrderDetails.Inventory = await _sneakerShopService.GetInventoryAsync(cartPreview.inventoryID);
                Inventory inventory = OrderDetails.Inventory;
                inventory.Quantity = inventory.Quantity - cartPreview.quantity;
                if (inventory.Quantity == 0)
                {
                    await _sneakerShopService.ClearCartWhereNoInventory(inventory.ID);
                }
                await _sneakerShopService.UpdateInventoryAsync(inventory.ID, inventory);
                Order.OrderDetails.Add(OrderDetails);
            }

            await _sneakerShopService.CreateOrderAsync(Order);

            if (User.ID != "")
            {
                await _sneakerShopService.ClearCartForUserAsync(User.ID);
                HttpContext.Session.SetString("Cart", "0");
            }
            else
            {
                await _sneakerShopService.ClearCartForGuestAsync(HttpContext.Session.Id);
                HttpContext.Session.SetString("Cart", "0");
            }
            stopwatch.Stop();
            _logger.LogInformation("Order Create Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Index");
        }
    }
}
