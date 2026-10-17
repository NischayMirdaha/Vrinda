using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("health_institution")]
public class HealthInstitution
{
    [Column("id")]
    public long Id { get; set; }

    [Column("survey_entry_id")]
    public long SurveyEntryId { get; set; }

    [Column("institution_type")]
    public string? InstitutionType { get; set; }

    [Column("institution_type_other")]
    public string? InstitutionTypeOther { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("contact_no")]
    public string? ContactNo { get; set; }

    [Column("email_website")]
    public string? EmailWebsite { get; set; }

    [Column("doctors_approved")]
    public double? DoctorsApproved { get; set; }

    [Column("doctors_working")]
    public double? DoctorsWorking { get; set; }

    [Column("other_staff_approved")]
    public double? OtherStaffApproved { get; set; }

    [Column("other_staff_working")]
    public double? OtherStaffWorking { get; set; }

    [Column("services_summary")]
    public string? ServicesSummary { get; set; }

    [Column("has_maternity")]
    public short? HasMaternity { get; set; }

    [Column("has_lab")]
    public short? HasLab { get; set; }

    [Column("has_clinic")]
    public short? HasClinic { get; set; }

    [Column("has_xray")]
    public short? HasXray { get; set; }

    [Column("has_vaccination")]
    public short? HasVaccination { get; set; }

    [Column("has_ct_scan")]
    public short? HasCtScan { get; set; }

    [Column("has_counseling")]
    public short? HasCounseling { get; set; }

    [Column("has_safe_motherhood")]
    public short? HasSafeMotherhood { get; set; }

    [Column("has_blood_transfusion")]
    public short? HasBloodTransfusion { get; set; }

    [Column("bcg_vaccinated")]
    public double? BcgVaccinated { get; set; }

    [Column("dpt_hepb_vaccinated")]
    public double? DptHepbVaccinated { get; set; }

    [Column("polio_3rd_vaccinated")]
    public double? Polio3rdVaccinated { get; set; }

    [Column("measles_rubella_vaccinated")]
    public double? MeaslesRubellaVaccinated { get; set; }

    [Column("je_vaccinated")]
    public double? JeVaccinated { get; set; }

    [Column("under5_monitored")]
    public double? Under5Monitored { get; set; }

    [Column("under5_malnourished")]
    public double? Under5Malnourished { get; set; }

    [Column("delivery_last_year")]
    public double? DeliveryLastYear { get; set; }

    [Column("tt1_pregnant")]
    public double? Tt1Pregnant { get; set; }

    [Column("tt2_pregnant")]
    public double? Tt2Pregnant { get; set; }

    [Column("anc_4visits_pregnant")]
    public double? Anc4VisitsPregnant { get; set; }

    [Column("institutional_delivery")]
    public double? InstitutionalDelivery { get; set; }

    [Column("skilled_home_delivery")]
    public double? SkilledHomeDelivery { get; set; }

    [Column("unskilled_home_delivery")]
    public double? UnskilledHomeDelivery { get; set; }

    [Column("maternal_death_6wk")]
    public double? MaternalDeath6Wk { get; set; }

    [Column("live_births")]
    public double? LiveBirths { get; set; }

    [Column("still_births")]
    public double? StillBirths { get; set; }

    [Column("neonatal_death_28d")]
    public double? NeonatalDeath28D { get; set; }

    [Column("women_15_48")]
    public double? Women1548 { get; set; }

    [Column("fp_permanent_female")]
    public double? FpPermanentFemale { get; set; }

    [Column("fp_temporary_female")]
    public double? FpTemporaryFemale { get; set; }

    [Column("fp_permanent_male")]
    public double? FpPermanentMale { get; set; }

    [Column("fp_temporary_male")]
    public double? FpTemporaryMale { get; set; }

    [Column("fchv_count")]
    public double? FchvCount { get; set; }

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

    [Column("end_marker")]
    public string? EndMarker { get; set; }

    // Navigation
    public SurveyEntry SurveyEntry { get; set; } = null!;
}
