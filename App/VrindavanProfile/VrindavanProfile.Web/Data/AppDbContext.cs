using Microsoft.EntityFrameworkCore;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Services;

namespace VrindavanProfile.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Institutional survey tables
    public DbSet<SurveyEntry> SurveyEntries { get; set; }
    public DbSet<EducationalInstitution> EducationalInstitutions { get; set; }
    public DbSet<HealthInstitution> HealthInstitutions { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<SecurityBody> SecurityBodies { get; set; }
    public DbSet<FinancialInstitution> FinancialInstitutions { get; set; }
    public DbSet<HotelResort> HotelResorts { get; set; }
    public DbSet<Bridge> Bridges { get; set; }
    public DbSet<Industry> Industries { get; set; }
    public DbSet<NgoClubGroup> NgoClubGroups { get; set; }
    public DbSet<LandUse> LandUses { get; set; }
    public DbSet<NaturalResource> NaturalResources { get; set; }
    public DbSet<CulturalHeritage> CulturalHeritages { get; set; }
    public DbSet<TradeBusiness> TradeBusinesses { get; set; }
    public DbSet<Forest> Forests { get; set; }
    public DbSet<Road> Roads { get; set; }
    public DbSet<WardStatistics> WardStatistics { get; set; }

    // Household survey tables
    public DbSet<Household> Households { get; set; }
    public DbSet<FamilyMember> FamilyMembers { get; set; }
    public DbSet<HouseDetail> HouseDetails { get; set; }
    public DbSet<HhDeathRecord> HhDeathRecords { get; set; }
    
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Institutional relationships (1:1)
        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.EducationalInstitution)
            .WithOne(ei => ei.SurveyEntry)
            .HasForeignKey<EducationalInstitution>(ei => ei.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.HealthInstitution)
            .WithOne(hi => hi.SurveyEntry)
            .HasForeignKey<HealthInstitution>(hi => hi.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.Building)
            .WithOne(b => b.SurveyEntry)
            .HasForeignKey<Building>(b => b.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.SecurityBody)
            .WithOne(sb => sb.SurveyEntry)
            .HasForeignKey<SecurityBody>(sb => sb.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.FinancialInstitution)
            .WithOne(fi => fi.SurveyEntry)
            .HasForeignKey<FinancialInstitution>(fi => fi.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.HotelResort)
            .WithOne(hr => hr.SurveyEntry)
            .HasForeignKey<HotelResort>(hr => hr.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.Bridge)
            .WithOne(br => br.SurveyEntry)
            .HasForeignKey<Bridge>(br => br.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.Industry)
            .WithOne(ind => ind.SurveyEntry)
            .HasForeignKey<Industry>(ind => ind.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.NgoClubGroup)
            .WithOne(ngo => ngo.SurveyEntry)
            .HasForeignKey<NgoClubGroup>(ngo => ngo.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.LandUse)
            .WithOne(lu => lu.SurveyEntry)
            .HasForeignKey<LandUse>(lu => lu.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.NaturalResource)
            .WithOne(nr => nr.SurveyEntry)
            .HasForeignKey<NaturalResource>(nr => nr.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.CulturalHeritage)
            .WithOne(ch => ch.SurveyEntry)
            .HasForeignKey<CulturalHeritage>(ch => ch.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.TradeBusiness)
            .WithOne(tb => tb.SurveyEntry)
            .HasForeignKey<TradeBusiness>(tb => tb.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.Forest)
            .WithOne(f => f.SurveyEntry)
            .HasForeignKey<Forest>(f => f.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.Road)
            .WithOne(r => r.SurveyEntry)
            .HasForeignKey<Road>(r => r.SurveyEntryId);

        modelBuilder.Entity<SurveyEntry>()
            .HasOne(se => se.WardStatistics)
            .WithOne(ws => ws.SurveyEntry)
            .HasForeignKey<WardStatistics>(ws => ws.SurveyEntryId);

        // Household relationships (1:N)
        modelBuilder.Entity<Household>()
            .HasMany(h => h.FamilyMembers)
            .WithOne(fm => fm.Household)
            .HasForeignKey(fm => fm.HouseholdId);

        modelBuilder.Entity<Household>()
            .HasMany(h => h.HouseDetails)
            .WithOne(hd => hd.Household)
            .HasForeignKey(hd => hd.HouseholdId);

        modelBuilder.Entity<Household>()
            .HasMany(h => h.DeathRecords)
            .WithOne(dr => dr.Household)
            .HasForeignKey(dr => dr.HouseholdId);
    }

    public override int SaveChanges()
    {
        UpdateAgesFromDob();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAgesFromDob();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAgesFromDob()
    {
        var entries = ChangeTracker.Entries<FamilyMember>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var fm = entry.Entity;
            if (!string.IsNullOrWhiteSpace(fm.DobBs))
            {
                var calculatedAge = NepaliDateHelper.CalculateAgeBs(fm.DobBs);
                if (calculatedAge.HasValue)
                {
                    fm.Age = calculatedAge.Value;
                }
            }
        }
    }
}
