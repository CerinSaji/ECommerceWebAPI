using ECommerceWebAPI.Models;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public IGenericRepository<Product> Products { get; private set; }
    public IOrderRepository Orders { get; private set; }
    public IGenericRepository<OrderItem> OrderItems { get; private set; }

    public IGenericRepository<Customer> Customers { get; private set; }
    public UnitOfWork(DbContext context)
    {
        _context = context;
        Products = new GenericRepository<Product>(_context);
        Orders = new OrderRepository(_context);
        Customers = new GenericRepository<Customer>(_context);
        OrderItems = new GenericRepository<OrderItem>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        // EF Core handles the transaction here automatically!
        return await _context.SaveChangesAsync();
    }

    public void Dispose() => _context.Dispose();
}