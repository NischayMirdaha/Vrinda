using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("land_use")]
public class LandUse
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("land_type")] public string? LandType { get; set; }
    [Column("ownership")] public string? Ownership { get; set; }
    [Column("boundary")] public string? Boundary { get; set; }
    [Column("history")] public string? History { get; set; }
    [Column("public_area_type")] public string? PublicAreaType { get; set; }
    [Column("disaster_summary")] public string? DisasterSummary { get; set; }
    [Column("disaster_flood")] public short? DisasterFlood { get; set; }
    [Column("disaster_landslide")] public short? DisasterLandslide { get; set; }
    [Column("disaster_river_cut")] public short? DisasterRiverCut { get; set; }
    [Column("disaster_inundation")] public short? DisasterInundation { get; set; }
    [Column("disaster_drought")] public short? DisasterDrought { get; set; }
    [Column("total_area")] public double? TotalArea { get; set; }
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
