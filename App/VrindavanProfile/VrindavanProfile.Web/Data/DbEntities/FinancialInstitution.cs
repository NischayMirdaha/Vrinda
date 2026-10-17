using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("financial_institution")]
public class FinancialInstitution
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("institution_type")] public string? InstitutionType { get; set; }
    [Column("name")] public string? Name { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("contact_no")] public string? ContactNo { get; set; }
    [Column("email_website")] public string? EmailWebsite { get; set; }
    [Column("building_level")] public string? BuildingLevel { get; set; }
    [Column("own_building")] public string? OwnBuilding { get; set; }
    [Column("female_share_members")] public double? FemaleShareMembers { get; set; }
    [Column("male_share_members")] public double? MaleShareMembers { get; set; }
    [Column("coop_type_summary")] public string? CoopTypeSummary { get; set; }
    [Column("coop_female_member")] public short? CoopFemaleMember { get; set; }
    [Column("coop_male_member")] public short? CoopMaleMember { get; set; }
    [Column("coop_joint")] public short? CoopJoint { get; set; }
    [Column("coop_agri_savings")] public short? CoopAgriSavings { get; set; }
    [Column("coop_loan")] public short? CoopLoan { get; set; }
    [Column("coop_multipurpose")] public short? CoopMultipurpose { get; set; }
    [Column("coop_other")] public short? CoopOther { get; set; }
    [Column("coop_other_detail")] public string? CoopOtherDetail { get; set; }
    [Column("total_share_capital")] public double? TotalShareCapital { get; set; }
    [Column("total_savings")] public double? TotalSavings { get; set; }
    [Column("female_borrowers")] public double? FemaleBorrowers { get; set; }
    [Column("male_borrowers")] public double? MaleBorrowers { get; set; }
    [Column("max_loan_amount")] public double? MaxLoanAmount { get; set; }
    [Column("min_loan_amount")] public double? MinLoanAmount { get; set; }
    [Column("loan_type_summary")] public string? LoanTypeSummary { get; set; }
    [Column("loan_agriculture")] public short? LoanAgriculture { get; set; }
    [Column("loan_livestock")] public short? LoanLivestock { get; set; }
    [Column("loan_building")] public short? LoanBuilding { get; set; }
    [Column("loan_hire_purchase")] public short? LoanHirePurchase { get; set; }
    [Column("loan_medical")] public short? LoanMedical { get; set; }
    [Column("loan_house")] public short? LoanHouse { get; set; }
    [Column("loan_other")] public short? LoanOther { get; set; }
    [Column("loan_other_detail")] public string? LoanOtherDetail { get; set; }
    [Column("govt_support_amount")] public double? GovtSupportAmount { get; set; }
    [Column("external_support_summary")] public string? ExternalSupportSummary { get; set; }
    [Column("has_govt_support")] public short? HasGovtSupport { get; set; }
    [Column("has_ngo_support")] public short? HasNgoSupport { get; set; }
    [Column("no_support")] public short? NoSupport { get; set; }
    [Column("other_support_summary")] public string? OtherSupportSummary { get; set; }
    [Column("support_furniture")] public short? SupportFurniture { get; set; }
    [Column("support_goods")] public short? SupportGoods { get; set; }
    [Column("support_training")] public short? SupportTraining { get; set; }
    [Column("support_capacity")] public short? SupportCapacity { get; set; }
    [Column("support_computer")] public short? SupportComputer { get; set; }
    [Column("support_stationery")] public short? SupportStationery { get; set; }
    [Column("support_other")] public short? SupportOther { get; set; }
    [Column("ngo_support_amount")] public double? NgoSupportAmount { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
