namespace ECommerceWebAPI.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;    
public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 

    public string? Id { get; set; } //primary key

    [BsonRepresentation(BsonType.ObjectId)] 
    //must be referenced to a valid customer in the database
    public string CustomerId { get; set; } //foreign key
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}