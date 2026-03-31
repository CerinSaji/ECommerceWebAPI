using MongoDB.Driver;
using ECommerceWebAPI.Models; // Ensure this matches your project name

namespace ECommerceWebAPI.Data;

public class MongoDbService
{
        public readonly IMongoDatabase _database;

        public MongoDbService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Collections act like "Tables" in SQL
        public IMongoCollection<Product> Products => 
            _database.GetCollection<Product>("Products");

        public IMongoCollection<Order> Orders => 
            _database.GetCollection<Order>("Orders");
            
        public IMongoCollection<Category> Categories => 
            _database.GetCollection<Category>("Categories");

        public IMongoCollection<OrderItem> OrderItems => 
            _database.GetCollection<OrderItem>("OrderItems");
        
        public IMongoCollection<Customer> Customers => 
            _database.GetCollection<Customer>("Customers");
}
