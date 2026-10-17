namespace VrindavanProfile.Web.Models.ViewModels;

/// <summary>
/// Top-level model for the interactive webmap page.
/// </summary>
public class MapViewModel
{
    /// <summary>
    /// All map markers (institutions + households).
    /// </summary>
    public List<MapMarker> Markers { get; set; } = new();

    /// <summary>
    /// Summary counts per entity type — for the legend / filter panel.
    /// </summary>
    public List<MapTypeSummary> TypeSummaries { get; set; } = new();

    /// <summary>
    /// Available ward numbers for filtering.
    /// </summary>
    public List<int> WardNumbers { get; set; } = new();

    /// <summary>
    /// Currently filtered ward number (null = all wards).
    /// </summary>
    public int? FilteredWardNo { get; set; }

    /// <summary>
    /// Currently filtered entity type (null = all types).
    /// </summary>
    public string? FilteredEntityType { get; set; }

    /// <summary>
    /// Total markers count.
    /// </summary>
    public int TotalMarkers => Markers.Count;

    public List<WardInfo> WardDetails { get; set; } = new();
}

/// <summary>
/// A single point on the map with lat/long, popup info, and type/color.
/// </summary>
public class MapMarker
{
    /// <summary>
    /// Unique identifier for this marker.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Entity type — e.g. "शैक्षिक संस्था", "स्वास्थ्य संस्था", "घरपरिवार", etc.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// English entity type key — e.g. "educational_institution", "household", etc.
    /// Used for filtering and icon selection.
    /// </summary>
    public string EntityTypeKey { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the popup.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Additional detail for the popup (type, address, etc.).
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Ward number.
    /// </summary>
    public int? WardNo { get; set; }

    /// <summary>
    /// GPS Latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// GPS Longitude.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Marker color (CSS hex) — assigned based on entity type.
    /// </summary>
    public string Color { get; set; } = "#2D6A4F";

    /// <summary>
    /// Bootstrap Icon class for the marker — assigned based on entity type.
    /// </summary>
    public string Icon { get; set; } = "bi-geo-alt";
}

/// <summary>
/// Summary count per entity type — shown in the legend / filter panel.
/// </summary>
public class MapTypeSummary
{
    /// <summary>
    /// Display name in Nepali — e.g. "शैक्षिक संस्था".
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// English key — e.g. "educational_institution".
    /// </summary>
    public string EntityTypeKey { get; set; } = string.Empty;

    /// <summary>
    /// Number of markers of this type.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Marker color for this type.
    /// </summary>
    public string Color { get; set; } = "#2D6A4F";

    /// <summary>
    /// Bootstrap Icon class.
    /// </summary>
    public string Icon { get; set; } = "bi-geo-alt";
}
