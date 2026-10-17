using Microsoft.AspNetCore.Mvc;
using Load.Models;
using Load.Services;

namespace Load.Controllers;

public class HouseholdController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogger<HouseholdController> _logger;

    public HouseholdController(IConfiguration config, ILogger<HouseholdController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> UploadHousehold(IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
        {
            ViewBag.Error = "Please select household JSON file(s).";
            return View("Index");
        }

        try
        {
            var connStr = _config.GetConnectionString("CensusDb")!;
            var logger = LoggerFactory.Create(b => b.AddConsole())
                .CreateLogger<ImportService>();
            var svc = new ImportService(connStr, logger);

            var result = await svc.ImportMultipleAsync(
                files, "parent", "household", HouseholdMapping.Map);
            
            result.TableRowCounts = await svc.GetTableRowCountsAsync();

            return View("Result", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Household import failed");
            ViewBag.Error = $"Import failed: {ex.Message}";
            return View("Index");
        }
    }

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> UploadFamily(IFormFileCollection files)
    {
        return await ImportChild(files, "family_member", "Family Members");
    }

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> UploadHouseDetail(IFormFileCollection files)
    {
        return await ImportChild(files, "house_detail", "House Details");
    }

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> UploadDeath(IFormFileCollection files)
    {
        return await ImportChild(files, "hh_death_record", "Death Records");
    }

    private async Task<IActionResult> ImportChild(
        IFormFileCollection files, string tableName, string displayName)
    {
        if (files == null || files.Count == 0)
        {
            ViewBag.Error = $"Please select {displayName} JSON file(s).";
            return View("Index");
        }

        try
        {
            var connStr = _config.GetConnectionString("CensusDb")!;
            var logger = LoggerFactory.Create(b => b.AddConsole())
                .CreateLogger<ImportService>();
            var svc = new ImportService(connStr, logger);

            var mapping = tableName switch
            {
                "family_member" => ChildTableMapping.FamilyMap,
                "house_detail" => ChildTableMapping.HouseDetailMap,
                "hh_death_record" => ChildTableMapping.DeathRecordMap,
                _ => throw new Exception($"Unknown table: {tableName}")
            };

            var result = await svc.ImportMultipleAsync(
                files, "child", tableName, mapping, parentTable: "household");

            return View("Result", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Table} import failed", displayName);
            ViewBag.Error = $"Import failed: {ex.Message}";
            return View("Index");
        }
    }
}