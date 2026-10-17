using Microsoft.AspNetCore.Mvc;
using Load.Models;
using Load.Services;

namespace Load.Controllers;

public class ValidateController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [RequestSizeLimit(600_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 600_000_000)]
    public async Task<IActionResult> Check(IFormFile file, string fileType = "institutional")
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.Error = "Please select a JSON file.";
            return View("Index");
        }

        string[] requiredKeys = fileType switch
        {
            "institutional" => new[] {
                "_id", "_uuid", "_index", "_submission_time",
                "विषयगतक्षेत्र छान्नुहोस्", "१.२ वडा नं" },
            "household" => new[] {
                "_id", "_uuid", "_index", "_submission_time",
                "१.२  वडा  नम्बर" },
            "family" => new[] {
                "_index", "_parent_index", "_submission__id",
                "_parent_table_name", "४.१. नाम" },
            "house_detail" => new[] {
                "_index", "_parent_index", "_submission__id",
                "_parent_table_name" },
            "death" => new[] {
                "_index", "_parent_index", "_submission__id",
                "_parent_table_name",
                "५.२.१. मृत्यु हुनेको नाम लेख्नुहोस" },
            _ => new[] { "_id" }
        };

        var validator = new JsonValidatorService();
        using var stream = file.OpenReadStream();
        var result = await validator.ValidateAsync(
            stream, file.FileName, file.Length, requiredKeys);

        return View("Result", result);
    }
}