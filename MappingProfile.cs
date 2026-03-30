using AutoMapper;
using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<ProductCreateDto, Product>();

        // Category Mappings
        CreateMap<Category, CategoryDto>().ReverseMap();

        // Customer Mappings
        CreateMap<Customer, CustomerDto>().ReverseMap();

        // Order Mappings
        CreateMap<OrderRequestDto, Order>();
        CreateMap<Order, OrderResponseDto>();
        CreateMap<OrderItemRequestDto, OrderItem>();
    }
}