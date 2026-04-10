using ECommerceWebAPI.Models;

public interface IUnitOfWork : IDisposable
{
    // Expose specific repositories
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<OrderItem> OrderItems { get; }
    IOrderRepository Orders { get; }
    
    // The "Big Save" button
    Task<int> CompleteAsync();
}