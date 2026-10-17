using VrindavanProfile.Web.Models.ViewModels;

namespace VrindavanProfile.Web.Services.Interfaces;

public interface IHouseholdService
{
    /// <summary>
    /// Get a specific household table by its number (1–65), optionally filtered by ward.
    /// </summary>
    Task<TableViewModel?> GetTableAsync(int tableId, int? wardNo = null);

    /// <summary>
    /// Get metadata list of all 65 household tables (for the Index page).
    /// </summary>
    Task<List<TableMeta>> GetTableListAsync();

    /// <summary>
    /// Get ward numbers that have household data.
    /// </summary>
    Task<List<int>> GetWardNumbersAsync();
}
