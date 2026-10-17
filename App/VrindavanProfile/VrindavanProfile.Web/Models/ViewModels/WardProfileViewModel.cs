namespace VrindavanProfile.Web.Models.ViewModels;

/// <summary>
/// Ward profile page model — contains ward selector data + filtered dashboard
/// </summary>
public class WardProfileViewModel
{
    /// <summary>
    /// Currently selected ward (null = all wards / municipality-wide)
    /// </summary>
    public int? SelectedWard { get; set; }

    /// <summary>
    /// All wards with summary statistics
    /// </summary>
    public List<WardInfo> Wards { get; set; } = new();

    /// <summary>
    /// Dashboard data (stats + charts) filtered by selected ward
    /// </summary>
    public DashboardViewModel Dashboard { get; set; } = new();

    /// <summary>
    /// Page title based on selection
    /// </summary>
    public string PageTitle => SelectedWard.HasValue
        ? $"वडा नं. {SelectedWard.Value} — विस्तृत प्रोफाइल"
        : "वडा प्रोफाइल — समग्र तथ्याङ्क";
}

/// <summary>
/// Summary info for a single ward (used in dropdown + overview cards)
/// </summary>
public class WardInfo
{
    public int WardNo { get; set; }

    /// <summary>
    /// Display name: "वडा नं. १", "वडा नं. २", etc.
    /// </summary>
    public string WardName => $"वडा नं. {WardNo}";

    public int HouseholdCount { get; set; }
    public int Population { get; set; }
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }
}