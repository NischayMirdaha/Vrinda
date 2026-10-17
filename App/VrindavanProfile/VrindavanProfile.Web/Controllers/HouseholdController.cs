using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

public class HouseholdController : Controller
{
    private readonly IHouseholdService _householdService;

    private readonly ILogger<HouseholdController> _logger;

    public HouseholdController(
        IHouseholdService householdService,
        ILogger<HouseholdController> logger)
    {
        _householdService = householdService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Household — Lists all 65 household tables
    /// </summary>
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "घरपरिवार तथ्याङ्क";
        ViewData["MetaDescription"] = "वृन्दावन नगरपालिकाको ६५ वटा घरपरिवार तालिकाहरू — जनसंख्या, शिक्षा, स्वास्थ्य, आर्थिक, पूर्वाधार, कृषि, आवास, जोखिम तथ्याङ्क";
        
        var tables = await _householdService.GetTableListAsync();
        return View(tables);
    }

    /// <summary>
    /// GET: /Household/Table/5       — Table 5, all wards
    /// GET: /Household/Table/5?wardNo=3 — Table 5, ward 3 only
    /// </summary>
    public async Task<IActionResult> Table(int id, int? wardNo)
    {
        if (id < 1 || id > 65)
            return NotFound();

        try
        {
            var table = await _householdService.GetTableAsync(id, wardNo);
            if (table == null)
                return NotFound();

            ViewData["Title"] = table.PageTitle;
            return View(table);
        }
        catch (NotImplementedException)
        {
            ViewData["Title"] = $"तालिका नं. {id}";
            return View("TableNotReady", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading household table {TableId}", id);
            ViewData["Title"] = $"तालिका नं. {id}";
            return View("Error");
        }
    }

    /// <summary>
    /// GET: /Household/Print/5?wardNo=3 — Print-friendly table view
    /// </summary>
    public async Task<IActionResult> Print(int id, int? wardNo)
    {
        if (id < 1 || id > 65)
            return NotFound();

        var table = await _householdService.GetTableAsync(id, wardNo);
        if (table == null)
            return NotFound();

        ViewData["Title"] = table.PageTitle;
        return View(table);
    }
}
