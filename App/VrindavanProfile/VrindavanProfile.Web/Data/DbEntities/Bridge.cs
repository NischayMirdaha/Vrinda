using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("bridge")]
public class Bridge
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("bridge_type")] public string? BridgeType { get; set; }
    [Column("bridge_condition")] public string? BridgeCondition { get; set; }
    [Column("repair_summary")] public string? RepairSummary { get; set; }
    [Column("repair_wire_bar")] public short? RepairWireBar { get; set; }
    [Column("repair_side_wall")] public short? RepairSideWall { get; set; }
    [Column("repair_road_hole")] public short? RepairRoadHole { get; set; }
    [Column("repair_foundation_crack")] public short? RepairFoundationCrack { get; set; }
    [Column("repair_foundation_broken")] public short? RepairFoundationBroken { get; set; }
    [Column("length_meters")] public double? LengthMeters { get; set; }
    [Column("width_meters")] public double? WidthMeters { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
