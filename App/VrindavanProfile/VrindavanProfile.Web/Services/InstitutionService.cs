using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Caching;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

public class InstitutionService : IInstitutionService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public InstitutionService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // ═══════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════

    public async Task<TableViewModel?> GetTableAsync(int tableId, int? wardNo = null)
    {
        var key = CacheKeys.InstitutionTable(tableId, wardNo);

        if (_cache.TryGetValue(key, out TableViewModel? cached))
            return cached;

        var result = await BuildTableAsync(tableId, wardNo);

        if (result == null)
            return null;

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.SurveyDataDuration,
            Size = 1
        });

        return result;
    }

    private async Task<TableViewModel?> BuildTableAsync(int tableId, int? wardNo)
    {
        return tableId switch
        {
            // ═══ वडा तथ्याङ्क (ward_statistics) ═══
            1 => await GetTable01Async(wardNo),   // व्यक्तिगत घटना विवरण
            2 => await GetTable02Async(wardNo),   // सामाजिक सुरक्षा कार्यक्रम

            // ═══ शैक्षिक संस्था ═══
            3 => await InstCatTable<EducationalInstitution>(3, "शैक्षिक संस्था विवरण", "शैक्षिक संस्था", null, wardNo, "InstitutionType", null),
            4 => await GetTable04Async(wardNo),   // शिक्षक दरबन्दी
            5 => await GetTable05Async(wardNo),   // विद्यार्थी संख्या
            6 => await GetTable06Async(wardNo),   // शैक्षिक उपलब्धी/उतिर्ण दर
            7 => await GetTable07Async(wardNo),   // छात्रवृत्ती विवरण
            8 => await GetTable08Async(wardNo),   // विद्यालय पूर्वाधार
            40 => await GetTable40Async(wardNo),  // बाल क्लब/बालमैत्री
            41 => await GetTable41Async(wardNo),  // विद्यालय सुधार योजना

            // ═══ स्वास्थ्य संस्था ═══
            9 => await InstCatTable<HealthInstitution>(9, "स्वास्थ्य संस्था विवरण", "स्वास्थ्य", null, wardNo, "InstitutionType", null),
            10 => await GetTable10Async(wardNo),  // स्वास्थ्य सेवा सुविधा
            11 => await GetTable11Async(wardNo),  // खोप विवरण
            12 => await GetTable12Async(wardNo),  // प्रसुति/मातृ स्वास्थ्य
            13 => await GetTable13Async(wardNo),  // परिवार नियोजन
            43 => await GetTable43Async(wardNo),  // FCHV विवरण

            // ═══ सुरक्षा ═══
            14 => await InstCatTable<SecurityBody>(14, "सुरक्षा निकाय", "सुरक्षा", null, wardNo, "BodyType", null),

            // ═══ वित्तीय संस्था ═══
            15 => await InstCatTable<FinancialInstitution>(15, "वित्तीय संस्था विवरण", "वित्तीय", null, wardNo, "InstitutionType", null),
            16 => await GetTable16Async(wardNo),  // सहकारी प्रकार
            17 => await GetTable17Async(wardNo),  // ऋण प्रकार
            18 => await GetTable18Async(wardNo),  // बाह्य सहयोग

            // ═══ होटल/पुल/उद्योग ═══
            19 => await InstCatTable<HotelResort>(19, "होटल/रिसोर्ट", "होटल तथा पर्यटन", null, wardNo, "HotelType", null),
            20 => await InstCatTable<Bridge>(20, "पुल/पुलेसा", "पूर्वाधार", null, wardNo, "BridgeType", null),
            21 => await InstCatTable<Industry>(21, "उद्योग", "उद्योग तथा व्यापार", null, wardNo, "IndustryType", null),
            22 => await InstCatTable<NgoClubGroup>(22, "गैरसरकारी संस्था", "सामाजिक", null, wardNo, "OrgType", null),

            // ═══ भू-उपयोग / प्राकृतिक / सांस्कृतिक ═══
            23 => await InstCatTable<LandUse>(23, "भू-उपयोग", "भू-उपयोग", null, wardNo, "LandType", null),
            24 => await GetTable24Async(wardNo),  // भू-उपयोग विपद्
            25 => await InstCatTable<NaturalResource>(25, "प्राकृतिक श्रोत", "प्राकृतिक स्रोत", null, wardNo, "ResourceType", null),
            26 => await InstCatTable<CulturalHeritage>(26, "सांस्कृतिक सम्पदा", "सांस्कृतिक सम्पदा", null, wardNo, "HeritageType", null),

            // ═══ व्यापार / वन / सडक / भवन ═══
            27 => await InstCatTable<TradeBusiness>(27, "व्यापार/व्यवसाय", "उद्योग तथा व्यापार", null, wardNo, "Ownership", null),
            28 => await InstCatTable<Forest>(28, "वन विवरण", "वन", null, wardNo, "ForestType", null),
            29 => await InstCatTable<Road>(29, "सडक विवरण", "पूर्वाधार", null, wardNo, "RoadType", null),
            30 => await InstCatTable<Building>(30, "भवन विवरण", "भवन", null, wardNo, "BuildingType", null),
            31 => await GetTable31Async(wardNo),  // भवन प्रयोजन

            // ═══ कृषि (from household) ═══
            32 => await GetTable32Async(wardNo),  // तालिम विवरण
            33 => await GetTable33Async(wardNo),  // दलहन उत्पादन
            34 => await GetTable34Async(wardNo),  // नगदेबाली उत्पादन
            35 => await GetTable35Async(wardNo),  // पोल्ट्री
            36 => await GetTable36Async(wardNo),  // माछापालन
            37 => await GetTable37Async(wardNo),  // मौरीपालन
            38 => await GetTable38Async(wardNo),  // कृषि सिंचाइ
            39 => await GetTable39Async(wardNo),  // जडिबुटी

            // ═══ वडा कार्यालय ═══
            42 => await GetTable42Async(wardNo),  // वडा कार्यालय विवरण

            _ => null
        };
    }

    public async Task<List<int>> GetWardNumbersAsync()
    {
        if (_cache.TryGetValue(CacheKeys.WardNumbers, out List<int>? cached))
            return cached!;

        var hhWards = await _db.Households
            .Where(h => h.WardNo != null)
            .Select(h => h.WardNo!.Value)
            .Distinct()
            .ToListAsync();

        var instWards = await _db.SurveyEntries
            .Where(s => s.WardNo != null)
            .Select(s => s.WardNo!.Value)
            .Distinct()
            .ToListAsync();

        var result = hhWards.Union(instWards).Distinct().OrderBy(w => w).ToList();

        _cache.Set(CacheKeys.WardNumbers, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.MetadataDuration,
            Size = 1
        });

        return result;
    }

    public async Task<List<TableMeta>> GetTableListAsync()
    {
        if (_cache.TryGetValue(CacheKeys.InstitutionTableList, out List<TableMeta>? cached))
            return cached!;

        var result = BuildTableList();

        _cache.Set(CacheKeys.InstitutionTableList, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.MetadataDuration,
            Size = 1
        });

        return result;
    }

    private static List<TableMeta> BuildTableList()
    {
        return new List<TableMeta>
        {
            // ═══ वडा तथ्याङ्क ═══
            new() { TableNumber = 1, Title = "व्यक्तिगत घटना विवरण", Category = "वडा तथ्याङ्क", DataSource = "ward_statistics", Icon = "bi-card-checklist", AccentColor = "#457B9D", Description = "जन्म, मृत्यु, विवाह, सम्बन्धविच्छेद, प्रवासन दर्ता" },
            new() { TableNumber = 2, Title = "सामाजिक सुरक्षा कार्यक्रम", Category = "वडा तथ्याङ्क", DataSource = "ward_statistics", Icon = "bi-shield-check", AccentColor = "#457B9D", Description = "ज्येष्ठ नागरिक, अपाङ्गता, बाल सुरक्षा, दलित भत्ता" },
            // ═══ शैक्षिक ═══
            new() { TableNumber = 3, Title = "शैक्षिक संस्था विवरण", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-school", AccentColor = "#52B788", Description = "संस्थाको प्रकार अनुसार विवरण" },
            new() { TableNumber = 4, Title = "शिक्षक दरबन्दी विवरण", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-person-workspace", AccentColor = "#52B788", Description = "स्वीकृत तथा कार्यरत शिक्षक/शिक्षिका" },
            new() { TableNumber = 5, Title = "विद्यार्थी संख्या", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-mortarboard", AccentColor = "#52B788", Description = "तह अनुसार विद्यार्थी विवरण" },
            new() { TableNumber = 6, Title = "शैक्षिक उपलब्धी/उतिर्ण दर", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-graph-up", AccentColor = "#52B788", Description = "कक्षा ८, १०, १२ उतिर्ण दर" },
            new() { TableNumber = 7, Title = "छात्रवृत्ती विवरण", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-award", AccentColor = "#52B788", Description = "छात्रवृत्ती प्राप्त विद्यार्थी" },
            new() { TableNumber = 8, Title = "विद्यालय पूर्वाधार", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-building", AccentColor = "#52B788", Description = "भवन, पुस्तकालय, प्रयोगशाला" },
            new() { TableNumber = 40, Title = "बाल क्लब/बालमैत्री", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-emoji-smile", AccentColor = "#52B788", Description = "बाल क्लब र बालमैत्री शिक्षण" },
            new() { TableNumber = 41, Title = "विद्यालय सुधार योजना", Category = "शैक्षिक संस्था", DataSource = "educational_institution", Icon = "bi-clipboard-check", AccentColor = "#52B788", Description = "विद्यालय सुधार योजना तथा SMC" },
            // ═══ स्वास्थ्य ═══
            new() { TableNumber = 9, Title = "स्वास्थ्य संस्था विवरण", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-hospital", AccentColor = "#E76F51", Description = "संस्थाको प्रकार अनुसार विवरण" },
            new() { TableNumber = 10, Title = "स्वास्थ्य सेवा सुविधा", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-heart-pulse", AccentColor = "#E76F51", Description = "सेवा सुविधा उपलब्धता" },
            new() { TableNumber = 11, Title = "खोप विवरण", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-capsule", AccentColor = "#E76F51", Description = "खोप लगाएका बालबालिका" },
            new() { TableNumber = 12, Title = "प्रसुति/मातृ स्वास्थ्य", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-baby", AccentColor = "#E76F51", Description = "प्रसुति, जन्म, मृत्यु विवरण" },
            new() { TableNumber = 13, Title = "परिवार नियोजन", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-people", AccentColor = "#E76F51", Description = "स्थायी र अस्थायी साधन प्रयोग" },
            new() { TableNumber = 43, Title = "FCHV विवरण", Category = "स्वास्थ्य", DataSource = "health_institution", Icon = "bi-person-heart", AccentColor = "#E76F51", Description = "महिला स्वास्थ्य स्वयम्सेविका" },
            // ═══ सुरक्षा ═══
            new() { TableNumber = 14, Title = "सुरक्षा निकाय", Category = "सुरक्षा", DataSource = "security_body", Icon = "bi-shield-lock", AccentColor = "#457B9D", Description = "सुरक्षा निकायको प्रकार" },
            // ═══ वित्तीय ═══
            new() { TableNumber = 15, Title = "वित्तीय संस्था विवरण", Category = "वित्तीय", DataSource = "financial_institution", Icon = "bi-bank", AccentColor = "#E9C46A", Description = "संस्थाको प्रकार अनुसार" },
            new() { TableNumber = 16, Title = "सहकारी प्रकार", Category = "वित्तीय", DataSource = "financial_institution", Icon = "bi-bank2", AccentColor = "#E9C46A", Description = "सहकारीको प्रकार" },
            new() { TableNumber = 17, Title = "ऋण प्रकार", Category = "वित्तीय", DataSource = "financial_institution", Icon = "bi-cash-coin", AccentColor = "#E9C46A" },
            new() { TableNumber = 18, Title = "बाह्य सहयोग", Category = "वित्तीय", DataSource = "financial_institution", Icon = "bi-hand-thumbs-up", AccentColor = "#E9C46A" },
            // ═══ पूर्वाधार ═══
            new() { TableNumber = 19, Title = "होटल/रिसोर्ट", Category = "होटल तथा पर्यटन", DataSource = "hotel_resort", Icon = "bi-building", AccentColor = "#D4A373" },
            new() { TableNumber = 20, Title = "पुल/पुलेसा", Category = "पूर्वाधार", DataSource = "bridge", Icon = "bi-signpost-2", AccentColor = "#D4A373" },
            new() { TableNumber = 21, Title = "उद्योग", Category = "उद्योग तथा व्यापार", DataSource = "industry", Icon = "bi-gear", AccentColor = "#D4A373" },
            new() { TableNumber = 22, Title = "गैरसरकारी संस्था", Category = "सामाजिक", DataSource = "ngo_club_group", Icon = "bi-people", AccentColor = "#D4A373" },
            new() { TableNumber = 23, Title = "भू-उपयोग", Category = "भू-उपयोग", DataSource = "land_use", Icon = "bi-map", AccentColor = "#52B788" },
            new() { TableNumber = 24, Title = "भू-उपयोग विपद्", Category = "भू-उपयोग", DataSource = "land_use", Icon = "bi-exclamation-triangle", AccentColor = "#E76F51" },
            new() { TableNumber = 25, Title = "प्राकृतिक श्रोत", Category = "प्राकृतिक स्रोत", DataSource = "natural_resource", Icon = "bi-gem", AccentColor = "#52B788" },
            new() { TableNumber = 26, Title = "सांस्कृतिक सम्पदा", Category = "सांस्कृतिक सम्पदा", DataSource = "cultural_heritage", Icon = "bi-palette", AccentColor = "#B5838D" },
            new() { TableNumber = 27, Title = "व्यापार/व्यवसाय", Category = "उद्योग तथा व्यापार", DataSource = "trade_business", Icon = "bi-shop", AccentColor = "#D4A373" },
            new() { TableNumber = 28, Title = "वन विवरण", Category = "वन", DataSource = "forest", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 29, Title = "सडक विवरण", Category = "पूर्वाधार", DataSource = "road", Icon = "bi-road", AccentColor = "#D4A373" },
            new() { TableNumber = 30, Title = "भवन विवरण", Category = "भवन", DataSource = "building", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 31, Title = "भवन प्रयोजन", Category = "भवन", DataSource = "building", Icon = "bi-building", AccentColor = "#B5838D" },
            // ═══ कृषि ═══
            new() { TableNumber = 32, Title = "तालिम विवरण", Category = "कृषि", DataSource = "family_member", Icon = "bi-journal-check", AccentColor = "#52B788", Description = "प्राप्त तालिम विवरण" },
            new() { TableNumber = 33, Title = "दलहन उत्पादन", Category = "कृषि", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 34, Title = "नगदेबाली उत्पादन", Category = "कृषि", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 35, Title = "पोल्ट्री", Category = "कृषि", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 36, Title = "माछापालन", Category = "कृषि", DataSource = "household", Icon = "bi-water", AccentColor = "#52B788" },
            new() { TableNumber = 37, Title = "मौरीपालन", Category = "कृषि", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 38, Title = "कृषि सिंचाइ", Category = "कृषि", DataSource = "household", Icon = "bi-droplet", AccentColor = "#52B788" },
            new() { TableNumber = 39, Title = "जडिबुटी", Category = "कृषि", DataSource = "household", Icon = "bi-flower1", AccentColor = "#52B788" },
            // ═══ वडा कार्यालय ═══
            new() { TableNumber = 42, Title = "वडा कार्यालय विवरण", Category = "वडा तथ्याङ्क", DataSource = "ward_statistics", Icon = "bi-building", AccentColor = "#457B9D" }
        };
    }

    // ═══════════════════════════════════════════════════
    // BASE QUERIES
    // ═══════════════════════════════════════════════════

    private IQueryable<SurveyEntry> SurveyBase() =>
        _db.SurveyEntries.AsNoTracking().Where(s => s.WardNo != null);

    private IQueryable<WardStatistics> WsBase() =>
        _db.WardStatistics.AsNoTracking()
            .Where(ws => ws.SurveyEntry != null && ws.SurveyEntry.WardNo != null);

    private IQueryable<Household> HhBase() =>
        _db.Households.AsNoTracking().Where(h => h.WardNo != null);

    private IQueryable<FamilyMember> FmBase() =>
        _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Household != null && fm.Household.WardNo != null);

    // ═══════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════

    private static string Fmt(int val) => val.ToString("#,##0");
    private static string Fmt(long val) => val.ToString("#,##0");
    private static string Fmt(double? val) => val.HasValue ? ((long)val.Value).ToString("#,##0") : "—";

    // Generic categorical pivot for any institutional entity linked to SurveyEntry
    private async Task<TableViewModel> InstCatTable<TEntity>(
        int tableNo, string title, string category, string? description,
        int? wardNo, string propertyName, int? topN) where TEntity : class
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await _db.Set<TEntity>().AsNoTracking()
            .Where(e => EF.Property<SurveyEntry?>(e, "SurveyEntry") != null
                && EF.Property<SurveyEntry?>(e, "SurveyEntry")!.WardNo != null)
            .Select(e => new
            {
                W = EF.Property<SurveyEntry?>(e, "SurveyEntry")!.WardNo!.Value,
                C = EF.Property<string?>(e, propertyName)
            })
            .ToListAsync();

        var filtered = raw.Where(x => !string.IsNullOrEmpty(x.C)).ToList();

        var grouped = filtered
            .GroupBy(x => (x.W, x.C!))
            .Select(g => (WardNo: g.Key.W, Category: g.Key.Item2, Count: g.Count()))
            .ToList();

        var allCats = grouped
            .GroupBy(x => x.Category)
            .Select(g => (Cat: g.Key, Total: g.Sum(x => x.Count)))
            .OrderByDescending(x => x.Total)
            .ToList();

        var columns = topN.HasValue
            ? allCats.Take(topN.Value).Select(x => x.Cat).ToList()
            : allCats.Select(x => x.Cat).ToList();

        bool hasOther = topN.HasValue && allCats.Count > topN.Value;

        return BuildPivotTable(tableNo, title, category, description,
            typeof(TEntity).Name.ToLower(), wardNos, wardNo, columns, hasOther, grouped);
    }

    // Pivot table builder (same pattern as HouseholdService)
    private TableViewModel BuildPivotTable(
        int tableNo, string title, string category, string? description, string? dataSource,
        List<int> allWardNos, int? filteredWardNo,
        List<string> columns, bool hasOtherColumn,
        List<(int WardNo, string Category, int Count)> rawData)
    {
        var displayWards = filteredWardNo.HasValue
            ? allWardNos.Where(w => w == filteredWardNo.Value).ToList()
            : allWardNos;

        var headers = new List<string> { "वडा नम्बर" };
        headers.AddRange(columns);
        if (hasOtherColumn) headers.Add("अन्य");
        headers.Add("जम्मा");

        var rows = new List<TableRow>();
        foreach (var w in displayWards.OrderBy(w => w))
        {
            var wardData = rawData.Where(x => x.WardNo == w).ToList();
            var cells = new List<string>();
            var rawVals = new List<double?>();
            long total = 0;

            foreach (var col in columns)
            {
                int count = wardData.Where(x => x.Category == col).Sum(x => x.Count);
                cells.Add(Fmt(count));
                rawVals.Add(count);
                total += count;
            }

            if (hasOtherColumn)
            {
                int other = wardData.Where(x => !columns.Contains(x.Category)).Sum(x => x.Count);
                cells.Add(Fmt(other));
                rawVals.Add(other);
                total += other;
            }

            cells.Add(Fmt(total));
            rawVals.Add(total);

            rows.Add(new TableRow
            {
                RowLabel = $"वडा नं. {w}",
                WardNo = w,
                Cells = cells,
                RawValues = rawVals
            });
        }

        var totalCells = new List<string>();
        var totalRawVals = new List<double?>();
        long grandTotal = 0;

        foreach (var col in columns)
        {
            int colTotal = rawData.Where(x => x.Category == col).Sum(x => x.Count);
            totalCells.Add(Fmt(colTotal));
            totalRawVals.Add(colTotal);
            grandTotal += colTotal;
        }

        if (hasOtherColumn)
        {
            int otherTotal = rawData.Where(x => !columns.Contains(x.Category)).Sum(x => x.Count);
            totalCells.Add(Fmt(otherTotal));
            totalRawVals.Add(otherTotal);
            grandTotal += otherTotal;
        }

        totalCells.Add(Fmt(grandTotal));
        totalRawVals.Add(grandTotal);

        return new TableViewModel
        {
            TableNumber = tableNo,
            Title = title,
            Category = category,
            Description = description,
            DataSource = dataSource,
            ColumnHeaders = headers,
            Rows = rows,
            TotalRow = new TableRow
            {
                RowLabel = "जम्मा",
                Cells = totalCells,
                RawValues = totalRawVals
            },
            FilteredWardNo = filteredWardNo,
            TotalWards = allWardNos.Count,
            MaxTableNumber = 43,
            SectionUrl = "/Institution",
            SectionTitle = "संस्थागत तथ्याङ्क"
        };
    }

    // Sum-table builder
    private TableViewModel BuildSumTable(
        int tableNo, string title, string category, string? description,
        string dataSource, List<int> allWardNos, int? filteredWardNo,
        List<string> columnLabels, Dictionary<int, List<double>> wardValues)
    {
        var displayWards = filteredWardNo.HasValue
            ? allWardNos.Where(w => w == filteredWardNo.Value).ToList()
            : allWardNos;

        var headers = new List<string> { "वडा नम्बर" };
        headers.AddRange(columnLabels);
        headers.Add("जम्मा");

        var colCount = columnLabels.Count;
        var totals = new double[colCount + 1];
        var rows = new List<TableRow>();

        foreach (var w in displayWards.OrderBy(w => w))
        {
            var vals = wardValues.ContainsKey(w)
                ? wardValues[w]
                : Enumerable.Repeat(0.0, colCount).ToList();

            double rowTotal = vals.Sum();
            var cells = new List<string>();
            var rawVals = new List<double?>();

            for (int i = 0; i < colCount; i++)
            {
                cells.Add(Fmt(vals[i]));
                rawVals.Add(vals[i]);
                totals[i] += vals[i];
            }
            cells.Add(Fmt(rowTotal));
            rawVals.Add(rowTotal);
            totals[colCount] += rowTotal;

            rows.Add(new TableRow
            {
                RowLabel = $"वडा नं. {w}",
                WardNo = w,
                Cells = cells,
                RawValues = rawVals
            });
        }

        var totalCells = new List<string>();
        var totalRawVals = new List<double?>();
        foreach (var t in totals)
        {
            totalCells.Add(Fmt(t));
            totalRawVals.Add(t);
        }

        return new TableViewModel
        {
            TableNumber = tableNo,
            Title = title,
            Category = category,
            Description = description,
            DataSource = dataSource,
            ColumnHeaders = headers,
            Rows = rows,
            TotalRow = new TableRow
            {
                RowLabel = "जम्मा",
                Cells = totalCells,
                RawValues = totalRawVals
            },
            FilteredWardNo = filteredWardNo,
            TotalWards = allWardNos.Count,
            MaxTableNumber = 43,
            SectionUrl = "/Institution",
            SectionTitle = "संस्थागत तथ्याङ्क"
        };
    }

    private static void EnsureWards(Dictionary<int, List<double>> dict, List<int> wardNos, int colCount)
    {
        foreach (var w in wardNos)
        {
            if (!dict.ContainsKey(w))
                dict[w] = Enumerable.Repeat(0.0, colCount).ToList();
        }
    }

    private static Dictionary<int, List<double>> SumByWard<T>(
        IEnumerable<T> data, Func<T, int> wardSel, Func<T, List<double>> valSel)
    {
        return data.GroupBy(wardSel).ToDictionary(
            g => g.Key,
            g => g.SelectMany(valSel).ToList());
    }

    // ═══════════════════════════════════════════════════
    // TABLE 1: व्यक्तिगत घटना विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable01Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await WsBase()
            .GroupBy(ws => ws.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Birth = g.Sum(x => x.BirthRegistrations ?? 0),
                Marriage = g.Sum(x => x.MarriageRegistrations ?? 0),
                Death = g.Sum(x => x.DeathRegistrations ?? 0),
                Divorce = g.Sum(x => x.DivorceRegistrations ?? 0),
                MigOut = g.Sum(x => x.MigrationOutHouseholds ?? 0),
                MigIn = g.Sum(x => x.MigrationInHouseholds ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "जन्म दर्ता", "विवाह दर्ता", "मृत्यु दर्ता", "सम्बन्धविच्छेद", "प्रवासन (बाहिर)", "प्रवासन (भित्र)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Birth, x.Marriage, x.Death, x.Divorce, x.MigOut, x.MigIn });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(1, "व्यक्तिगत घटना विवरण", "वडा तथ्याङ्क",
            "जन्म, मृत्यु, विवाह, सम्बन्धविच्छेद, प्रवासन दर्ता", "ward_statistics",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 2: सामाजिक सुरक्षा कार्यक्रम
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable02Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await WsBase()
            .GroupBy(ws => ws.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                SeniorM = g.Sum(x => x.SeniorMale ?? 0),
                SeniorF = g.Sum(x => x.SeniorFemale ?? 0),
                SingleWomen = g.Sum(x => x.SingleWomenCount ?? 0),
                FullDisM = g.Sum(x => x.FullDisabledMale ?? 0),
                FullDisF = g.Sum(x => x.FullDisabledFemale ?? 0),
                PartDisM = g.Sum(x => x.PartialDisabledMale ?? 0),
                PartDisF = g.Sum(x => x.PartialDisabledFemale ?? 0),
                ChildM = g.Sum(x => x.ChildProtectionMale ?? 0),
                ChildF = g.Sum(x => x.ChildProtectionFemale ?? 0),
                DalitM = g.Sum(x => x.DalitAllowanceMale ?? 0),
                DalitF = g.Sum(x => x.DalitAllowanceFemale ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "ज्येष्ठ (पुरूष)", "ज्येष्ठ (महिला)", "एकल महिला",
            "पूर्ण अपाङ्ग (पुरूष)", "पूर्ण अपाङ्ग (महिला)",
            "आंशिक अपाङ्ग (पुरूष)", "आंशिक अपाङ्ग (महिला)",
            "बाल सुरक्षा (पुरूष)", "बाल सुरक्षा (महिला)",
            "दलित भत्ता (पुरूष)", "दलित भत्ता (महिला)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.SeniorM, x.SeniorF, x.SingleWomen,
                x.FullDisM, x.FullDisF, x.PartDisM, x.PartDisF,
                x.ChildM, x.ChildF, x.DalitM, x.DalitF
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(2, "सामाजिक सुरक्षा कार्यक्रम", "वडा तथ्याङ्क",
            "ज्येष्ठ नागरिक, अपाङ्गता, बाल सुरक्षा, दलित भत्ता", "ward_statistics",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 4: शिक्षक दरबन्दी विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable04Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .GroupBy(e => e.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                AppFPre = g.Sum(x => x.ApprovedFemalePrePrimary ?? 0),
                AppFBasic = g.Sum(x => x.ApprovedFemaleBasic ?? 0),
                AppFSec = g.Sum(x => x.ApprovedFemaleSecondary ?? 0),
                AppMPre = g.Sum(x => x.ApprovedMalePrePrimary ?? 0),
                AppMBasic = g.Sum(x => x.ApprovedMaleBasic ?? 0),
                AppMSec = g.Sum(x => x.ApprovedMaleSecondary ?? 0),
                WorkFPre = g.Sum(x => x.WorkingFemalePrePrimary ?? 0),
                WorkFBasic = g.Sum(x => x.WorkingFemaleBasic ?? 0),
                WorkFSec = g.Sum(x => x.WorkingFemaleSecondary ?? 0),
                WorkMPre = g.Sum(x => x.WorkingMalePrePrimary ?? 0),
                WorkMBasic = g.Sum(x => x.WorkingMaleBasic ?? 0),
                WorkMSec = g.Sum(x => x.WorkingMaleSecondary ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "स्वीकृत महिला (पूर्व-प्रा)", "स्वीकृत महिला (आधार)", "स्वीकृत महिला (मा.वि.)",
            "स्वीकृत पुरूष (पूर्व-प्रा)", "स्वीकृत पुरूष (आधार)", "स्वीकृत पुरूष (मा.वि.)",
            "कार्यरत महिला (पूर्व-प्रा)", "कार्यरत महिला (आधार)", "कार्यरत महिला (मा.वि.)",
            "कार्यरत पुरूष (पूर्व-प्रा)", "कार्यरत पुरूष (आधार)", "कार्यरत पुरूष (मा.वि.)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.AppFPre, x.AppFBasic, x.AppFSec,
                x.AppMPre, x.AppMBasic, x.AppMSec,
                x.WorkFPre, x.WorkFBasic, x.WorkFSec,
                x.WorkMPre, x.WorkMBasic, x.WorkMSec
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(4, "शिक्षक दरबन्दी विवरण", "शैक्षिक संस्था",
            "स्वीकृत तथा कार्यरत शिक्षक/शिक्षिका विवरण", "educational_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 5: विद्यार्थी संख्या
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable05Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .GroupBy(e => e.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                FPre = g.Sum(x => x.FemaleStudentsPrePrimary ?? 0),
                FBasic = g.Sum(x => x.FemaleStudentsBasic ?? 0),
                FSec = g.Sum(x => x.FemaleStudentsSecondary ?? 0),
                MPre = g.Sum(x => x.MaleStudentsPrePrimary ?? 0),
                MBasic = g.Sum(x => x.MaleStudentsBasic ?? 0),
                MSec = g.Sum(x => x.MaleStudentsSecondary ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "महिला (पूर्व-प्रा)", "महिला (आधार)", "महिला (मा.वि.)",
            "पुरूष (पूर्व-प्रा)", "पुरूष (आधार)", "पुरूष (मा.वि.)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.FPre, x.FBasic, x.FSec, x.MPre, x.MBasic, x.MSec });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(5, "विद्यार्थी संख्या", "शैक्षिक संस्था",
            "तह अनुसार विद्यार्थी विवरण", "educational_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 6: शैक्षिक उपलब्धी/उतिर्ण दर
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable06Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .GroupBy(e => e.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Ach8 = g.Sum(x => x.AchievementClass8 ?? 0),
                Ach10 = g.Sum(x => x.AchievementClass10 ?? 0),
                Ach12 = g.Sum(x => x.AchievementClass12 ?? 0),
                Pass8 = g.Sum(x => x.PassRateClass8 ?? 0),
                Pass10 = g.Sum(x => x.PassRateClass10 ?? 0),
                Pass12 = g.Sum(x => x.PassRateClass12 ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "उपलब्धी कक्षा ८", "उपलब्धी कक्षा १०", "उपलब्धी कक्षा १२",
            "उतिर्ण दर कक्षा ८", "उतिर्ण दर कक्षा १०", "उतिर्ण दर कक्षा १२"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Ach8, x.Ach10, x.Ach12, x.Pass8, x.Pass10, x.Pass12 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(6, "शैक्षिक उपलब्धी/उतिर्ण दर", "शैक्षिक संस्था",
            "कक्षा ८, १०, १२ उतिर्ण दर तथा उपलब्धी", "educational_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 7: छात्रवृत्ती विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable07Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .GroupBy(e => e.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                DalitM = g.Sum(x => x.ScholarshipDalitMale ?? 0),
                DalitF = g.Sum(x => x.ScholarshipDalitFemale ?? 0),
                DisabledM = g.Sum(x => x.ScholarshipDisabledMale ?? 0),
                DisabledF = g.Sum(x => x.ScholarshipDisabledFemale ?? 0),
                TalentedM = g.Sum(x => x.ScholarshipTalentedMale ?? 0),
                TalentedF = g.Sum(x => x.ScholarshipTalentedFemale ?? 0),
                PoorM = g.Sum(x => x.ScholarshipPoorMale ?? 0),
                PoorF = g.Sum(x => x.ScholarshipPoorFemale ?? 0),
                MuslimM = g.Sum(x => x.ScholarshipMuslimMale ?? 0),
                MuslimF = g.Sum(x => x.ScholarshipMuslimFemale ?? 0),
                OtherM = g.Sum(x => x.ScholarshipOtherMale ?? 0),
                OtherF = g.Sum(x => x.ScholarshipOtherFemale ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "दलित (पुरूष)", "दलित (महिला)", "अपाङ्ग (पुरूष)", "अपाङ्ग (महिला)",
            "प्रतिभावान (पुरूष)", "प्रतिभावान (महिला)", "गरिब (पुरूष)", "गरिब (महिला)",
            "मुस्लिम (पुरूष)", "मुस्लिम (महिला)", "अन्य (पुरूष)", "अन्य (महिला)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.DalitM, x.DalitF, x.DisabledM, x.DisabledF,
                x.TalentedM, x.TalentedF, x.PoorM, x.PoorF,
                x.MuslimM, x.MuslimF, x.OtherM, x.OtherF
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(7, "छात्रवृत्ती विवरण", "शैक्षिक संस्था",
            "छात्रवृत्ती प्राप्त विद्यार्थी विवरण", "educational_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 8: विद्यालय पूर्वाधार
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable08Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .GroupBy(e => e.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Kutchi = g.Sum(x => x.KutchiBuildingCount ?? 0),
                Pakki = g.Sum(x => x.PakkiBuildingCount ?? 0),
                Rooms = g.Sum(x => x.PakkiRoomCount ?? 0 + x.KutchiRoomCount ?? 0),
                Schools = g.Count()
            })
            .ToListAsync();

        // Count yes/no for library, lab, playground, water, etc.
        var infraRaw = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .Select(e => new
            {
                W = e.SurveyEntry!.WardNo!.Value,
                Lib = e.HasLibrary,
                Lab = e.HasScienceLab,
                Play = e.HasPlayground,
                Water = e.HasCleanDrinkingWater,
                Disabled = e.HasDisabledResourceClass,
                Internet = e.InternetAvailable
            })
            .ToListAsync();

        var infraCounts = infraRaw
            .GroupBy(x => x.W)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Lib = g.Count(x => x.Lib == "छ" || x.Lib == "हो"),
                    Lab = g.Count(x => x.Lab == "छ" || x.Lab == "हो"),
                    Play = g.Count(x => x.Play == "छ" || x.Play == "हो"),
                    Water = g.Count(x => x.Water == "छ" || x.Water == "हो"),
                    Disabled = g.Count(x => x.Disabled == "छ" || x.Disabled == "हो"),
                    Internet = g.Count(x => x.Internet == "छ" || x.Internet == "हो")
                });

        var labels = new List<string>
        {
            "विद्यालय संख्या", "कच्ची भवन", "पक्की भवन", "कोठा संख्या",
            "पुस्तकालय भएका", "विज्ञान प्रयोगशाला", "खेलमैदान",
            "स्वच्छ खानेपानी", "अपाङ्ग स्रोत कक्षा", "इन्टरनेट"
        };

        var wardValues = new Dictionary<int, List<double>>();
        foreach (var d in data)
        {
            var infra = infraCounts.GetValueOrDefault(d.W);
            wardValues[d.W] = new List<double>
            {
                d.Schools, d.Kutchi, d.Pakki, d.Rooms,
                infra.Lib, infra.Lab, infra.Play,
                infra.Water, infra.Disabled, infra.Internet
            };
        }

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(8, "विद्यालय पूर्वाधार", "शैक्षिक संस्था",
            "भवन, पुस्तकालय, प्रयोगशाला लगायत पूर्वाधार", "educational_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 10: स्वास्थ्य सेवा सुविधा
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable10Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.HealthInstitutions.AsNoTracking()
            .Where(h => h.SurveyEntry != null && h.SurveyEntry.WardNo != null)
            .GroupBy(h => h.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Maternity = g.Sum(x => x.HasMaternity ?? 0),
                Lab = g.Sum(x => x.HasLab ?? 0),
                Clinic = g.Sum(x => x.HasClinic ?? 0),
                Xray = g.Sum(x => x.HasXray ?? 0),
                Vaccine = g.Sum(x => x.HasVaccination ?? 0),
                CtScan = g.Sum(x => x.HasCtScan ?? 0),
                Counsel = g.Sum(x => x.HasCounseling ?? 0),
                SafeMother = g.Sum(x => x.HasSafeMotherhood ?? 0),
                Blood = g.Sum(x => x.HasBloodTransfusion ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "प्रसुति", "प्रयोगशाला", "ओपीडी", "एक्स-रे",
            "खोप", "सिटी स्क्यान", "परामर्श", "सुरक्षित मातृत्व", "रक्त परिसञ्चार"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Maternity, x.Lab, x.Clinic, x.Xray,
                x.Vaccine, x.CtScan, x.Counsel, x.SafeMother, x.Blood
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(10, "स्वास्थ्य सेवा सुविधा", "स्वास्थ्य",
            "स्वास्थ्य सेवा सुविधा उपलब्धता", "health_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 11: खोप विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable11Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.HealthInstitutions.AsNoTracking()
            .Where(h => h.SurveyEntry != null && h.SurveyEntry.WardNo != null)
            .GroupBy(h => h.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Bcg = g.Sum(x => x.BcgVaccinated ?? 0),
                Dpt = g.Sum(x => x.DptHepbVaccinated ?? 0),
                Polio = g.Sum(x => x.Polio3rdVaccinated ?? 0),
                Mr = g.Sum(x => x.MeaslesRubellaVaccinated ?? 0),
                Je = g.Sum(x => x.JeVaccinated ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "बिसिजी", "डिपिटी-हेपबी", "पोलियो (तेस्रो)", "दादुरा-रुबेला", "जापानीज इन्सेफलाइटिस" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Bcg, x.Dpt, x.Polio, x.Mr, x.Je });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(11, "खोप विवरण", "स्वास्थ्य",
            "खोप लगाएका बालबालिकाको विवरण", "health_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 12: प्रसुति/मातृ स्वास्थ्य
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable12Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.HealthInstitutions.AsNoTracking()
            .Where(h => h.SurveyEntry != null && h.SurveyEntry.WardNo != null)
            .GroupBy(h => h.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Delivery = g.Sum(x => x.DeliveryLastYear ?? 0),
                InstDel = g.Sum(x => x.InstitutionalDelivery ?? 0),
                SkillHome = g.Sum(x => x.SkilledHomeDelivery ?? 0),
                UnskillHome = g.Sum(x => x.UnskilledHomeDelivery ?? 0),
                MatDeath = g.Sum(x => x.MaternalDeath6Wk ?? 0),
                LiveBirth = g.Sum(x => x.LiveBirths ?? 0),
                StillBirth = g.Sum(x => x.StillBirths ?? 0),
                NeoDeath = g.Sum(x => x.NeonatalDeath28D ?? 0),
                Tt1 = g.Sum(x => x.Tt1Pregnant ?? 0),
                Tt2 = g.Sum(x => x.Tt2Pregnant ?? 0),
                Anc4 = g.Sum(x => x.Anc4VisitsPregnant ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "कुल प्रसुति", "संस्थागत प्रसुति", "दक्ष गृह प्रसुति", "अदक्ष गृह प्रसुति",
            "मातृ मृत्यु (६ हप्ता)", "जीवित जन्म", "मृत जन्म", "नवजात मृत्यु (२८ दिन)",
            "टिटी १", "टिटी २", "एएनसी ४ भ्रमण"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Delivery, x.InstDel, x.SkillHome, x.UnskillHome,
                x.MatDeath, x.LiveBirth, x.StillBirth, x.NeoDeath,
                x.Tt1, x.Tt2, x.Anc4
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(12, "प्रसुति/मातृ स्वास्थ्य", "स्वास्थ्य",
            "प्रसुति, जन्म, मातृ मृत्यु विवरण", "health_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 13: परिवार नियोजन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable13Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.HealthInstitutions.AsNoTracking()
            .Where(h => h.SurveyEntry != null && h.SurveyEntry.WardNo != null)
            .GroupBy(h => h.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                PermF = g.Sum(x => x.FpPermanentFemale ?? 0),
                TempF = g.Sum(x => x.FpTemporaryFemale ?? 0),
                PermM = g.Sum(x => x.FpPermanentMale ?? 0),
                TempM = g.Sum(x => x.FpTemporaryMale ?? 0),
                Women = g.Sum(x => x.Women1548 ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "स्थायी महिला", "अस्थायी महिला", "स्थायी पुरूष", "अस्थायी पुरूष",
            "प्रजनन उमेर महिला (१५-४८)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.PermF, x.TempF, x.PermM, x.TempM, x.Women });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(13, "परिवार नियोजन", "स्वास्थ्य",
            "परिवार नियोजन साधन प्रयोग विवरण", "health_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 16: सहकारी प्रकार
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable16Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.FinancialInstitutions.AsNoTracking()
            .Where(f => f.SurveyEntry != null && f.SurveyEntry.WardNo != null)
            .GroupBy(f => f.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Agri = g.Sum(x => x.CoopAgriSavings ?? 0),
                Loan = g.Sum(x => x.CoopLoan ?? 0),
                Multi = g.Sum(x => x.CoopMultipurpose ?? 0),
                Other = g.Sum(x => x.CoopOther ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "कृषि बचत", "ऋण", "बहुउद्देश्यीय", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Agri, x.Loan, x.Multi, x.Other });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(16, "सहकारी प्रकार", "वित्तीय",
            "सहकारी संस्थाको प्रकार अनुसार", "financial_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 17: ऋण प्रकार
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable17Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.FinancialInstitutions.AsNoTracking()
            .Where(f => f.SurveyEntry != null && f.SurveyEntry.WardNo != null)
            .GroupBy(f => f.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Agri = g.Sum(x => x.LoanAgriculture ?? 0),
                Livestock = g.Sum(x => x.LoanLivestock ?? 0),
                Building = g.Sum(x => x.LoanBuilding ?? 0),
                Hire = g.Sum(x => x.LoanHirePurchase ?? 0),
                Medical = g.Sum(x => x.LoanMedical ?? 0),
                House = g.Sum(x => x.LoanHouse ?? 0),
                Other = g.Sum(x => x.LoanOther ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "कृषि", "पशुपालन", "निर्माण", "किस्तबन्धा", "स्वास्थ्य", "घर", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Agri, x.Livestock, x.Building, x.Hire, x.Medical, x.House, x.Other });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(17, "ऋण प्रकार", "वित्तीय",
            "ऋणको प्रकार अनुसार वित्तीय संस्था", "financial_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 18: बाह्य सहयोग
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable18Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.FinancialInstitutions.AsNoTracking()
            .Where(f => f.SurveyEntry != null && f.SurveyEntry.WardNo != null)
            .GroupBy(f => f.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Govt = g.Sum(x => x.HasGovtSupport ?? 0),
                Ngo = g.Sum(x => x.HasNgoSupport ?? 0),
                None = g.Sum(x => x.NoSupport ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "सरकारी सहयोग", "गैरसरकारी सहयोग", "कुनै सहयोग छैन" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Govt, x.Ngo, x.None });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(18, "बाह्य सहयोग", "वित्तीय",
            "बाह्य सहयोग प्राप्त वित्तीय संस्था", "financial_institution",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 24: भू-उपयोग विपद्
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable24Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.LandUses.AsNoTracking()
            .Where(l => l.SurveyEntry != null && l.SurveyEntry.WardNo != null)
            .GroupBy(l => l.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Flood = g.Sum(x => x.DisasterFlood ?? 0),
                Landslide = g.Sum(x => x.DisasterLandslide ?? 0),
                RiverCut = g.Sum(x => x.DisasterRiverCut ?? 0),
                Inundation = g.Sum(x => x.DisasterInundation ?? 0),
                Drought = g.Sum(x => x.DisasterDrought ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "बाढी", "पहिरो", "नदी कटान", "डुबान", "खडेरी" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Flood, x.Landslide, x.RiverCut, x.Inundation, x.Drought });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(24, "भू-उपयोग विपद्", "भू-उपयोग",
            "प्राकृतिक विपद् जोखिम क्षेत्र", "land_use",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 31: भवन प्रयोजन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable31Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.Buildings.AsNoTracking()
            .Where(b => b.SurveyEntry != null && b.SurveyEntry.WardNo != null)
            .GroupBy(b => b.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Admin = g.Sum(x => x.PurposeAdmin ?? 0),
                Education = g.Sum(x => x.PurposeEducation ?? 0),
                Health = g.Sum(x => x.PurposeHealth ?? 0),
                Agriculture = g.Sum(x => x.PurposeAgriculture ?? 0),
                Cooperative = g.Sum(x => x.PurposeCooperative ?? 0),
                Assembly = g.Sum(x => x.PurposeAssembly ?? 0),
                Other = g.Sum(x => x.PurposeOther ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "प्रशासन", "शिक्षा", "स्वास्थ्य", "कृषि", "सहकारी", "सभा", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Admin, x.Education, x.Health, x.Agriculture, x.Cooperative, x.Assembly, x.Other });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(31, "भवन प्रयोजन", "भवन",
            "भवनको प्रयोजन अनुसार विवरण", "building",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 32: तालिम विवरण (family_member training)
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable32Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await FmBase()
            .Where(fm => fm.HasVocationalTraining != null && fm.HasVocationalTraining != "")
            .Select(fm => new
            {
                W = fm.Household!.WardNo!.Value,
                Sewing = fm.TrainingSewing,
                Construction = fm.TrainingConstruction,
                Carpentry = fm.TrainingCarpentry,
                Metalwork = fm.TrainingMetalwork,
                Electrical = fm.TrainingElectrical,
                Plumbing = fm.TrainingPlumbing,
                Automobile = fm.TrainingAutomobile,
                Agriculture = fm.TrainingAgriculture,
                PublicHealth = fm.TrainingPublicHealth,
                AnimalHealth = fm.TrainingAnimalHealth,
                Tourism = fm.TrainingTourism,
                Arts = fm.TrainingArts,
                Other = fm.TrainingOther
            })
            .ToListAsync();

        var grouped = raw.GroupBy(x => x.W).ToDictionary(
            g => g.Key,
            g => new List<double>
            {
                g.Sum(x => x.Sewing ?? 0), g.Sum(x => x.Construction ?? 0),
                g.Sum(x => x.Carpentry ?? 0), g.Sum(x => x.Metalwork ?? 0),
                g.Sum(x => x.Electrical ?? 0), g.Sum(x => x.Plumbing ?? 0),
                g.Sum(x => x.Automobile ?? 0), g.Sum(x => x.Agriculture ?? 0),
                g.Sum(x => x.PublicHealth ?? 0), g.Sum(x => x.AnimalHealth ?? 0),
                g.Sum(x => x.Tourism ?? 0), g.Sum(x => x.Arts ?? 0),
                g.Sum(x => x.Other ?? 0)
            });

        var labels = new List<string>
        {
            "सिलाइ", "निर्माण", "काठ", "धातु", "विद्युत",
            "प्लम्बिङ", "अटोमोबाइल", "कृषि", "सार्वजनिक स्वास्थ्य",
            "पशु स्वास्थ्य", "पर्यटन", "कला", "अन्य"
        };

        EnsureWards(grouped, wardNos, labels.Count);

        return BuildSumTable(32, "तालिम विवरण", "कृषि",
            "प्राप्त तालिम विवरण", "family_member",
            wardNos, wardNo, labels, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 33: दलहन उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable33Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Black = g.Sum(h => h.PulseBlackDal ?? 0),
                Rahar = g.Sum(h => h.PulseRahar ?? 0),
                Masuro = g.Sum(h => h.PulseMasuro ?? 0),
                Chana = g.Sum(h => h.PulseChana ?? 0),
                Simi = g.Sum(h => h.PulseSimi ?? 0),
                Bhattmas = g.Sum(h => h.PulseBhattmas ?? 0),
                Mung = g.Sum(h => h.PulseMung ?? 0),
                Khesari = g.Sum(h => h.PulseKhesari ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "मास", "रहर", "मसुरो", "चना", "सिमी", "भटमास", "मुग", "खेसारी" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Black, x.Rahar, x.Masuro, x.Chana, x.Simi, x.Bhattmas, x.Mung, x.Khesari });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(33, "दलहन उत्पादन", "कृषि",
            "दलहन बाली उत्पादन विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 34: नगदेबाली उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable34Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Ginger = g.Sum(h => h.CashGingerTurmeric ?? 0),
                Onion = g.Sum(h => h.CashOnionGarlic ?? 0),
                Mushroom = g.Sum(h => h.CashMushroom ?? 0),
                Sugarcane = g.Sum(h => h.CashSugarcane ?? 0),
                Chili = g.Sum(h => h.CashChili ?? 0),
                Peanut = g.Sum(h => h.CashPeanut ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "अदुवा/बेसार", "प्याज/लसुन", "च्याउ", "उखु", "खुर्सानी", "बदाम" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Ginger, x.Onion, x.Mushroom, x.Sugarcane, x.Chili, x.Peanut });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(34, "नगदेबाली उत्पादन", "कृषि",
            "नगदेबाली उत्पादन विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 35: पोल्ट्री
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable35Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Count = g.Sum(h => h.PoultryCount ?? 0),
                Egg = g.Sum(h => h.PoultryEggAnnual ?? 0),
                Meat = g.Sum(h => h.PoultryMeatSoldKg ?? 0),
                Income = g.Sum(h => h.PoultryAnnualIncome ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "कुखुरा संख्या", "अण्डा (वार्षिक)", "मासु (के.जी.)", "आम्दानी (रु.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Count, x.Egg, x.Meat, x.Income });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(35, "पोल्ट्री", "कृषि",
            "कुखुरापालन विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 36: माछापालन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable36Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Ponds = g.Sum(h => h.FishPondCount ?? 0),
                Area = g.Sum(h => h.FishPondArea ?? 0),
                Prod = g.Sum(h => h.FishProductionKg ?? 0),
                Income = g.Sum(h => h.FishAnnualIncome ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "पोखरी संख्या", "क्षेत्रफल", "उत्पादन (के.जी.)", "आम्दानी (रु.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Ponds, x.Area, x.Prod, x.Income });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(36, "माछापालन", "कृषि",
            "माछापालन विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 37: मौरीपालन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable37Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Hives = g.Sum(h => h.BeeHiveCount ?? 0),
                Honey = g.Sum(h => h.HoneyProductionKg ?? 0),
                Income = g.Sum(h => h.HoneyAnnualIncome ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "चौतारी संख्या", "मह उत्पादन (के.जी.)", "आम्दानी (रु.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Hives, x.Honey, x.Income });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(37, "मौरीपालन", "कृषि",
            "मौरीपालन तथा मह उत्पादन", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 38: कृषि सिंचाइ
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable38Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Canal = g.Sum(h => h.IrrigationCanal ?? 0),
                Tubewell = g.Sum(h => h.IrrigationTubewell ?? 0),
                Electric = g.Sum(h => h.IrrigationElectric ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "नहर", "ट्युबवेल", "विद्युत" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Canal, x.Tubewell, x.Electric });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(38, "कृषि सिंचाइ", "कृषि",
            "सिंचाइको साधन अनुसार विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 39: जडिबुटी (institutional — same data as HH table 65)
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable39Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                H1 = g.Sum(h => h.HerbHarroBarro ?? 0),
                H2 = g.Sum(h => h.HerbGurjo ?? 0),
                H3 = g.Sum(h => h.HerbTulsi ?? 0),
                H4 = g.Sum(h => h.HerbAmla ?? 0),
                H5 = g.Sum(h => h.HerbSarpagandha ?? 0),
                H6 = g.Sum(h => h.HerbAjwain ?? 0),
                H7 = g.Sum(h => h.HerbSutli ?? 0),
                H8 = g.Sum(h => h.HerbAloeVera ?? 0),
                H9 = g.Sum(h => h.HerbBojho ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "हर्रो/बर्रो", "गुर्जो", "तुलसी", "अमला", "सर्पगन्धा",
            "अजवाइन", "सुतली", "अलोभेरा", "बोझो"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.H1, x.H2, x.H3, x.H4, x.H5, x.H6, x.H7, x.H8, x.H9 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(39, "जडिबुटी", "कृषि",
            "जडिबुटी प्रजाति अनुसार विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 40: बाल क्लब/बालमैत्री
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable40Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .Select(e => new
            {
                W = e.SurveyEntry!.WardNo!.Value,
                Club = e.HasChildClub,
                Friendly = e.ChildFriendlyTeaching
            })
            .ToListAsync();

        var grouped = raw.GroupBy(x => x.W).ToDictionary(
            g => g.Key,
            g => new List<double>
            {
                g.Count(x => x.Club == "छ" || x.Club == "हो"),
                g.Count(x => x.Friendly == "छ" || x.Friendly == "हो")
            });

        var labels = new List<string> { "बाल क्लब भएका विद्यालय", "बालमैत्री शिक्षण" };

        EnsureWards(grouped, wardNos, labels.Count);

        return BuildSumTable(40, "बाल क्लब/बालमैत्री", "शैक्षिक संस्था",
            "बाल क्लब र बालमैत्री शिक्षण विवरण", "educational_institution",
            wardNos, wardNo, labels, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 41: विद्यालय सुधार योजना
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable41Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await _db.EducationalInstitutions.AsNoTracking()
            .Where(e => e.SurveyEntry != null && e.SurveyEntry.WardNo != null)
            .Select(e => new
            {
                W = e.SurveyEntry!.WardNo!.Value,
                Plan = e.SchoolImprovementPlan,
                SmcForm = e.SmcFormationStatus,
                SmcMeet = e.SmcRegularMeeting
            })
            .ToListAsync();

        var grouped = raw.GroupBy(x => x.W).ToDictionary(
            g => g.Key,
            g => new List<double>
            {
                g.Count(x => x.Plan == "छ" || x.Plan == "हो"),
                g.Count(x => x.SmcForm == "छ" || x.SmcForm == "हो"),
                g.Count(x => x.SmcMeet == "छ" || x.SmcMeet == "हो")
            });

        var labels = new List<string>
        {
            "सुधार योजना भएका विद्यालय",
            "SMC गठन भएका",
            "SMC नियमित बैठक"
        };

        EnsureWards(grouped, wardNos, labels.Count);

        return BuildSumTable(41, "विद्यालय सुधार योजना", "शैक्षिक संस्था",
            "विद्यालय सुधार योजना तथा SMC विवरण", "educational_institution",
            wardNos, wardNo, labels, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 42: वडा कार्यालय विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable42Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await WsBase()
            .GroupBy(ws => ws.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Birth = g.Sum(x => x.BirthRegistrations ?? 0),
                Marriage = g.Sum(x => x.MarriageRegistrations ?? 0),
                Death = g.Sum(x => x.DeathRegistrations ?? 0),
                Divorce = g.Sum(x => x.DivorceRegistrations ?? 0),
                MigOut = g.Sum(x => x.MigrationOutHouseholds ?? 0),
                MigIn = g.Sum(x => x.MigrationInHouseholds ?? 0),
                Citizen = g.Sum(x => x.CitizenshipRecommendations ?? 0),
                Passport = g.Sum(x => x.PassportRecommendations ?? 0)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "जन्म दर्ता", "विवाह दर्ता", "मृत्यु दर्ता", "सम्बन्धविच्छेद",
            "प्रवासन (बाहिर)", "प्रवासन (भित्र)",
            "नागरिकता सिफारिस", "राहदानी सिफारिस"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Birth, x.Marriage, x.Death, x.Divorce,
                x.MigOut, x.MigIn, x.Citizen, x.Passport
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(42, "वडा कार्यालय विवरण", "वडा तथ्याङ्क",
            "वडा कार्यालयमा उपलब्ध सेवा विवरण", "ward_statistics",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 43: FCHV विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable43Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await _db.HealthInstitutions.AsNoTracking()
            .Where(h => h.SurveyEntry != null && h.SurveyEntry.WardNo != null)
            .GroupBy(h => h.SurveyEntry!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Fchv = g.Sum(x => x.FchvCount ?? 0)
            })
            .ToListAsync();

        var labels = new List<string> { "FCHV संख्या" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Fchv });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(43, "FCHV विवरण", "स्वास्थ्य",
            "महिला स्वास्थ्य स्वयम्सेविका संख्या", "health_institution",
            wardNos, wardNo, labels, wardValues);
    }
}
