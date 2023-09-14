using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SneakerShopMongoDB.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        public string? InventoryID { get; set; }
        public string? UserID { get; set; }
        public string? SessionID { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Inventory Inventory { get; set; }
        public User user { get; set; }
    }
}
