using Microsoft.AspNetCore.Mvc;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Controllers;

/// <summary>
/// Handles language switching (Nepali ↔ English).
/// Part 15: Language Translation Toggle
/// </summary>
public class LanguageController : Controller
{
    private readonly ILanguageService _langService;

    public LanguageController(ILanguageService langService)
    {
        _langService = langService;
    }

    /// <summary>
    /// Switch language and redirect back to the referring page.
    /// GET /Language/Switch?lang=en
    /// GET /Language/Switch?lang=ne
    /// </summary>
    public IActionResult Switch(string lang, string? returnUrl = null)
    {
        _langService.SetLanguage(lang);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        // Fallback to referer header
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            var uri = new Uri(referer);
            var localPath = uri.PathAndQuery;
            if (Url.IsLocalUrl(localPath))
                return Redirect(localPath);
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// API endpoint: returns the translation JSON for the requested (or current) language.
    /// GET /Language/Api?lang=en
    /// </summary>
    [HttpGet]
    public IActionResult Api(string? lang = null)
    {
        lang ??= _langService.CurrentLang;
        if (lang != "ne" && lang != "en") lang = "ne";

        var translations = _langService.GetTranslations(lang);

        // Convert to flat dictionary { "nav.home": "Home", ... }
        var flat = new Dictionary<string, string>();
        foreach (var kvp in translations)
        {
            if (kvp.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                flat[kvp.Key] = kvp.Value.GetString() ?? "";
        }

        return Json(flat);
    }
}
