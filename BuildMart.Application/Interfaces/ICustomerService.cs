using BuildMart.Application.DTOs.Auth;
using BuildMart.Application.DTOs.Order;

namespace BuildMart.Application.Interfaces;

public interface ICustomerService
{
    Task<List<UserDto>> GetAllCustomersAsync();
    Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId);
}
