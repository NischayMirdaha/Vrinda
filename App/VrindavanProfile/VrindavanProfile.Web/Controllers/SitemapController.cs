using Microsoft.AspNetCore.Mvc;

namespace VrindavanProfile.Web.Controllers;

/// <summary>
/// Generates a dynamic XML sitemap for SEO (Part 13D).
/// </summary>
public class SitemapController : Controller
{
    private const string BaseUrl = "https://vrindavanmun.gov.np";
    
    [HttpGet("/Sitemap")]
    public IActionResult Html()
    {
        ViewData["CurrentLang"] = "ne";
        return View();
    }

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600)] // Cache for 1 hour
    public IActionResult Index()
    {
        var urls = new List<SitemapUrl>();

        // ─── Static Pages ───
        urls.Add(new SitemapUrl(BaseUrl, 1.0, "daily"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Home/About", 0.7, "monthly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Ward", 0.8, "weekly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Ward/Compare", 0.7, "weekly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Household", 0.9, "weekly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Institution", 0.9, "weekly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Map", 0.8, "weekly"));
        urls.Add(new SitemapUrl($"{BaseUrl}/Search", 0.5, "monthly"));

        // ─── Household Tables (1-65) ───
        for (int i = 1; i <= 65; i++)
        {
            urls.Add(new SitemapUrl($"{BaseUrl}/Household/Table/{i}", 0.6, "monthly"));
        }

        // ─── Institution Tables (1-43) ───
        for (int i = 1; i <= 43; i++)
        {
            urls.Add(new SitemapUrl($"{BaseUrl}/Institution/Table/{i}", 0.6, "monthly"));
        }

        ViewData["Urls"] = urls;
        Response.ContentType = "application/xml";
        return View();
    }

    public record SitemapUrl(string Loc, double Priority, string Changefreq);
}
