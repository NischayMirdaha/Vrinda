using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("household")]
public class Household
{
    [Column("id")] public long Id { get; set; }

    // ODK metadata
    [Column("odk_id")] public long? OdkId { get; set; }
    [Column("uuid")] public string? Uuid { get; set; }
    [Column("submission_time")] public string? SubmissionTime { get; set; }
    [Column("validation_status")] public string? ValidationStatus { get; set; }
    [Column("notes")] public string? Notes { get; set; }
    [Column("status")] public string? Status { get; set; }
    [Column("submitted_by")] public string? SubmittedBy { get; set; }
    [Column("version")] public string? Version { get; set; }
    [Column("tags")] public string? Tags { get; set; }
    [Column("odk_index")] public int? OdkIndex { get; set; }

    // Session
    [Column("survey_start")] public string? SurveyStart { get; set; }
    [Column("survey_end")] public string? SurveyEnd { get; set; }
    [Column("device_id")] public string? DeviceId { get; set; }
    [Column("phone_number")] public string? PhoneNumber { get; set; }

    // Consent
    [Column("consent_text")] public string? ConsentText { get; set; }

    // Section 1
    [Column("municipality")] public string? Municipality { get; set; }
    [Column("ward_no")] [Range(1, 9, ErrorMessage = "Ward number must be between 1 and 9.")] public int? WardNo { get; set; }
    [Column("enumerator_name")] public string? EnumeratorName { get; set; }

    // Section 2: Location
    [Column("house_serial_no")] public double? HouseSerialNo { get; set; }
    [Column("settlement_name")] public string? SettlementName { get; set; }
    [Column("road_name")] public string? RoadName { get; set; }
    [Column("road_type")] public string? RoadType { get; set; }
    [Column("distance_from_center")] public string? DistanceFromCenter { get; set; }

    // Section 3: Respondent
    [Column("respondent_name")] public string? RespondentName { get; set; }
    [Column("respondent_phone")] public string? RespondentPhone { get; set; }
    [Column("respondent_relation")] public string? RespondentRelation { get; set; }
    [Column("respondent_relation_other")] public string? RespondentRelationOther { get; set; }
    [Column("migrated_last_5years")] public string? MigratedLast5Years { get; set; }
    [Column("migrated_from")] public string? MigratedFrom { get; set; }
    [Column("residence_type")] public string? ResidenceType { get; set; }
    [Column("permanent_address")] public string? PermanentAddress { get; set; }
    [Column("other_ward")] public string? OtherWard { get; set; }
    [Column("citizenship_status")] public string? CitizenshipStatus { get; set; }
    [Column("family_member_count")] public double? FamilyMemberCount { get; set; }

    // Section 5: Death
    [Column("death_in_last_year")] public string? DeathInLastYear { get; set; }
    [Column("death_count")] public double? DeathCount { get; set; }

    // Section 6: Livestock insurance
    [Column("livestock_insured")] public string? LivestockInsured { get; set; }
    [Column("livestock_insured_count")] public double? LivestockInsuredCount { get; set; }
    [Column("nearest_market_name")] public string? NearestMarketName { get; set; }

    // Section 7: Facilities
    [Column("market_travel_time")] public string? MarketTravelTime { get; set; }
    [Column("first_health_visit")] public string? FirstHealthVisit { get; set; }
    [Column("hospital_travel_time")] public string? HospitalTravelTime { get; set; }
    [Column("child_labor")] public string? ChildLabor { get; set; }
    [Column("drinking_water_source")] public string? DrinkingWaterSource { get; set; }
    [Column("drinking_water_other")] public string? DrinkingWaterOther { get; set; }
    [Column("cooking_fuel_primary")] public string? CookingFuelPrimary { get; set; }
    [Column("cooking_fuel_secondary")] public string? CookingFuelSecondary { get; set; }
    [Column("stove_type")] public string? StoveType { get; set; }
    [Column("light_source")] public string? LightSource { get; set; }
    [Column("light_source_other")] public string? LightSourceOther { get; set; }
    [Column("waste_management")] public string? WasteManagement { get; set; }
    [Column("toilet_type")] public string? ToiletType { get; set; }

    // 7.12: Assets
    [Column("assets_summary")] public string? AssetsSummary { get; set; }
    [Column("asset_radio")] public short? AssetRadio { get; set; }
    [Column("asset_bicycle")] public short? AssetBicycle { get; set; }
    [Column("asset_television")] public short? AssetTelevision { get; set; }
    [Column("asset_phone")] public short? AssetPhone { get; set; }
    [Column("asset_computer")] public short? AssetComputer { get; set; }
    [Column("asset_internet")] public short? AssetInternet { get; set; }
    [Column("asset_motorcycle")] public short? AssetMotorcycle { get; set; }
    [Column("asset_car")] public short? AssetCar { get; set; }
    [Column("asset_refrigerator")] public short? AssetRefrigerator { get; set; }
    [Column("asset_tractor")] public short? AssetTractor { get; set; }
    [Column("asset_bus_truck")] public short? AssetBusTruck { get; set; }
    [Column("asset_tipper_truck")] public short? AssetTipperTruck { get; set; }
    [Column("asset_excavator")] public short? AssetExcavator { get; set; }
    [Column("asset_none")] public short? AssetNone { get; set; }

    // 7.13-7.14: Education
    [Column("school_travel_time")] public string? SchoolTravelTime { get; set; }
    [Column("school_type_summary")] public string? SchoolTypeSummary { get; set; }
    [Column("school_type_govt")] public short? SchoolTypeGovt { get; set; }
    [Column("school_type_private")] public short? SchoolTypePrivate { get; set; }
    [Column("school_type_madrasa")] public short? SchoolTypeMadrasa { get; set; }

    // Section 8: Agriculture
    [Column("has_agriculture_land")] public string? HasAgricultureLand { get; set; }
    [Column("own_land_kattha")] public double? OwnLandKattha { get; set; }
    [Column("others_land_kattha")] public double? OthersLandKattha { get; set; }
    [Column("other_ownership_kattha")] public double? OtherOwnershipKattha { get; set; }
    [Column("khet_area_kattha")] public double? KhetAreaKattha { get; set; }
    [Column("bari_area_kattha")] public double? BariAreaKattha { get; set; }
    [Column("fallow_land_kattha")] public double? FallowLandKattha { get; set; }
    [Column("irrigation_status")] public string? IrrigationStatus { get; set; }
    [Column("irrigation_medium_summary")] public string? IrrigationMediumSummary { get; set; }
    [Column("irrigation_canal")] public short? IrrigationCanal { get; set; }
    [Column("irrigation_tubewell")] public short? IrrigationTubewell { get; set; }
    [Column("irrigation_electric")] public short? IrrigationElectric { get; set; }
    [Column("farming_method")] public string? FarmingMethod { get; set; }

    // 9.1: Cereals
    [Column("has_cereal_crops")] public string? HasCerealCrops { get; set; }
    [Column("cereal_crop_summary")] public string? CerealCropSummary { get; set; }
    [Column("cereal_rice")] public short? CerealRice { get; set; }
    [Column("cereal_maize")] public short? CerealMaize { get; set; }
    [Column("cereal_wheat")] public short? CerealWheat { get; set; }
    [Column("cereal_barley")] public short? CerealBarley { get; set; }

    // Rice
    [Column("rice_area")] public double? RiceArea { get; set; }
    [Column("rice_production_kg")] public double? RiceProductionKg { get; set; }
    [Column("rice_sales_rupees")] public double? RiceSalesRupees { get; set; }
    [Column("rice_deficit_rupees")] public double? RiceDeficitRupees { get; set; }
    // Maize
    [Column("maize_area")] public double? MaizeArea { get; set; }
    [Column("maize_production_kg")] public double? MaizeProductionKg { get; set; }
    [Column("maize_sales_rupees")] public double? MaizeSalesRupees { get; set; }
    [Column("maize_deficit_rupees")] public double? MaizeDeficitRupees { get; set; }
    // Wheat
    [Column("wheat_area")] public double? WheatArea { get; set; }
    [Column("wheat_production_kg")] public double? WheatProductionKg { get; set; }
    [Column("wheat_sales_rupees")] public double? WheatSalesRupees { get; set; }
    [Column("wheat_deficit_rupees")] public double? WheatDeficitRupees { get; set; }
    // Barley
    [Column("barley_area")] public double? BarleyArea { get; set; }
    [Column("barley_production_kg")] public double? BarleyProductionKg { get; set; }
    [Column("barley_sales_rupees")] public double? BarleySalesRupees { get; set; }
    [Column("barley_deficit_rupees")] public double? BarleyDeficitRupees { get; set; }

    // 9.2: Pulses
    [Column("has_pulse_crops")] public string? HasPulseCrops { get; set; }
    [Column("pulse_crop_summary")] public string? PulseCropSummary { get; set; }
    [Column("pulse_black_dal")] public short? PulseBlackDal { get; set; }
    [Column("pulse_rahar")] public short? PulseRahar { get; set; }
    [Column("pulse_masuro")] public short? PulseMasuro { get; set; }
    [Column("pulse_chana")] public short? PulseChana { get; set; }
    [Column("pulse_simi")] public short? PulseSimi { get; set; }
    [Column("pulse_bhattmas")] public short? PulseBhattmas { get; set; }
    [Column("pulse_mung")] public short? PulseMung { get; set; }
    [Column("pulse_khesari")] public short? PulseKhesari { get; set; }

    // Pulse production 1-8
    [Column("pulse_1_area")] public double? Pulse1Area { get; set; }
    [Column("pulse_1_production_kg")] public double? Pulse1ProductionKg { get; set; }
    [Column("pulse_1_sales_rupees")] public double? Pulse1SalesRupees { get; set; }
    [Column("pulse_1_deficit_rupees")] public double? Pulse1DeficitRupees { get; set; }
    [Column("pulse_2_area")] public double? Pulse2Area { get; set; }
    [Column("pulse_2_production_kg")] public double? Pulse2ProductionKg { get; set; }
    [Column("pulse_2_sales_rupees")] public double? Pulse2SalesRupees { get; set; }
    [Column("pulse_2_deficit_rupees")] public double? Pulse2DeficitRupees { get; set; }
    [Column("pulse_3_area")] public double? Pulse3Area { get; set; }
    [Column("pulse_3_production_kg")] public double? Pulse3ProductionKg { get; set; }
    [Column("pulse_3_sales_rupees")] public double? Pulse3SalesRupees { get; set; }
    [Column("pulse_3_deficit_rupees")] public double? Pulse3DeficitRupees { get; set; }
    [Column("pulse_4_area")] public double? Pulse4Area { get; set; }
    [Column("pulse_4_production_kg")] public double? Pulse4ProductionKg { get; set; }
    [Column("pulse_4_sales_rupees")] public double? Pulse4SalesRupees { get; set; }
    [Column("pulse_4_deficit_rupees")] public double? Pulse4DeficitRupees { get; set; }
    [Column("pulse_5_area")] public double? Pulse5Area { get; set; }
    [Column("pulse_5_production_kg")] public double? Pulse5ProductionKg { get; set; }
    [Column("pulse_5_sales_rupees")] public double? Pulse5SalesRupees { get; set; }
    [Column("pulse_5_deficit_rupees")] public double? Pulse5DeficitRupees { get; set; }
    [Column("pulse_6_area")] public double? Pulse6Area { get; set; }
    [Column("pulse_6_production_kg")] public double? Pulse6ProductionKg { get; set; }
    [Column("pulse_6_sales_rupees")] public double? Pulse6SalesRupees { get; set; }
    [Column("pulse_6_deficit_rupees")] public double? Pulse6DeficitRupees { get; set; }
    [Column("pulse_7_area")] public double? Pulse7Area { get; set; }
    [Column("pulse_7_production_kg")] public double? Pulse7ProductionKg { get; set; }
    [Column("pulse_7_sales_rupees")] public double? Pulse7SalesRupees { get; set; }
    [Column("pulse_7_deficit_rupees")] public double? Pulse7DeficitRupees { get; set; }
    [Column("pulse_8_area")] public double? Pulse8Area { get; set; }
    [Column("pulse_8_production_kg")] public double? Pulse8ProductionKg { get; set; }
    [Column("pulse_8_sales_rupees")] public double? Pulse8SalesRupees { get; set; }
    [Column("pulse_8_deficit_rupees")] public double? Pulse8DeficitRupees { get; set; }

    // 9.3: Oilseeds
    [Column("has_oilseed_crops")] public string? HasOilseedCrops { get; set; }
    [Column("oilseed_crop_summary")] public string? OilseedCropSummary { get; set; }
    [Column("oilseed_tori")] public short? OilseedTori { get; set; }
    [Column("oilseed_til")] public short? OilseedTil { get; set; }
    [Column("oilseed_sunflower")] public short? OilseedSunflower { get; set; }
    [Column("oilseed_other")] public short? OilseedOther { get; set; }
    [Column("oilseed_1_area")] public double? Oilseed1Area { get; set; }
    [Column("oilseed_1_production_kg")] public double? Oilseed1ProductionKg { get; set; }
    [Column("oilseed_1_sales_rupees")] public double? Oilseed1SalesRupees { get; set; }
    [Column("oilseed_1_deficit_rupees")] public double? Oilseed1DeficitRupees { get; set; }
    [Column("oilseed_2_area")] public double? Oilseed2Area { get; set; }
    [Column("oilseed_2_production_kg")] public double? Oilseed2ProductionKg { get; set; }
    [Column("oilseed_2_sales_rupees")] public double? Oilseed2SalesRupees { get; set; }
    [Column("oilseed_2_deficit_rupees")] public double? Oilseed2DeficitRupees { get; set; }
    [Column("oilseed_3_area")] public double? Oilseed3Area { get; set; }
    [Column("oilseed_3_production_kg")] public double? Oilseed3ProductionKg { get; set; }
    [Column("oilseed_3_sales_rupees")] public double? Oilseed3SalesRupees { get; set; }
    [Column("oilseed_3_deficit_rupees")] public double? Oilseed3DeficitRupees { get; set; }
    [Column("oilseed_4_area")] public double? Oilseed4Area { get; set; }
    [Column("oilseed_4_production_kg")] public double? Oilseed4ProductionKg { get; set; }
    [Column("oilseed_4_sales_rupees")] public double? Oilseed4SalesRupees { get; set; }
    [Column("oilseed_4_deficit_rupees")] public double? Oilseed4DeficitRupees { get; set; }

    // 9.4: Cash crops
    [Column("has_cash_crops")] public string? HasCashCrops { get; set; }
    [Column("cash_crop_summary")] public string? CashCropSummary { get; set; }
    [Column("cash_ginger_turmeric")] public short? CashGingerTurmeric { get; set; }
    [Column("cash_onion_garlic")] public short? CashOnionGarlic { get; set; }
    [Column("cash_mushroom")] public short? CashMushroom { get; set; }
    [Column("cash_sugarcane")] public short? CashSugarcane { get; set; }
    [Column("cash_chili")] public short? CashChili { get; set; }
    [Column("cash_peanut")] public short? CashPeanut { get; set; }
    [Column("cash_1_area")] public double? Cash1Area { get; set; }
    [Column("cash_1_production_kg")] public double? Cash1ProductionKg { get; set; }
    [Column("cash_1_sales_rupees")] public double? Cash1SalesRupees { get; set; }
    [Column("cash_1_deficit_rupees")] public double? Cash1DeficitRupees { get; set; }
    [Column("cash_2_area")] public double? Cash2Area { get; set; }
    [Column("cash_2_production_kg")] public double? Cash2ProductionKg { get; set; }
    [Column("cash_2_sales_rupees")] public double? Cash2SalesRupees { get; set; }
    [Column("cash_2_deficit_rupees")] public double? Cash2DeficitRupees { get; set; }
    [Column("cash_3_area")] public double? Cash3Area { get; set; }
    [Column("cash_3_production_kg")] public double? Cash3ProductionKg { get; set; }
    [Column("cash_3_sales_rupees")] public double? Cash3SalesRupees { get; set; }
    [Column("cash_3_deficit_rupees")] public double? Cash3DeficitRupees { get; set; }
    [Column("cash_4_area")] public double? Cash4Area { get; set; }
    [Column("cash_4_production_kg")] public double? Cash4ProductionKg { get; set; }
    [Column("cash_4_sales_rupees")] public double? Cash4SalesRupees { get; set; }
    [Column("cash_4_deficit_rupees")] public double? Cash4DeficitRupees { get; set; }
    [Column("cash_5_area")] public double? Cash5Area { get; set; }
    [Column("cash_5_production_kg")] public double? Cash5ProductionKg { get; set; }
    [Column("cash_5_sales_rupees")] public double? Cash5SalesRupees { get; set; }
    [Column("cash_5_deficit_rupees")] public double? Cash5DeficitRupees { get; set; }
    [Column("cash_6_area")] public double? Cash6Area { get; set; }
    [Column("cash_6_production_kg")] public double? Cash6ProductionKg { get; set; }
    [Column("cash_6_sales_rupees")] public double? Cash6SalesRupees { get; set; }
    [Column("cash_6_deficit_rupees")] public double? Cash6DeficitRupees { get; set; }

    // 9.5: Fruits
    [Column("has_fruit_crops")] public string? HasFruitCrops { get; set; }
    [Column("fruit_crop_summary")] public string? FruitCropSummary { get; set; }
    [Column("fruit_mango")] public short? FruitMango { get; set; }
    [Column("fruit_lemon")] public short? FruitLemon { get; set; }
    [Column("fruit_pomelo")] public short? FruitPomelo { get; set; }
    [Column("fruit_pomegranate")] public short? FruitPomegranate { get; set; }
    [Column("fruit_banana")] public short? FruitBanana { get; set; }
    [Column("fruit_amba")] public short? FruitAmba { get; set; }
    [Column("fruit_lichi")] public short? FruitLichi { get; set; }
    [Column("fruit_sarifa")] public short? FruitSarifa { get; set; }
    [Column("fruit_1_area")] public double? Fruit1Area { get; set; }
    [Column("fruit_1_production_kg")] public double? Fruit1ProductionKg { get; set; }
    [Column("fruit_1_sales_rupees")] public double? Fruit1SalesRupees { get; set; }
    [Column("fruit_1_deficit_rupees")] public double? Fruit1DeficitRupees { get; set; }
    [Column("fruit_2_area")] public double? Fruit2Area { get; set; }
    [Column("fruit_2_production_kg")] public double? Fruit2ProductionKg { get; set; }
    [Column("fruit_2_sales_rupees")] public double? Fruit2SalesRupees { get; set; }
    [Column("fruit_2_deficit_rupees")] public double? Fruit2DeficitRupees { get; set; }
    [Column("fruit_3_area")] public double? Fruit3Area { get; set; }
    [Column("fruit_3_production_kg")] public double? Fruit3ProductionKg { get; set; }
    [Column("fruit_3_sales_rupees")] public double? Fruit3SalesRupees { get; set; }
    [Column("fruit_3_deficit_rupees")] public double? Fruit3DeficitRupees { get; set; }
    [Column("fruit_4_area")] public double? Fruit4Area { get; set; }
    [Column("fruit_4_production_kg")] public double? Fruit4ProductionKg { get; set; }
    [Column("fruit_4_sales_rupees")] public double? Fruit4SalesRupees { get; set; }
    [Column("fruit_4_deficit_rupees")] public double? Fruit4DeficitRupees { get; set; }
    [Column("fruit_5_area")] public double? Fruit5Area { get; set; }
    [Column("fruit_5_production_kg")] public double? Fruit5ProductionKg { get; set; }
    [Column("fruit_5_sales_rupees")] public double? Fruit5SalesRupees { get; set; }
    [Column("fruit_5_deficit_rupees")] public double? Fruit5DeficitRupees { get; set; }
    [Column("fruit_6_area")] public double? Fruit6Area { get; set; }
    [Column("fruit_6_production_kg")] public double? Fruit6ProductionKg { get; set; }
    [Column("fruit_6_sales_rupees")] public double? Fruit6SalesRupees { get; set; }
    [Column("fruit_6_deficit_rupees")] public double? Fruit6DeficitRupees { get; set; }
    [Column("fruit_7_area")] public double? Fruit7Area { get; set; }
    [Column("fruit_7_production_kg")] public double? Fruit7ProductionKg { get; set; }
    [Column("fruit_7_sales_rupees")] public double? Fruit7SalesRupees { get; set; }
    [Column("fruit_7_deficit_rupees")] public double? Fruit7DeficitRupees { get; set; }
    [Column("fruit_8_area")] public double? Fruit8Area { get; set; }
    [Column("fruit_8_production_kg")] public double? Fruit8ProductionKg { get; set; }
    [Column("fruit_8_sales_rupees")] public double? Fruit8SalesRupees { get; set; }
    [Column("fruit_8_deficit_rupees")] public double? Fruit8DeficitRupees { get; set; }

    // 9.6: Herbs (3 metrics: area, production, sales — no deficit)
    [Column("has_herb_crops")] public string? HasHerbCrops { get; set; }
    [Column("herb_crop_summary")] public string? HerbCropSummary { get; set; }
    [Column("herb_harro_barro")] public short? HerbHarroBarro { get; set; }
    [Column("herb_gurjo")] public short? HerbGurjo { get; set; }
    [Column("herb_tulsi")] public short? HerbTulsi { get; set; }
    [Column("herb_amla")] public short? HerbAmla { get; set; }
    [Column("herb_sarpagandha")] public short? HerbSarpagandha { get; set; }
    [Column("herb_ajwain")] public short? HerbAjwain { get; set; }
    [Column("herb_sutli")] public short? HerbSutli { get; set; }
    [Column("herb_aloe_vera")] public short? HerbAloeVera { get; set; }
    [Column("herb_bojho")] public short? HerbBojho { get; set; }
    [Column("herb_1_area")] public double? Herb1Area { get; set; }
    [Column("herb_1_production_kg")] public double? Herb1ProductionKg { get; set; }
    [Column("herb_1_sales_rupees")] public double? Herb1SalesRupees { get; set; }
    [Column("herb_2_area")] public double? Herb2Area { get; set; }
    [Column("herb_2_production_kg")] public double? Herb2ProductionKg { get; set; }
    [Column("herb_2_sales_rupees")] public double? Herb2SalesRupees { get; set; }
    [Column("herb_3_area")] public double? Herb3Area { get; set; }
    [Column("herb_3_production_kg")] public double? Herb3ProductionKg { get; set; }
    [Column("herb_3_sales_rupees")] public double? Herb3SalesRupees { get; set; }
    [Column("herb_4_area")] public double? Herb4Area { get; set; }
    [Column("herb_4_production_kg")] public double? Herb4ProductionKg { get; set; }
    [Column("herb_4_sales_rupees")] public double? Herb4SalesRupees { get; set; }
    [Column("herb_5_area")] public double? Herb5Area { get; set; }
    [Column("herb_5_production_kg")] public double? Herb5ProductionKg { get; set; }
    [Column("herb_5_sales_rupees")] public double? Herb5SalesRupees { get; set; }
    [Column("herb_6_area")] public double? Herb6Area { get; set; }
    [Column("herb_6_production_kg")] public double? Herb6ProductionKg { get; set; }
    [Column("herb_6_sales_rupees")] public double? Herb6SalesRupees { get; set; }
    [Column("herb_7_area")] public double? Herb7Area { get; set; }
    [Column("herb_7_production_kg")] public double? Herb7ProductionKg { get; set; }
    [Column("herb_7_sales_rupees")] public double? Herb7SalesRupees { get; set; }
    [Column("herb_8_area")] public double? Herb8Area { get; set; }
    [Column("herb_8_production_kg")] public double? Herb8ProductionKg { get; set; }
    [Column("herb_8_sales_rupees")] public double? Herb8SalesRupees { get; set; }
    [Column("herb_9_area")] public double? Herb9Area { get; set; }
    [Column("herb_9_production_kg")] public double? Herb9ProductionKg { get; set; }
    [Column("herb_9_sales_rupees")] public double? Herb9SalesRupees { get; set; }

    // 10: Livestock
    [Column("has_livestock")] public string? HasLivestock { get; set; }

    // 10.1: Dairy
    [Column("dairy_summary")] public string? DairySummary { get; set; }
    [Column("dairy_cow")] public short? DairyCow { get; set; }
    [Column("dairy_buffalo")] public short? DairyBuffalo { get; set; }
    [Column("dairy_other")] public short? DairyOther { get; set; }
    // Cow
    [Column("cow_count")] public double? CowCount { get; set; }
    [Column("cow_milk_daily")] public double? CowMilkDaily { get; set; }
    [Column("cow_milk_sold")] public double? CowMilkSold { get; set; }
    [Column("cow_curd_sold")] public double? CowCurdSold { get; set; }
    [Column("cow_annual_income")] public double? CowAnnualIncome { get; set; }
    // Buffalo
    [Column("buffalo_count")] public double? BuffaloCount { get; set; }
    [Column("buffalo_milk_daily")] public double? BuffaloMilkDaily { get; set; }
    [Column("buffalo_milk_sold")] public double? BuffaloMilkSold { get; set; }
    [Column("buffalo_curd_sold")] public double? BuffaloCurdSold { get; set; }
    [Column("buffalo_annual_income")] public double? BuffaloAnnualIncome { get; set; }
    // Other dairy
    [Column("other_dairy_count")] public double? OtherDairyCount { get; set; }
    [Column("other_dairy_milk_daily")] public double? OtherDairyMilkDaily { get; set; }
    [Column("other_dairy_milk_sold")] public double? OtherDairyMilkSold { get; set; }
    [Column("other_dairy_curd_sold")] public double? OtherDairyCurdSold { get; set; }
    [Column("other_dairy_annual_income")] public double? OtherDairyAnnualIncome { get; set; }

    // 10.2: Meat livestock
    [Column("meat_summary")] public string? MeatSummary { get; set; }
    [Column("meat_goat_sheep")] public short? MeatGoatSheep { get; set; }
    [Column("meat_calf")] public short? MeatCalf { get; set; }
    [Column("meat_pig")] public short? MeatPig { get; set; }
    [Column("meat_other")] public short? MeatOther { get; set; }
    // Goat/sheep
    [Column("goat_count")] public double? GoatCount { get; set; }
    [Column("goat_annual_sold")] public double? GoatAnnualSold { get; set; }
    [Column("goat_annual_income")] public double? GoatAnnualIncome { get; set; }
    // Calf
    [Column("calf_count")] public double? CalfCount { get; set; }
    [Column("calf_annual_sold")] public double? CalfAnnualSold { get; set; }
    [Column("calf_annual_income")] public double? CalfAnnualIncome { get; set; }
    // Pig
    [Column("pig_count")] public double? PigCount { get; set; }
    [Column("pig_annual_sold")] public double? PigAnnualSold { get; set; }
    [Column("pig_annual_income")] public double? PigAnnualIncome { get; set; }
    // Other meat
    [Column("other_meat_count")] public double? OtherMeatCount { get; set; }
    [Column("other_meat_annual_sold")] public double? OtherMeatAnnualSold { get; set; }
    [Column("other_meat_annual_income")] public double? OtherMeatAnnualIncome { get; set; }

    // 11: Poultry
    [Column("poultry_summary")] public string? PoultrySummary { get; set; }
    [Column("poultry_chicken")] public short? PoultryChicken { get; set; }
    [Column("poultry_duck")] public short? PoultryDuck { get; set; }
    [Column("poultry_pigeon")] public short? PoultryPigeon { get; set; }
    [Column("poultry_battai")] public short? PoultryBattai { get; set; }
    [Column("poultry_count")] public double? PoultryCount { get; set; }
    [Column("poultry_egg_annual")] public double? PoultryEggAnnual { get; set; }
    [Column("poultry_meat_sold_kg")] public double? PoultryMeatSoldKg { get; set; }
    [Column("poultry_annual_income")] public double? PoultryAnnualIncome { get; set; }

    // 12: Fish farming
    [Column("has_fish_farming")] public string? HasFishFarming { get; set; }
    [Column("fish_pond_count")] public double? FishPondCount { get; set; }
    [Column("fish_pond_area")] public double? FishPondArea { get; set; }
    [Column("fish_production_kg")] public double? FishProductionKg { get; set; }
    [Column("fish_annual_income")] public double? FishAnnualIncome { get; set; }
    [Column("fish_subsidy")] public string? FishSubsidy { get; set; }

    // 13: Beekeeping
    [Column("has_beekeeping")] public string? HasBeekeeping { get; set; }
    [Column("bee_hive_count")] public double? BeeHiveCount { get; set; }
    [Column("honey_production_kg")] public double? HoneyProductionKg { get; set; }
    [Column("honey_annual_income")] public double? HoneyAnnualIncome { get; set; }
    [Column("bee_subsidy")] public string? BeeSubsidy { get; set; }

    // 14: Income
    [Column("income_vegetables")] public double? IncomeVegetables { get; set; }
    [Column("income_labor")] public double? IncomeLabor { get; set; }
    [Column("income_trade")] public double? IncomeTrade { get; set; }
    [Column("income_foreign")] public double? IncomeForeign { get; set; }
    [Column("income_job")] public double? IncomeJob { get; set; }
    [Column("income_other")] public double? IncomeOther { get; set; }

    // 15: Expenditure
    [Column("expense_food")] public double? ExpenseFood { get; set; }
    [Column("expense_clothes")] public double? ExpenseClothes { get; set; }
    [Column("expense_education")] public double? ExpenseEducation { get; set; }
    [Column("expense_health")] public double? ExpenseHealth { get; set; }
    [Column("expense_festivals")] public double? ExpenseFestivals { get; set; }
    [Column("expense_agriculture")] public double? ExpenseAgriculture { get; set; }
    [Column("expense_fuel")] public double? ExpenseFuel { get; set; }
    [Column("expense_mobile_internet")] public double? ExpenseMobileInternet { get; set; }
    [Column("food_sufficiency_months")] public string? FoodSufficiencyMonths { get; set; }

    // 15.10: Deficit spending
    [Column("deficit_spending_summary")] public string? DeficitSpendingSummary { get; set; }
    [Column("deficit_job")] public short? DeficitJob { get; set; }
    [Column("deficit_daily_wage")] public short? DeficitDailyWage { get; set; }
    [Column("deficit_foreign_job")] public short? DeficitForeignJob { get; set; }
    [Column("deficit_trade")] public short? DeficitTrade { get; set; }
    [Column("deficit_loan")] public short? DeficitLoan { get; set; }

    // 16: Banking
    [Column("has_bank_account")] public string? HasBankAccount { get; set; }
    [Column("bank_account_summary")] public string? BankAccountSummary { get; set; }
    [Column("bank_account_bank")] public short? BankAccountBank { get; set; }
    [Column("bank_account_coop")] public short? BankAccountCoop { get; set; }
    [Column("bank_account_savings")] public short? BankAccountSavings { get; set; }
    [Column("bank_account_moneylender")] public short? BankAccountMoneylender { get; set; }
    [Column("bank_account_other")] public short? BankAccountOther { get; set; }
    [Column("has_loan")] public string? HasLoan { get; set; }
    [Column("loan_amount")] public string? LoanAmount { get; set; }
    [Column("loan_source_summary")] public string? LoanSourceSummary { get; set; }
    [Column("loan_from_bank")] public short? LoanFromBank { get; set; }
    [Column("loan_from_coop")] public short? LoanFromCoop { get; set; }
    [Column("loan_from_moneylender")] public short? LoanFromMoneylender { get; set; }
    [Column("loan_from_relatives")] public short? LoanFromRelatives { get; set; }
    [Column("loan_type")] public string? LoanType { get; set; }
    [Column("loan_purpose_summary")] public string? LoanPurposeSummary { get; set; }
    [Column("loan_purpose_business")] public short? LoanPurposeBusiness { get; set; }
    [Column("loan_purpose_house")] public short? LoanPurposeHouse { get; set; }
    [Column("loan_purpose_medical")] public short? LoanPurposeMedical { get; set; }
    [Column("loan_purpose_education")] public short? LoanPurposeEducation { get; set; }
    [Column("loan_purpose_other")] public short? LoanPurposeOther { get; set; }
    [Column("loan_purpose_other_detail")] public string? LoanPurposeOtherDetail { get; set; }

    // 17-18: Housing & disaster
    [Column("house_info_note")] public string? HouseInfoNote { get; set; }
    [Column("house_count")] public double? HouseCount { get; set; }
    [Column("earthquake_resistant")] public string? EarthquakeResistant { get; set; }
    [Column("disaster_damaged")] public string? DisasterDamaged { get; set; }
    [Column("received_subsidy")] public string? ReceivedSubsidy { get; set; }
    [Column("subsidy_installments")] public string? SubsidyInstallments { get; set; }
    [Column("house_rebuilt")] public string? HouseRebuilt { get; set; }
    [Column("has_family_risk_plan")] public string? HasFamilyRiskPlan { get; set; }
    [Column("disaster_risk")] public string? DisasterRisk { get; set; }
    [Column("disaster_risk_summary")] public string? DisasterRiskSummary { get; set; }
    [Column("risk_earthquake")] public short? RiskEarthquake { get; set; }
    [Column("risk_flood")] public short? RiskFlood { get; set; }
    [Column("risk_landslide")] public short? RiskLandslide { get; set; }
    [Column("risk_lightning")] public short? RiskLightning { get; set; }
    [Column("risk_storm")] public short? RiskStorm { get; set; }
    [Column("risk_cold_wave")] public short? RiskColdWave { get; set; }
    [Column("risk_inundation")] public short? RiskInundation { get; set; }

    // 19-20: Photo & GPS
    [Column("photo")] public string? Photo { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }

    // Closing & extras
    [Column("closing_note")] public string? ClosingNote { get; set; }
    [Column("distance_from_center_2")] public string? DistanceFromCenter2 { get; set; }
    [Column("ghee_sold")] public double? GheeSold { get; set; }
    [Column("ghee_sold_2")] public double? GheeSold2 { get; set; }
    [Column("ghee_sold_3")] public double? GheeSold3 { get; set; }

    [Column("created_at")] public string? CreatedAt { get; set; }

    // Navigation
    public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
    public ICollection<HouseDetail> HouseDetails { get; set; } = new List<HouseDetail>();
    public ICollection<HhDeathRecord> DeathRecords { get; set; } = new List<HhDeathRecord>();
}
