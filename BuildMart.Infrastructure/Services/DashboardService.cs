using BuildMart.Application.DTOs.Admin;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using BuildMart.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private const int LowStockThreshold = 10;

    public DashboardService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var totalSales = await _context.Orders
            .Where(o => o.OrderStatus != OrderStatus.Cancelled)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        var totalOrders = await _context.Orders.CountAsync();
        var totalProducts = await _context.Products.CountAsync();
        var totalCustomers = (await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer))).Count;
        var pendingOrders = await _context.Orders.CountAsync(o => o.OrderStatus == OrderStatus.Pending);

        var lowStock = await _context.Products
            .AsNoTracking()
            .Where(p => p.StockQuantity <= LowStockThreshold && p.IsAvailable)
            .OrderBy(p => p.StockQuantity)
            .Select(p => new LowStockProductDto { Id = p.Id, Name = p.Name, StockQuantity = p.StockQuantity, SKU = p.SKU })
            .Take(10)
            .ToListAsync();

        var byStatus = await _context.Orders
            .AsNoTracking()
            .GroupBy(o => o.OrderStatus)
            .Select(g => new SalesByStatusDto { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var recent = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .Select(o => new RecentOrderDto
            {
                Id = o.Id,
                CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus.ToString(),
                OrderDate = o.OrderDate
            })
            .ToListAsync();

        return new DashboardStatsDto
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            PendingOrders = pendingOrders,
            LowStockProducts = lowStock,
            OrdersByStatus = byStatus,
            RecentOrders = recent
        };
    }
}
