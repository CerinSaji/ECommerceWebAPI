namespace ECommerceWebAPI.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class OrderItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } //primary key
    public string OrderId { get; set; } //foreign key
    public string ProductId { get; set; } //foreign key
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}