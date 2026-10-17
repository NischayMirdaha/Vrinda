namespace VrindavanProfile.Web.Models.ViewModels;

/// <summary>
/// Generic table model — renders any of the 65 household tables or 43 institutional tables.
/// </summary>
public class TableViewModel
{
    public int TableNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? CategoryEn { get; set; }
    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DataSource { get; set; }
    public List<string> ColumnHeaders { get; set; } = new();
    public List<TableRow> Rows { get; set; } = new();
    public TableRow? TotalRow { get; set; }
    public int? FilteredWardNo { get; set; }
    public int TotalWards { get; set; }
    public string CurrentLang { get; set; } = "ne";

    public int MaxTableNumber { get; set; } = 65;
    public string SectionUrl { get; set; } = "/Household";
    public string SectionTitle { get; set; } = "घरपरिवार तथ्याङ्क";
    public string? SectionTitleEn { get; set; } = "Household Data";

    public bool HasData => Rows.Count > 0;

    public string GetTitle() => CurrentLang == "en" && !string.IsNullOrEmpty(TitleEn) ? TitleEn : Title;
    public string GetCategory() => CurrentLang == "en" && !string.IsNullOrEmpty(CategoryEn) ? CategoryEn : Category;
    public string GetDescription() => CurrentLang == "en" && !string.IsNullOrEmpty(DescriptionEn) ? DescriptionEn : (Description ?? "");
    public string GetSectionTitle() => CurrentLang == "en" && !string.IsNullOrEmpty(SectionTitleEn) ? SectionTitleEn : SectionTitle;

    public string PageTitle => CurrentLang == "en"
        ? $"Table No. {TableNumber} — {GetTitle()}"
        : $"तालिका नं. {TableNumber} — {Title}";
}

public class TableRow
{
    public string RowLabel { get; set; } = string.Empty;
    public int? WardNo { get; set; }
    public List<string> Cells { get; set; } = new();
    public List<double?> RawValues { get; set; } = new();
}

/// <summary>
/// Metadata for one table — used on the table listing page.
/// </summary>
public class TableMeta
{
    public int TableNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? CategoryEn { get; set; }
    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DataSource { get; set; }
    public string Icon { get; set; } = "bi-table";
    public string AccentColor { get; set; } = "#003B6F";

    public string GetTitle(string lang) => lang == "en" && !string.IsNullOrEmpty(TitleEn) ? TitleEn : Title;
    public string GetCategory(string lang) => lang == "en" && !string.IsNullOrEmpty(CategoryEn) ? CategoryEn : Category;
    public string GetDescription(string lang) => lang == "en" && !string.IsNullOrEmpty(DescriptionEn) ? DescriptionEn : (Description ?? "");
}
