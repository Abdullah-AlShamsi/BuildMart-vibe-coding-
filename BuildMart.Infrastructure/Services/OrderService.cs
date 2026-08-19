using BuildMart.Application.DTOs.Order;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using BuildMart.Domain.Exceptions;
using BuildMart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildMart.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null || !cart.Items.Any())
        {
            throw new BadRequestException("Your cart is empty.");
        }

        // Re-validate stock for every item right before committing the order —
        // it may have changed since the item was added to the cart.
        foreach (var item in cart.Items)
        {
            if (!item.Product.IsAvailable)
            {
                throw new BadRequestException($"'{item.Product.Name}' is no longer available.");
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                throw new BadRequestException(
                    $"Only {item.Product.StockQuantity} unit(s) of '{item.Product.Name}' are in stock.");
            }
        }

        if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
        {
            throw new BadRequestException($"Invalid payment method '{dto.PaymentMethod}'.");
        }

        await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = dto.ShippingAddress,
                PhoneNumber = dto.PhoneNumber,
                PaymentMethod = paymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                OrderStatus = OrderStatus.Pending
            };

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var unitPrice = item.Product.EffectivePrice;
                var lineTotal = unitPrice * item.Quantity;
                total += lineTotal;

                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = lineTotal
                });

                // Deduct stock at order creation time.
                item.Product.StockQuantity -= item.Quantity;
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetOrderByIdAsync(order.Id, userId, isAdmin: false);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, bool isAdmin)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException("Order", orderId);

        if (!isAdmin && order.UserId != userId)
        {
            throw new ForbiddenException("You do not have access to this order.");
        }

        return MapToDto(order);
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException("Order", orderId);

        if (!Enum.TryParse<OrderStatus>(dto.OrderStatus, true, out var newStatus))
        {
            throw new BadRequestException($"Invalid order status '{dto.OrderStatus}'.");
        }

        // Restock items if an order is cancelled after being placed.
        if (newStatus == OrderStatus.Cancelled && order.OrderStatus != OrderStatus.Cancelled)
        {
            foreach (var item in order.Items)
            {
                item.Product.StockQuantity += item.Quantity;
            }
        }

        order.OrderStatus = newStatus;

        if (newStatus == OrderStatus.Delivered)
        {
            order.PaymentStatus = PaymentStatus.Paid;
        }

        await _context.SaveChangesAsync();

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order) => new()
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
    };
}
