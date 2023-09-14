using System.ComponentModel.DataAnnotations;

namespace SneakerShopMongoDB.Models
{
    public class OrderDetails
    {
        public string? InventoryID { get; set; }
        public int Quantity { get; set; }
        public Inventory Inventory { get; set; }
    }
}
