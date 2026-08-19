using BuildMart.Application.DTOs.Order;

namespace BuildMart.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
    Task<List<OrderDto>> GetUserOrdersAsync(string userId);
    Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, bool isAdmin);
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
}
