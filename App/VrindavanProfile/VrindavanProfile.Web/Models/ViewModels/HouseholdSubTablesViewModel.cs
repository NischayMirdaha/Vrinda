using System.Collections.Generic;
using VrindavanProfile.Web.Data.DbEntities;
namespace VrindavanProfile.Web.Models.ViewModels;

public class HouseholdSubTablesViewModel
{
    public long HouseholdId { get; set; }
    public List<FamilyMember> FamilyMembers { get; set; } = new();
    public List<HouseDetail> HouseDetails { get; set; } = new();
    public List<HhDeathRecord> DeathRecords { get; set; } = new();
}