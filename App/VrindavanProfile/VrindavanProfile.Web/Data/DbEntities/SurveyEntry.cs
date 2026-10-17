using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("survey_entry")]
public class SurveyEntry
{
    [Column("id")]
    public long Id { get; set; }

    [Column("odk_id")]
    public long? OdkId { get; set; }

    [Column("uuid")]
    public string? Uuid { get; set; }

    [Column("submission_time")]
    public string? SubmissionTime { get; set; }

    [Column("validation_status")]
    public string? ValidationStatus { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("submitted_by")]
    public string? SubmittedBy { get; set; }

    [Column("version")]
    public string? Version { get; set; }

    [Column("tags")]
    public string? Tags { get; set; }

    [Column("odk_index")]
    public int? OdkIndex { get; set; }

    [Column("survey_start")]
    public string? SurveyStart { get; set; }

    [Column("survey_end")]
    public string? SurveyEnd { get; set; }

    [Column("survey_date")]
    public string? SurveyDate { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("device_id")]
    public string? DeviceId { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("consent_text")]
    public string? ConsentText { get; set; }

    [Column("municipality")]
    public string? Municipality { get; set; }

    [Column("ward_no")]
    [Range(1, 9, ErrorMessage = "Ward number must be between 1 and 9.")]
    public int? WardNo { get; set; }

    [Column("enumerator_name")]
    public string? EnumeratorName { get; set; }

    [Column("subject_area")]
    public string? SubjectArea { get; set; }

    [Column("created_at")]
    public string? CreatedAt { get; set; }

    // Navigation properties
    public EducationalInstitution? EducationalInstitution { get; set; }
    public HealthInstitution? HealthInstitution { get; set; }
    public Building? Building { get; set; }
    public SecurityBody? SecurityBody { get; set; }
    public FinancialInstitution? FinancialInstitution { get; set; }
    public HotelResort? HotelResort { get; set; }
    public Bridge? Bridge { get; set; }
    public Industry? Industry { get; set; }
    public NgoClubGroup? NgoClubGroup { get; set; }
    public LandUse? LandUse { get; set; }
    public NaturalResource? NaturalResource { get; set; }
    public CulturalHeritage? CulturalHeritage { get; set; }
    public TradeBusiness? TradeBusiness { get; set; }
    public Forest? Forest { get; set; }
    public Road? Road { get; set; }
    public WardStatistics? WardStatistics { get; set; }
}
