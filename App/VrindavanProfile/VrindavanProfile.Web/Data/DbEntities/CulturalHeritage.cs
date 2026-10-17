using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("cultural_heritage")]
public class CulturalHeritage
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("heritage_type")] public string? HeritageType { get; set; }
    [Column("name")] public string? Name { get; set; }
    [Column("annual_income")] public double? AnnualIncome { get; set; }
    [Column("annual_visitors")] public double? AnnualVisitors { get; set; }
    [Column("total_area_sqkm")] public double? TotalAreaSqkm { get; set; }
    [Column("construction_year")] public string? ConstructionYear { get; set; }
    [Column("has_guides")] public string? HasGuides { get; set; }
    [Column("guide_count")] public double? GuideCount { get; set; }
    [Column("significance")] public string? Significance { get; set; }
    [Column("boundary")] public string? Boundary { get; set; }
    [Column("history")] public string? History { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
