using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(string id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException($"Category {id} not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task UpdateCategoryAsync(int id, CategoryDto categoryDto)
    {
        if (id != categoryDto.Id) throw new ArgumentException("ID mismatch");
        var categoryInDb = await _unitOfWork.Categories.GetByIdAsync(id);
        if (categoryInDb == null) throw new KeyNotFoundException($"Category {id} not found.");
        _mapper.Map(categoryDto, categoryInDb); // Map updated fields onto existing entity
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException($"Category {id} not found.");
        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.CompleteAsync();
    }
}