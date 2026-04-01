using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ECommerceWebAPI.Models;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } //primary key
    public string Name { get; set; }
    public string Description { get; set; }
}