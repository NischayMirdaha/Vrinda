using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

public class InstitutionController : Controller
{
    private readonly IInstitutionService _institutionService;

    private readonly ILogger<InstitutionController> _logger;

    public InstitutionController(
        IInstitutionService institutionService,
        ILogger<InstitutionController> logger)
    {
        _institutionService = institutionService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Institution — Lists all 43 institutional tables
    /// </summary>
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "संस्थागत तथ्याङ्क";
        ViewData["MetaDescription"] = "वृन्दावन नगरपालिकाको ४३ वटा संस्थागत तालिकाहरू — शैक्षिक संस्था, स्वास्थ्य, वित्तीय, पूर्वाधार, कृषि, सुरक्षा तथ्याङ्क";

        var tables = await _institutionService.GetTableListAsync();
        return View(tables);
    }

    /// <summary>
    /// GET: /Institution/Table/5       — Table 5, all wards
    /// GET: /Institution/Table/5?wardNo=3 — Table 5, ward 3 only
    /// </summary>
    public async Task<IActionResult> Table(int id, int? wardNo)
    {
        if (id < 1 || id > 43)
            return NotFound();

        try
        {
            var table = await _institutionService.GetTableAsync(id, wardNo);
            if (table == null)
                return NotFound();

            ViewData["Title"] = table.PageTitle;
            return View("~/Views/Household/Table.cshtml", table);
        }
        catch (NotImplementedException)
        {
            ViewData["Title"] = $"तालिका नं. {id}";
            return View("TableNotReady", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading institutional table {TableId}", id);
            ViewData["Title"] = $"तालिका नं. {id}";
            return View("Error");
        }
    }

    /// <summary>
    /// GET: /Institution/Print/5?wardNo=3 — Print-friendly table view
    /// </summary>
    public async Task<IActionResult> Print(int id, int? wardNo)
    {
        if (id < 1 || id > 43)
            return NotFound();

        var table = await _institutionService.GetTableAsync(id, wardNo);
        if (table == null)
            return NotFound();

        ViewData["Title"] = table.PageTitle;
        return View("~/Views/Household/Print.cshtml", table);
    }
}
