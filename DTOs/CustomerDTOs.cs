namespace ECommerceWebAPI.DTOs;

public class CustomerDto {
    public string? Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}