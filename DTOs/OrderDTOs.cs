namespace ECommerceWebAPI.DTOs;

// OrderItemRequestDto.cs
public class OrderItemRequestDto {
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice; // Calculated property
    }

// OrderRequestDto.cs
public class OrderRequestDto {
    public int CustomerId { get; set; }
    public List<OrderItemRequestDto> Items { get; set; } = new();
}

public class OrderResponseDto {
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponseDto> Items { get; set; } = new();

}