using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("hh_death_record")]
public class HhDeathRecord
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

    // Deceased info
    [Column("deceased_name")] public string? DeceasedName { get; set; }
    [Column("deceased_gender")] public string? DeceasedGender { get; set; }
    [Column("deceased_age_group")] public string? DeceasedAgeGroup { get; set; }
    [Column("deceased_age_detail")] public double? DeceasedAgeDetail { get; set; }
    [Column("cause_summary")] public string? CauseSummary { get; set; }

    // Cause categories
    [Column("cause_communicable")] public short? CauseCommunicable { get; set; }
    [Column("cause_non_communicable")] public short? CauseNonCommunicable { get; set; }
    [Column("cause_lifestyle")] public short? CauseLifestyle { get; set; }
    [Column("cause_other")] public short? CauseOther { get; set; }

    // Communicable diseases
    [Column("communicable_summary")] public string? CommunicableSummary { get; set; }
    [Column("communicable_pneumonia")] public short? CommunicablePneumonia { get; set; }
    [Column("communicable_tb")] public short? CommunicableTb { get; set; }
    [Column("communicable_hiv_aids")] public short? CommunicableHivAids { get; set; }
    [Column("communicable_hepatitis")] public short? CommunicableHepatitis { get; set; }
    [Column("communicable_dengue")] public short? CommunicableDengue { get; set; }
    [Column("communicable_covid19")] public short? CommunicableCovid19 { get; set; }
    [Column("communicable_malaria")] public short? CommunicableMalaria { get; set; }
    [Column("communicable_typhoid")] public short? CommunicableTyphoid { get; set; }
    [Column("communicable_diarrhea")] public short? CommunicableDiarrhea { get; set; }
    [Column("communicable_meningitis")] public short? CommunicableMeningitis { get; set; }

    // Non-communicable diseases
    [Column("noncommunicable_summary")] public string? NoncommunicableSummary { get; set; }
    [Column("noncommunicable_heart")] public short? NoncommunicableHeart { get; set; }
    [Column("noncommunicable_cancer")] public short? NoncommunicableCancer { get; set; }
    [Column("noncommunicable_diabetes")] public short? NoncommunicableDiabetes { get; set; }
    [Column("noncommunicable_respiratory")] public short? NoncommunicableRespiratory { get; set; }
    [Column("noncommunicable_kidney")] public short? NoncommunicableKidney { get; set; }
    [Column("noncommunicable_hypertension")] public short? NoncommunicableHypertension { get; set; }
    [Column("noncommunicable_dementia")] public short? NoncommunicableDementia { get; set; }

    // Lifestyle causes
    [Column("lifestyle_summary")] public string? LifestyleSummary { get; set; }
    [Column("lifestyle_obesity")] public short? LifestyleObesity { get; set; }
    [Column("lifestyle_mental_stress")] public short? LifestyleMentalStress { get; set; }
    [Column("lifestyle_alcohol")] public short? LifestyleAlcohol { get; set; }
    [Column("lifestyle_smoking")] public short? LifestyleSmoking { get; set; }

    // Other causes
    [Column("other_cause_summary")] public string? OtherCauseSummary { get; set; }
    [Column("other_road_accident")] public short? OtherRoadAccident { get; set; }
    [Column("other_heart_attack")] public short? OtherHeartAttack { get; set; }
    [Column("other_suicide")] public short? OtherSuicide { get; set; }
    [Column("other_animal_bite")] public short? OtherAnimalBite { get; set; }
    [Column("other_natural_disaster")] public short? OtherNaturalDisaster { get; set; }
    [Column("other_pregnancy")] public short? OtherPregnancy { get; set; }
    [Column("deceased_age_years")] public string? DeceasedAgeYears { get; set; }

    // Navigation
    public Household? Household { get; set; }
}
