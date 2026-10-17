using System.Text.Json;
using Microsoft.AspNetCore.Http;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

/// <summary>
/// Manages language preference via cookie and provides translation lookups.
/// Part 15: Language Translation Toggle (Nepali ↔ English)
/// </summary>
public class LanguageService : ILanguageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;
    private readonly Dictionary<string, Dictionary<string, JsonElement>> _cache = new();
    private static readonly SemaphoreSlim _loadLock = new(1, 1);

    private const string CookieName = "vrindavan_lang";
    private const string DefaultLang = "ne";

    public LanguageService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
    {
        _httpContextAccessor = httpContextAccessor;
        _env = env;
    }

    /// <summary>Current language code: "ne" or "en"</summary>
    public string CurrentLang
    {
        get
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return DefaultLang;

            // 1. Check query string (?lang=en)
            var qs = ctx.Request.Query["lang"].ToString();
            if (qs == "ne" || qs == "en") return qs;

            // 2. Check cookie
            var cookie = ctx.Request.Cookies[CookieName];
            if (cookie == "ne" || cookie == "en") return cookie;

            return DefaultLang;
        }
    }

    /// <summary>Whether the current language is English</summary>
    public bool IsEnglish => CurrentLang == "en";

    /// <summary>Set language preference (writes cookie)</summary>
    public void SetLanguage(string lang)
    {
        if (lang != "ne" && lang != "en") lang = DefaultLang;
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null) return;

        ctx.Response.Cookies.Append(CookieName, lang, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            HttpOnly = false,
            SameSite = SameSiteMode.Lax
        });
    }

    /// <summary>Get a translation by dot-separated key path</summary>
    public string T(string key)
    {
        return T(key, CurrentLang);
    }

    /// <summary>Get a translation from a specific language</summary>
    public string T(string key, string lang)
    {
        var translations = GetTranslations(lang);
        if (translations.Count == 0) return key;

        return ResolveKey(key, translations);
    }

    /// <summary>Get translations dictionary for current language</summary>
    public Dictionary<string, JsonElement> GetTranslations()
    {
        return GetTranslations(CurrentLang);
    }

    /// <summary>Get translations dictionary for a given language</summary>
    public Dictionary<string, JsonElement> GetTranslations(string lang)
    {
        if (string.IsNullOrEmpty(lang)) lang = DefaultLang;
        if (_cache.TryGetValue(lang, out var cached)) return cached;

        _loadLock.Wait();
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(lang, out cached)) return cached;

            var filePath = Path.Combine(_env.WebRootPath, "lang", $"{lang}.json");
            if (!File.Exists(filePath))
            {
                _cache[lang] = new Dictionary<string, JsonElement>();
                return _cache[lang];
            }

            var json = File.ReadAllText(filePath);
            var doc = JsonDocument.Parse(json);
            var dict = new Dictionary<string, JsonElement>();
            FlattenJson(doc.RootElement, "", dict);
            _cache[lang] = dict;
            return dict;
        }
        finally
        {
            _loadLock.Release();
        }
    }

    /// <summary>Resolve a dot-separated key like "nav.home" from a flat dictionary</summary>
    private static string ResolveKey(string key, Dictionary<string, JsonElement> dict)
    {
        // Direct key match (already flattened)
        if (dict.TryGetValue(key, out var element))
        {
            if (element.ValueKind == JsonValueKind.String)
                return element.GetString() ?? key;
        }

        // Try with dot notation → underscore join
        var dotKey = key.Replace("_", ".");
        if (dict.TryGetValue(dotKey, out element))
        {
            if (element.ValueKind == JsonValueKind.String)
                return element.GetString() ?? key;
        }

        return key;
    }

    /// <summary>Flatten a nested JSON object into dot-separated keys</summary>
    private static void FlattenJson(JsonElement element, string prefix, Dictionary<string, JsonElement> dict)
    {
        if (element.ValueKind != JsonValueKind.Object) return;

        foreach (var prop in element.EnumerateObject())
        {
            var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
            if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                FlattenJson(prop.Value, key, dict);
            }
            else
            {
                dict[key] = prop.Value;
            }
        }
    }
}
