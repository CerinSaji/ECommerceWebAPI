using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;
using ECommerceWebAPI.Data;
using MongoDB.Driver;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly MongoDbService _mongoService;

        public OrderController(MongoDbService mongoService, IMapper mapper)
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _mongoService.Orders
                .Find(_ => true)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(string id)
        {
            var order = await _mongoService.Orders
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();
                //.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return _mapper.Map<OrderResponseDto>(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> PostOrder(OrderRequestDto orderDto)
        {
            // Start a transaction to ensure database integrity
            // using var transaction = await _mongoService._database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

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

                //there is a list of items in the order
                foreach (var itemDto in orderDto.Items)
                {
                    var cursor = await _mongoService.Products.FindAsync(p => p.Id == itemDto.ProductId);
                    var product = await cursor.FirstOrDefaultAsync();
                    
                    if (product == null)
                        return BadRequest($"Product ID {itemDto.ProductId} not found.");

                    if (product.StockQuantity < itemDto.Quantity)
                        return BadRequest($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}");

                    // Reduce Stock
                    Console.WriteLine($"Reducing stock for {product.Name}: {product.StockQuantity} - {itemDto.Quantity}");
                    product.StockQuantity -= itemDto.Quantity;

                    var update = Builders<Product>.Update.Set(p => p.StockQuantity, product.StockQuantity);
                    await _mongoService.Products.UpdateOneAsync(p => p.Id == product.Id, update);
                    Console.WriteLine($"New stock for {product.Name}: {product.StockQuantity}");

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

                await _mongoService.Orders.InsertOneAsync(order);
                //await _.SaveChangesAsync();

                // Commit the transaction
                //await transaction.CommitAsync();

                var response = _mapper.Map<OrderResponseDto>(order);
                return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _mongoService.Orders
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            //restore stock when an order is deleted
            foreach (var item in order.OrderItems)
            {
                var cursor = await _mongoService.Products.FindAsync(p => p.Id == item.ProductId);
                var product = await cursor.FirstOrDefaultAsync();
                if (product != null) product.StockQuantity += item.Quantity;
            }

            await _mongoService.Orders.DeleteOneAsync(o => o.Id == id);
            //await _mongoService.SaveChangesAsync();

            return NoContent();
        }

        //GET: api/Order/ByCustomer/5
        [HttpGet("ByCustomer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(string customerId)
        {
            var orders = await _mongoService.Orders
                .Find(o => o.CustomerId == customerId)
                .ToListAsync();

            if (!orders.Any()) return NotFound($"No orders found for Customer ID {customerId}");

            return Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
        }

        //PATCH: api/Order/Status/5
        [HttpPatch("Status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string newStatus)
        {
            var order = await _mongoService.Orders
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // Validate newStatus against allowed values (e.g., "Pending", "Shipped", "Delivered", "Cancelled")
            var allowedStatuses = new[] { "Pending", "Shipped", "Delivered", "Cancelled" };
            if (!allowedStatuses.Contains(newStatus))
                return BadRequest($"Invalid status. Allowed values are: {string.Join(", ", allowedStatuses)}");

            var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
            await _mongoService.Orders.UpdateOneAsync(o => o.Id == id, update);

            return NoContent();
        }
    }
}