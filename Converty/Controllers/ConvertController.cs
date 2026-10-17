using System.IO.Compression;
using Converty.Services;
using Microsoft.AspNetCore.Mvc;

namespace Converty.Controllers;

public class ConvertController : Controller
{
    private readonly ExcelToJsonService _service;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ConvertController> _logger;

    public ConvertController(ExcelToJsonService service, IWebHostEnvironment env, ILogger<ConvertController> logger)
    {
        _service = service;
        _env     = env;
        _logger  = logger;
    }

    // GET: /Convert
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Convert/Upload
    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> Upload(IFormFile excelFile)
    {
        var outputFolder = Path.Combine(_env.ContentRootPath, "TempOutputs");
        
        // ── Clean up old sessions (>1 hour old) ──
        CleanupOldSessions(outputFolder);

        // ── Validation ────────────────────────────────────────────────────────
        if (excelFile == null || excelFile.Length == 0)
        {
            return BadRequest(new { error = "Please select an Excel file before converting." });
        }

        var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".xls" && ext != ".xlsb")
        {
            return BadRequest(new { error = "Unsupported file type. Please upload a .xlsx, .xls, or .xlsb file." });
        }

        try
        {
            _logger.LogInformation("Starting conversion of file: {FileName} ({Size} bytes)",
                excelFile.FileName, excelFile.Length);

            await using var stream = excelFile.OpenReadStream();
            var result = await _service.ConvertAsync(stream, outputFolder);

            _logger.LogInformation("Conversion complete. Session: {SessionId}, Sheets: {Count}", result.SessionId, result.Sheets.Count);
            
            return Json(new
            {
                sessionId = result.SessionId,
                excelFileName = excelFile.FileName,
                sheets = result.Sheets,
                metadataFileName = result.MetadataFileName,
                metadataFileSize = result.MetadataFileSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Conversion failed for file: {FileName}", excelFile.FileName);
            return StatusCode(500, new { error = $"Conversion failed: {ex.Message}" });
        }
    }

    // POST: /Convert/Download
    [HttpPost]
    public IActionResult Download([FromForm] DownloadRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SessionId))
        {
            TempData["Error"] = "Invalid download request.";
            return RedirectToAction(nameof(Index));
        }

        var outputFolder = Path.Combine(_env.ContentRootPath, "TempOutputs");
        var sessionFolder = Path.Combine(outputFolder, request.SessionId);

        if (!Directory.Exists(sessionFolder))
        {
            TempData["Error"] = "Session has expired or does not exist. Please upload your file again.";
            return RedirectToAction(nameof(Index));
        }

        var zipFileName = $"Converty_Download_{Guid.NewGuid():N}.zip";
        var zipPath = Path.Combine(outputFolder, zipFileName);

        try
        {
            using (var zipFileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 131072))
            using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create, leaveOpen: false))
            {
                foreach (var sheet in request.Sheets)
                {
                    if (sheet.DownloadMode == "single")
                    {
                        var sourceFile = Path.Combine(sessionFolder, "single", $"{sheet.SafeName}.json");
                        if (System.IO.File.Exists(sourceFile))
                        {
                            archive.CreateEntryFromFile(sourceFile, $"{sheet.SafeName}.json", CompressionLevel.Fastest);
                        }
                    }
                    else if (sheet.DownloadMode == "split")
                    {
                        var splitDir = Path.Combine(sessionFolder, "split");
                        if (Directory.Exists(splitDir))
                        {
                            var pattern = $"{sheet.SafeName}_*.json";
                            var files = Directory.GetFiles(splitDir, pattern);
                            foreach (var file in files)
                            {
                                archive.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Fastest);
                            }
                        }
                    }
                }

                if (request.IncludeMetadata)
                {
                    var metaFile = Path.Combine(sessionFolder, "excel_schema_metadata.txt");
                    if (System.IO.File.Exists(metaFile))
                    {
                        archive.CreateEntryFromFile(metaFile, "excel_schema_metadata.txt", CompressionLevel.Optimal);
                    }
                }
            }

            // Immediately clean up session directory since the zip file is fully built
            if (Directory.Exists(sessionFolder))
            {
                Directory.Delete(sessionFolder, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to package and zip files for session: {SessionId}", request.SessionId);
            TempData["Error"] = $"Download preparation failed: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }

        // Stream ZIP file to browser and delete on close
        var fileStream = new FileStream(
            zipPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.None,
            4096,
            FileOptions.DeleteOnClose);

        // Set cookie so frontend knows download has started
        Response.Cookies.Append("ConvertyDownloadComplete", "true", new CookieOptions
        {
            HttpOnly = false,
            Path = "/",
            SameSite = SameSiteMode.Lax
        });

        var cleanFileName = Path.GetFileNameWithoutExtension(request.ExcelFileName);
        var downloadName = $"Converty_{cleanFileName}.zip";
        return File(fileStream, "application/zip", downloadName);
    }

    private void CleanupOldSessions(string outputFolder)
    {
        try
        {
            if (!Directory.Exists(outputFolder)) return;

            var now = DateTime.Now;
            var subdirs = Directory.GetDirectories(outputFolder);
            foreach (var dir in subdirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                if (now.Subtract(dirInfo.CreationTime).TotalHours > 1)
                {
                    Directory.Delete(dir, true);
                }
            }

            var files = Directory.GetFiles(outputFolder, "Converty_Download_*.zip");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (now.Subtract(fileInfo.CreationTime).TotalHours > 1)
                {
                    System.IO.File.Delete(file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup old sessions");
        }
    }
}

public class DownloadRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string ExcelFileName { get; set; } = string.Empty;
    public List<SheetDownloadChoice> Sheets { get; set; } = new();
    public bool IncludeMetadata { get; set; }
}

public class SheetDownloadChoice
{
    public string SheetName { get; set; } = string.Empty;
    public string SafeName { get; set; } = string.Empty;
    public string DownloadMode { get; set; } = string.Empty; // "single", "split", or "none"
}
