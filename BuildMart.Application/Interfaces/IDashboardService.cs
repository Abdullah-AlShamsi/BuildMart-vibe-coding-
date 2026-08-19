using BuildMart.Application.DTOs.Admin;

namespace BuildMart.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
