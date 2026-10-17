using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("hotel_resort")]
public class HotelResort
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("hotel_type")] public string? HotelType { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("hotel_name")] public string? HotelName { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("is_registered")] public string? IsRegistered { get; set; }
    [Column("registered_at")] public string? RegisteredAt { get; set; }
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
