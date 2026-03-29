namespace ECommerceWebAPI.Models;

public class Order
{
    public int Id { get; set; } //primary key
    public int CustomerId { get; set; } //foreign key
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}