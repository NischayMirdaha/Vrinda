using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("security_body")]
public class SecurityBody
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("body_type")] public string? BodyType { get; set; }
    [Column("personnel_count")] public double? PersonnelCount { get; set; }
    [Column("name")] public string? Name { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("contact_no")] public string? ContactNo { get; set; }
    [Column("email_website")] public string? EmailWebsite { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
