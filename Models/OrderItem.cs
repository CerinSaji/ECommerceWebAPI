namespace ECommerceWebAPI.Models;

public class OrderItem
{
    public int Id { get; set; } //primary key
    public int OrderId { get; set; } //foreign key
    public int ProductId { get; set; } //foreign key
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}