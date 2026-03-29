using Microsoft.EntityFrameworkCore;

namespace ECommerceWebAPI.Models;

public class ApplicationContext : DbContext
{
    //Constructor to initialize the context with options
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    //Define DbSet properties for each entity
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}