using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;


public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(DbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        return await _dbSet.Where(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task UpdateStatusAsync(int orderId, string status)
    {
        var order = await GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            Update(order); //generic repo method is used to update order
        }
    }
}