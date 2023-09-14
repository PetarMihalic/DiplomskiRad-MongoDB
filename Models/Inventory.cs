using Microsoft.EntityFrameworkCore.Migrations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SneakerShopMongoDB.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        public string? SneakerID { get; set; }
        public float Size { get; set; }
        public int Quantity { get; set; }
        public Sneaker? Sneaker { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
