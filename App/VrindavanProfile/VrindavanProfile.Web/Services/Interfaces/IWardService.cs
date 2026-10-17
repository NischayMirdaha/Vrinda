using VrindavanProfile.Web.Models.ViewModels;

namespace VrindavanProfile.Web.Services.Interfaces;

public interface IWardService
{
    /// <summary>
    /// Get full ward profile: ward list + filtered dashboard data
    /// </summary>
    Task<WardProfileViewModel> GetWardProfileAsync(int? wardNo = null);

    /// <summary>
    /// Get all wards with summary stats (for dropdown + overview cards)
    /// </summary>
    Task<List<WardInfo>> GetWardListAsync();

    /// <summary>
    /// Get total number of wards
    /// </summary>
    Task<int> GetTotalWardsAsync();
}