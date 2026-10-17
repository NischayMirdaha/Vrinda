namespace VrindavanProfile.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int? WardNo { get; set; }
    public int TotalWards { get; set; }
    public List<StatCounter> StatCounters { get; set; } = new();
    public List<ChartData> Charts { get; set; } = new();
    public string CurrentLang { get; set; } = "ne";

    public string PageTitle => WardNo.HasValue
        ? (CurrentLang == "en"
            ? $"Ward No. {WardNo.Value} Profile"
            : $"वडा नं. {WardNo.Value} प्रोफाइल")
        : (CurrentLang == "en"
            ? "Vrindavan Municipality — Overview"
            : "वृन्दावन नगरपालिका — समग्र तथ्याङ्क");
}

public class StatCounter
{
    public string Label { get; set; } = string.Empty;
    public string? LabelEn { get; set; }
    public int Value { get; set; }
    public string Icon { get; set; } = "bi-bar-chart";
    public string AccentColor { get; set; } = "#003B6F";

    public string GetLabel(string lang) => lang == "en" && !string.IsNullOrEmpty(LabelEn) ? LabelEn : Label;
}

public class ChartData
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string ChartType { get; set; } = "doughnut";
    public List<ChartItem> Items { get; set; } = new();
    public string? DetailLink { get; set; }
    public string? DetailLinkText { get; set; } = "विवरण हेर्नुहोस्";
    public string? DetailLinkTextEn { get; set; } = "View Details";
    public string Category { get; set; } = string.Empty;
    public string? CategoryEn { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Total => Items.Sum(i => i.Value);

    public string GetTitle(string lang) => lang == "en" && !string.IsNullOrEmpty(TitleEn) ? TitleEn : Title;
    public string GetCategory(string lang) => lang == "en" && !string.IsNullOrEmpty(CategoryEn) ? CategoryEn : Category;
    public string GetDetailLinkText(string lang) => lang == "en" && !string.IsNullOrEmpty(DetailLinkTextEn) ? DetailLinkTextEn : (DetailLinkText ?? "विवरण हेर्नुहोस्");

    /// <summary>
    /// For stacked/grouped bar charts — multiple datasets.
    /// </summary>
    public List<ChartDataset>? Datasets { get; set; }
}

public class ChartDataset
{
    public string Label { get; set; } = string.Empty;
    public List<double> Values { get; set; } = new();
}

public class ChartItem
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public int? MaleCount { get; set; }
    public int? FemaleCount { get; set; }
}
