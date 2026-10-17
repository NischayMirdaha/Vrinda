using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("educational_institution")]
public class EducationalInstitution
{
    [Column("id")]
    public long Id { get; set; }

    [Column("survey_entry_id")]
    public long SurveyEntryId { get; set; }

    [Column("institution_type")]
    public string? InstitutionType { get; set; }

    [Column("institution_detail")]
    public string? InstitutionDetail { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("established_date")]
    public string? EstablishedDate { get; set; }

    [Column("contact_no")]
    public string? ContactNo { get; set; }

    [Column("email_website")]
    public string? EmailWebsite { get; set; }

    // Staffing - approved
    [Column("approved_female_pre_primary")]
    public double? ApprovedFemalePrePrimary { get; set; }

    [Column("approved_female_basic")]
    public double? ApprovedFemaleBasic { get; set; }

    [Column("approved_female_secondary")]
    public double? ApprovedFemaleSecondary { get; set; }

    [Column("approved_male_pre_primary")]
    public double? ApprovedMalePrePrimary { get; set; }

    [Column("approved_male_basic")]
    public double? ApprovedMaleBasic { get; set; }

    [Column("approved_male_secondary")]
    public double? ApprovedMaleSecondary { get; set; }

    // Staffing - working
    [Column("working_female_pre_primary")]
    public double? WorkingFemalePrePrimary { get; set; }

    [Column("working_female_basic")]
    public double? WorkingFemaleBasic { get; set; }

    [Column("working_female_secondary")]
    public double? WorkingFemaleSecondary { get; set; }

    [Column("working_male_pre_primary")]
    public double? WorkingMalePrePrimary { get; set; }

    [Column("working_male_basic")]
    public double? WorkingMaleBasic { get; set; }

    [Column("working_male_secondary")]
    public double? WorkingMaleSecondary { get; set; }

    [Column("subject_teacher_available")]
    public string? SubjectTeacherAvailable { get; set; }

    [Column("internet_available")]
    public string? InternetAvailable { get; set; }

    [Column("male_teachers_using_internet")]
    public double? MaleTeachersUsingInternet { get; set; }

    [Column("female_teachers_using_internet")]
    public double? FemaleTeachersUsingInternet { get; set; }

    // Students
    [Column("female_students_pre_primary")]
    public double? FemaleStudentsPrePrimary { get; set; }

    [Column("female_students_basic")]
    public double? FemaleStudentsBasic { get; set; }

    [Column("female_students_secondary")]
    public double? FemaleStudentsSecondary { get; set; }

    [Column("male_students_pre_primary")]
    public double? MaleStudentsPrePrimary { get; set; }

    [Column("male_students_basic")]
    public double? MaleStudentsBasic { get; set; }

    [Column("male_students_secondary")]
    public double? MaleStudentsSecondary { get; set; }

    // Enrollment & Retention
    [Column("gross_enrollment_pre_primary")]
    public double? GrossEnrollmentPrePrimary { get; set; }

    [Column("gross_enrollment_basic")]
    public double? GrossEnrollmentBasic { get; set; }

    [Column("gross_enrollment_secondary")]
    public double? GrossEnrollmentSecondary { get; set; }

    [Column("retention_pre_primary")]
    public double? RetentionPrePrimary { get; set; }

    [Column("retention_basic")]
    public double? RetentionBasic { get; set; }

    [Column("retention_secondary")]
    public double? RetentionSecondary { get; set; }

    // Achievement & Pass rates
    [Column("achievement_class_8")]
    public double? AchievementClass8 { get; set; }

    [Column("achievement_class_10")]
    public double? AchievementClass10 { get; set; }

    [Column("achievement_class_12")]
    public double? AchievementClass12 { get; set; }

    [Column("pass_rate_class_8")]
    public double? PassRateClass8 { get; set; }

    [Column("pass_rate_class_10")]
    public double? PassRateClass10 { get; set; }

    [Column("pass_rate_class_12")]
    public double? PassRateClass12 { get; set; }

    // Infrastructure
    [Column("kutchi_building_count")]
    public double? KutchiBuildingCount { get; set; }

    [Column("kutchi_room_count")]
    public double? KutchiRoomCount { get; set; }

    [Column("pakki_room_count")]
    public double? PakkiRoomCount { get; set; }

    [Column("pakki_building_count")]
    public double? PakkiBuildingCount { get; set; }

    [Column("has_library")]
    public string? HasLibrary { get; set; }

    [Column("has_science_lab")]
    public string? HasScienceLab { get; set; }

    [Column("desk_bench_capacity")]
    public double? DeskBenchCapacity { get; set; }

    [Column("has_playground")]
    public string? HasPlayground { get; set; }

    [Column("compound_wall_type")]
    public string? CompoundWallType { get; set; }

    [Column("toilet_facility")]
    public string? ToiletFacility { get; set; }

    [Column("integrated_curriculum")]
    public string? IntegratedCurriculum { get; set; }

    [Column("has_clean_drinking_water")]
    public string? HasCleanDrinkingWater { get; set; }

    [Column("has_disabled_resource_class")]
    public string? HasDisabledResourceClass { get; set; }

    [Column("has_child_club")]
    public string? HasChildClub { get; set; }

    [Column("child_friendly_teaching")]
    public string? ChildFriendlyTeaching { get; set; }

    [Column("school_improvement_plan")]
    public string? SchoolImprovementPlan { get; set; }

    // Student counts
    [Column("total_male_students")]
    public double? TotalMaleStudents { get; set; }

    [Column("total_female_students")]
    public double? TotalFemaleStudents { get; set; }

    [Column("basic_completed_male")]
    public double? BasicCompletedMale { get; set; }

    [Column("basic_completed_female")]
    public double? BasicCompletedFemale { get; set; }

    // Scholarships
    [Column("scholarship_male")]
    public double? ScholarshipMale { get; set; }

    [Column("scholarship_female")]
    public double? ScholarshipFemale { get; set; }

    [Column("midday_meal_male")]
    public double? MiddayMealMale { get; set; }

    [Column("midday_meal_female")]
    public double? MiddayMealFemale { get; set; }

    [Column("technical_class9_male")]
    public double? TechnicalClass9Male { get; set; }

    [Column("technical_class9_female")]
    public double? TechnicalClass9Female { get; set; }

    [Column("science_class11_male")]
    public double? ScienceClass11Male { get; set; }

    [Column("science_class11_female")]
    public double? ScienceClass11Female { get; set; }

    [Column("scholarship_dalit_male")]
    public double? ScholarshipDalitMale { get; set; }

    [Column("scholarship_dalit_female")]
    public double? ScholarshipDalitFemale { get; set; }

    [Column("scholarship_disabled_male")]
    public double? ScholarshipDisabledMale { get; set; }

    [Column("scholarship_disabled_female")]
    public double? ScholarshipDisabledFemale { get; set; }

    [Column("scholarship_talented_male")]
    public double? ScholarshipTalentedMale { get; set; }

    [Column("scholarship_talented_female")]
    public double? ScholarshipTalentedFemale { get; set; }

    [Column("scholarship_poor_male")]
    public double? ScholarshipPoorMale { get; set; }

    [Column("scholarship_poor_female")]
    public double? ScholarshipPoorFemale { get; set; }

    [Column("scholarship_other_male")]
    public double? ScholarshipOtherMale { get; set; }

    [Column("scholarship_other_female")]
    public double? ScholarshipOtherFemale { get; set; }

    [Column("scholarship_muslim_male")]
    public double? ScholarshipMuslimMale { get; set; }

    [Column("scholarship_muslim_female")]
    public double? ScholarshipMuslimFemale { get; set; }

    // SMC
    [Column("smc_formation_status")]
    public string? SmcFormationStatus { get; set; }

    [Column("smc_regular_meeting")]
    public string? SmcRegularMeeting { get; set; }

    // GPS
    [Column("geo_point")]
    public string? GeoPoint { get; set; }

    [Column("latitude")]
    public double? Latitude { get; set; }

    [Column("longitude")]
    public double? Longitude { get; set; }

    [Column("altitude")]
    public double? Altitude { get; set; }

    [Column("gps_precision")]
    public double? GpsPrecision { get; set; }

    [Column("photo")]
    public string? Photo { get; set; }

    [Column("photo_url")]
    public string? PhotoUrl { get; set; }

    // Navigation
    public SurveyEntry SurveyEntry { get; set; } = null!;
}
