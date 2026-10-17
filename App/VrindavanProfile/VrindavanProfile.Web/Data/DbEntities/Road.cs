using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("road")]
public class Road
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("road_name")] public string? RoadName { get; set; }
    [Column("road_type")] public string? RoadType { get; set; }
    [Column("length_in_ward")] public double? LengthInWard { get; set; }
    [Column("width")] public double? Width { get; set; }
    [Column("construction_year")] public string? ConstructionYear { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
