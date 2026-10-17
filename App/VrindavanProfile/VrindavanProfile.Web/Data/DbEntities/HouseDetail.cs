using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("house_detail")]
public class HouseDetail
{
    [Column("id")] public long Id { get; set; }
    [Column("household_id")] public long? HouseholdId { get; set; }

    // Repeat metadata
    [Column("repeat_index")] public int? RepeatIndex { get; set; }
    [Column("parent_table_name")] public string? ParentTableName { get; set; }
    [Column("parent_index")] public int? ParentIndex { get; set; }
    [Column("submission_id")] public long? SubmissionId { get; set; }
    [Column("submission_uuid")] public string? SubmissionUuid { get; set; }
    [Column("submission_time")] public string? SubmissionTime { get; set; }
    [Column("submission_validation")] public string? SubmissionValidation { get; set; }
    [Column("submission_notes")] public string? SubmissionNotes { get; set; }
    [Column("submission_status")] public string? SubmissionStatus { get; set; }
    [Column("submission_submitted_by")] public string? SubmissionSubmittedBy { get; set; }
    [Column("submission_version")] public string? SubmissionVersion { get; set; }
    [Column("submission_tags")] public string? SubmissionTags { get; set; }

    // House details
    [Column("ownership_type")] public string? OwnershipType { get; set; }
    [Column("total_rooms")] public double? TotalRooms { get; set; }
    [Column("rooms_rented")] public double? RoomsRented { get; set; }
    [Column("roof_type")] public string? RoofType { get; set; }
    [Column("roof_other")] public string? RoofOther { get; set; }
    [Column("roof_other_detail")] public string? RoofOtherDetail { get; set; }
    [Column("foundation_type")] public string? FoundationType { get; set; }
    [Column("foundation_other")] public string? FoundationOther { get; set; }
    [Column("has_building_permit")] public string? HasBuildingPermit { get; set; }
    [Column("meets_building_code")] public string? MeetsBuildingCode { get; set; }

    // Navigation
    public Household? Household { get; set; }
}
