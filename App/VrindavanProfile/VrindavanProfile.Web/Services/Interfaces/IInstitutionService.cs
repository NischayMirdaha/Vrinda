using VrindavanProfile.Web.Models.ViewModels;

namespace VrindavanProfile.Web.Services.Interfaces;

public interface IInstitutionService
{
    /// <summary>
    /// Get a specific institutional table by its number (1–43), optionally filtered by ward.
    /// </summary>
    Task<TableViewModel?> GetTableAsync(int tableId, int? wardNo = null);

    /// <summary>
    /// Get metadata list of all 43 institutional tables (for the Index page).
    /// </summary>
    Task<List<TableMeta>> GetTableListAsync();

    /// <summary>
    /// Get ward numbers that have institutional data.
    /// </summary>
    Task<List<int>> GetWardNumbersAsync();
}
