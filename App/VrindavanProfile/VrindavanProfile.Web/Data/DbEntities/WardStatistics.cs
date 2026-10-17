using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("ward_statistics")]
public class WardStatistics
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("birth_registrations")] public double? BirthRegistrations { get; set; }
    [Column("marriage_registrations")] public double? MarriageRegistrations { get; set; }
    [Column("death_registrations")] public double? DeathRegistrations { get; set; }
    [Column("divorce_registrations")] public double? DivorceRegistrations { get; set; }
    [Column("migration_out_households")] public double? MigrationOutHouseholds { get; set; }
    [Column("migration_in_households")] public double? MigrationInHouseholds { get; set; }
    [Column("citizenship_recommendations")] public double? CitizenshipRecommendations { get; set; }
    [Column("passport_recommendations")] public double? PassportRecommendations { get; set; }
    [Column("senior_female")] public double? SeniorFemale { get; set; }
    [Column("senior_male")] public double? SeniorMale { get; set; }
    [Column("senior_allowance_total")] public double? SeniorAllowanceTotal { get; set; }
    [Column("single_women_count")] public double? SingleWomenCount { get; set; }
    [Column("single_women_allowance")] public double? SingleWomenAllowance { get; set; }
    [Column("full_disabled_male")] public double? FullDisabledMale { get; set; }
    [Column("full_disabled_female")] public double? FullDisabledFemale { get; set; }
    [Column("full_disabled_allowance")] public double? FullDisabledAllowance { get; set; }
    [Column("partial_disabled_male")] public double? PartialDisabledMale { get; set; }
    [Column("partial_disabled_female")] public double? PartialDisabledFemale { get; set; }
    [Column("partial_disabled_allowance")] public double? PartialDisabledAllowance { get; set; }
    [Column("child_protection_male")] public double? ChildProtectionMale { get; set; }
    [Column("child_protection_female")] public double? ChildProtectionFemale { get; set; }
    [Column("child_protection_allowance")] public double? ChildProtectionAllowance { get; set; }
    [Column("dalit_allowance_male")] public double? DalitAllowanceMale { get; set; }
    [Column("dalit_allowance_female")] public double? DalitAllowanceFemale { get; set; }
    [Column("dalit_allowance_total")] public double? DalitAllowanceTotal { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
