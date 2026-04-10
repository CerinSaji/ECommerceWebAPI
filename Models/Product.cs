using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ECommerceWebAPI.Models;
public class Product
{
    public int Id { get; set; } //primary key
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; } //foreign key
}
