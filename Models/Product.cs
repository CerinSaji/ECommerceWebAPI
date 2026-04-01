using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ECommerceWebAPI.Models;
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; } //primary key
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? CategoryId { get; set; } //foreign key
}
