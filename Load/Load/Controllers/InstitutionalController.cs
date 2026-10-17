using Microsoft.AspNetCore.Mvc;
using Load.Models;
using Load.Services;

namespace Load.Controllers;

public class InstitutionalController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogger<InstitutionalController> _logger;

    public InstitutionalController(IConfiguration config, ILogger<InstitutionalController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> Upload(IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
        {
            ViewBag.Error = "Please select JSON file(s).";
            return View("Index");
        }

        try
        {
            var connStr = _config.GetConnectionString("CensusDb")!;
            var logger = LoggerFactory.Create(b => b.AddConsole())
                .CreateLogger<ImportService>();
            var svc = new ImportService(connStr, logger);

            var result = await svc.ImportMultipleAsync(
                files, "institutional", "survey_entry",
                InstitutionalMapping.Map,
                InstitutionalMapping.SubjectKeywords);
            
            result.TableRowCounts = await svc.GetTableRowCountsAsync();

            return View("Result", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Institutional import failed");
            ViewBag.Error = $"Import failed: {ex.Message}";
            return View("Index");
        }
    }
}