-- ================================================================
-- VRINDAVAN MUNICIPALITY DIGITAL PROFILE DATABASE
-- Complete Schema — Institutional + Household Surveys
-- ================================================================
-- Database: vrindavan_municipality (PostgreSQL)
-- No PostGIS — plain lat/long for Leaflet.js
-- Designed for: municipality profile web application
--   - Filter by ward, institution type, demographics
--   - Aggregate counts, sums, averages
--   - Geo display on maps
-- ================================================================

-- ================================================================
-- INSTITUTIONAL SURVEY — survey_entry + 16 child tables
-- Source: KoboToolbox, 1326 rows, 478 keys per row
-- ================================================================

-- ================================================================
-- 1. SURVEY_ENTRY — Parent (1 row per institutional submission)
-- ================================================================
CREATE TABLE survey_entry (
    id                  BIGSERIAL PRIMARY KEY,
    odk_id              BIGINT,
    uuid                TEXT UNIQUE,
    submission_time     TIMESTAMPTZ,
    validation_status   TEXT,
    notes               TEXT,
    status              TEXT,
    submitted_by        TEXT,
    version             TEXT,
    tags                TEXT,
    odk_index           INTEGER,
    survey_start        TIMESTAMPTZ,
    survey_end          TIMESTAMPTZ,
    survey_date         TIMESTAMPTZ,
    username            TEXT,
    device_id           TEXT,
    phone_number        TEXT,
    consent_text        TEXT,
    municipality        TEXT,
    ward_no             INTEGER,
    enumerator_name     TEXT,
    subject_area        TEXT,
    created_at          TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_se_ward ON survey_entry (ward_no);
CREATE INDEX idx_se_subject ON survey_entry (subject_area);
CREATE INDEX idx_se_uuid ON survey_entry (uuid);
CREATE INDEX idx_se_odk ON survey_entry (odk_id);

-- ================================================================
-- 2. EDUCATIONAL INSTITUTION
-- ================================================================
CREATE TABLE educational_institution (
    id                              BIGSERIAL PRIMARY KEY,
    survey_entry_id                 BIGINT NOT NULL REFERENCES survey_entry(id),
    institution_type                TEXT,
    institution_detail              TEXT,
    name                            TEXT,
    established_date                TEXT,
    contact_no                      TEXT,
    email_website                   TEXT,
    approved_female_pre_primary     DOUBLE PRECISION,
    approved_female_basic           DOUBLE PRECISION,
    approved_female_secondary       DOUBLE PRECISION,
    approved_male_pre_primary       DOUBLE PRECISION,
    approved_male_basic             DOUBLE PRECISION,
    approved_male_secondary         DOUBLE PRECISION,
    working_female_pre_primary      DOUBLE PRECISION,
    working_female_basic            DOUBLE PRECISION,
    working_female_secondary        DOUBLE PRECISION,
    working_male_pre_primary        DOUBLE PRECISION,
    working_male_basic              DOUBLE PRECISION,
    working_male_secondary          DOUBLE PRECISION,
    subject_teacher_available       TEXT,
    internet_available              TEXT,
    male_teachers_using_internet    DOUBLE PRECISION,
    female_teachers_using_internet  DOUBLE PRECISION,
    female_students_pre_primary     DOUBLE PRECISION,
    female_students_basic           DOUBLE PRECISION,
    female_students_secondary       DOUBLE PRECISION,
    male_students_pre_primary       DOUBLE PRECISION,
    male_students_basic             DOUBLE PRECISION,
    male_students_secondary         DOUBLE PRECISION,
    gross_enrollment_pre_primary    DOUBLE PRECISION,
    gross_enrollment_basic          DOUBLE PRECISION,
    gross_enrollment_secondary      DOUBLE PRECISION,
    retention_pre_primary           DOUBLE PRECISION,
    retention_basic                 DOUBLE PRECISION,
    retention_secondary             DOUBLE PRECISION,
    achievement_class_8             DOUBLE PRECISION,
    achievement_class_10            DOUBLE PRECISION,
    achievement_class_12            DOUBLE PRECISION,
    pass_rate_class_8               DOUBLE PRECISION,
    pass_rate_class_10              DOUBLE PRECISION,
    pass_rate_class_12              DOUBLE PRECISION,
    kutchi_building_count           DOUBLE PRECISION,
    kutchi_room_count               DOUBLE PRECISION,
    pakki_room_count                DOUBLE PRECISION,
    pakki_building_count            DOUBLE PRECISION,
    has_library                     TEXT,
    has_science_lab                 TEXT,
    desk_bench_capacity             DOUBLE PRECISION,
    has_playground                  TEXT,
    compound_wall_type              TEXT,
    toilet_facility                 TEXT,
    integrated_curriculum           TEXT,
    has_clean_drinking_water        TEXT,
    has_disabled_resource_class     TEXT,
    has_child_club                  TEXT,
    child_friendly_teaching         TEXT,
    school_improvement_plan         TEXT,
    total_male_students             DOUBLE PRECISION,
    total_female_students           DOUBLE PRECISION,
    basic_completed_male            DOUBLE PRECISION,
    basic_completed_female          DOUBLE PRECISION,
    scholarship_male                DOUBLE PRECISION,
    scholarship_female              DOUBLE PRECISION,
    midday_meal_male                DOUBLE PRECISION,
    midday_meal_female              DOUBLE PRECISION,
    technical_class9_male           DOUBLE PRECISION,
    technical_class9_female         DOUBLE PRECISION,
    science_class11_male            DOUBLE PRECISION,
    science_class11_female          DOUBLE PRECISION,
    scholarship_dalit_male          DOUBLE PRECISION,
    scholarship_dalit_female        DOUBLE PRECISION,
    scholarship_disabled_male       DOUBLE PRECISION,
    scholarship_disabled_female     DOUBLE PRECISION,
    scholarship_talented_male       DOUBLE PRECISION,
    scholarship_talented_female     DOUBLE PRECISION,
    scholarship_poor_male           DOUBLE PRECISION,
    scholarship_poor_female         DOUBLE PRECISION,
    scholarship_other_male          DOUBLE PRECISION,
    scholarship_other_female        DOUBLE PRECISION,
    scholarship_muslim_male         DOUBLE PRECISION,
    scholarship_muslim_female       DOUBLE PRECISION,
    smc_formation_status            TEXT,
    smc_regular_meeting             TEXT,
    geo_point                       TEXT,
    latitude                        DOUBLE PRECISION,
    longitude                       DOUBLE PRECISION,
    altitude                        DOUBLE PRECISION,
    gps_precision                   DOUBLE PRECISION,
    photo                           TEXT,
    photo_url                       TEXT
);
CREATE INDEX idx_edu_se ON educational_institution (survey_entry_id);
CREATE INDEX idx_edu_ll ON educational_institution (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 3. HEALTH INSTITUTION
-- ================================================================
CREATE TABLE health_institution (
    id                          BIGSERIAL PRIMARY KEY,
    survey_entry_id             BIGINT NOT NULL REFERENCES survey_entry(id),
    institution_type            TEXT,
    institution_type_other      TEXT,
    name                        TEXT,
    address                     TEXT,
    contact_no                  TEXT,
    email_website               TEXT,
    doctors_approved            DOUBLE PRECISION,
    doctors_working             DOUBLE PRECISION,
    other_staff_approved        DOUBLE PRECISION,
    other_staff_working         DOUBLE PRECISION,
    services_summary            TEXT,
    has_maternity               SMALLINT,
    has_lab                     SMALLINT,
    has_clinic                  SMALLINT,
    has_xray                    SMALLINT,
    has_vaccination             SMALLINT,
    has_ct_scan                 SMALLINT,
    has_counseling              SMALLINT,
    has_safe_motherhood         SMALLINT,
    has_blood_transfusion       SMALLINT,
    bcg_vaccinated              DOUBLE PRECISION,
    dpt_hepb_vaccinated         DOUBLE PRECISION,
    polio_3rd_vaccinated        DOUBLE PRECISION,
    measles_rubella_vaccinated  DOUBLE PRECISION,
    je_vaccinated               DOUBLE PRECISION,
    under5_monitored            DOUBLE PRECISION,
    under5_malnourished         DOUBLE PRECISION,
    delivery_last_year          DOUBLE PRECISION,
    tt1_pregnant                DOUBLE PRECISION,
    tt2_pregnant                DOUBLE PRECISION,
    anc_4visits_pregnant        DOUBLE PRECISION,
    institutional_delivery      DOUBLE PRECISION,
    skilled_home_delivery       DOUBLE PRECISION,
    unskilled_home_delivery     DOUBLE PRECISION,
    maternal_death_6wk          DOUBLE PRECISION,
    live_births                 DOUBLE PRECISION,
    still_births                DOUBLE PRECISION,
    neonatal_death_28d          DOUBLE PRECISION,
    women_15_48                 DOUBLE PRECISION,
    fp_permanent_female         DOUBLE PRECISION,
    fp_temporary_female         DOUBLE PRECISION,
    fp_permanent_male           DOUBLE PRECISION,
    fp_temporary_male           DOUBLE PRECISION,
    fchv_count                  DOUBLE PRECISION,
    geo_point                   TEXT,
    latitude                    DOUBLE PRECISION,
    longitude                   DOUBLE PRECISION,
    altitude                    DOUBLE PRECISION,
    gps_precision               DOUBLE PRECISION,
    photo                       TEXT,
    photo_url                   TEXT,
    end_marker                  TEXT
);
CREATE INDEX idx_health_se ON health_institution (survey_entry_id);
CREATE INDEX idx_health_ll ON health_institution (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 4. BUILDING
-- ================================================================
CREATE TABLE building (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    building_type       TEXT,
    purpose_summary     TEXT,
    purpose_kutchi      SMALLINT,
    purpose_pakki       SMALLINT,
    purpose_semi_pakki  SMALLINT,
    purpose_admin       SMALLINT,
    purpose_education   SMALLINT,
    purpose_health      SMALLINT,
    purpose_agriculture SMALLINT,
    purpose_cooperative SMALLINT,
    purpose_assembly    SMALLINT,
    purpose_other       SMALLINT,
    purpose_other_detail TEXT,
    capacity            TEXT,
    usage_status        TEXT,
    usage_status_other  TEXT,
    ownership           TEXT,
    name                TEXT,
    address             TEXT,
    contact_no          TEXT,
    building_material   TEXT,
    construction_status TEXT,
    construction_date   TIMESTAMPTZ,
    total_floors        DOUBLE PRECISION,
    total_rooms         DOUBLE PRECISION,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_bld_se ON building (survey_entry_id);
CREATE INDEX idx_bld_ll ON building (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 5. SECURITY BODY
-- ================================================================
CREATE TABLE security_body (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    body_type           TEXT,
    personnel_count     DOUBLE PRECISION,
    name                TEXT,
    address             TEXT,
    contact_no          TEXT,
    email_website       TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_sec_se ON security_body (survey_entry_id);
CREATE INDEX idx_sec_ll ON security_body (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 6. FINANCIAL INSTITUTION
-- ================================================================
CREATE TABLE financial_institution (
    id                      BIGSERIAL PRIMARY KEY,
    survey_entry_id         BIGINT NOT NULL REFERENCES survey_entry(id),
    institution_type        TEXT,
    name                    TEXT,
    address                 TEXT,
    contact_no              TEXT,
    email_website           TEXT,
    building_level          TEXT,
    own_building            TEXT,
    female_share_members    DOUBLE PRECISION,
    male_share_members      DOUBLE PRECISION,
    coop_type_summary       TEXT,
    coop_female_member      SMALLINT,
    coop_male_member        SMALLINT,
    coop_joint              SMALLINT,
    coop_agri_savings       SMALLINT,
    coop_loan               SMALLINT,
    coop_multipurpose       SMALLINT,
    coop_other              SMALLINT,
    coop_other_detail       TEXT,
    total_share_capital     DOUBLE PRECISION,
    total_savings           DOUBLE PRECISION,
    female_borrowers        DOUBLE PRECISION,
    male_borrowers          DOUBLE PRECISION,
    max_loan_amount         DOUBLE PRECISION,
    min_loan_amount         DOUBLE PRECISION,
    loan_type_summary       TEXT,
    loan_agriculture        SMALLINT,
    loan_livestock          SMALLINT,
    loan_building           SMALLINT,
    loan_hire_purchase      SMALLINT,
    loan_medical            SMALLINT,
    loan_house              SMALLINT,
    loan_other              SMALLINT,
    loan_other_detail       TEXT,
    govt_support_amount     DOUBLE PRECISION,
    external_support_summary TEXT,
    has_govt_support        SMALLINT,
    has_ngo_support         SMALLINT,
    no_support              SMALLINT,
    other_support_summary   TEXT,
    support_furniture       SMALLINT,
    support_goods           SMALLINT,
    support_training        SMALLINT,
    support_capacity        SMALLINT,
    support_computer        SMALLINT,
    support_stationery      SMALLINT,
    support_other           SMALLINT,
    ngo_support_amount      DOUBLE PRECISION,
    geo_point               TEXT,
    latitude                DOUBLE PRECISION,
    longitude               DOUBLE PRECISION,
    altitude                DOUBLE PRECISION,
    gps_precision           DOUBLE PRECISION,
    photo                   TEXT,
    photo_url               TEXT
);
CREATE INDEX idx_fin_se ON financial_institution (survey_entry_id);
CREATE INDEX idx_fin_ll ON financial_institution (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 7. HOTEL / RESORT
-- ================================================================
CREATE TABLE hotel_resort (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    hotel_type          TEXT,
    respondent_name     TEXT,
    hotel_name          TEXT,
    address             TEXT,
    is_registered       TEXT,
    registered_at       TEXT,
    contact_no          TEXT,
    email_website       TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_htl_se ON hotel_resort (survey_entry_id);
CREATE INDEX idx_htl_ll ON hotel_resort (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 8. BRIDGE
-- ================================================================
CREATE TABLE bridge (
    id                          BIGSERIAL PRIMARY KEY,
    survey_entry_id             BIGINT NOT NULL REFERENCES survey_entry(id),
    bridge_type                 TEXT,
    bridge_condition            TEXT,
    repair_summary              TEXT,
    repair_wire_bar             SMALLINT,
    repair_side_wall            SMALLINT,
    repair_road_hole            SMALLINT,
    repair_foundation_crack     SMALLINT,
    repair_foundation_broken    SMALLINT,
    length_meters               DOUBLE PRECISION,
    width_meters                DOUBLE PRECISION,
    respondent_name             TEXT,
    address                     TEXT,
    photo                       TEXT,
    photo_url                   TEXT,
    geo_point                   TEXT,
    latitude                    DOUBLE PRECISION,
    longitude                   DOUBLE PRECISION,
    altitude                    DOUBLE PRECISION,
    gps_precision               DOUBLE PRECISION
);
CREATE INDEX idx_brg_se ON bridge (survey_entry_id);
CREATE INDEX idx_brg_ll ON bridge (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 9. INDUSTRY
-- ================================================================
CREATE TABLE industry (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    ownership           TEXT,
    industry_type       TEXT,
    products            TEXT,
    products_detail     TEXT,
    other_products      TEXT,
    employment_summary  TEXT,
    employment_female   DOUBLE PRECISION,
    employment_male     DOUBLE PRECISION,
    employment_total    DOUBLE PRECISION,
    is_registered       TEXT,
    registered_at       TEXT,
    respondent_name     TEXT,
    contact_no          TEXT,
    address             TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_ind_se ON industry (survey_entry_id);
CREATE INDEX idx_ind_ll ON industry (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 10. NGO / CLUB / GROUP
-- ================================================================
CREATE TABLE ngo_club_group (
    id                      BIGSERIAL PRIMARY KEY,
    survey_entry_id         BIGINT NOT NULL REFERENCES survey_entry(id),
    org_type                TEXT,
    org_name                TEXT,
    respondent_name         TEXT,
    address                 TEXT,
    contact_no              TEXT,
    established_date_bs     TIMESTAMPTZ,
    registered_at           TEXT,
    annual_transaction      DOUBLE PRECISION,
    priority_areas          TEXT,
    priority_other          TEXT,
    donor_list              TEXT,
    working_area            TEXT,
    active_programs         DOUBLE PRECISION,
    total_employees         DOUBLE PRECISION,
    female_members          DOUBLE PRECISION,
    male_members            DOUBLE PRECISION,
    female_beneficiaries    DOUBLE PRECISION,
    male_beneficiaries      DOUBLE PRECISION,
    geo_point               TEXT,
    latitude                DOUBLE PRECISION,
    longitude               DOUBLE PRECISION,
    altitude                DOUBLE PRECISION,
    gps_precision           DOUBLE PRECISION,
    photo                   TEXT,
    photo_url               TEXT
);
CREATE INDEX idx_ngo_se ON ngo_club_group (survey_entry_id);
CREATE INDEX idx_ngo_ll ON ngo_club_group (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 11. LAND USE
-- ================================================================
CREATE TABLE land_use (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    land_type           TEXT,
    ownership           TEXT,
    boundary            TEXT,
    history             TEXT,
    public_area_type    TEXT,
    disaster_summary    TEXT,
    disaster_flood      SMALLINT,
    disaster_landslide  SMALLINT,
    disaster_river_cut  SMALLINT,
    disaster_inundation SMALLINT,
    disaster_drought    SMALLINT,
    total_area          DOUBLE PRECISION,
    respondent_name     TEXT,
    address             TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_land_se ON land_use (survey_entry_id);
CREATE INDEX idx_land_ll ON land_use (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 12. NATURAL RESOURCE
-- ================================================================
CREATE TABLE natural_resource (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    resource_type       TEXT,
    name                TEXT,
    total_area          DOUBLE PRECISION,
    total_households    DOUBLE PRECISION,
    ownership           TEXT,
    boundary            TEXT,
    history             TEXT,
    respondent_name     TEXT,
    address             TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_nat_se ON natural_resource (survey_entry_id);
CREATE INDEX idx_nat_ll ON natural_resource (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 13. CULTURAL HERITAGE
-- ================================================================
CREATE TABLE cultural_heritage (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    heritage_type       TEXT,
    name                TEXT,
    annual_income       DOUBLE PRECISION,
    annual_visitors     DOUBLE PRECISION,
    total_area_sqkm     DOUBLE PRECISION,
    construction_year   TIMESTAMPTZ,
    has_guides          TEXT,
    guide_count         DOUBLE PRECISION,
    significance        TEXT,
    boundary            TEXT,
    history             TEXT,
    respondent_name     TEXT,
    address             TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_cul_se ON cultural_heritage (survey_entry_id);
CREATE INDEX idx_cul_ll ON cultural_heritage (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 14. TRADE / BUSINESS
-- ================================================================
CREATE TABLE trade_business (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    ownership           TEXT,
    business_name       TEXT,
    respondent_name     TEXT,
    address             TEXT,
    operator_name       TEXT,
    contact_no          TEXT,
    is_registered       TEXT,
    registration_no     TEXT,
    registered_at       TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION,
    photo               TEXT,
    photo_url           TEXT
);
CREATE INDEX idx_trd_se ON trade_business (survey_entry_id);
CREATE INDEX idx_trd_ll ON trade_business (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 15. FOREST
-- ================================================================
CREATE TABLE forest (
    id                          BIGSERIAL PRIMARY KEY,
    survey_entry_id             BIGINT NOT NULL REFERENCES survey_entry(id),
    forest_name                 TEXT,
    forest_type                 TEXT,
    area_in_ward                TEXT,
    associated_households       DOUBLE PRECISION,
    main_plants                 TEXT,
    main_animals                TEXT,
    herbs_medicinal             TEXT,
    annual_income               DOUBLE PRECISION,
    timber_production_mt        DOUBLE PRECISION,
    non_timber_production_mt    DOUBLE PRECISION,
    geo_point                   TEXT,
    latitude                    DOUBLE PRECISION,
    longitude                   DOUBLE PRECISION,
    altitude                    DOUBLE PRECISION,
    gps_precision               DOUBLE PRECISION
);
CREATE INDEX idx_for_se ON forest (survey_entry_id);
CREATE INDEX idx_for_ll ON forest (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 16. ROAD
-- ================================================================
CREATE TABLE road (
    id                  BIGSERIAL PRIMARY KEY,
    survey_entry_id     BIGINT NOT NULL REFERENCES survey_entry(id),
    road_name           TEXT,
    road_type           TEXT,
    length_in_ward      DOUBLE PRECISION,
    width               DOUBLE PRECISION,
    construction_year   TIMESTAMPTZ,
    respondent_name     TEXT,
    photo               TEXT,
    photo_url           TEXT,
    geo_point           TEXT,
    latitude            DOUBLE PRECISION,
    longitude           DOUBLE PRECISION,
    altitude            DOUBLE PRECISION,
    gps_precision       DOUBLE PRECISION
);
CREATE INDEX idx_road_se ON road (survey_entry_id);
CREATE INDEX idx_road_ll ON road (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 17. WARD STATISTICS
-- ================================================================
CREATE TABLE ward_statistics (
    id                          BIGSERIAL PRIMARY KEY,
    survey_entry_id             BIGINT NOT NULL REFERENCES survey_entry(id),
    birth_registrations         DOUBLE PRECISION,
    marriage_registrations      DOUBLE PRECISION,
    death_registrations         DOUBLE PRECISION,
    divorce_registrations       DOUBLE PRECISION,
    migration_out_households    DOUBLE PRECISION,
    migration_in_households     DOUBLE PRECISION,
    citizenship_recommendations DOUBLE PRECISION,
    passport_recommendations    DOUBLE PRECISION,
    senior_female               DOUBLE PRECISION,
    senior_male                 DOUBLE PRECISION,
    senior_allowance_total      DOUBLE PRECISION,
    single_women_count          DOUBLE PRECISION,
    single_women_allowance      DOUBLE PRECISION,
    full_disabled_male          DOUBLE PRECISION,
    full_disabled_female        DOUBLE PRECISION,
    full_disabled_allowance     DOUBLE PRECISION,
    partial_disabled_male       DOUBLE PRECISION,
    partial_disabled_female     DOUBLE PRECISION,
    partial_disabled_allowance  DOUBLE PRECISION,
    child_protection_male       DOUBLE PRECISION,
    child_protection_female     DOUBLE PRECISION,
    child_protection_allowance  DOUBLE PRECISION,
    dalit_allowance_male        DOUBLE PRECISION,
    dalit_allowance_female      DOUBLE PRECISION,
    dalit_allowance_total       DOUBLE PRECISION,
    geo_point                   TEXT,
    latitude                    DOUBLE PRECISION,
    longitude                   DOUBLE PRECISION,
    altitude                    DOUBLE PRECISION,
    gps_precision               DOUBLE PRECISION
);
CREATE INDEX idx_ward_se ON ward_statistics (survey_entry_id);
CREATE INDEX idx_ward_ll ON ward_statistics (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- HOUSEHOLD SURVEY — household + 3 child repeat groups
-- Source: KoboToolbox, 8482 + 43013 + 8647 + 118 rows
-- ================================================================

-- ================================================================
-- 18. HOUSEHOLD — Parent (1 row per household)
-- ================================================================
CREATE TABLE household (
    id                      BIGSERIAL PRIMARY KEY,

    -- ODK metadata
    odk_id                  BIGINT,
    uuid                    TEXT UNIQUE,
    submission_time         TIMESTAMPTZ,
    validation_status       TEXT,
    notes                   TEXT,
    status                  TEXT,
    submitted_by            TEXT,
    version                 TEXT,
    tags                    TEXT,
    odk_index               INTEGER,

    -- Session
    survey_start            TIMESTAMPTZ,
    survey_end              TIMESTAMPTZ,
    device_id               TEXT,
    phone_number            TEXT,

    -- Consent
    consent_text            TEXT,

    -- Section 1
    municipality            TEXT,
    ward_no                 INTEGER,
    enumerator_name         TEXT,

    -- Section 2: Location
    house_serial_no         DOUBLE PRECISION,
    settlement_name         TEXT,
    road_name               TEXT,
    road_type               TEXT,
    distance_from_center    TEXT,

    -- Section 3: Respondent
    respondent_name         TEXT,
    respondent_phone        TEXT,
    respondent_relation     TEXT,
    respondent_relation_other TEXT,
    migrated_last_5years    TEXT,
    migrated_from           TEXT,
    residence_type          TEXT,
    permanent_address       TEXT,
    other_ward              TEXT,
    citizenship_status      TEXT,
    family_member_count     DOUBLE PRECISION,

    -- Section 5: Death
    death_in_last_year      TEXT,
    death_count             DOUBLE PRECISION,

    -- Section 6: Livestock insurance
    livestock_insured       TEXT,
    livestock_insured_count DOUBLE PRECISION,
    nearest_market_name     TEXT,

    -- Section 7: Facilities
    market_travel_time      TEXT,
    first_health_visit      TEXT,
    hospital_travel_time    TEXT,
    child_labor             TEXT,
    drinking_water_source   TEXT,
    drinking_water_other    TEXT,
    cooking_fuel_primary    TEXT,
    cooking_fuel_secondary  TEXT,
    stove_type              TEXT,
    light_source            TEXT,
    light_source_other      TEXT,
    waste_management        TEXT,
    toilet_type             TEXT,

    -- 7.12: Assets (multi-select, 0/1)
    assets_summary          TEXT,
    asset_radio             SMALLINT,
    asset_bicycle           SMALLINT,
    asset_television        SMALLINT,
    asset_phone             SMALLINT,
    asset_computer          SMALLINT,
    asset_internet          SMALLINT,
    asset_motorcycle        SMALLINT,
    asset_car               SMALLINT,
    asset_refrigerator      SMALLINT,
    asset_tractor           SMALLINT,
    asset_bus_truck         SMALLINT,
    asset_tipper_truck      SMALLINT,
    asset_excavator         SMALLINT,
    asset_none              SMALLINT,

    -- 7.13-7.14: Education
    school_travel_time      TEXT,
    school_type_summary     TEXT,
    school_type_govt        SMALLINT,
    school_type_private     SMALLINT,
    school_type_madrasa     SMALLINT,

    -- Section 8: Agriculture
    has_agriculture_land    TEXT,
    own_land_kattha         DOUBLE PRECISION,
    others_land_kattha      DOUBLE PRECISION,
    other_ownership_kattha  DOUBLE PRECISION,
    khet_area_kattha        DOUBLE PRECISION,
    bari_area_kattha        DOUBLE PRECISION,
    fallow_land_kattha      DOUBLE PRECISION,
    irrigation_status       TEXT,
    irrigation_medium_summary TEXT,
    irrigation_canal        SMALLINT,
    irrigation_tubewell     SMALLINT,
    irrigation_electric     SMALLINT,
    farming_method          TEXT,

    -- 9.1: Cereals
    has_cereal_crops        TEXT,
    cereal_crop_summary     TEXT,
    cereal_rice             SMALLINT,
    cereal_maize            SMALLINT,
    cereal_wheat            SMALLINT,
    cereal_barley           SMALLINT,

    -- Crop production: Rice (no suffix)
    rice_area               DOUBLE PRECISION,
    rice_production_kg      DOUBLE PRECISION,
    rice_sales_rupees       DOUBLE PRECISION,
    rice_deficit_rupees     DOUBLE PRECISION,
    -- Maize (_2)
    maize_area              DOUBLE PRECISION,
    maize_production_kg     DOUBLE PRECISION,
    maize_sales_rupees      DOUBLE PRECISION,
    maize_deficit_rupees    DOUBLE PRECISION,
    -- Wheat (_3)
    wheat_area              DOUBLE PRECISION,
    wheat_production_kg     DOUBLE PRECISION,
    wheat_sales_rupees      DOUBLE PRECISION,
    wheat_deficit_rupees    DOUBLE PRECISION,
    -- Barley (_4)
    barley_area             DOUBLE PRECISION,
    barley_production_kg    DOUBLE PRECISION,
    barley_sales_rupees     DOUBLE PRECISION,
    barley_deficit_rupees   DOUBLE PRECISION,

    -- 9.2: Pulses
    has_pulse_crops         TEXT,
    pulse_crop_summary      TEXT,
    pulse_black_dal         SMALLINT,
    pulse_rahar             SMALLINT,
    pulse_masuro            SMALLINT,
    pulse_chana             SMALLINT,
    pulse_simi              SMALLINT,
    pulse_bhattmas          SMALLINT,
    pulse_mung              SMALLINT,
    pulse_khesari           SMALLINT,
    -- Pulse production: _5 through _12
    pulse_1_area            DOUBLE PRECISION,
    pulse_1_production_kg   DOUBLE PRECISION,
    pulse_1_sales_rupees    DOUBLE PRECISION,
    pulse_1_deficit_rupees  DOUBLE PRECISION,
    pulse_2_area            DOUBLE PRECISION,
    pulse_2_production_kg   DOUBLE PRECISION,
    pulse_2_sales_rupees    DOUBLE PRECISION,
    pulse_2_deficit_rupees  DOUBLE PRECISION,
    pulse_3_area            DOUBLE PRECISION,
    pulse_3_production_kg   DOUBLE PRECISION,
    pulse_3_sales_rupees    DOUBLE PRECISION,
    pulse_3_deficit_rupees  DOUBLE PRECISION,
    pulse_4_area            DOUBLE PRECISION,
    pulse_4_production_kg   DOUBLE PRECISION,
    pulse_4_sales_rupees    DOUBLE PRECISION,
    pulse_4_deficit_rupees  DOUBLE PRECISION,
    pulse_5_area            DOUBLE PRECISION,
    pulse_5_production_kg   DOUBLE PRECISION,
    pulse_5_sales_rupees    DOUBLE PRECISION,
    pulse_5_deficit_rupees  DOUBLE PRECISION,
    pulse_6_area            DOUBLE PRECISION,
    pulse_6_production_kg   DOUBLE PRECISION,
    pulse_6_sales_rupees    DOUBLE PRECISION,
    pulse_6_deficit_rupees  DOUBLE PRECISION,
    pulse_7_area            DOUBLE PRECISION,
    pulse_7_production_kg   DOUBLE PRECISION,
    pulse_7_sales_rupees    DOUBLE PRECISION,
    pulse_7_deficit_rupees  DOUBLE PRECISION,
    pulse_8_area            DOUBLE PRECISION,
    pulse_8_production_kg   DOUBLE PRECISION,
    pulse_8_sales_rupees    DOUBLE PRECISION,
    pulse_8_deficit_rupees  DOUBLE PRECISION,

    -- 9.3: Oilseeds
    has_oilseed_crops       TEXT,
    oilseed_crop_summary    TEXT,
    oilseed_tori            SMALLINT,
    oilseed_til             SMALLINT,
    oilseed_sunflower       SMALLINT,
    oilseed_other           SMALLINT,
    oilseed_1_area          DOUBLE PRECISION,
    oilseed_1_production_kg DOUBLE PRECISION,
    oilseed_1_sales_rupees  DOUBLE PRECISION,
    oilseed_1_deficit_rupees DOUBLE PRECISION,
    oilseed_2_area          DOUBLE PRECISION,
    oilseed_2_production_kg DOUBLE PRECISION,
    oilseed_2_sales_rupees  DOUBLE PRECISION,
    oilseed_2_deficit_rupees DOUBLE PRECISION,
    oilseed_3_area          DOUBLE PRECISION,
    oilseed_3_production_kg DOUBLE PRECISION,
    oilseed_3_sales_rupees  DOUBLE PRECISION,
    oilseed_3_deficit_rupees DOUBLE PRECISION,
    oilseed_4_area          DOUBLE PRECISION,
    oilseed_4_production_kg DOUBLE PRECISION,
    oilseed_4_sales_rupees  DOUBLE PRECISION,
    oilseed_4_deficit_rupees DOUBLE PRECISION,

    -- 9.4: Cash crops
    has_cash_crops          TEXT,
    cash_crop_summary       TEXT,
    cash_ginger_turmeric    SMALLINT,
    cash_onion_garlic       SMALLINT,
    cash_mushroom           SMALLINT,
    cash_sugarcane          SMALLINT,
    cash_chili              SMALLINT,
    cash_peanut             SMALLINT,
    cash_1_area             DOUBLE PRECISION,
    cash_1_production_kg    DOUBLE PRECISION,
    cash_1_sales_rupees     DOUBLE PRECISION,
    cash_1_deficit_rupees   DOUBLE PRECISION,
    cash_2_area             DOUBLE PRECISION,
    cash_2_production_kg    DOUBLE PRECISION,
    cash_2_sales_rupees     DOUBLE PRECISION,
    cash_2_deficit_rupees   DOUBLE PRECISION,
    cash_3_area             DOUBLE PRECISION,
    cash_3_production_kg    DOUBLE PRECISION,
    cash_3_sales_rupees     DOUBLE PRECISION,
    cash_3_deficit_rupees   DOUBLE PRECISION,
    cash_4_area             DOUBLE PRECISION,
    cash_4_production_kg    DOUBLE PRECISION,
    cash_4_sales_rupees     DOUBLE PRECISION,
    cash_4_deficit_rupees   DOUBLE PRECISION,
    cash_5_area             DOUBLE PRECISION,
    cash_5_production_kg    DOUBLE PRECISION,
    cash_5_sales_rupees     DOUBLE PRECISION,
    cash_5_deficit_rupees   DOUBLE PRECISION,
    cash_6_area             DOUBLE PRECISION,
    cash_6_production_kg    DOUBLE PRECISION,
    cash_6_sales_rupees     DOUBLE PRECISION,
    cash_6_deficit_rupees   DOUBLE PRECISION,

    -- 9.5: Fruits
    has_fruit_crops         TEXT,
    fruit_crop_summary      TEXT,
    fruit_mango             SMALLINT,
    fruit_lemon             SMALLINT,
    fruit_pomelo            SMALLINT,
    fruit_pomegranate       SMALLINT,
    fruit_banana            SMALLINT,
    fruit_amba              SMALLINT,
    fruit_lichi             SMALLINT,
    fruit_sarifa            SMALLINT,
    fruit_1_area            DOUBLE PRECISION,
    fruit_1_production_kg   DOUBLE PRECISION,
    fruit_1_sales_rupees    DOUBLE PRECISION,
    fruit_1_deficit_rupees  DOUBLE PRECISION,
    fruit_2_area            DOUBLE PRECISION,
    fruit_2_production_kg   DOUBLE PRECISION,
    fruit_2_sales_rupees    DOUBLE PRECISION,
    fruit_2_deficit_rupees  DOUBLE PRECISION,
    fruit_3_area            DOUBLE PRECISION,
    fruit_3_production_kg   DOUBLE PRECISION,
    fruit_3_sales_rupees    DOUBLE PRECISION,
    fruit_3_deficit_rupees  DOUBLE PRECISION,
    fruit_4_area            DOUBLE PRECISION,
    fruit_4_production_kg   DOUBLE PRECISION,
    fruit_4_sales_rupees    DOUBLE PRECISION,
    fruit_4_deficit_rupees  DOUBLE PRECISION,
    fruit_5_area            DOUBLE PRECISION,
    fruit_5_production_kg   DOUBLE PRECISION,
    fruit_5_sales_rupees    DOUBLE PRECISION,
    fruit_5_deficit_rupees  DOUBLE PRECISION,
    fruit_6_area            DOUBLE PRECISION,
    fruit_6_production_kg   DOUBLE PRECISION,
    fruit_6_sales_rupees    DOUBLE PRECISION,
    fruit_6_deficit_rupees  DOUBLE PRECISION,
    fruit_7_area            DOUBLE PRECISION,
    fruit_7_production_kg   DOUBLE PRECISION,
    fruit_7_sales_rupees    DOUBLE PRECISION,
    fruit_7_deficit_rupees  DOUBLE PRECISION,
    fruit_8_area            DOUBLE PRECISION,
    fruit_8_production_kg   DOUBLE PRECISION,
    fruit_8_sales_rupees    DOUBLE PRECISION,
    fruit_8_deficit_rupees  DOUBLE PRECISION,

    -- 9.6: Herbs (3 metrics: area, production, sales — no deficit)
    has_herb_crops          TEXT,
    herb_crop_summary       TEXT,
    herb_harro_barro        SMALLINT,
    herb_gurjo              SMALLINT,
    herb_tulsi              SMALLINT,
    herb_amla               SMALLINT,
    herb_sarpagandha        SMALLINT,
    herb_ajwain             SMALLINT,
    herb_sutli              SMALLINT,
    herb_aloe_vera          SMALLINT,
    herb_bojho              SMALLINT,
    herb_1_area             DOUBLE PRECISION,
    herb_1_production_kg    DOUBLE PRECISION,
    herb_1_sales_rupees     DOUBLE PRECISION,
    herb_2_area             DOUBLE PRECISION,
    herb_2_production_kg    DOUBLE PRECISION,
    herb_2_sales_rupees     DOUBLE PRECISION,
    herb_3_area             DOUBLE PRECISION,
    herb_3_production_kg    DOUBLE PRECISION,
    herb_3_sales_rupees     DOUBLE PRECISION,
    herb_4_area             DOUBLE PRECISION,
    herb_4_production_kg    DOUBLE PRECISION,
    herb_4_sales_rupees     DOUBLE PRECISION,
    herb_5_area             DOUBLE PRECISION,
    herb_5_production_kg    DOUBLE PRECISION,
    herb_5_sales_rupees     DOUBLE PRECISION,
    herb_6_area             DOUBLE PRECISION,
    herb_6_production_kg    DOUBLE PRECISION,
    herb_6_sales_rupees     DOUBLE PRECISION,
    herb_7_area             DOUBLE PRECISION,
    herb_7_production_kg    DOUBLE PRECISION,
    herb_7_sales_rupees     DOUBLE PRECISION,
    herb_8_area             DOUBLE PRECISION,
    herb_8_production_kg    DOUBLE PRECISION,
    herb_8_sales_rupees     DOUBLE PRECISION,
    herb_9_area             DOUBLE PRECISION,
    herb_9_production_kg    DOUBLE PRECISION,
    herb_9_sales_rupees     DOUBLE PRECISION,

    -- 10: Livestock
    has_livestock           TEXT,

    -- 10.1: Dairy (cow, buffalo, other)
    dairy_summary           TEXT,
    dairy_cow               SMALLINT,
    dairy_buffalo           SMALLINT,
    dairy_other             SMALLINT,
    -- Cow data (no suffix)
    cow_count               DOUBLE PRECISION,
    cow_milk_daily          DOUBLE PRECISION,
    cow_milk_sold           DOUBLE PRECISION,
    cow_curd_sold           DOUBLE PRECISION,
    cow_annual_income       DOUBLE PRECISION,
    -- Buffalo data (_2)
    buffalo_count           DOUBLE PRECISION,
    buffalo_milk_daily      DOUBLE PRECISION,
    buffalo_milk_sold       DOUBLE PRECISION,
    buffalo_curd_sold       DOUBLE PRECISION,
    buffalo_annual_income   DOUBLE PRECISION,
    -- Other dairy (_3)
    other_dairy_count       DOUBLE PRECISION,
    other_dairy_milk_daily  DOUBLE PRECISION,
    other_dairy_milk_sold   DOUBLE PRECISION,
    other_dairy_curd_sold   DOUBLE PRECISION,
    other_dairy_annual_income DOUBLE PRECISION,

    -- 10.2: Meat livestock
    meat_summary            TEXT,
    meat_goat_sheep         SMALLINT,
    meat_calf               SMALLINT,
    meat_pig                SMALLINT,
    meat_other              SMALLINT,
    -- Goat/sheep data (no suffix)
    goat_count              DOUBLE PRECISION,
    goat_annual_sold        DOUBLE PRECISION,
    goat_annual_income      DOUBLE PRECISION,
    -- Calf data (_2)
    calf_count              DOUBLE PRECISION,
    calf_annual_sold        DOUBLE PRECISION,
    calf_annual_income      DOUBLE PRECISION,
    -- Pig data (_3)
    pig_count               DOUBLE PRECISION,
    pig_annual_sold         DOUBLE PRECISION,
    pig_annual_income       DOUBLE PRECISION,
    -- Other meat (_4)
    other_meat_count        DOUBLE PRECISION,
    other_meat_annual_sold  DOUBLE PRECISION,
    other_meat_annual_income DOUBLE PRECISION,

    -- 11: Poultry
    poultry_summary         TEXT,
    poultry_chicken         SMALLINT,
    poultry_duck            SMALLINT,
    poultry_pigeon          SMALLINT,
    poultry_battai          SMALLINT,
    poultry_count           DOUBLE PRECISION,
    poultry_egg_annual      DOUBLE PRECISION,
    poultry_meat_sold_kg    DOUBLE PRECISION,
    poultry_annual_income   DOUBLE PRECISION,

    -- 12: Fish farming
    has_fish_farming        TEXT,
    fish_pond_count         DOUBLE PRECISION,
    fish_pond_area          DOUBLE PRECISION,
    fish_production_kg      DOUBLE PRECISION,
    fish_annual_income      DOUBLE PRECISION,
    fish_subsidy            TEXT,

    -- 13: Beekeeping
    has_beekeeping          TEXT,
    bee_hive_count          DOUBLE PRECISION,
    honey_production_kg     DOUBLE PRECISION,
    honey_annual_income     DOUBLE PRECISION,
    bee_subsidy             TEXT,

    -- 14: Income
    income_vegetables       DOUBLE PRECISION,
    income_labor            DOUBLE PRECISION,
    income_trade            DOUBLE PRECISION,
    income_foreign          DOUBLE PRECISION,
    income_job              DOUBLE PRECISION,
    income_other            DOUBLE PRECISION,

    -- 15: Expenditure
    expense_food            DOUBLE PRECISION,
    expense_clothes         DOUBLE PRECISION,
    expense_education       DOUBLE PRECISION,
    expense_health          DOUBLE PRECISION,
    expense_festivals       DOUBLE PRECISION,
    expense_agriculture     DOUBLE PRECISION,
    expense_fuel            DOUBLE PRECISION,
    expense_mobile_internet DOUBLE PRECISION,
    food_sufficiency_months TEXT,

    -- 15.10: Deficit spending
    deficit_spending_summary TEXT,
    deficit_job             SMALLINT,
    deficit_daily_wage      SMALLINT,
    deficit_foreign_job     SMALLINT,
    deficit_trade           SMALLINT,
    deficit_loan            SMALLINT,

    -- 16: Banking
    has_bank_account        TEXT,
    bank_account_summary    TEXT,
    bank_account_bank       SMALLINT,
    bank_account_coop       SMALLINT,
    bank_account_savings    SMALLINT,
    bank_account_moneylender SMALLINT,
    bank_account_other      SMALLINT,
    has_loan                TEXT,
    loan_amount             TEXT,
    loan_source_summary     TEXT,
    loan_from_bank          SMALLINT,
    loan_from_coop          SMALLINT,
    loan_from_moneylender   SMALLINT,
    loan_from_relatives     SMALLINT,
    loan_type               TEXT,
    loan_purpose_summary    TEXT,
    loan_purpose_business   SMALLINT,
    loan_purpose_house      SMALLINT,
    loan_purpose_medical    SMALLINT,
    loan_purpose_education  SMALLINT,
    loan_purpose_other      SMALLINT,
    loan_purpose_other_detail TEXT,

    -- 17-18: Housing & disaster
    house_info_note         TEXT,
    house_count             DOUBLE PRECISION,
    earthquake_resistant    TEXT,
    disaster_damaged        TEXT,
    received_subsidy        TEXT,
    subsidy_installments    TEXT,
    house_rebuilt            TEXT,
    has_family_risk_plan    TEXT,
    disaster_risk           TEXT,
    disaster_risk_summary   TEXT,
    risk_earthquake         SMALLINT,
    risk_flood              SMALLINT,
    risk_landslide          SMALLINT,
    risk_lightning          SMALLINT,
    risk_storm              SMALLINT,
    risk_cold_wave          SMALLINT,
    risk_inundation         SMALLINT,

    -- 19-20: Photo & GPS
    photo                   TEXT,
    photo_url               TEXT,
    geo_point               TEXT,
    latitude                DOUBLE PRECISION,
    longitude               DOUBLE PRECISION,
    altitude                DOUBLE PRECISION,
    gps_precision           DOUBLE PRECISION,

    -- Closing & extras
    closing_note            TEXT,
    distance_from_center_2  TEXT,
    ghee_sold               DOUBLE PRECISION,
    ghee_sold_2             DOUBLE PRECISION,
    ghee_sold_3             DOUBLE PRECISION,

    created_at              TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_hh_ward ON household (ward_no);
CREATE INDEX idx_hh_uuid ON household (uuid);
CREATE INDEX idx_hh_odk ON household (odk_id);
CREATE INDEX idx_hh_ll ON household (latitude, longitude) WHERE latitude IS NOT NULL;

-- ================================================================
-- 19. FAMILY MEMBER (1 row per person, ~43013 rows)
-- ================================================================
CREATE TABLE family_member (
    id                      BIGSERIAL PRIMARY KEY,
    household_id            BIGINT REFERENCES household(id),

    -- Repeat metadata
    repeat_index            INTEGER,
    parent_table_name       TEXT,
    parent_index            INTEGER,
    submission_id           BIGINT,
    submission_uuid         TEXT,
    submission_time         TIMESTAMPTZ,
    submission_validation   TEXT,
    submission_notes        TEXT,
    submission_status       TEXT,
    submission_submitted_by TEXT,
    submission_version      TEXT,
    submission_tags         TEXT,

    -- Section header
    section_note            TEXT,

    -- 4.1-4.9: Personal
    full_name               TEXT,
    surname                 TEXT,
    caste                   TEXT,
    gender                  TEXT,
    relation_to_head        TEXT,
    age                     DOUBLE PRECISION,
    language                TEXT,
    language_other          TEXT,
    religion                TEXT,
    religion_other          TEXT,
    marital_status          TEXT,

    -- 4.10-4.12: Education
    education_level         TEXT,
    education_detail        TEXT,
    literacy_status         TEXT,
    expertise_area          TEXT,
    expertise_other         TEXT,

    -- 4.13: Occupation
    occupation              TEXT,
    occupation_other        TEXT,
    govt_job_type           TEXT,
    self_employment_type    TEXT,
    self_employment_skill   TEXT,
    self_employment_agri    TEXT,
    self_employment_business TEXT,
    self_employment_service TEXT,
    self_employment_home    TEXT,

    -- 4.14-4.15: Training
    has_vocational_training TEXT,
    training_summary        TEXT,
    training_sewing         SMALLINT,
    training_construction   SMALLINT,
    training_carpentry      SMALLINT,
    training_metalwork      SMALLINT,
    training_electrical     SMALLINT,
    training_plumbing       SMALLINT,
    training_automobile     SMALLINT,
    training_agriculture    SMALLINT,
    training_public_health  SMALLINT,
    training_animal_health  SMALLINT,
    training_tourism        SMALLINT,
    training_arts           SMALLINT,
    training_other          SMALLINT,
    training_other_detail   TEXT,

    -- 4.16-4.18: Residence
    residence_status        TEXT,
    abroad_country          TEXT,
    abroad_purpose          TEXT,

    -- 4.19-4.21: Health
    disability_status       TEXT,
    health_status           TEXT,
    disease_summary         TEXT,
    disease_heart           SMALLINT,
    disease_cancer          SMALLINT,
    disease_diabetes        SMALLINT,
    disease_arthritis       SMALLINT,
    disease_addiction       SMALLINT,
    disease_hiv_aids        SMALLINT,
    disease_asthma          SMALLINT,
    disease_kidney          SMALLINT,
    disease_paralysis       SMALLINT,
    disease_spinal_injury   SMALLINT,

    -- 4.22-4.23: Insurance
    social_security_status  TEXT,
    has_insurance           TEXT,
    insurance_summary       TEXT,
    insurance_life          SMALLINT,
    insurance_health        SMALLINT,
    insurance_other         SMALLINT,
    insurance_other_detail  TEXT
);
CREATE INDEX idx_fm_hh ON family_member (household_id);
CREATE INDEX idx_fm_sub ON family_member (submission_id);
CREATE INDEX idx_fm_gender ON family_member (gender);
CREATE INDEX idx_fm_age ON family_member (age);

-- ================================================================
-- 20. HOUSE DETAIL (1 row per house, ~8647 rows)
-- ================================================================
CREATE TABLE house_detail (
    id                      BIGSERIAL PRIMARY KEY,
    household_id            BIGINT REFERENCES household(id),

    -- Repeat metadata
    repeat_index            INTEGER,
    parent_table_name       TEXT,
    parent_index            INTEGER,
    submission_id           BIGINT,
    submission_uuid         TEXT,
    submission_time         TIMESTAMPTZ,
    submission_validation   TEXT,
    submission_notes        TEXT,
    submission_status       TEXT,
    submission_submitted_by TEXT,
    submission_version      TEXT,
    submission_tags         TEXT,

    -- 17: House details
    ownership_type          TEXT,
    total_rooms             DOUBLE PRECISION,
    rooms_rented            DOUBLE PRECISION,
    roof_type               TEXT,
    roof_other              TEXT,
    roof_other_detail       TEXT,
    foundation_type         TEXT,
    foundation_other        TEXT,
    has_building_permit     TEXT,
    meets_building_code     TEXT
);
CREATE INDEX idx_hd_hh ON house_detail (household_id);
CREATE INDEX idx_hd_sub ON house_detail (submission_id);

-- ================================================================
-- 21. HH DEATH RECORD (1 row per death, ~118 rows)
-- ================================================================
CREATE TABLE hh_death_record (
    id                      BIGSERIAL PRIMARY KEY,
    household_id            BIGINT REFERENCES household(id),

    -- Repeat metadata
    repeat_index            INTEGER,
    parent_table_name       TEXT,
    parent_index            INTEGER,
    submission_id           BIGINT,
    submission_uuid         TEXT,
    submission_time         TIMESTAMPTZ,
    submission_validation   TEXT,
    submission_notes        TEXT,
    submission_status       TEXT,
    submission_submitted_by TEXT,
    submission_version      TEXT,
    submission_tags         TEXT,

    -- 5.2: Deceased
    deceased_name           TEXT,
    deceased_gender         TEXT,
    deceased_age_group      TEXT,
    deceased_age_detail     DOUBLE PRECISION,
    cause_summary           TEXT,
    cause_communicable      SMALLINT,
    cause_non_communicable  SMALLINT,
    cause_lifestyle         SMALLINT,
    cause_other             SMALLINT,
    communicable_summary    TEXT,
    communicable_pneumonia  SMALLINT,
    communicable_tb         SMALLINT,
    communicable_hiv_aids   SMALLINT,
    communicable_hepatitis  SMALLINT,
    communicable_dengue     SMALLINT,
    communicable_covid19    SMALLINT,
    communicable_malaria    SMALLINT,
    communicable_typhoid    SMALLINT,
    communicable_diarrhea   SMALLINT,
    communicable_meningitis SMALLINT,
    noncommunicable_summary TEXT,
    noncommunicable_heart   SMALLINT,
    noncommunicable_cancer  SMALLINT,
    noncommunicable_diabetes SMALLINT,
    noncommunicable_respiratory SMALLINT,
    noncommunicable_kidney  SMALLINT,
    noncommunicable_hypertension SMALLINT,
    noncommunicable_dementia SMALLINT,
    lifestyle_summary       TEXT,
    lifestyle_obesity       SMALLINT,
    lifestyle_mental_stress SMALLINT,
    lifestyle_alcohol       SMALLINT,
    lifestyle_smoking       SMALLINT,
    other_cause_summary     TEXT,
    other_road_accident     SMALLINT,
    other_heart_attack      SMALLINT,
    other_suicide           SMALLINT,
    other_animal_bite       SMALLINT,
    other_natural_disaster  SMALLINT,
    other_pregnancy         SMALLINT,
    deceased_age_years      TEXT
);
CREATE INDEX idx_hdr_hh ON hh_death_record (household_id);
CREATE INDEX idx_hdr_sub ON hh_death_record (submission_id);
