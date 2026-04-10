namespace ECommerceWebAPI.Models;
  
public class Order
{
    public int Id { get; set; } //primary key
    //must be referenced to a valid customer in the database
    public int CustomerId { get; set; } //foreign key
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}