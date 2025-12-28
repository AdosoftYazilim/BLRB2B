using AutoMapper;
using BLRB2B.Application.Dto;
using BLRB2B.Application.Interfaces;
using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;

namespace BLRB2B.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        return customer == null ? null : _mapper.Map<CustomerDto>(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync()
    {
        var customers = await _customerRepository.GetActiveCustomersAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto)
    {
        // Check if email is unique
        if (!await _customerRepository.IsEmailUniqueAsync(dto.Email))
        {
            throw new InvalidOperationException($"Customer with email '{dto.Email}' already exists.");
        }

        // Check if tax number is unique
        if (!string.IsNullOrEmpty(dto.TaxNumber) && !await _customerRepository.IsTaxNumberUniqueAsync(dto.TaxNumber))
        {
            throw new InvalidOperationException($"Customer with tax number '{dto.TaxNumber}' already exists.");
        }

        var customer = _mapper.Map<Customer>(dto);
        var createdCustomer = await _customerRepository.AddAsync(customer);
        return _mapper.Map<CustomerDto>(createdCustomer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(CustomerUpdateDto dto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(dto.Id);
        if (existingCustomer == null)
        {
            return null;
        }

        // Check if email is unique
        if (!await _customerRepository.IsEmailUniqueAsync(dto.Email, dto.Id))
        {
            throw new InvalidOperationException($"Customer with email '{dto.Email}' already exists.");
        }

        // Check if tax number is unique
        if (!string.IsNullOrEmpty(dto.TaxNumber) && !await _customerRepository.IsTaxNumberUniqueAsync(dto.TaxNumber, dto.Id))
        {
            throw new InvalidOperationException($"Customer with tax number '{dto.TaxNumber}' already exists.");
        }

        _mapper.Map(dto, existingCustomer);
        existingCustomer.UpdatedAt = DateTime.UtcNow;
        await _customerRepository.UpdateAsync(existingCustomer);
        return _mapper.Map<CustomerDto>(existingCustomer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return false;
        }

        await _customerRepository.DeleteAsync(customer);
        return true;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return await _customerRepository.IsEmailUniqueAsync(email, excludeId);
    }

    public async Task<bool> IsTaxNumberUniqueAsync(string taxNumber, int? excludeId = null)
    {
        return await _customerRepository.IsTaxNumberUniqueAsync(taxNumber, excludeId);
    }
}
