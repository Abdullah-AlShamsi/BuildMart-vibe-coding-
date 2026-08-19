namespace BuildMart.Application.DTOs.Admin;

public class DashboardStatsDto
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int PendingOrders { get; set; }
    public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    public List<SalesByStatusDto> OrdersByStatus { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class LowStockProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = string.Empty;
}

public class SalesByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}
