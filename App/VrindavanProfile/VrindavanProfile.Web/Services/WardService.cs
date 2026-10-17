using Microsoft.EntityFrameworkCore;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

public class WardService : IWardService
{
    private readonly IDashboardService _dashboardService;
    private readonly AppDbContext _db;

    public WardService(IDashboardService dashboardService, AppDbContext db)
    {
        _dashboardService = dashboardService;
        _db = db;
    }

    public async Task<WardProfileViewModel> GetWardProfileAsync(int? wardNo = null)
    {
        var wards = await GetWardListAsync();

        // If ward selected, get filtered dashboard; otherwise empty dashboard
        DashboardViewModel dashboard;
        if (wardNo.HasValue)
        {
            dashboard = await _dashboardService.GetDashboardDataAsync(wardNo.Value);
        }
        else
        {
            // Show municipality-wide overview when no ward selected
            dashboard = await _dashboardService.GetDashboardDataAsync(null);
        }

        return new WardProfileViewModel
        {
            SelectedWard = wardNo,
            Wards = wards,
            Dashboard = dashboard
        };
    }

    public async Task<List<WardInfo>> GetWardListAsync()
    {
        // Get ward numbers
        var wardNos = await _db.Households
            .Where(h => h.WardNo != null)
            .Select(h => h.WardNo!.Value)
            .Distinct()
            .OrderBy(w => w)
            .ToListAsync();

        // Get household count per ward
        var hhCounts = await _db.Households
            .Where(h => h.WardNo != null)
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new { WardNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.WardNo, x => x.Count);

        // Get population by gender per ward (via family_member → household)
        var popData = await _db.FamilyMembers
            .AsNoTracking()
            .Where(fm => fm.Household != null && fm.Household.WardNo != null)
            .GroupBy(fm => fm.Household!.WardNo!.Value)
            .Select(g => new
            {
                WardNo = g.Key,
                Total = g.Count(),
                Male = g.Count(fm => fm.Gender == "पुरूष"),
                Female = g.Count(fm => fm.Gender == "महिला")
            })
            .ToDictionaryAsync(x => x.WardNo);

        return wardNos.Select(w => new WardInfo
        {
            WardNo = w,
            HouseholdCount = hhCounts.GetValueOrDefault(w),
            Population = popData.GetValueOrDefault(w)?.Total ?? 0,
            MaleCount = popData.GetValueOrDefault(w)?.Male ?? 0,
            FemaleCount = popData.GetValueOrDefault(w)?.Female ?? 0
        }).ToList();
    }

    public async Task<int> GetTotalWardsAsync()
    {
        return await _db.Households
            .Where(h => h.WardNo != null)
            .Select(h => h.WardNo!.Value)
            .Distinct()
            .CountAsync();
    }
}