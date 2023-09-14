using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SneakerShopMongoDB.Models;
using System.Data;
using static SneakerShopMongoDB.Pages.Carts.IndexModel;

namespace SneakerShopMongoDB.Services
{
    public class SneakerShopService
    {
        private readonly IMongoCollection<Sneaker> Sneakers;
        private readonly IMongoCollection<Inventory> Inventories;
        private readonly IMongoCollection<Order> Orders;
        private readonly IMongoCollection<Cart> Carts;
        private readonly IMongoCollection<User> Users;

        public SneakerShopService(IOptions<SneakerShopDatabaseSettings> settings) {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            Sneakers = mongoDatabase.GetCollection<Sneaker>(settings.Value.SneakerCollectionName);
            Inventories = mongoDatabase.GetCollection<Inventory>(settings.Value.InventoryCollectionName);
            Orders = mongoDatabase.GetCollection<Order>(settings.Value.OrderCollectionName);
            Carts = mongoDatabase.GetCollection<Cart>(settings.Value.CartCollectionName);
            Users = mongoDatabase.GetCollection<User>(settings.Value.UserCollectionName);
        }
        /*SNEAKER CRUD*/
        public async Task<List<Sneaker>> GetSneakerAsync() =>
        await Sneakers.Find(_ => true).ToListAsync();

        public async Task<Sneaker?> GetSneakerAsync(string id) =>
            await Sneakers.Find(x => x.ID == id).FirstOrDefaultAsync();

        public async Task CreateSneakerAsync(Sneaker newSneaker) =>
            await Sneakers.InsertOneAsync(newSneaker);

        public async Task UpdateSneakerAsync(string id, Sneaker updatedSneaker) =>
            await Sneakers.ReplaceOneAsync(x => x.ID == id, updatedSneaker);

        public async Task RemoveSneakerAsync(string id) =>
            await Sneakers.DeleteOneAsync(x => x.ID == id);

        /*INVENTORY CRUD*/
        public async Task<List<Inventory>> GetInventoryAsync()
        {
            var inventory = from i in Inventories.AsQueryable().ToList()
                            join s in Sneakers.AsQueryable().ToList() on i.SneakerID equals s.ID into joinedSneaker
                            select new Inventory
                            {
                                ID = i.ID,
                                SneakerID = i.SneakerID,
                                Size = i.Size,
                                Quantity = i.Quantity,
                                Sneaker = joinedSneaker.FirstOrDefault()
                            };
            return inventory.ToList();
        }

        public async Task<Inventory?> GetInventoryAsync(string id) =>
            await Inventories.Find(x => x.ID == id).FirstOrDefaultAsync();

        public async Task<List<Inventory>> GetInventoryForSneakerAsync(string id) =>
        await Inventories.Find(x => x.SneakerID == id).ToListAsync();

        public async Task<List<Inventory>> GetNotEmptyInventoryForSneakerAsync(string id) =>
        await Inventories.Find(x => x.SneakerID == id && x.Quantity > 0).ToListAsync();

        public async Task CreateInventoryAsync(Inventory newInventory) =>
            await Inventories.InsertOneAsync(newInventory);

        public async Task UpdateInventoryAsync(string id, Inventory updatedInventory) =>
            await Inventories.ReplaceOneAsync(x => x.ID == id, updatedInventory);

        public async Task RemoveInventoryAsync(string id) =>
            await Inventories.DeleteOneAsync(x => x.ID == id);

        /*ORDER CRUD*/
        public async Task<List<Order>> GetOrderAsync() =>
        await Orders.Find(_ => true).ToListAsync();

        public async Task<Order?> GetOrderAsync(string id) =>
            await Orders.Find(x => x.ID == id).FirstOrDefaultAsync();

        public async Task<List<Order>> GetOrderWithUserAsync()
        {
            var order = from o in Orders.AsQueryable().ToList()
                        join u in Users.AsQueryable().ToList() on o.UserID equals u.ID into joinedUser
                        select new Order
                        {
                            ID = o.ID,
                            UserID = o.UserID,
                            Name = o.Name,
                            CreatedDate = o.CreatedDate,
                            PaymentType = o.PaymentType,
                            Status = o.Status,
                            User = joinedUser.FirstOrDefault(),
                            OrderDetails = o.OrderDetails
                        };
            return order.ToList();
        }

        public async Task<List<Order>> GetAllOrdersForUserAsync(string id)
        {
            var order = from o in Orders.AsQueryable().ToList()
                        join u in Users.AsQueryable().ToList() on o.UserID equals u.ID into joinedUser
                        where o.UserID == id
                        select new Order
                        {
                            ID = o.ID,
                            UserID = o.UserID,
                            Name = o.Name,
                            CreatedDate = o.CreatedDate,
                            PaymentType = o.PaymentType,
                            Status = o.Status,
                            User = joinedUser.FirstOrDefault(),
                            OrderDetails = o.OrderDetails
                        };
            return order.ToList();
        }

        public async Task CreateOrderAsync(Order newOrder) =>
            await Orders.InsertOneAsync(newOrder);

        public async Task UpdateOrderAsync(string id, Order updatedOrder) =>
            await Orders.ReplaceOneAsync(x => x.ID == id, updatedOrder);

        public async Task RemoveOrderAsync(string id) =>
            await Orders.DeleteOneAsync(x => x.ID == id);

        /*CART CRUD*/
        public async Task<List<Cart>> GetCartAsync() =>
        await Carts.Find(_ => true).ToListAsync();

        public async Task<Cart?> GetCartAsync(string id) =>
            await Carts.Find(x => x.ID == id).FirstOrDefaultAsync();
        public async Task<List<Cart>> GetCartByUserIdAsync(string id) =>
            await Carts.Find(x => x.UserID == id).ToListAsync();

        public async Task CreateCartAsync(Cart newCart) =>
            await Carts.InsertOneAsync(newCart);

        public async Task UpdateCartAsync(string id, Cart updatedCart) =>
            await Carts.ReplaceOneAsync(x => x.ID == id, updatedCart);

        public async Task RemoveFromCartAsync(string id) =>
            await Carts.DeleteOneAsync(x => x.ID == id);

        public async Task ClearCartForUserAsync(string id) =>
            await Carts.DeleteManyAsync(x => x.UserID == id);
        public async Task ClearCartForGuestAsync(string id) =>
            await Carts.DeleteManyAsync(x => x.SessionID == id);

        public async Task ClearCartWhereNoInventory(string id) =>
            await Carts.DeleteManyAsync(x => x.InventoryID == id);

        public async Task<List<CartPreview>> GetCartDetailsGuestAsync(string id)
        {
            var listCart =  from inv in Inventories.AsQueryable().ToList()
                            join sne in Sneakers.AsQueryable().ToList() on inv.SneakerID equals sne.ID
                            join car in Carts.AsQueryable().ToList() on inv.ID equals car.InventoryID
                            where car.SessionID == id
                            select new CartPreview
                            {
                                cartID = car.ID,
                                picture1 = "data:image;base64," + Convert.ToBase64String(sne.Picture1),
                                name = sne.Name,
                                size = inv.Size,
                                quantity = car.Quantity,
                                inventoryID = inv.ID,
                                price = sne.Price,
                                total = (float)(car.Quantity * sne.Price)
                            };
            return listCart.ToList();
        }

        public async Task<List<CartPreview>> GetCartDetailsUserAsync(string id)
        {
            var listCart = (from inv in Inventories.AsQueryable().ToList()
                            join sne in Sneakers.AsQueryable().ToList() on inv.SneakerID equals sne.ID
                            join car in Carts.AsQueryable().ToList() on inv.ID equals car.InventoryID
                            where car.UserID == id
                            select new CartPreview
                            {
                                cartID = car.ID,
                                picture1 = "data:image;base64," + Convert.ToBase64String(sne.Picture1),
                                name = sne.Name,
                                size = inv.Size,
                                quantity = car.Quantity,
                                inventoryID = inv.ID,
                                price = sne.Price,
                                total = (float)(car.Quantity * sne.Price)
                            });
            return listCart.ToList();
        }

        /*USER CRUD*/
        public async Task<List<User>> GetUserAsync() =>
        await Users.Find(_ => true).ToListAsync();
        public async Task<User?> GetUserByEmailAsync(string email) =>
            await Users.Find(x => x.Email == email).FirstOrDefaultAsync();
        public async Task<User?> GetUserAsync(string id) =>
            await Users.Find(x => x.ID == id).FirstOrDefaultAsync();

        public async Task CreateUserAsync(User newUser) =>
            await Users.InsertOneAsync(newUser);

        public async Task UpdateUserAsync(string id, User updatedUser) =>
            await Users.ReplaceOneAsync(x => x.ID == id, updatedUser);

        public async Task RemoveUserAsync(string id) =>
            await Users.DeleteOneAsync(x => x.ID == id);
    }
}
