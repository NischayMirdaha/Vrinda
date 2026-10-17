using VrindavanProfile.Web.Models.ViewModels;

namespace VrindavanProfile.Web.Services.Interfaces;

public interface IMapService
{
    /// <summary>
    /// Get all map markers — institutions and households combined.
    /// Optionally filter by ward number and/or entity type.
    /// </summary>
    Task<MapViewModel> GetMapDataAsync(int? wardNo = null, string? entityType = null);

    /// <summary>
    /// Get all institution markers only.
    /// </summary>
    Task<List<MapMarker>> GetInstitutionMarkersAsync(int? wardNo = null);

    /// <summary>
    /// Get all household markers only.
    /// </summary>
    Task<List<MapMarker>> GetHouseholdMarkersAsync(int? wardNo = null);

    /// <summary>
    /// Get distinct ward numbers that have map data.
    /// </summary>
    Task<List<int>> GetWardNumbersAsync();

    /// <summary>
    /// Get summary counts by entity type for the map legend.
    /// </summary>
    Task<List<MapTypeSummary>> GetTypeSummariesAsync();
}
