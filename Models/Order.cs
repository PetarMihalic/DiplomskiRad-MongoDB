using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SneakerShopMongoDB.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        [DisplayFormat(NullDisplayText = "Guest")]
        public string? UserID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Created At")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
		[DisplayFormat(NullDisplayText = "Guest")]
		public User? User { get; set; }
        public IList<OrderDetails> OrderDetails { get; set; }
    }
}
