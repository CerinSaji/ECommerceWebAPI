namespace ECommerceWebAPI.DTOs;

public class ProductResponseDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    //public string CategoryName { get; set; } = string.Empty; // Flattened
    public int StockQuantity { get; set; }
}

public class ProductCreateDto {
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int StockQuantity { get; set; }
}