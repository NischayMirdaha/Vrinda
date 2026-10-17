using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("ngo_club_group")]
public class NgoClubGroup
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("org_type")] public string? OrgType { get; set; }
    [Column("org_name")] public string? OrgName { get; set; }
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("contact_no")] public string? ContactNo { get; set; }
    [Column("established_date_bs")] public string? EstablishedDateBs { get; set; }
    [Column("registered_at")] public string? RegisteredAt { get; set; }
    [Column("annual_transaction")] public double? AnnualTransaction { get; set; }
    [Column("priority_areas")] public string? PriorityAreas { get; set; }
    [Column("priority_other")] public string? PriorityOther { get; set; }
    [Column("donor_list")] public string? DonorList { get; set; }
    [Column("working_area")] public string? WorkingArea { get; set; }
    [Column("active_programs")] public double? ActivePrograms { get; set; }
    [Column("total_employees")] public double? TotalEmployees { get; set; }
    [Column("female_members")] public double? FemaleMembers { get; set; }
    [Column("male_members")] public double? MaleMembers { get; set; }
    [Column("female_beneficiaries")] public double? FemaleBeneficiaries { get; set; }
    [Column("male_beneficiaries")] public double? MaleBeneficiaries { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
