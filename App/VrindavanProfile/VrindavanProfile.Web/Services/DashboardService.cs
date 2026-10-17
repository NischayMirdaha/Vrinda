using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Caching;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public DashboardService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // ─── Aggregate methods ─────────────────────────────

    public async Task<DashboardViewModel> GetDashboardDataAsync(int? wardNo = null)
    {
        // Stat counters and charts are cached individually — just compose them.
        return new DashboardViewModel
        {
            WardNo = wardNo,
            TotalWards = (await GetWardNumbersAsync()).Count,
            StatCounters = await GetStatCountersAsync(wardNo),
            Charts = await GetAllChartsAsync(wardNo)
        };
    }

    public async Task<List<StatCounter>> GetStatCountersAsync(int? wardNo = null)
    {
        var key = CacheKeys.DashboardStats(wardNo);

        if (_cache.TryGetValue(key, out List<StatCounter>? cached))
            return cached!;

        var result = await BuildStatCountersAsync(wardNo);

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.SurveyDataDuration,
            Size = 1
        });

        return result;
    }

    public async Task<List<ChartData>> GetAllChartsAsync(int? wardNo = null)
    {
        var key = CacheKeys.DashboardCharts(wardNo);

        if (_cache.TryGetValue(key, out List<ChartData>? cached))
            return cached!;

        var result = await BuildAllChartsAsync(wardNo);

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.SurveyDataDuration,
            Size = 2 // charts are heavier
        });

        return result;
    }

    public async Task<List<int>> GetWardNumbersAsync()
    {
        if (_cache.TryGetValue(CacheKeys.WardNumbers, out List<int>? cached))
            return cached!;

        var result = await _db.Households
            .Where(h => h.WardNo != null)
            .Select(h => h.WardNo!.Value)
            .Distinct()
            .OrderBy(w => w)
            .ToListAsync();

        _cache.Set(CacheKeys.WardNumbers, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.MetadataDuration,
            Size = 1
        });

        return result;
    }

    // ─── Base query helpers (ward filter) ───────────────

    private IQueryable<Household> BaseHouseholdQuery(int? wardNo)
    {
        var q = _db.Households.AsNoTracking().Where(h => h.WardNo != null);
        return wardNo.HasValue ? q.Where(h => h.WardNo == wardNo.Value) : q;
    }

    private IQueryable<FamilyMember> BaseFamilyMemberQuery(int? wardNo)
    {
        var q = _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Household != null && fm.Household.WardNo != null);
        return wardNo.HasValue
            ? q.Where(fm => fm.Household!.WardNo == wardNo.Value)
            : q;
    }

    private IQueryable<HouseDetail> BaseHouseDetailQuery(int? wardNo)
    {
        var q = _db.HouseDetails.AsNoTracking()
            .Where(hd => hd.Household != null && hd.Household.WardNo != null);
        return wardNo.HasValue
            ? q.Where(hd => hd.Household!.WardNo == wardNo.Value)
            : q;
    }

    private IQueryable<HhDeathRecord> BaseDeathRecordQuery(int? wardNo)
    {
        var q = _db.HhDeathRecords.AsNoTracking()
            .Where(dr => dr.Household != null && dr.Household.WardNo != null);
        return wardNo.HasValue
            ? q.Where(dr => dr.Household!.WardNo == wardNo.Value)
            : q;
    }

    // Helper: build ChartData from a list of items
    private static ChartData MakeChart(
        string id, string title, string type, string category,
        int sortOrder, string? link, List<ChartItem> items,
        string? titleEn = null, string? categoryEn = null)
    {
        return new ChartData
        {
            Id = id,
            Title = title,
            TitleEn = titleEn,
            ChartType = type,
            Category = category,
            CategoryEn = categoryEn,
            SortOrder = sortOrder,
            DetailLink = link,
            Items = items
        };
    }

    /// <summary>
    /// Raw stat-counter computation (no cache). Wraps multiple COUNT queries
    /// into a single database round-trip via raw SQL when possible.
    /// </summary>
    private async Task<List<StatCounter>> BuildStatCountersAsync(int? wardNo)
    {
        var hhQuery = BaseHouseholdQuery(wardNo);
        var fmQuery = BaseFamilyMemberQuery(wardNo);

        // Perform COUNT queries sequentially (no Task.WhenAll) to avoid
        // "A second operation was started on this context instance before a previous operation completed"
        // DbContext is NOT thread-safe. This was the root cause of the Home/dashboard crash.
        var totalHH = await hhQuery.CountAsync();
        var totalPop = await fmQuery.CountAsync();
        var maleCount = await fmQuery
            .CountAsync(fm => fm.Gender == "पुरूष" || fm.Gender == "Male" || fm.Gender == "male");
        var femaleCount = await fmQuery
            .CountAsync(fm => fm.Gender == "महिला" || fm.Gender == "Female" || fm.Gender == "female");
        var disabledCount = await fmQuery
            .CountAsync(fm => fm.DisabilityStatus != null
                        && fm.DisabilityStatus != ""
                        && fm.DisabilityStatus != "छैन"
                        && fm.DisabilityStatus != "No"
                        && fm.DisabilityStatus != "none"
                        && fm.DisabilityStatus != "सपांग"
                        && fm.DisabilityStatus != "सपाङ्ग"
                        && fm.DisabilityStatus != "सपाङ्गता नभएको");
        var abroadCount = await fmQuery
            .CountAsync(fm => fm.ResidenceStatus == "विदेश" || fm.ResidenceStatus == "Abroad"
                        || fm.AbroadCountry != null);
        var childrenCount = await fmQuery.CountAsync(fm => fm.Age != null && fm.Age < 16);
        var seniorCount = await fmQuery.CountAsync(fm => fm.Age != null && fm.Age >= 60);

        return new List<StatCounter>
        {
            new() { Label = "घरपरिवार", LabelEn = "Households", Value = totalHH, Icon = "bi-house", AccentColor = "#003B6F" },
            new() { Label = "जनसंख्या", LabelEn = "Population", Value = totalPop, Icon = "bi-people", AccentColor = "#DC143C" },
            new() { Label = "पुरूष", LabelEn = "Male", Value = maleCount, Icon = "bi-person", AccentColor = "#0066B3" },
            new() { Label = "महिला", LabelEn = "Female", Value = femaleCount, Icon = "bi-person-fill", AccentColor = "#E8384F" },
            new() { Label = "बालबालिका (०-१५)", LabelEn = "Children (0-15)", Value = childrenCount, Icon = "bi-emoji-smile", AccentColor = "#0EA5E9" },
            new() { Label = "ज्येष्ठ नागरिक (६०+)", LabelEn = "Senior Citizens (60+)", Value = seniorCount, Icon = "bi-person-wheelchair", AccentColor = "#C5A55A" },
            new() { Label = "अपाङ्गता", LabelEn = "Persons with Disability", Value = disabledCount, Icon = "bi-universal-access", AccentColor = "#0D9488" },
            new() { Label = "विदेशमा", LabelEn = "Abroad", Value = abroadCount, Icon = "bi-airplane", AccentColor = "#002147" }
        };
    }

    /// <summary>
    /// Raw chart computation (no cache). Runs all chart queries in parallel.
    /// </summary>
    private async Task<List<ChartData>> BuildAllChartsAsync(int? wardNo)
    {
        // Build all 36 charts SEQUENTIALLY (one await at a time).
        // This prevents "A second operation was started on this context instance"
        // because DbContext is not thread-safe. (The previous List<Task> + Task.WhenAll
        // was the direct cause of the repeated concurrency exceptions on homepage load.)
        var results = new List<ChartData>
        {
            await GetGenderDistributionAsync(wardNo),
            await GetCasteEthnicityAsync(wardNo),
            await GetMotherTongueAsync(wardNo),
            await GetReligionAsync(wardNo),
            await GetLiteracyStatusAsync(wardNo),
            await GetEducationLevelAsync(wardNo),
            await GetMaritalStatusAsync(wardNo),
            await GetOccupationAsync(wardNo),
            await GetResidenceStatusAsync(wardNo),
            await GetDisabilityStatusAsync(wardNo),
            await GetAgePyramidAsync(wardNo),
            await GetDrinkingWaterSourceAsync(wardNo),
            await GetCookingFuelAsync(wardNo),
            await GetLightSourceAsync(wardNo),
            await GetToiletTypeAsync(wardNo),
            await GetWasteManagementAsync(wardNo),
            await GetHouseOwnershipAsync(wardNo),
            await GetRoofTypeAsync(wardNo),
            await GetFoundationTypeAsync(wardNo),
            await GetIncomeSourcesAsync(wardNo),
            await GetExpenditureCategoriesAsync(wardNo),
            await GetIncomeVsExpenditureAsync(wardNo),
            await GetFoodSufficiencyAsync(wardNo),
            await GetBankAccountStatusAsync(wardNo),
            await GetLoanPurposesAsync(wardNo),
            await GetDisasterRisksAsync(wardNo),
            await GetAssetsOwnershipAsync(wardNo),
            await GetLivestockSummaryAsync(wardNo),
            await GetForeignEmploymentAsync(wardNo),
            await GetAgriculturalLandAsync(wardNo),
            await GetIrrigationStatusAsync(wardNo),
            await GetCropProductionAsync(wardNo),
            await GetHealthVisitPlaceAsync(wardNo),
            await GetInsuranceStatusAsync(wardNo),
            await GetSocialSecurityAsync(wardNo),
            await GetDeathCausesAsync(wardNo)
        };

        return results.OrderBy(c => c.SortOrder).ToList();
    }

    // ─── POPULATION CHARTS ─────────────────────────────

    public async Task<ChartData> GetGenderDistributionAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.Gender != null)
            .GroupBy(fm => fm.Gender)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("gender", "लिङ्ग अनुसार जनसंख्या", "doughnut", "जनसंख्या", 1, "/Household/Table/1",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Population by Gender", categoryEn: "Population");
    }

    public async Task<ChartData> GetCasteEthnicityAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.Caste != null && fm.Caste != "")
            .GroupBy(fm => fm.Caste)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .Take(15)
            .ToListAsync();

        return MakeChart("caste", "जातजाती अनुसार जनसंख्या", "bar", "जनसंख्या", 2, "/Household/Table/4",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Population by Caste/Ethnicity", categoryEn: "Population");
    }

    public async Task<ChartData> GetMotherTongueAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.Language != null && fm.Language != "")
            .GroupBy(fm => fm.Language)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .Take(12)
            .ToListAsync();

        return MakeChart("language", "मातृभाषाको आधारमा जनसंख्या", "bar", "जनसंख्या", 3, "/Household/Table/5",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Population by Mother Tongue", categoryEn: "Population");
    }

    public async Task<ChartData> GetReligionAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.Religion != null && fm.Religion != "")
            .GroupBy(fm => fm.Religion)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("religion", "धर्मको आधारमा जनसंख्या", "doughnut", "जनसंख्या", 4, "/Household/Table/6",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Population by Religion", categoryEn: "Population");
    }

    public async Task<ChartData> GetMaritalStatusAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.MaritalStatus != null && fm.MaritalStatus != "")
            .GroupBy(fm => fm.MaritalStatus)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("marital", "वैवाहिक स्थिति", "doughnut", "जनसंख्या", 5, "/Household/Table/13",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Marital Status", categoryEn: "Population");
    }

    public async Task<ChartData> GetOccupationAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.Occupation != null && fm.Occupation != "")
            .GroupBy(fm => fm.Occupation)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .Take(15)
            .ToListAsync();

        return MakeChart("occupation", "प्रमुख पेशा अनुसार जनसंख्या", "bar", "जनसंख्या", 6, "/Household/Table/14",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Population by Main Occupation", categoryEn: "Population");
    }

    public async Task<ChartData> GetResidenceStatusAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.ResidenceStatus != null && fm.ResidenceStatus != "")
            .GroupBy(fm => fm.ResidenceStatus)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("residence", "बसोबासको अवस्था", "doughnut", "जनसंख्या", 7, "/Household/Table/11",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Residence Status", categoryEn: "Population");
    }

    public async Task<ChartData> GetDisabilityStatusAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.DisabilityStatus != null
                        && fm.DisabilityStatus != ""
                        && fm.DisabilityStatus != "छैन"
                        && fm.DisabilityStatus != "No"
                        && fm.DisabilityStatus != "none"
                        && fm.DisabilityStatus != "सपांग"
                        && fm.DisabilityStatus != "सपाङ्ग"
                        && fm.DisabilityStatus != "सपाङ्गता नभएको")
            .GroupBy(fm => fm.DisabilityStatus)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("disability", "अपाङ्गताको स्थिति", "doughnut", "जनसंख्या", 8, "/Household/Table/12",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Disability Status", categoryEn: "Population");
    }

    // ─── EDUCATION & HEALTH CHARTS ─────────────────────

    public async Task<ChartData> GetLiteracyStatusAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.LiteracyStatus != null && fm.LiteracyStatus != "")
            .GroupBy(fm => fm.LiteracyStatus)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("literacy", "साक्षरताको स्थिति", "doughnut", "शिक्षा तथा स्वास्थ्य", 10, "/Household/Table/7",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Literacy Status", categoryEn: "Education & Health");
    }

    public async Task<ChartData> GetEducationLevelAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.EducationLevel != null && fm.EducationLevel != "")
            .GroupBy(fm => fm.EducationLevel)
            .Select(g => new
            {
                Label = g.Key!,
                Count = g.Count(),
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female")
            })
            .OrderByDescending(x => x.Count)
            .Take(12)
            .ToListAsync();

        return MakeChart("education", "शैक्षिकस्तर अनुसार जनसंख्या", "bar", "शिक्षा तथा स्वास्थ्य", 11, "/Household/Table/8",
            data.Select(d => new ChartItem
            {
                Label = d.Label, Value = d.Count,
                MaleCount = d.Male, FemaleCount = d.Female
            }).ToList(),
            titleEn: "Population by Education Level", categoryEn: "Education & Health");
    }

    public async Task<ChartData> GetHealthVisitPlaceAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.FirstHealthVisit != null && h.FirstHealthVisit != "")
            .GroupBy(h => h.FirstHealthVisit)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("health_visit", "विरामी परेमा पहिले जाने स्थान", "doughnut", "शिक्षा तथा स्वास्थ्य", 12, "/Household/Table/18",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "First Health Visit Place", categoryEn: "Education & Health");
    }

    public async Task<ChartData> GetInsuranceStatusAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.HasInsurance != null && fm.HasInsurance != "")
            .GroupBy(fm => fm.HasInsurance)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("insurance", "बिमा स्थिति", "doughnut", "शिक्षा तथा स्वास्थ्य", 13, "/Household/Table/16",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Insurance Status", categoryEn: "Education & Health");
    }

    public async Task<ChartData> GetSocialSecurityAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.SocialSecurityStatus != null && fm.SocialSecurityStatus != "")
            .GroupBy(fm => fm.SocialSecurityStatus)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("social_security", "सामाजिक सुरक्षा भत्ता", "doughnut", "शिक्षा तथा स्वास्थ्य", 14, "/Household/Table/21",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Social Security Allowance", categoryEn: "Education & Health");
    }

    public async Task<ChartData> GetDeathCausesAsync(int? wardNo = null)
    {
        var q = BaseDeathRecordQuery(wardNo);
        var sums = await q
            .GroupBy(dr => 1)
            .Select(g => new
            {
                Communicable = (int)g.Sum(dr => dr.CauseCommunicable ?? 0),
                NonCommunicable = (int)g.Sum(dr => dr.CauseNonCommunicable ?? 0),
                Lifestyle = (int)g.Sum(dr => dr.CauseLifestyle ?? 0),
                Other = (int)g.Sum(dr => dr.CauseOther ?? 0)
            })
            .FirstOrDefaultAsync();

        if (sums == null)
            return MakeChart("death_cause", "मृत्युको कारण", "doughnut", "शिक्षा तथा स्वास्थ्य", 15, "/Household/Table/15",
                new List<ChartItem>(),
            titleEn: "Causes of Death", categoryEn: "Education & Health");

        var items = new List<ChartItem>
        {
            new() { Label = "संक्रामक रोग", Value = sums.Communicable },
            new() { Label = "गैर-संक्रामक रोग", Value = sums.NonCommunicable },
            new() { Label = "जीवनशैली", Value = sums.Lifestyle },
            new() { Label = "अन्य", Value = sums.Other }
        }
        .Where(i => i.Value > 0)
        .ToList();

        return MakeChart("death_cause", "मृत्युको कारण", "doughnut", "शिक्षा तथा स्वास्थ्य", 15, "/Household/Table/15", items);
    }

    // ─── FACILITY CHARTS ───────────────────────────────

    public async Task<ChartData> GetDrinkingWaterSourceAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.DrinkingWaterSource != null && h.DrinkingWaterSource != "")
            .GroupBy(h => h.DrinkingWaterSource)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("water", "खानेपानीको स्रोत", "bar", "सुविधा तथा पूर्वाधार", 20, "/Household/Table/29",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Drinking Water Source", categoryEn: "Facilities & Infrastructure");
    }

    public async Task<ChartData> GetCookingFuelAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.CookingFuelPrimary != null && h.CookingFuelPrimary != "")
            .GroupBy(h => h.CookingFuelPrimary)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("fuel", "खाना पकाउने इन्धन", "doughnut", "सुविधा तथा पूर्वाधार", 21, "/Household/Table/30",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Cooking Fuel", categoryEn: "Facilities & Infrastructure");
    }

    public async Task<ChartData> GetLightSourceAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.LightSource != null && h.LightSource != "")
            .GroupBy(h => h.LightSource)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("light", "बत्तीको स्रोत", "doughnut", "सुविधा तथा पूर्वाधार", 22, "/Household/Table/32",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Source of Electricity", categoryEn: "Facilities & Infrastructure");
    }

    public async Task<ChartData> GetToiletTypeAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.ToiletType != null && h.ToiletType != "")
            .GroupBy(h => h.ToiletType)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("toilet", "शौचालयको प्रकार", "doughnut", "सुविधा तथा पूर्वाधार", 23, "/Household/Table/34",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Type of Toilet", categoryEn: "Facilities & Infrastructure");
    }

    public async Task<ChartData> GetWasteManagementAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.WasteManagement != null && h.WasteManagement != "")
            .GroupBy(h => h.WasteManagement)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("waste", "फोहोर व्यवस्थापन", "doughnut", "सुविधा तथा पूर्वाधार", 24, "/Household/Table/33",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Waste Management", categoryEn: "Facilities & Infrastructure");
    }

    // ─── HOUSING CHARTS ────────────────────────────────

    public async Task<ChartData> GetHouseOwnershipAsync(int? wardNo = null)
    {
        var q = BaseHouseDetailQuery(wardNo);
        var data = await q
            .Where(hd => hd.OwnershipType != null && hd.OwnershipType != "")
            .GroupBy(hd => hd.OwnershipType)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("ownership", "स्वामित्व", "doughnut", "आवास", 30, "/Household/Table/45",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "House Ownership", categoryEn: "Housing");
    }

    public async Task<ChartData> GetRoofTypeAsync(int? wardNo = null)
    {
        var q = BaseHouseDetailQuery(wardNo);
        var data = await q
            .Where(hd => hd.RoofType != null && hd.RoofType != "")
            .GroupBy(hd => hd.RoofType)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("roof", "छानाको प्रकार", "doughnut", "आवास", 31, "/Household/Table/47",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Roof Type", categoryEn: "Housing");
    }

    public async Task<ChartData> GetFoundationTypeAsync(int? wardNo = null)
    {
        var q = BaseHouseDetailQuery(wardNo);
        var data = await q
            .Where(hd => hd.FoundationType != null && hd.FoundationType != "")
            .GroupBy(hd => hd.FoundationType)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("foundation", "घरको जग", "doughnut", "आवास", 32, "/Household/Table/50",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "House Foundation", categoryEn: "Housing");
    }

    // ─── ECONOMIC CHARTS ───────────────────────────────

    public async Task<ChartData> GetIncomeSourcesAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var totals = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Veg = g.Sum(h => h.IncomeVegetables ?? 0),
                Labor = g.Sum(h => h.IncomeLabor ?? 0),
                Trade = g.Sum(h => h.IncomeTrade ?? 0),
                Foreign = g.Sum(h => h.IncomeForeign ?? 0),
                Job = g.Sum(h => h.IncomeJob ?? 0),
                Other = g.Sum(h => h.IncomeOther ?? 0)
            })
            .FirstOrDefaultAsync();

        if (totals == null)
            return MakeChart("income", "वार्षिक आम्दानी स्रोत", "bar", "आर्थिक अवस्था", 40, "/Household/Table/37",
                new List<ChartItem>(),
            titleEn: "Annual Income Sources", categoryEn: "Economic Status");

        var items = new List<ChartItem>
        {
            new() { Label = "तरकारी/फलफूल", Value = (int)totals.Veg },
            new() { Label = "मजदुरी", Value = (int)totals.Labor },
            new() { Label = "व्यापार", Value = (int)totals.Trade },
            new() { Label = "विदेश रोजगार", Value = (int)totals.Foreign },
            new() { Label = "जागिर/सेवा", Value = (int)totals.Job },
            new() { Label = "अन्य", Value = (int)totals.Other }
        };

        return MakeChart("income", "वार्षिक आम्दानी स्रोत", "bar", "आर्थिक अवस्था", 40, "/Household/Table/37", items);
    }

    public async Task<ChartData> GetExpenditureCategoriesAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var totals = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Food = g.Sum(h => h.ExpenseFood ?? 0),
                Clothes = g.Sum(h => h.ExpenseClothes ?? 0),
                Education = g.Sum(h => h.ExpenseEducation ?? 0),
                Health = g.Sum(h => h.ExpenseHealth ?? 0),
                Festivals = g.Sum(h => h.ExpenseFestivals ?? 0),
                Agriculture = g.Sum(h => h.ExpenseAgriculture ?? 0),
                Fuel = g.Sum(h => h.ExpenseFuel ?? 0),
                Mobile = g.Sum(h => h.ExpenseMobileInternet ?? 0)
            })
            .FirstOrDefaultAsync();

        if (totals == null)
            return MakeChart("expense", "वार्षिक खर्च", "bar", "आर्थिक अवस्था", 41, "/Household/Table/38",
                new List<ChartItem>(),
            titleEn: "Annual Expenditure", categoryEn: "Economic Status");

        var items = new List<ChartItem>
        {
            new() { Label = "खाना", Value = (int)totals.Food },
            new() { Label = "लुगा", Value = (int)totals.Clothes },
            new() { Label = "शिक्षा", Value = (int)totals.Education },
            new() { Label = "स्वास्थ्य", Value = (int)totals.Health },
            new() { Label = "चाडपर्व", Value = (int)totals.Festivals },
            new() { Label = "कृषि", Value = (int)totals.Agriculture },
            new() { Label = "इन्धन", Value = (int)totals.Fuel },
            new() { Label = "मोबाइल/इन्टरनेट", Value = (int)totals.Mobile }
        };

        return MakeChart("expense", "वार्षिक खर्च", "bar", "आर्थिक अवस्था", 41, "/Household/Table/38", items);
    }

    public async Task<ChartData> GetFoodSufficiencyAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.FoodSufficiencyMonths != null && h.FoodSufficiencyMonths != "")
            .GroupBy(h => h.FoodSufficiencyMonths)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderBy(x => x.Label)
            .ToListAsync();

        return MakeChart("food", "खाद्य सुरक्षा (महिना)", "bar", "आर्थिक अवस्था", 42, "/Household/Table/39",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Food Sufficiency (Months)", categoryEn: "Economic Status");
    }

    public async Task<ChartData> GetBankAccountStatusAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.HasBankAccount != null && h.HasBankAccount != "")
            .GroupBy(h => h.HasBankAccount)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("bank", "बैंक खाता", "doughnut", "आर्थिक अवस्था", 43, "/Household/Table/19",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Bank Account", categoryEn: "Economic Status");
    }

    public async Task<ChartData> GetLoanPurposesAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var sums = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Business = (int)g.Sum(h => h.LoanPurposeBusiness ?? 0),
                House = (int)g.Sum(h => h.LoanPurposeHouse ?? 0),
                Medical = (int)g.Sum(h => h.LoanPurposeMedical ?? 0),
                Education = (int)g.Sum(h => h.LoanPurposeEducation ?? 0),
                Other = (int)g.Sum(h => h.LoanPurposeOther ?? 0)
            })
            .FirstOrDefaultAsync();

        if (sums == null)
            return MakeChart("loan_purpose", "ऋणको उद्देश्य", "bar", "आर्थिक अवस्था", 44, "/Household/Table/22",
                new List<ChartItem>(),
            titleEn: "Purpose of Loan", categoryEn: "Economic Status");

        var items = new List<ChartItem>
        {
            new() { Label = "व्यापार", Value = sums.Business },
            new() { Label = "घर निर्माण", Value = sums.House },
            new() { Label = "स्वास्थ्य", Value = sums.Medical },
            new() { Label = "शिक्षा", Value = sums.Education },
            new() { Label = "अन्य", Value = sums.Other }
        };

        return MakeChart("loan_purpose", "ऋणको उद्देश्य", "bar", "आर्थिक अवस्था", 44, "/Household/Table/22", items);
    }

    // ─── RISK & ASSETS ─────────────────────────────────

    public async Task<ChartData> GetDisasterRisksAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var sums = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Earthquake = (int)g.Sum(h => h.RiskEarthquake ?? 0),
                Flood = (int)g.Sum(h => h.RiskFlood ?? 0),
                Landslide = (int)g.Sum(h => h.RiskLandslide ?? 0),
                Lightning = (int)g.Sum(h => h.RiskLightning ?? 0),
                Storm = (int)g.Sum(h => h.RiskStorm ?? 0),
                ColdWave = (int)g.Sum(h => h.RiskColdWave ?? 0),
                Inundation = (int)g.Sum(h => h.RiskInundation ?? 0)
            })
            .FirstOrDefaultAsync();

        if (sums == null)
            return MakeChart("risk", "प्राकृतिक प्रकोप जोखिम", "bar", "जोखिम तथा सम्पत्ति", 50, "/Household/Table/55",
                new List<ChartItem>(),
            titleEn: "Natural Disaster Risks", categoryEn: "Risk & Assets");

        var items = new List<ChartItem>
        {
            new() { Label = "भूकम्प", Value = sums.Earthquake },
            new() { Label = "बाढी", Value = sums.Flood },
            new() { Label = "पहिरो", Value = sums.Landslide },
            new() { Label = "चट्याङ", Value = sums.Lightning },
            new() { Label = "आँधी", Value = sums.Storm },
            new() { Label = "शीतलहर", Value = sums.ColdWave },
            new() { Label = "डुबान", Value = sums.Inundation }
        };

        return MakeChart("risk", "प्राकृतिक प्रकोप जोखिम", "bar", "जोखिम तथा सम्पत्ति", 50, "/Household/Table/55", items);
    }

    public async Task<ChartData> GetAssetsOwnershipAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var sums = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Radio = (int)g.Sum(h => h.AssetRadio ?? 0),
                Bicycle = (int)g.Sum(h => h.AssetBicycle ?? 0),
                TV = (int)g.Sum(h => h.AssetTelevision ?? 0),
                Phone = (int)g.Sum(h => h.AssetPhone ?? 0),
                Computer = (int)g.Sum(h => h.AssetComputer ?? 0),
                Internet = (int)g.Sum(h => h.AssetInternet ?? 0),
                Motorcycle = (int)g.Sum(h => h.AssetMotorcycle ?? 0),
                Car = (int)g.Sum(h => h.AssetCar ?? 0),
                Fridge = (int)g.Sum(h => h.AssetRefrigerator ?? 0),
                Tractor = (int)g.Sum(h => h.AssetTractor ?? 0),
                Bus = (int)g.Sum(h => h.AssetBusTruck ?? 0),
                Tipper = (int)g.Sum(h => h.AssetTipperTruck ?? 0),
                Excavator = (int)g.Sum(h => h.AssetExcavator ?? 0),
                None = (int)g.Sum(h => h.AssetNone ?? 0)
            })
            .FirstOrDefaultAsync();

        if (sums == null)
            return MakeChart("assets", "घरपरिवारमा उपलब्ध सुविधाहरू", "bar", "जोखिम तथा सम्पत्ति", 51, "/Household/Table/35",
                new List<ChartItem>(),
            titleEn: "Household Assets & Facilities", categoryEn: "Risk & Assets");

        var items = new List<ChartItem>
        {
            new() { Label = "रेडियो", Value = sums.Radio },
            new() { Label = "साइकल", Value = sums.Bicycle },
            new() { Label = "टेलिभिजन", Value = sums.TV },
            new() { Label = "मोबाइल", Value = sums.Phone },
            new() { Label = "कम्प्युटर", Value = sums.Computer },
            new() { Label = "इन्टरनेट", Value = sums.Internet },
            new() { Label = "मोटरसाइकल", Value = sums.Motorcycle },
            new() { Label = "गाडी", Value = sums.Car },
            new() { Label = "फ्रिज", Value = sums.Fridge },
            new() { Label = "ट्र्याक्टर", Value = sums.Tractor },
            new() { Label = "बस/ट्रक", Value = sums.Bus },
            new() { Label = "टिपर", Value = sums.Tipper },
            new() { Label = "जे.सी.बी.", Value = sums.Excavator },
            new() { Label = "केही छैन", Value = sums.None }
        };

        return MakeChart("assets", "घरपरिवारमा उपलब्ध सुविधाहरू", "bar", "जोखिम तथा सम्पत्ति", 51, "/Household/Table/35", items);
    }

    // ─── AGRICULTURE CHARTS ────────────────────────────

    public async Task<ChartData> GetLivestockSummaryAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var sums = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                Cow = (int)g.Sum(h => h.DairyCow ?? 0),
                Buffalo = (int)g.Sum(h => h.DairyBuffalo ?? 0),
                OtherDairy = (int)g.Sum(h => h.DairyOther ?? 0),
                Goat = (int)g.Sum(h => h.MeatGoatSheep ?? 0),
                Calf = (int)g.Sum(h => h.MeatCalf ?? 0),
                Pig = (int)g.Sum(h => h.MeatPig ?? 0),
                OtherMeat = (int)g.Sum(h => h.MeatOther ?? 0),
                Chicken = (int)g.Sum(h => h.PoultryChicken ?? 0),
                Duck = (int)g.Sum(h => h.PoultryDuck ?? 0),
                Pigeon = (int)g.Sum(h => h.PoultryPigeon ?? 0),
                Battai = (int)g.Sum(h => h.PoultryBattai ?? 0)
            })
            .FirstOrDefaultAsync();

        if (sums == null)
            return MakeChart("livestock", "चौपाया पशुपन्छी", "doughnut", "कृषि तथा पशुपालन", 60, "/Household/Table/40",
                new List<ChartItem>(),
            titleEn: "Livestock & Poultry", categoryEn: "Agriculture & Livestock");

        var items = new List<ChartItem>
        {
            new() { Label = "गाई", Value = sums.Cow },
            new() { Label = "भैंसी", Value = sums.Buffalo },
            new() { Label = "अन्य दुधालु", Value = sums.OtherDairy },
            new() { Label = "बाख्रा/भेडा", Value = sums.Goat },
            new() { Label = "बाच्छा", Value = sums.Calf },
            new() { Label = "सुँगुर", Value = sums.Pig },
            new() { Label = "अन्य मासु", Value = sums.OtherMeat },
            new() { Label = "कुखुरा", Value = sums.Chicken },
            new() { Label = "हाँस", Value = sums.Duck },
            new() { Label = "परेवा", Value = sums.Pigeon },
            new() { Label = "बट्टाइ", Value = sums.Battai }
        }
        .Where(i => i.Value > 0)
        .OrderByDescending(i => i.Value)
        .ToList();

        return MakeChart("livestock", "चौपाया पशुपन्छी", "doughnut", "कृषि तथा पशुपालन", 60, "/Household/Table/40", items);
    }

    public async Task<ChartData> GetForeignEmploymentAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);
        var data = await q
            .Where(fm => fm.AbroadCountry != null && fm.AbroadCountry != "")
            .GroupBy(fm => fm.AbroadCountry)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(12)
            .ToListAsync();

        return MakeChart("foreign", "बैदेशिक रोजगार (मुलुक)", "bar", "कृषि तथा पशुपालन", 61, "/Household/Table/63",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Foreign Employment (Country)", categoryEn: "Agriculture & Livestock");
    }

    public async Task<ChartData> GetAgriculturalLandAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.HasAgricultureLand != null && h.HasAgricultureLand != "")
            .GroupBy(h => h.HasAgricultureLand)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("agri_land", "कृषि जग्गा", "doughnut", "कृषि तथा पशुपालन", 62, "/Household/Table/36",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Agricultural Land", categoryEn: "Agriculture & Livestock");
    }

    public async Task<ChartData> GetIrrigationStatusAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var data = await q
            .Where(h => h.IrrigationStatus != null && h.IrrigationStatus != "")
            .GroupBy(h => h.IrrigationStatus)
            .Select(g => new { Label = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return MakeChart("irrigation", "सिंचाईको अवस्था", "doughnut", "कृषि तथा पशुपालन", 63, "/Household/Table/36",
            data.Select(d => new ChartItem { Label = d.Label, Value = d.Count }).ToList(),
            titleEn: "Irrigation Status", categoryEn: "Agriculture & Livestock");
    }

    // ─── PART 9B: ADDITIONAL CHARTS ─────────────────────

    /// <summary>
    /// Age Pyramid — 5-year age groups with male (negative) / female (positive) horizontal bars.
    /// </summary>
    public async Task<ChartData> GetAgePyramidAsync(int? wardNo = null)
    {
        var q = BaseFamilyMemberQuery(wardNo);

        var data = await q
            .Where(fm => fm.Age != null && fm.Age >= 0)
            .GroupBy(fm => new
            {
                AgeGroup = fm.Age < 5 ? "0-4" :
                           fm.Age < 10 ? "5-9" :
                           fm.Age < 15 ? "10-14" :
                           fm.Age < 20 ? "15-19" :
                           fm.Age < 25 ? "20-24" :
                           fm.Age < 30 ? "25-29" :
                           fm.Age < 35 ? "30-34" :
                           fm.Age < 40 ? "35-39" :
                           fm.Age < 45 ? "40-44" :
                           fm.Age < 50 ? "45-49" :
                           fm.Age < 55 ? "50-54" :
                           fm.Age < 60 ? "55-59" :
                           fm.Age < 65 ? "60-64" :
                           fm.Age < 70 ? "65-69" :
                           fm.Age < 75 ? "70-74" :
                           fm.Age < 80 ? "75-79" : "80+"
            })
            .Select(g => new
            {
                AgeGroup = g.Key.AgeGroup,
                Male = g.Count(x => x.Gender == "पुरूष" || x.Gender == "Male" || x.Gender == "male"),
                Female = g.Count(x => x.Gender == "महिला" || x.Gender == "Female" || x.Gender == "female")
            })
            .ToListAsync();

        // Define the ordering
        var ageOrder = new List<string>
        {
            "80+", "75-79", "70-74", "65-69", "60-64", "55-59", "50-54",
            "45-49", "40-44", "35-39", "30-34", "25-29", "20-24",
            "15-19", "10-14", "5-9", "0-4"
        };

        var ordered = ageOrder
            .Select(a => new
            {
                AgeGroup = a,
                Male = data.FirstOrDefault(d => d.AgeGroup == a)?.Male ?? 0,
                Female = data.FirstOrDefault(d => d.AgeGroup == a)?.Female ?? 0
            })
            .ToList();

        return MakeChart("age_pyramid", "उमेर पिरामिड", "agePyramid", "जनसंख्या", 9, "/Household/Table/2",
            ordered.Select(d => new ChartItem
            {
                Label = d.AgeGroup,
                Value = d.Male + d.Female,
                MaleCount = d.Male,
                FemaleCount = d.Female
            }).ToList(),
            titleEn: "Age Pyramid", categoryEn: "Population");
    }

    /// <summary>
    /// Income vs Expenditure — grouped bar comparing major income sources vs expense categories.
    /// </summary>
    public async Task<ChartData> GetIncomeVsExpenditureAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);
        var totals = await q
            .GroupBy(h => 1)
            .Select(g => new
            {
                // Income sources
                Veg = (int)g.Sum(h => h.IncomeVegetables ?? 0),
                Labor = (int)g.Sum(h => h.IncomeLabor ?? 0),
                Trade = (int)g.Sum(h => h.IncomeTrade ?? 0),
                Foreign = (int)g.Sum(h => h.IncomeForeign ?? 0),
                Job = (int)g.Sum(h => h.IncomeJob ?? 0),
                OtherIncome = (int)g.Sum(h => h.IncomeOther ?? 0),
                // Expense categories
                Food = (int)g.Sum(h => h.ExpenseFood ?? 0),
                Education = (int)g.Sum(h => h.ExpenseEducation ?? 0),
                Health = (int)g.Sum(h => h.ExpenseHealth ?? 0),
                OtherExpense = (int)(g.Sum(h => h.ExpenseClothes ?? 0) + g.Sum(h => h.ExpenseFestivals ?? 0)
                    + g.Sum(h => h.ExpenseAgriculture ?? 0) + g.Sum(h => h.ExpenseFuel ?? 0) + g.Sum(h => h.ExpenseMobileInternet ?? 0))
            })
            .FirstOrDefaultAsync();

        if (totals == null)
            return MakeChart("income_vs_expense", "आम्दानी र खर्च तुलना", "bar", "आर्थिक अवस्था", 41, "/Household/Table/37",
                new List<ChartItem>(),
                titleEn: "Income vs Expenditure", categoryEn: "Economic Status");

        var totalIncome = totals.Veg + totals.Labor + totals.Trade + totals.Foreign + totals.Job + totals.OtherIncome;
        var totalExpense = totals.Food + totals.Education + totals.Health + totals.OtherExpense;

        var items = new List<ChartItem>
        {
            new() { Label = "कृषि/तरकारी", Value = totals.Veg },
            new() { Label = "मजदुरी", Value = totals.Labor },
            new() { Label = "व्यापार", Value = totals.Trade },
            new() { Label = "विदेश", Value = totals.Foreign },
            new() { Label = "जागिर", Value = totals.Job },
            new() { Label = "खाना", Value = totals.Food },
            new() { Label = "शिक्षा", Value = totals.Education },
            new() { Label = "स्वास्थ्य", Value = totals.Health }
        };

        // Use stacked bar with two datasets: income and expense side by side
        return new ChartData
        {
            Id = "income_vs_expense",
            Title = "आम्दानी र खर्च तुलना",
            TitleEn = "Income vs Expenditure",
            ChartType = "bar",
            Category = "आर्थिक अवस्था",
            CategoryEn = "Economic Status",
            SortOrder = 41,
            DetailLink = "/Household/Table/37",
            DetailLinkText = "विवरण हेर्नुहोस्",
            DetailLinkTextEn = "View Details",
            Items = new List<ChartItem>
            {
                new() { Label = "कुल आम्दानी", Value = totalIncome },
                new() { Label = "कुल खर्च", Value = totalExpense }
            }
        };
    }

    /// <summary>
    /// Crop Production — stacked bar showing households growing each cereal crop by ward.
    /// Cereal columns are short flags (0/1) — count of households per crop per ward.
    /// </summary>
    public async Task<ChartData> GetCropProductionAsync(int? wardNo = null)
    {
        var q = BaseHouseholdQuery(wardNo);

        var data = await q
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                Ward = g.Key,
                Rice = (int)g.Sum(h => h.CerealRice ?? 0),
                Maize = (int)g.Sum(h => h.CerealMaize ?? 0),
                Wheat = (int)g.Sum(h => h.CerealWheat ?? 0),
                Barley = (int)g.Sum(h => h.CerealBarley ?? 0)
            })
            .OrderBy(x => x.Ward)
            .ToListAsync();

        var labels = data.Select(d => $"वडा {d.Ward}").ToList();

        return new ChartData
        {
            Id = "crop_production",
            Title = "अन्नबाली खेती गर्ने घरपरिवार",
            TitleEn = "Households Growing Cereal Crops",
            ChartType = "stackedBar",
            Category = "कृषि तथा पशुपालन",
            CategoryEn = "Agriculture & Livestock",
            SortOrder = 64,
            DetailLink = "/Household/Table/59",
            DetailLinkText = "विवरण हेर्नुहोस्",
            DetailLinkTextEn = "View Details",
            Items = labels.Select(l => new ChartItem { Label = l, Value = 0 }).ToList(),
            Datasets = new List<ChartDataset>
            {
                new() { Label = "धान", Values = data.Select(d => (double)d.Rice).ToList() },
                new() { Label = "मकै", Values = data.Select(d => (double)d.Maize).ToList() },
                new() { Label = "गहुँ", Values = data.Select(d => (double)d.Wheat).ToList() },
                new() { Label = "जौ", Values = data.Select(d => (double)d.Barley).ToList() }
            }
        };
    }
}
