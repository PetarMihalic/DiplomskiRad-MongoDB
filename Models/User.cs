using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SneakerShopMongoDB.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "Created At")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<Order> Orders { get; set; }
        public Cart Cart { get; set; }
    }
}
