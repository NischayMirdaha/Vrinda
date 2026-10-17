namespace VrindavanProfile.Web.Models.ViewModels;

/// <summary>
/// Unified search result item for the global search page and overlay.
/// Represents a table, page, or any searchable entity.
/// </summary>
public class SearchResultItem
{
    public int TableNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string SectionLabel { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-table";
    public string AccentColor { get; set; } = "#2D6A4F";
}
