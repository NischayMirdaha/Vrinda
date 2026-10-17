using System.ComponentModel.DataAnnotations.Schema;
using VrindavanProfile.Web.Services;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("family_member")]
public class FamilyMember
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

    [Column("section_note")] public string? SectionNote { get; set; }

    // Personal
    [Column("full_name")] public string? FullName { get; set; }
    [Column("surname")] public string? Surname { get; set; }
    [Column("caste")] public string? Caste { get; set; }
    [Column("gender")] public string? Gender { get; set; }
    [Column("relation_to_head")] public string? RelationToHead { get; set; }
    [Column("age")] public double? Age { get; set; }

    /// <summary>Date of birth in Bikram Sambat, stored as "YYYY-MM-DD".</summary>
    [Column("dob_bs")] public string? DobBs { get; set; }

    /// <summary>Age calculated live from DobBs every time it is accessed (null if no DOB).</summary>
    [NotMapped]
    public int? AgeFromDob => NepaliDateHelper.CalculateAgeBs(DobBs);

    /// <summary>Effective age: prefers DOB-based live calculation, falls back to stored Age.</summary>
    [NotMapped]
    public int? EffectiveAge => AgeFromDob ?? (Age.HasValue ? (int)Age.Value : (int?)null);
    [Column("language")] public string? Language { get; set; }
    [Column("language_other")] public string? LanguageOther { get; set; }
    [Column("religion")] public string? Religion { get; set; }
    [Column("religion_other")] public string? ReligionOther { get; set; }
    [Column("marital_status")] public string? MaritalStatus { get; set; }

    // Education
    [Column("education_level")] public string? EducationLevel { get; set; }
    [Column("education_detail")] public string? EducationDetail { get; set; }
    [Column("literacy_status")] public string? LiteracyStatus { get; set; }
    [Column("expertise_area")] public string? ExpertiseArea { get; set; }
    [Column("expertise_other")] public string? ExpertiseOther { get; set; }

    // Occupation
    [Column("occupation")] public string? Occupation { get; set; }
    [Column("occupation_other")] public string? OccupationOther { get; set; }
    [Column("govt_job_type")] public string? GovtJobType { get; set; }
    [Column("self_employment_type")] public string? SelfEmploymentType { get; set; }
    [Column("self_employment_skill")] public string? SelfEmploymentSkill { get; set; }
    [Column("self_employment_agri")] public string? SelfEmploymentAgri { get; set; }
    [Column("self_employment_business")] public string? SelfEmploymentBusiness { get; set; }
    [Column("self_employment_service")] public string? SelfEmploymentService { get; set; }
    [Column("self_employment_home")] public string? SelfEmploymentHome { get; set; }

    // Training
    [Column("has_vocational_training")] public string? HasVocationalTraining { get; set; }
    [Column("training_summary")] public string? TrainingSummary { get; set; }
    [Column("training_sewing")] public short? TrainingSewing { get; set; }
    [Column("training_construction")] public short? TrainingConstruction { get; set; }
    [Column("training_carpentry")] public short? TrainingCarpentry { get; set; }
    [Column("training_metalwork")] public short? TrainingMetalwork { get; set; }
    [Column("training_electrical")] public short? TrainingElectrical { get; set; }
    [Column("training_plumbing")] public short? TrainingPlumbing { get; set; }
    [Column("training_automobile")] public short? TrainingAutomobile { get; set; }
    [Column("training_agriculture")] public short? TrainingAgriculture { get; set; }
    [Column("training_public_health")] public short? TrainingPublicHealth { get; set; }
    [Column("training_animal_health")] public short? TrainingAnimalHealth { get; set; }
    [Column("training_tourism")] public short? TrainingTourism { get; set; }
    [Column("training_arts")] public short? TrainingArts { get; set; }
    [Column("training_other")] public short? TrainingOther { get; set; }
    [Column("training_other_detail")] public string? TrainingOtherDetail { get; set; }

    // Residence
    [Column("residence_status")] public string? ResidenceStatus { get; set; }
    [Column("abroad_country")] public string? AbroadCountry { get; set; }
    [Column("abroad_purpose")] public string? AbroadPurpose { get; set; }

    // Health
    [Column("disability_status")] public string? DisabilityStatus { get; set; }
    [Column("health_status")] public string? HealthStatus { get; set; }
    [Column("disease_summary")] public string? DiseaseSummary { get; set; }
    [Column("disease_heart")] public short? DiseaseHeart { get; set; }
    [Column("disease_cancer")] public short? DiseaseCancer { get; set; }
    [Column("disease_diabetes")] public short? DiseaseDiabetes { get; set; }
    [Column("disease_arthritis")] public short? DiseaseArthritis { get; set; }
    [Column("disease_addiction")] public short? DiseaseAddiction { get; set; }
    [Column("disease_hiv_aids")] public short? DiseaseHivAids { get; set; }
    [Column("disease_asthma")] public short? DiseaseAsthma { get; set; }
    [Column("disease_kidney")] public short? DiseaseKidney { get; set; }
    [Column("disease_paralysis")] public short? DiseaseParalysis { get; set; }
    [Column("disease_spinal_injury")] public short? DiseaseSpinalInjury { get; set; }

    // Insurance
    [Column("social_security_status")] public string? SocialSecurityStatus { get; set; }
    [Column("has_insurance")] public string? HasInsurance { get; set; }
    [Column("insurance_summary")] public string? InsuranceSummary { get; set; }
    [Column("insurance_life")] public short? InsuranceLife { get; set; }
    [Column("insurance_health")] public short? InsuranceHealth { get; set; }
    [Column("insurance_other")] public short? InsuranceOther { get; set; }
    [Column("insurance_other_detail")] public string? InsuranceOtherDetail { get; set; }

    // Navigation
    public Household? Household { get; set; }
}
