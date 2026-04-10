namespace ECommerceWebAPI.DTOs;

public class CustomerDto {
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}