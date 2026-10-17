using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("building")]
public class Building
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("building_type")] public string? BuildingType { get; set; }
    [Column("purpose_summary")] public string? PurposeSummary { get; set; }
    [Column("purpose_kutchi")] public short? PurposeKutchi { get; set; }
    [Column("purpose_pakki")] public short? PurposePakki { get; set; }
    [Column("purpose_semi_pakki")] public short? PurposeSemiPakki { get; set; }
    [Column("purpose_admin")] public short? PurposeAdmin { get; set; }
    [Column("purpose_education")] public short? PurposeEducation { get; set; }
    [Column("purpose_health")] public short? PurposeHealth { get; set; }
    [Column("purpose_agriculture")] public short? PurposeAgriculture { get; set; }
    [Column("purpose_cooperative")] public short? PurposeCooperative { get; set; }
    [Column("purpose_assembly")] public short? PurposeAssembly { get; set; }
    [Column("purpose_other")] public short? PurposeOther { get; set; }
    [Column("purpose_other_detail")] public string? PurposeOtherDetail { get; set; }
    [Column("capacity")] public string? Capacity { get; set; }
    [Column("usage_status")] public string? UsageStatus { get; set; }
    [Column("usage_status_other")] public string? UsageStatusOther { get; set; }
    [Column("ownership")] public string? Ownership { get; set; }
    [Column("name")] public string? Name { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("contact_no")] public string? ContactNo { get; set; }
    [Column("building_material")] public string? BuildingMaterial { get; set; }
    [Column("construction_status")] public string? ConstructionStatus { get; set; }
    [Column("construction_date")] public string? ConstructionDate { get; set; }
    [Column("total_floors")] public double? TotalFloors { get; set; }
    [Column("total_rooms")] public double? TotalRooms { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
