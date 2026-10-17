using VrindavanProfile.Web.Models.ViewModels;

namespace VrindavanProfile.Web.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(int? wardNo = null);
    Task<List<StatCounter>> GetStatCountersAsync(int? wardNo = null);
    Task<List<ChartData>> GetAllChartsAsync(int? wardNo = null);

    Task<ChartData> GetGenderDistributionAsync(int? wardNo = null);
    Task<ChartData> GetCasteEthnicityAsync(int? wardNo = null);
    Task<ChartData> GetMotherTongueAsync(int? wardNo = null);
    Task<ChartData> GetReligionAsync(int? wardNo = null);
    Task<ChartData> GetLiteracyStatusAsync(int? wardNo = null);
    Task<ChartData> GetEducationLevelAsync(int? wardNo = null);
    Task<ChartData> GetMaritalStatusAsync(int? wardNo = null);
    Task<ChartData> GetOccupationAsync(int? wardNo = null);
    Task<ChartData> GetResidenceStatusAsync(int? wardNo = null);
    Task<ChartData> GetDisabilityStatusAsync(int? wardNo = null);

    Task<ChartData> GetDrinkingWaterSourceAsync(int? wardNo = null);
    Task<ChartData> GetCookingFuelAsync(int? wardNo = null);
    Task<ChartData> GetLightSourceAsync(int? wardNo = null);
    Task<ChartData> GetToiletTypeAsync(int? wardNo = null);
    Task<ChartData> GetWasteManagementAsync(int? wardNo = null);

    Task<ChartData> GetHouseOwnershipAsync(int? wardNo = null);
    Task<ChartData> GetRoofTypeAsync(int? wardNo = null);
    Task<ChartData> GetFoundationTypeAsync(int? wardNo = null);

    Task<ChartData> GetIncomeSourcesAsync(int? wardNo = null);
    Task<ChartData> GetExpenditureCategoriesAsync(int? wardNo = null);
    Task<ChartData> GetFoodSufficiencyAsync(int? wardNo = null);
    Task<ChartData> GetBankAccountStatusAsync(int? wardNo = null);
    Task<ChartData> GetLoanPurposesAsync(int? wardNo = null);
    Task<ChartData> GetDisasterRisksAsync(int? wardNo = null);
    Task<ChartData> GetAssetsOwnershipAsync(int? wardNo = null);

    Task<ChartData> GetLivestockSummaryAsync(int? wardNo = null);
    Task<ChartData> GetForeignEmploymentAsync(int? wardNo = null);
    Task<ChartData> GetAgriculturalLandAsync(int? wardNo = null);
    Task<ChartData> GetIrrigationStatusAsync(int? wardNo = null);

    Task<ChartData> GetHealthVisitPlaceAsync(int? wardNo = null);
    Task<ChartData> GetInsuranceStatusAsync(int? wardNo = null);
    Task<ChartData> GetSocialSecurityAsync(int? wardNo = null);
    Task<ChartData> GetDeathCausesAsync(int? wardNo = null);

    Task<List<int>> GetWardNumbersAsync();
}
