using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public HomeController(IDashboardService dashboardService, ILogger<HomeController> logger, AppDbContext db, IWebHostEnvironment env)
    {
        _dashboardService = dashboardService;
        _logger = logger;
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var model = await _dashboardService.GetDashboardDataAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard data load failed");
            return View(new DashboardViewModel());
        }
    }

    /// <summary>
    /// GET: /Home/About — Municipality information page (Part 11F)
    /// </summary>
    public IActionResult About()
    {
        ViewData["Title"] = "नगरपालिका परिचय";
        
        ViewData["BreadcrumbItems"] = new List<(string Label, string? Url)> {
            ("गृहपृष्ठ", "/")
        };
        return View();
    }

    /// <summary>
    /// GET: /Home/Error — Global error page (Part 13A)
    /// Triggered by UseExceptionHandler for unhandled exceptions,
    /// and UseStatusCodePagesWithReExecute for non-success status codes.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        ViewData["Title"] = "त्रुटि";

        // Retrieve exception details from the middleware
        var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var statusCodeFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>();

        var requestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var requestPath = exceptionFeature?.Path ?? statusCodeFeature?.OriginalPath ?? Request.Path;

        _logger.LogError(
            "Error page hit — Status: {StatusCode}, Path: {Path}, RequestId: {RequestId}",
            statusCode ?? Response.StatusCode,
            requestPath,
            requestId);

        // Route to specific error views
        switch (statusCode ?? Response.StatusCode)
        {
            case 404:
                return View("NotFound", new ErrorViewModel
                {
                    StatusCode = 404,
                    RequestId = requestId,
                    RequestPath = requestPath
                });

            case 403:
                return View("Forbidden", new ErrorViewModel
                {
                    StatusCode = 403,
                    RequestId = requestId,
                    RequestPath = requestPath
                });

            default:
                return View(new ErrorViewModel
                {
                    StatusCode = statusCode ?? Response.StatusCode,
                    RequestId = requestId,
                    RequestPath = requestPath,
                    ShowException = _env.IsDevelopment()
                });
        }
    }

    // TEMP DIAGNOSTIC — visit /Home/Diag
    public async Task<IActionResult> Diag()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("=== GENDER VALUES ===");
        var genders = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Gender != null)
            .GroupBy(fm => fm.Gender)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt)
            .ToListAsync();
        foreach (var g in genders)
        {
            var hex = string.Join(" ", g.Val.Select(c => $"U+{((int)c):X4}"));
            sb.AppendLine($"[{g.Val}] hex=[{hex}] count={g.Cnt}");
        }

        sb.AppendLine("\n=== TOP 5 CASTE ===");
        var castes = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Caste != null && fm.Caste != "")
            .GroupBy(fm => fm.Caste)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var c in castes) sb.AppendLine($"[{c.Val}] count={c.Cnt}");

        sb.AppendLine("\n=== TOP 5 RELIGION ===");
        var rels = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Religion != null && fm.Religion != "")
            .GroupBy(fm => fm.Religion)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var r in rels) sb.AppendLine($"[{r.Val}] count={r.Cnt}");

        sb.AppendLine("\n=== TOP 5 DRINKING WATER ===");
        var waters = await _db.Households.AsNoTracking()
            .Where(h => h.DrinkingWaterSource != null && h.DrinkingWaterSource != "")
            .GroupBy(h => h.DrinkingWaterSource)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var w in waters) sb.AppendLine($"[{w.Val}] count={w.Cnt}");

        sb.AppendLine("\n=== TOP 5 COOKING FUEL ===");
        var fuels = await _db.Households.AsNoTracking()
            .Where(h => h.CookingFuelPrimary != null && h.CookingFuelPrimary != "")
            .GroupBy(h => h.CookingFuelPrimary)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var f in fuels) sb.AppendLine($"[{f.Val}] count={f.Cnt}");

        sb.AppendLine("\n=== TOP 5 LIGHT SOURCE ===");
        var lights = await _db.Households.AsNoTracking()
            .Where(h => h.LightSource != null && h.LightSource != "")
            .GroupBy(h => h.LightSource)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var l in lights) sb.AppendLine($"[{l.Val}] count={l.Cnt}");

        sb.AppendLine("\n=== TOP 5 MARITAL STATUS ===");
        var marital = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.MaritalStatus != null && fm.MaritalStatus != "")
            .GroupBy(fm => fm.MaritalStatus)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var m in marital) sb.AppendLine($"[{m.Val}] count={m.Cnt}");

        sb.AppendLine("\n=== TOP 5 OCCUPATION ===");
        var occs = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Occupation != null && fm.Occupation != "")
            .GroupBy(fm => fm.Occupation)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var o in occs) sb.AppendLine($"[{o.Val}] count={o.Cnt}");

        sb.AppendLine("\n=== TOP 5 RESIDENCE STATUS ===");
        var resid = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.ResidenceStatus != null && fm.ResidenceStatus != "")
            .GroupBy(fm => fm.ResidenceStatus)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var r in resid) sb.AppendLine($"[{r.Val}] count={r.Cnt}");

        sb.AppendLine("\n=== TOP 5 DISABILITY ===");
        var disab = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.DisabilityStatus != null && fm.DisabilityStatus != "")
            .GroupBy(fm => fm.DisabilityStatus)
            .Select(g => new { Val = g.Key!, Cnt = g.Count() })
            .OrderByDescending(x => x.Cnt).Take(5).ToListAsync();
        foreach (var d in disab) sb.AppendLine($"[{d.Val}] count={d.Cnt}");

        sb.AppendLine("\n=== HOUSEHOLD COUNT ===");
        sb.AppendLine((await _db.Households.CountAsync()).ToString());

        sb.AppendLine("\n=== FAMILY MEMBER COUNT ===");
        sb.AppendLine((await _db.FamilyMembers.CountAsync()).ToString());

        sb.AppendLine("\n=== HOUSE DETAIL COUNT ===");
        sb.AppendLine((await _db.HouseDetails.CountAsync()).ToString());

        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }
}
