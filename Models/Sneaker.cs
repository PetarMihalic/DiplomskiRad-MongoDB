using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SneakerShopMongoDB.Models
{
    public class Sneaker
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        [Required]
        public string Brand { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public byte[]? Picture1 { get; set; }
        public byte[]? Picture2 { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}
