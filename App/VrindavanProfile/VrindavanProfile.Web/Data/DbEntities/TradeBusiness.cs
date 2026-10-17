using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("trade_business")]
public class TradeBusiness
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("ownership")] public string? Ownership { get; set; }
    [Column("business_name")] public string? BusinessName { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("operator_name")] public string? OperatorName { get; set; }
    [Column("contact_no")] public string? ContactNo { get; set; }
    [Column("is_registered")] public string? IsRegistered { get; set; }
    [Column("registration_no")] public string? RegistrationNo { get; set; }
    [Column("registered_at")] public string? RegisteredAt { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
