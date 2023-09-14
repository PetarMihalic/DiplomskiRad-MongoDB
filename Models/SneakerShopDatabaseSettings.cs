using System.Security.Policy;

namespace SneakerShopMongoDB.Models
{
    public class SneakerShopDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string SneakerCollectionName { get; set; } = null!;
        public string InventoryCollectionName { get; set; } = null!;
        public string OrderCollectionName { get; set; } = null!;
        public string CartCollectionName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
    }
}
