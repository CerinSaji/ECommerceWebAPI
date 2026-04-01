namespace ECommerceWebAPI.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 

    public string? Id { get; set; } //primary key
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
}