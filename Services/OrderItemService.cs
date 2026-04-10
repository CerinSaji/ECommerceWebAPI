using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public class OrderItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderItemResponseDto>> GetOrderItemsAsync()
    {
        var items = await _unitOfWork.OrderItems.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderItemResponseDto>>(items);
    }

    public async Task<OrderItemResponseDto> GetOrderItemByIdAsync(int id)
    {
        var item = await _unitOfWork.OrderItems.GetByIdAsync(id);
        if (item == null) throw new KeyNotFoundException($"OrderItem {id} not found.");
        return _mapper.Map<OrderItemResponseDto>(item);
    }

    public async Task<OrderItemResponseDto> AddOrderItemAsync(OrderItemRequestDto orderItemDto)
    {
        var item = _mapper.Map<OrderItem>(orderItemDto);
        await _unitOfWork.OrderItems.AddAsync(item);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<OrderItemResponseDto>(item);
    }
}