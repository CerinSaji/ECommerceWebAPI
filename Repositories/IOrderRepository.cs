using ECommerceWebAPI.Models;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
    Task UpdateStatusAsync(int orderId, string status);
}