using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("natural_resource")]
public class NaturalResource
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("resource_type")] public string? ResourceType { get; set; }
    [Column("name")] public string? Name { get; set; }
    [Column("total_area")] public double? TotalArea { get; set; }
    [Column("total_households")] public double? TotalHouseholds { get; set; }
    [Column("ownership")] public string? Ownership { get; set; }
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
