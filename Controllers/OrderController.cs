using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public OrderController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) // Includes product details for the items
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return _mapper.Map<OrderResponseDto>(order);
        }

        // POST: api/Order (The Checkout Logic)
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> PostOrder(OrderRequestDto orderDto)
        {
            // Start a transaction to ensure database integrity
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                foreach (var itemDto in orderDto.Items)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    
                    if (product == null)
                        return BadRequest($"Product ID {itemDto.ProductId} not found.");

                    if (product.StockQuantity < itemDto.Quantity)
                        return BadRequest($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}");

                    // Reduce Stock
                    product.StockQuantity -= itemDto.Quantity;

                    // Calculate Price
                    var lineTotal = product.Price * itemDto.Quantity;
                    totalAmount += lineTotal;

                    // Add the separate OrderItem entity to the Order collection
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price // Lock in the price at purchase time
                    });
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                var response = _mapper.Map<OrderResponseDto>(order);
                return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Optional: Restore stock when an order is deleted
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null) product.StockQuantity += item.Quantity;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}