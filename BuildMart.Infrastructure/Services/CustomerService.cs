using BuildMart.Application.DTOs.Auth;
using BuildMart.Application.DTOs.Order;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using BuildMart.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetAllCustomersAsync()
    {
        var customers = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer));

        return customers
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new UserDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email ?? string.Empty,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                City = c.City,
                Role = nameof(UserRole.Customer),
                CreatedAt = c.CreatedAt
            })
            .ToList();
    }

    public async Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .Where(o => o.UserId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CustomerName = order.User.FullName,
            CustomerEmail = order.User.Email ?? string.Empty,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            PhoneNumber = order.PhoneNumber,
            PaymentMethod = order.PaymentMethod.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            OrderStatus = order.OrderStatus.ToString(),
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        }).ToList();
    }
}
