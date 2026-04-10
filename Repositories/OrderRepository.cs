using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;


public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(DbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        return await _dbSet.Where(o => o.CustomerId == customerId).ToListAsync();
    }
}