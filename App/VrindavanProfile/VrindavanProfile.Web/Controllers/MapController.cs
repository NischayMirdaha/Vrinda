using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

public class MapController : Controller
{
    private readonly IMapService _mapService;
    private readonly ILogger<MapController> _logger;

    public MapController(IMapService mapService, ILogger<MapController> logger)
    {
        _mapService = mapService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Map — Interactive map with all GPS points
    /// GET: /Map?wardNo=3 — Filtered by ward 3
    /// GET: /Map?entityType=health_institution — Filtered by type
    /// </summary>
    public async Task<IActionResult> Index(int? wardNo, string? entityType)
    {
        ViewData["Title"] = "अन्तरक्रियात्मक नक्सा";
        ViewData["MetaDescription"] = "वृन्दावन नगरपालिकाको अन्तरक्रियात्मक नक्सा — शैक्षिक संस्था, स्वास्थ्य, वित्तीय, पूर्वाधार र घरपरिवारको GPS अवस्थिति";

        try
        {
            var mapData = await _mapService.GetMapDataAsync(wardNo, entityType);
            return View(mapData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading map data");
            return View("Error");
        }
    }

    /// <summary>
    /// GET: /Map/Institutions — API: returns institution markers as JSON
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Institutions(int? wardNo)
    {
        var markers = await _mapService.GetInstitutionMarkersAsync(wardNo);
        return Json(markers);
    }

    /// <summary>
    /// GET: /Map/Households — API: returns household markers as JSON
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Households(int? wardNo)
    {
        var markers = await _mapService.GetHouseholdMarkersAsync(wardNo);
        return Json(markers);
    }

    /// <summary>
    /// GET: /Map/Summary — API: returns type summaries as JSON
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Summary()
    {
        var summaries = await _mapService.GetTypeSummariesAsync();
        return Json(summaries);
    }
}
