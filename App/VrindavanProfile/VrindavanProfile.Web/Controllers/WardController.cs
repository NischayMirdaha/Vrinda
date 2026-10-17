using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Services.Interfaces;
using VrindavanProfile.Web.Models.ViewModels;   // NEW

namespace VrindavanProfile.Web.Controllers;

public class WardController : Controller
{
    private readonly IHouseholdService _householdService;
    private readonly IInstitutionService _institutionService;
    private readonly IWardService _wardService;   // NEW
    private readonly ILogger<WardController> _logger;

    public WardController(
        IHouseholdService householdService,
        IInstitutionService institutionService,
        IWardService wardService,                 // NEW
        ILogger<WardController> logger)
    {
        _householdService = householdService;
        _institutionService = institutionService;
        _wardService = wardService;               // NEW
        _logger = logger;
    }

    /// <summary>
    /// GET: /Ward — Ward profile landing + detailed view with optional ward filter
    /// GET: /Ward?wardNo=3 — shows detailed stats + charts for Ward 3
    /// </summary>
    public async Task<IActionResult> Index(int? wardNo = null)
    {
        ViewData["Title"] = wardNo.HasValue 
            ? $"वडा नं. {wardNo} — विस्तृत प्रोफाइल" 
            : "वडा प्रोफाइल";

        ViewData["MetaDescription"] = "वृन्दावन नगरपालिकाका प्रत्येक वडाको विस्तृत तथ्याङ्क र वडा तुलना";

        var model = await _wardService.GetWardProfileAsync(wardNo);
        return View(model);   // We will update the view in later micro-chunks to actually use the model
    }

    /// <summary>
    /// GET: /Ward/Compare — Ward comparison page (unchanged for now)
    /// ...
    /// </summary>
    public async Task<IActionResult> Compare(string? wards, int? tableId)
    {
        // ... (your existing Compare code stays exactly the same for this chunk)
        ViewData["Title"] = "वडा तुलना";
        ViewData["MetaDescription"] = "वृन्दावन नगरपालिकाको वडाहरू बीच तथ्याङ्कको तुलना गर्नुहोस्";

        ViewData["BreadcrumbItems"] = new List<(string Label, string? Url)> {
            ("गृहपृष्ठ", "/"),
            ("वडा प्रोफाइल", "/Ward")
        };

        // Parse selected ward numbers
        List<int> selectedWards = new();
        if (!string.IsNullOrWhiteSpace(wards))
        {
            selectedWards = wards.Split(',')
                .Select(s => int.TryParse(s.Trim(), out var n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
        }

        ViewBag.SelectedWards = selectedWards;
        ViewBag.SelectedTableId = tableId;

        // C.1 — load full dashboard (stats + grouped charts) for each selected ward
        // (reuses the same IWardService / Dashboard data that powers the detailed Index view)
        var wardDashboards = new List<DashboardViewModel>();
        foreach (var wardNo in selectedWards)
        {
            var profile = await _wardService.GetWardProfileAsync(wardNo);
            wardDashboards.Add(profile.Dashboard);
        }
        ViewBag.WardDashboards = wardDashboards;

        // Get available ward numbers from household service
        var hhWards = await _householdService.GetWardNumbersAsync();
        ViewBag.AvailableWards = hhWards.OrderBy(w => w).ToList();

        // Get table lists for dropdown
        var hhTables = await _householdService.GetTableListAsync();
        var instTables = await _institutionService.GetTableListAsync();
        ViewBag.HouseholdTables = hhTables;
        ViewBag.InstitutionTables = instTables;

        // If wards and tableId selected, load comparison data
        if (selectedWards.Count >= 2 && tableId.HasValue)
        {
            var tableData = new List<(int WardNo, VrindavanProfile.Web.Models.ViewModels.TableViewModel Table)>();
            
            bool isInstitutionTable = tableId.Value >= 1000;
            int actualTableId = isInstitutionTable ? tableId.Value - 1000 : tableId.Value;
            
            foreach (var ward in selectedWards)
            {
                try
                {
                    VrindavanProfile.Web.Models.ViewModels.TableViewModel? t = isInstitutionTable
                        ? await _institutionService.GetTableAsync(actualTableId, ward)
                        : await _householdService.GetTableAsync(actualTableId, ward);
                    
                    if (t != null) tableData.Add((ward, t));
                }
                catch { /* skip unavailable tables */ }
            }
            ViewBag.ComparisonData = tableData;
        }

        return View();
    }
}