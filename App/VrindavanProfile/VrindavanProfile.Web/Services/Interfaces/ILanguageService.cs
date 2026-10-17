using System.Text.Json;

namespace VrindavanProfile.Web.Services.Interfaces;

/// <summary>
/// Manages language preference (ne/en) and provides translation lookups.
/// Part 15: Language Translation Toggle
/// </summary>
public interface ILanguageService
{
    /// <summary>Current language code: "ne" or "en"</summary>
    string CurrentLang { get; }

    /// <summary>Whether the current language is English</summary>
    bool IsEnglish { get; }

    /// <summary>Set language preference (writes cookie)</summary>
    void SetLanguage(string lang);

    /// <summary>Get a translation by dot-separated key path from the current language JSON</summary>
    string T(string key);

    /// <summary>Get a translation from a specific language</summary>
    string T(string key, string lang);

    /// <summary>Get the entire translation dictionary for the current language</summary>
    Dictionary<string, JsonElement> GetTranslations();

    /// <summary>Get the entire translation dictionary for a given language</summary>
    Dictionary<string, JsonElement> GetTranslations(string lang);
}
