using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;

[Table("forest")]
public class Forest
{
    [Column("id")] public long Id { get; set; }
    [Column("survey_entry_id")] public long SurveyEntryId { get; set; }
    [Column("forest_name")] public string? ForestName { get; set; }
    [Column("forest_type")] public string? ForestType { get; set; }
    [Column("area_in_ward")] public string? AreaInWard { get; set; }
    [Column("associated_households")] public double? AssociatedHouseholds { get; set; }
    [Column("main_plants")] public string? MainPlants { get; set; }
    [Column("main_animals")] public string? MainAnimals { get; set; }
    [Column("herbs_medicinal")] public string? HerbsMedicinal { get; set; }
    [Column("annual_income")] public double? AnnualIncome { get; set; }
    [Column("timber_production_mt")] public double? TimberProductionMt { get; set; }
    [Column("non_timber_production_mt")] public double? NonTimberProductionMt { get; set; }
    [Column("geo_point")] public string? GeoPoint { get; set; }
    [Column("latitude")] public double? Latitude { get; set; }
    [Column("longitude")] public double? Longitude { get; set; }
    [Column("altitude")] public double? Altitude { get; set; }
    [Column("gps_precision")] public double? GpsPrecision { get; set; }

    public SurveyEntry SurveyEntry { get; set; } = null!;
}
