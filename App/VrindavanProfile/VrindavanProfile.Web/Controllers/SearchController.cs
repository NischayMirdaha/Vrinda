using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

/// <summary>
/// Handles global search across all tables and data sources.
/// </summary>
public class SearchController : Controller
{
    private readonly IHouseholdService _householdService;
    private readonly IInstitutionService _institutionService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        IHouseholdService householdService,
        IInstitutionService institutionService,
        ILogger<SearchController> logger)
    {
        _householdService = householdService;
        _institutionService = institutionService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Search — Search page (full-screen overlay style)
    /// </summary>
    public async Task<IActionResult> Index(string? q)
    {
        ViewData["Title"] = "खोज्नुहोस्";
        ViewData["MetaDescription"] = "वृन्दावन नगरपालिका डिजिटल प्रोफाइलमा तालिका, शीर्षक वा नम्बर खोज्नुहोस्";

        // Breadcrumbs (Part 11B)
        ViewData["BreadcrumbItems"] = new List<(string Label, string? Url)> {
            ("गृहपृष्ठ", "/")
        };

        var householdTables = await _householdService.GetTableListAsync();
        var institutionTables = await _institutionService.GetTableListAsync();

        // Combine into a unified search index
        var allItems = new List<SearchResultItem>();

        foreach (var t in householdTables)
        {
            allItems.Add(new SearchResultItem
            {
                TableNumber = t.TableNumber,
                Title = t.Title,
                Category = t.Category,
                Description = t.Description ?? "",
                Section = "Household",
                SectionLabel = "घरपरिवार",
                Url = $"/Household/Table/{t.TableNumber}",
                Icon = t.Icon,
                AccentColor = t.AccentColor
            });
        }

        foreach (var t in institutionTables)
        {
            allItems.Add(new SearchResultItem
            {
                TableNumber = t.TableNumber,
                Title = t.Title,
                Category = t.Category,
                Description = t.Description ?? "",
                Section = "Institution",
                SectionLabel = "संस्थागत",
                Url = $"/Institution/Table/{t.TableNumber}",
                Icon = t.Icon,
                AccentColor = t.AccentColor
            });
        }

        // Also add pages
        allItems.Add(new SearchResultItem
        {
            TableNumber = 0,
            Title = "गृहपृष्ठ — ड्यासबोर्ड",
            Category = "पृष्ठ",
            Description = "समग्र तथ्याङ्क अवलोकन, चार्ट र तथ्याङ्क कार्ड",
            Section = "Page",
            SectionLabel = "पृष्ठ",
            Url = "/",
            Icon = "bi-house-door",
            AccentColor = "#2D6A4F"
        });

        allItems.Add(new SearchResultItem
        {
            TableNumber = 0,
            Title = "वडा प्रोफाइल",
            Category = "पृष्ठ",
            Description = "प्रत्येक वडाको विस्तृत तथ्याङ्क",
            Section = "Page",
            SectionLabel = "पृष्ठ",
            Url = "/Ward",
            Icon = "bi-geo-alt",
            AccentColor = "#40916C"
        });

        allItems.Add(new SearchResultItem
        {
            TableNumber = 0,
            Title = "घरपरिवार तथ्याङ्क",
            Category = "पृष्ठ",
            Description = "६५ वटा घरपरिवार तालिकाहरू",
            Section = "Page",
            SectionLabel = "पृष्ठ",
            Url = "/Household",
            Icon = "bi-people",
            AccentColor = "#52B788"
        });

        allItems.Add(new SearchResultItem
        {
            TableNumber = 0,
            Title = "संस्थागत तथ्याङ्क",
            Category = "पृष्ठ",
            Description = "४३ वटा संस्थागत तालिकाहरू",
            Section = "Page",
            SectionLabel = "पृष्ठ",
            Url = "/Institution",
            Icon = "bi-bank",
            AccentColor = "#457B9D"
        });

        allItems.Add(new SearchResultItem
        {
            TableNumber = 0,
            Title = "अन्तरक्रियात्मक नक्सा",
            Category = "पृष्ठ",
            Description = "संस्थागत तथा घरपरिवार नक्सा",
            Section = "Page",
            SectionLabel = "पृष्ठ",
            Url = "/Map",
            Icon = "bi-map",
            AccentColor = "#E76F51"
        });

        // If query provided, pre-filter server-side
        if (!string.IsNullOrWhiteSpace(q))
        {
            var ql = q.ToLower().Trim();
            allItems = allItems.Where(item =>
                item.Title.ToLower().Contains(ql) ||
                item.Category.ToLower().Contains(ql) ||
                item.Description.ToLower().Contains(ql) ||
                item.SectionLabel.ToLower().Contains(ql) ||
                item.TableNumber.ToString() == ql
            ).ToList();
        }

        return View(allItems);
    }

    /// <summary>
    /// GET: /Search/Api — Returns the full search index as JSON for client-side instant search.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Api()
    {
        var householdTables = await _householdService.GetTableListAsync();
        var institutionTables = await _institutionService.GetTableListAsync();

        var allItems = new List<object>();

        foreach (var t in householdTables)
        {
            allItems.Add(new
            {
                t.TableNumber,
                t.Title,
                t.Category,
                Description = t.Description ?? "",
                Section = "Household",
                SectionLabel = "घरपरिवार",
                Url = $"/Household/Table/{t.TableNumber}",
                t.Icon,
                t.AccentColor
            });
        }

        foreach (var t in institutionTables)
        {
            allItems.Add(new
            {
                t.TableNumber,
                t.Title,
                t.Category,
                Description = t.Description ?? "",
                Section = "Institution",
                SectionLabel = "संस्थागत",
                Url = $"/Institution/Table/{t.TableNumber}",
                t.Icon,
                t.AccentColor
            });
        }

        // Static pages
        allItems.Add(new { TableNumber = 0, Title = "गृहपृष्ठ — ड्यासबोर्ड", Category = "पृष्ठ", Description = "समग्र तथ्याङ्क अवलोकन", Section = "Page", SectionLabel = "पृष्ठ", Url = "/", Icon = "bi-house-door", AccentColor = "#2D6A4F" });
        allItems.Add(new { TableNumber = 0, Title = "वडा प्रोफाइल", Category = "पृष्ठ", Description = "प्रत्येक वडाको विस्तृत तथ्याङ्क", Section = "Page", SectionLabel = "पृष्ठ", Url = "/Ward", Icon = "bi-geo-alt", AccentColor = "#40916C" });
        allItems.Add(new { TableNumber = 0, Title = "घरपरिवार तथ्याङ्क", Category = "पृष्ठ", Description = "६५ वटा घरपरिवार तालिकाहरू", Section = "Page", SectionLabel = "पृष्ठ", Url = "/Household", Icon = "bi-people", AccentColor = "#52B788" });
        allItems.Add(new { TableNumber = 0, Title = "संस्थागत तथ्याङ्क", Category = "पृष्ठ", Description = "४३ वटा संस्थागत तालिकाहरू", Section = "Page", SectionLabel = "पृष्ठ", Url = "/Institution", Icon = "bi-bank", AccentColor = "#457B9D" });
        allItems.Add(new { TableNumber = 0, Title = "अन्तरक्रियात्मक नक्सा", Category = "पृष्ठ", Description = "संस्थागत तथा घरपरिवार नक्सा", Section = "Page", SectionLabel = "पृष्ठ", Url = "/Map", Icon = "bi-map", AccentColor = "#E76F51" });

        return Json(allItems);
    }
}
