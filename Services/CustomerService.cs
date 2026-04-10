using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public class CustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) throw new KeyNotFoundException($"Customer {id} not found.");
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> AddCustomerAsync(CustomerDto customerDto)
    {
        var customer = _mapper.Map<Customer>(customerDto);
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task UpdateCustomerAsync(int id, CustomerDto customerDto)
    {
        var customerInDb = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customerInDb == null) throw new KeyNotFoundException($"Customer {id} not found.");

        _mapper.Map(customerDto, customerInDb);
        _unitOfWork.Customers.Update(customerInDb);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) throw new KeyNotFoundException($"Customer {id} not found.");

        _unitOfWork.Customers.Delete(customer);
        await _unitOfWork.CompleteAsync();
    }
}