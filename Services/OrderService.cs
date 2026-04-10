using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Data;
using ECommerceWebAPI.Models;
using AutoMapper;

public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto requestDto)
    {
        var order = new Order
        {
            CustomerId = requestDto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            OrderItems = new List<OrderItem>()
        };

        decimal totalOrderAmount = 0;

        foreach (var itemDto in requestDto.Items)
        {
            // 1. Fetch Product via Unit of Work
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product {itemDto.ProductId} not found.");

            // 2. Validate Stock
            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}");

            // 3. Update Product Stock
            product.StockQuantity -= itemDto.Quantity;
            _unitOfWork.Products.Update(product);

            // 4. Create OrderItem Entity
            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price // Snapshot of price at time of order
            };

            order.OrderItems.Add(orderItem);
            totalOrderAmount += (orderItem.UnitPrice * orderItem.Quantity);
        }

        order.TotalAmount = totalOrderAmount;

        // 5. Save everything in ONE transaction
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CompleteAsync();

        // 6. Map to Response DTO
        return _mapper.Map<OrderResponseDto>(order);
    }
}