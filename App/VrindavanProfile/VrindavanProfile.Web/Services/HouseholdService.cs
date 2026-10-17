using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Caching;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

public class HouseholdService : IHouseholdService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public HouseholdService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // ═══════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════

    public async Task<TableViewModel?> GetTableAsync(int tableId, int? wardNo = null)
    {
        var key = CacheKeys.HouseholdTable(tableId, wardNo);

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
            1 => await GetTable01Async(wardNo),
            2 => await GetTable02Async(wardNo),
            3 => await GetTable03Async(wardNo),
            4 => await FmCatTable(4, "जातजाती अनुसार जनसंख्या", "जनसंख्या", "जातजाती र लिङ्ग अनुसार विवरण", wardNo, "Caste", 15),
            5 => await FmCatTable(5, "मातृभाषाको आधारमा जनसंख्या", "जनसंख्या", null, wardNo, "Language", 12),
            6 => await FmCatTable(6, "धर्मको आधारमा जनसंख्या", "जनसंख्या", null, wardNo, "Religion", null),
            7 => await FmCatTable(7, "साक्षरताको स्थिति", "शिक्षा तथा स्वास्थ्य", null, wardNo, "LiteracyStatus", null),
            8 => await FmCatTable(8, "शैक्षिकस्तर अनुसार जनसंख्या", "शिक्षा तथा स्वास्थ्य", null, wardNo, "EducationLevel", 12),
            9 => await FmCatTable(9, "विशेषज्ञको विवरण अनुसार जनसंख्या", "शिक्षा तथा स्वास्थ्य", null, wardNo, "ExpertiseArea", null),
            10 => await FmCatTable(10, "सरकारी जागिर विवरण", "शिक्षा तथा स्वास्थ्य", null, wardNo, "GovtJobType", null),
            11 => await FmCatTable(11, "बसोबासको अवस्था", "जनसंख्या", null, wardNo, "ResidenceStatus", null),
            12 => await FmCatTable(12, "अपाङ्गताको स्थिति", "शिक्षा तथा स्वास्थ्य", null, wardNo, "DisabilityStatus", null),
            13 => await FmCatTable(13, "वैवाहिक स्थिति", "जनसंख्या", null, wardNo, "MaritalStatus", null),
            14 => await FmCatTable(14, "प्रमुख पेशा अनुसार जनसंख्या", "जनसंख्या", null, wardNo, "Occupation", 15),
            15 => await GetTable15Async(wardNo),
            // ═══ शिक्षा तथा स्वास्थ्य ═══
            16 => await FmCatTable(16, "बिमा गरेका घरपरिवार", "शिक्षा तथा स्वास्थ्य", null, wardNo, "HasInsurance", null),
            17 => await FmCatTable(17, "परिवारका सदस्यहरुको स्वास्थ्य", "शिक्षा तथा स्वास्थ्य", null, wardNo, "HealthStatus", null),
            18 => await HhCatTable(18, "विरामी परेमा पहिले जाने स्थान", "शिक्षा तथा स्वास्थ्य", null, wardNo, "FirstHealthVisit", null),
            21 => await FmCatTable(21, "सामाजिक सुरक्षा भत्ता", "शिक्षा तथा स्वास्थ्य", null, wardNo, "SocialSecurityStatus", null),
            56 => await GetTable56Async(wardNo),
            57 => await HhCatTable(57, "बालबालिका काम", "शिक्षा तथा स्वास्थ्य", null, wardNo, "ChildLabor", null),

            // ═══ आर्थिक अवस्था ═══
            19 => await HhCatTable(19, "बैंक तथा वित्तिय संस्थामा खाता", "आर्थिक अवस्था", null, wardNo, "HasBankAccount", null),
            20 => await GetTable20Async(wardNo),
            22 => await GetTable22Async(wardNo),
            37 => await GetTable37Async(wardNo),
            38 => await GetTable38Async(wardNo),
            39 => await HhCatTable(39, "खाद्य सुरक्षा", "आर्थिक अवस्था", "खाद्य आपूर्ति पर्याप्त महिना", wardNo, "FoodSufficiencyMonths", null),

            // ═══ सुविधा तथा पूर्वाधार ═══
            23 => await HhCatTable(23, "बजार पुग्न लाग्ने समय", "सुविधा तथा पूर्वाधार", null, wardNo, "MarketTravelTime", null),
            24 => await HhCatTable(24, "स्वास्थ्य संस्था पुग्न लाग्ने समय", "सुविधा तथा पूर्वाधार", null, wardNo, "HospitalTravelTime", null),
            25 => await HhCatTable(25, "वडा कार्यालय पुग्न लाग्ने समय", "सुविधा तथा पूर्वाधार", null, wardNo, "SchoolTravelTime", null),
            26 => await HhCatTable(26, "नगरपालिका कार्यालय पुग्न लाग्ने समय", "सुविधा तथा पूर्वाधार", null, wardNo, "DistanceFromCenter2", null),
            27 => await HhCatTable(27, "नियमित गाडी चल्ने बाटो", "सुविधा तथा पूर्वाधार", null, wardNo, "RoadType", null),
            28 => await HhCatTable(28, "सडकमा पहुँच", "सुविधा तथा पूर्वाधार", null, wardNo, "DistanceFromCenter", null),
            29 => await HhCatTable(29, "खानेपानीको स्रोत", "सुविधा तथा पूर्वाधार", null, wardNo, "DrinkingWaterSource", null),
            30 => await HhCatTable(30, "खाना पकाउने इन्धन", "सुविधा तथा पूर्वाधार", null, wardNo, "CookingFuelPrimary", null),
            31 => await HhCatTable(31, "चुलोको प्रकार", "सुविधा तथा पूर्वाधार", null, wardNo, "StoveType", null),
            32 => await HhCatTable(32, "बत्तीको स्रोत", "सुविधा तथा पूर्वाधार", null, wardNo, "LightSource", null),
            33 => await HhCatTable(33, "फोहोर व्यवस्थापन", "सुविधा तथा पूर्वाधार", null, wardNo, "WasteManagement", null),
            34 => await HhCatTable(34, "शौचालयको प्रकार", "सुविधा तथा पूर्वाधार", null, wardNo, "ToiletType", null),
            35 => await GetTable35Async(wardNo),

            // ═══ कृषि तथा पशुपालन ═══
            36 => await HhCatTable(36, "कृषि जग्गा प्रयोग", "कृषि तथा पशुपालन", null, wardNo, "HasAgricultureLand", null),
            40 => await GetTable40Async(wardNo),
            58 => await GetTable58Async(wardNo),
            59 => await GetTable59Async(wardNo),
            60 => await GetTable60Async(wardNo),
            61 => await GetTable61Async(wardNo),
            62 => await GetTable62Async(wardNo),
            65 => await GetTable65Async(wardNo),

            // ═══ आवास ═══
            41 => await HhCatTable(41, "बसोबासको प्रकृति", "आवास", null, wardNo, "ResidenceType", null),
            42 => await GetTable42Async(wardNo),
            43 => await GetTable43Async(wardNo),
            44 => await GetTable44Async(wardNo),
            45 => await HdCatTable(45, "स्वामित्व", "आवास", null, wardNo, "OwnershipType", null),
            46 => await GetTable46Async(wardNo),
            47 => await HdCatTable(47, "छाना सम्बन्धी", "आवास", null, wardNo, "RoofType", null),
            48 => await GetTable48Async(wardNo),
            49 => await HdCatTable(49, "भवन मापदण्ड", "आवास", null, wardNo, "MeetsBuildingCode", null),
            50 => await HdCatTable(50, "घरको जग", "आवास", null, wardNo, "FoundationType", null),

            // ═══ जोखिम तथा सम्पत्ति ═══
            51 => await HhCatTable(51, "भूकम्प क्षति", "जोखिम तथा सम्पत्ति", null, wardNo, "EarthquakeResistant", null),
            52 => await HhCatTable(52, "अनुदान पाए", "जोखिम तथा सम्पत्ति", null, wardNo, "ReceivedSubsidy", null),
            53 => await HhCatTable(53, "अनुदान किस्ता", "जोखिम तथा सम्पत्ति", null, wardNo, "SubsidyInstallments", null),
            54 => await HhCatTable(54, "अनुदानले घर बने", "जोखिम तथा सम्पत्ति", null, wardNo, "HouseRebuilt", null),
            55 => await HhCatTable(55, "प्राकृतिक प्रकोप जोखिम", "जोखिम तथा सम्पत्ति", null, wardNo, "DisasterRisk", null),

            // ═══ जनसंख्या (बैदेशिक) ═══
            63 => await FmCatTable(63, "बैदेशिक रोजगार", "जनसंख्या", "विदेशमा रहेका देश अनुसार", wardNo, "AbroadCountry", 10),
            64 => await FmCatTable(64, "विदेश बस्नुको कारण", "जनसंख्या", null, wardNo, "AbroadPurpose", null),
            _ => null
        };
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

    public async Task<List<TableMeta>> GetTableListAsync()
    {
        if (_cache.TryGetValue(CacheKeys.HouseholdTableList, out List<TableMeta>? cached))
            return cached!;

        var result = BuildTableList();

        _cache.Set(CacheKeys.HouseholdTableList, result, new MemoryCacheEntryOptions
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
            // ═══ जनसंख्या ═══
            new() { TableNumber = 1, Title = "पारिवारिक तथा जनसंख्या विवरण", Category = "जनसंख्या", DataSource = "household + family_member", Icon = "bi-people", AccentColor = "#2D6A4F", Description = "घरमुली, जनसंख्या, लिङ्ग अनुसार विवरण" },
            new() { TableNumber = 2, Title = "उमेर अनुसार जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-people", AccentColor = "#2D6A4F", Description = "५ वर्षे उमेर समूह अनुसार पुरूष/महिला जनसंख्या" },
            new() { TableNumber = 3, Title = "उमेर वर्गीकरण अनुसार जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-people", AccentColor = "#2D6A4F", Description = "बालबालिका, युवा, वयस्क, ज्येष्ठ नागरिक" },
            new() { TableNumber = 4, Title = "जातजाती अनुसार जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-people", AccentColor = "#2D6A4F", Description = "जातजाती र लिङ्ग अनुसार विवरण" },
            new() { TableNumber = 5, Title = "मातृभाषाको आधारमा जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-people", AccentColor = "#2D6A4F" },
            new() { TableNumber = 6, Title = "धर्मको आधारमा जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-people", AccentColor = "#2D6A4F" },
            new() { TableNumber = 7, Title = "साक्षरताको स्थिति", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-book", AccentColor = "#457B9D" },
            new() { TableNumber = 8, Title = "शैक्षिकस्तर अनुसार जनसंख्या", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-book", AccentColor = "#457B9D" },
            new() { TableNumber = 9, Title = "विशेषज्ञको विवरण अनुसार जनसंख्या", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-book", AccentColor = "#457B9D" },
            new() { TableNumber = 10, Title = "सरकारी जागिर विवरण", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-briefcase", AccentColor = "#457B9D" },
            new() { TableNumber = 11, Title = "बसोबासको अवस्था", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-house", AccentColor = "#2D6A4F" },
            new() { TableNumber = 12, Title = "अपाङ्गताको स्थिति", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-universal-access", AccentColor = "#457B9D" },
            new() { TableNumber = 13, Title = "वैवाहिक स्थिति", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-heart", AccentColor = "#2D6A4F" },
            new() { TableNumber = 14, Title = "प्रमुख पेशा अनुसार जनसंख्या", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-briefcase", AccentColor = "#2D6A4F" },
            new() { TableNumber = 15, Title = "मृत्युको कारण अनुसार विवरण", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "hh_death_record", Icon = "bi-heart-pulse", AccentColor = "#457B9D" },
            // ═══ शिक्षा तथा स्वास्थ्य ═══
            new() { TableNumber = 16, Title = "बिमा गरेका घरपरिवार", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-shield-check", AccentColor = "#457B9D" },
            new() { TableNumber = 17, Title = "परिवारका सदस्यहरुको स्वास्थ्य", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-heart-pulse", AccentColor = "#457B9D" },
            new() { TableNumber = 18, Title = "विरामी परेमा पहिले जाने स्थान", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "household", Icon = "bi-hospital", AccentColor = "#457B9D" },
            new() { TableNumber = 19, Title = "बैंक तथा वित्तिय संस्थामा खाता", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-bank", AccentColor = "#E9C46A" },
            new() { TableNumber = 20, Title = "बैंक खाता विवरण", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-bank", AccentColor = "#E9C46A" },
            new() { TableNumber = 21, Title = "सामाजिक सुरक्षा भत्ता", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-shield", AccentColor = "#457B9D" },
            new() { TableNumber = 22, Title = "ऋणको उद्देश्य अनुसार", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-cash-stack", AccentColor = "#E9C46A" },
            // ═══ सुविधा तथा पूर्वाधार ═══
            new() { TableNumber = 23, Title = "बजार पुग्न लाग्ने समय", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-clock", AccentColor = "#D4A373" },
            new() { TableNumber = 24, Title = "स्वास्थ्य संस्था पुग्न लाग्ने समय", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-clock", AccentColor = "#D4A373" },
            new() { TableNumber = 25, Title = "वडा कार्यालय पुग्न लाग्ने समय", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-clock", AccentColor = "#D4A373" },
            new() { TableNumber = 26, Title = "नगरपालिका कार्यालय पुग्न लाग्ने समय", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-clock", AccentColor = "#D4A373" },
            new() { TableNumber = 27, Title = "नियमित गाडी चल्ने बाटो", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-signpost", AccentColor = "#D4A373" },
            new() { TableNumber = 28, Title = "सडकमा पहुँच", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-signpost", AccentColor = "#D4A373" },
            new() { TableNumber = 29, Title = "खानेपानीको स्रोत", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-droplet", AccentColor = "#D4A373" },
            new() { TableNumber = 30, Title = "खाना पकाउने इन्धन", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-fire", AccentColor = "#D4A373" },
            new() { TableNumber = 31, Title = "चुलोको प्रकार", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-fire", AccentColor = "#D4A373" },
            new() { TableNumber = 32, Title = "बत्तीको स्रोत", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-lightbulb", AccentColor = "#D4A373" },
            new() { TableNumber = 33, Title = "फोहोर व्यवस्थापन", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-recycle", AccentColor = "#D4A373" },
            new() { TableNumber = 34, Title = "शौचालयको प्रकार", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-house", AccentColor = "#D4A373" },
            new() { TableNumber = 35, Title = "घरपरिवारमा उपलब्ध सुविधाहरु", Category = "सुविधा तथा पूर्वाधार", DataSource = "household", Icon = "bi-tv", AccentColor = "#D4A373" },
            // ═══ कृषि तथा पशुपालन ═══
            new() { TableNumber = 36, Title = "कृषि जग्गा प्रयोग", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 37, Title = "वार्षिक आम्दानी", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-currency-rupee", AccentColor = "#E9C46A" },
            new() { TableNumber = 38, Title = "वार्षिक खर्च", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-currency-rupee", AccentColor = "#E9C46A" },
            new() { TableNumber = 39, Title = "खाद्य सुरक्षा", Category = "आर्थिक अवस्था", DataSource = "household", Icon = "bi-basket", AccentColor = "#E9C46A" },
            new() { TableNumber = 40, Title = "चौपाया पशुपन्छी", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            // ═══ आवास ═══
            new() { TableNumber = 41, Title = "बसोबासको प्रकृति", Category = "आवास", DataSource = "household", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 42, Title = "घर संख्या", Category = "आवास", DataSource = "household", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 43, Title = "घरको तल्ला", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 44, Title = "कोठा संख्या", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 45, Title = "स्वामित्व", Category = "आवास", DataSource = "house_detail", Icon = "bi-key", AccentColor = "#B5838D" },
            new() { TableNumber = 46, Title = "भाडामा बस्ने", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 47, Title = "छाना सम्बन्धी", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 48, Title = "अन्य प्रयोजन कोठा", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 49, Title = "भवन मापदण्ड", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            new() { TableNumber = 50, Title = "घरको जग", Category = "आवास", DataSource = "house_detail", Icon = "bi-building", AccentColor = "#B5838D" },
            // ═══ जोखिम तथा सम्पत्ति ═══
            new() { TableNumber = 51, Title = "भूकम्प क्षति", Category = "जोखिम तथा सम्पत्ति", DataSource = "household", Icon = "bi-exclamation-triangle", AccentColor = "#E76F51" },
            new() { TableNumber = 52, Title = "अनुदान पाए", Category = "जोखिम तथा सम्पत्ति", DataSource = "household", Icon = "bi-cash", AccentColor = "#E76F51" },
            new() { TableNumber = 53, Title = "अनुदान किस्ता", Category = "जोखिम तथा सम्पत्ति", DataSource = "household", Icon = "bi-cash", AccentColor = "#E76F51" },
            new() { TableNumber = 54, Title = "अनुदानले घर बने", Category = "जोखिम तथा सम्पत्ति", DataSource = "household", Icon = "bi-cash", AccentColor = "#E76F51" },
            new() { TableNumber = 55, Title = "प्राकृतिक प्रकोप जोखिम", Category = "जोखिम तथा सम्पत्ति", DataSource = "household", Icon = "bi-exclamation-triangle", AccentColor = "#E76F51" },
            new() { TableNumber = 56, Title = "बाल विवाह", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "family_member", Icon = "bi-exclamation-triangle", AccentColor = "#457B9D" },
            new() { TableNumber = 57, Title = "बालबालिका काम", Category = "शिक्षा तथा स्वास्थ्य", DataSource = "household", Icon = "bi-exclamation-triangle", AccentColor = "#457B9D" },
            new() { TableNumber = 58, Title = "मासु चौपाया", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 59, Title = "अन्नबाली उत्पादन", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 60, Title = "तेलहन उत्पादन", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 61, Title = "फलफुल उत्पादन", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 62, Title = "दुधजन्य चौपाया", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" },
            new() { TableNumber = 63, Title = "बैदेशिक रोजगार", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-airplane", AccentColor = "#2D6A4F" },
            new() { TableNumber = 64, Title = "विदेश बस्नुको कारण", Category = "जनसंख्या", DataSource = "family_member", Icon = "bi-airplane", AccentColor = "#2D6A4F" },
            new() { TableNumber = 65, Title = "जडिबुटी उत्पादन", Category = "कृषि तथा पशुपालन", DataSource = "household", Icon = "bi-tree", AccentColor = "#52B788" }
        };
    }

    // ═══════════════════════════════════════════════════
    // BASE QUERIES (no ward filter — all wards)
    // ═══════════════════════════════════════════════════

    private IQueryable<FamilyMember> FmBase() =>
        _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Household != null && fm.Household.WardNo != null);

    private IQueryable<Household> HhBase() =>
        _db.Households.AsNoTracking()
            .Where(h => h.WardNo != null);

    private IQueryable<HhDeathRecord> DrBase() =>
        _db.HhDeathRecords.AsNoTracking()
            .Where(dr => dr.Household != null && dr.Household.WardNo != null);

    // ═══════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════

    private static string Fmt(int val) => val.ToString("#,##0");
    private static string Fmt(long val) => val.ToString("#,##0");
    private static string Fmt(double? val) => val.HasValue ? ((long)val.Value).ToString("#,##0") : "—";

    private static string AgeBin(double age)
    {
        if (age < 5) return "०-४";
        if (age < 10) return "५-९";
        if (age < 15) return "१०-१४";
        if (age < 20) return "१५-१९";
        if (age < 25) return "२०-२४";
        if (age < 30) return "२५-२९";
        if (age < 35) return "३०-३४";
        if (age < 40) return "३५-३९";
        if (age < 45) return "४०-४४";
        if (age < 50) return "४५-४९";
        if (age < 55) return "५०-५४";
        if (age < 60) return "५५-५९";
        if (age < 65) return "६०-६४";
        if (age < 70) return "६५-६९";
        if (age < 75) return "७०-७४";
        return "७५+";
    }

    private static readonly List<string> AgeGroupLabels = new()
    {
        "०-४", "५-९", "१०-१४", "१५-१९", "२०-२४", "२५-२९",
        "३०-३४", "३५-३९", "४०-४४", "४५-४९", "५०-५४", "५५-५९",
        "६०-६४", "६५-६९", "७०-७४", "७५+"
    };

    private static readonly List<string> AgeCategoryLabels = new()
    {
        "बालबालिका (०-१५)", "युवा (१६-४०)", "वयस्क (४१-५९)", "ज्येष्ठ नागरिक (६०+)"
    };

    private static string AgeCategory(double age)
    {
        if (age < 16) return "बालबालिका (०-१५)";
        if (age < 41) return "युवा (१६-४०)";
        if (age < 60) return "वयस्क (४१-५९)";
        return "ज्येष्ठ नागरिक (६०+)";
    }

    // ═══════════════════════════════════════════════════
    // PIVOT TABLE BUILDER
    // Builds a ward-rows × category-columns table
    // ═══════════════════════════════════════════════════

    private TableViewModel BuildPivotTable(
        int tableNo, string title, string category, string? description, string? dataSource,
        List<int> allWardNos, int? filteredWardNo,
        List<string> columns, bool hasOtherColumn,
        List<(int WardNo, string Category, int Count)> rawData)
    {
        var displayWards = filteredWardNo.HasValue
            ? allWardNos.Where(w => w == filteredWardNo.Value).ToList()
            : allWardNos;

        // Column headers
        var headers = new List<string> { "वडा नम्बर" };
        headers.AddRange(columns);
        if (hasOtherColumn) headers.Add("अन्य");
        headers.Add("जम्मा");

        // Build ward rows
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

        // Build total row
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
                WardNo = null,
                Cells = totalCells,
                RawValues = totalRawVals
            },
            FilteredWardNo = filteredWardNo,
            TotalWards = allWardNos.Count
        };
    }

    // ═══════════════════════════════════════════════════
    // GENERIC FM CATEGORY TABLE
    // Loads (ward, category) pairs, groups in memory,
    // pivots into ward × category table
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> FmCatTable(
        int tableNo, string title, string category, string? description,
        int? wardNo, string propertyName, int? topN)
    {
        var wardNos = await GetWardNumbersAsync();

        // Load only ward_no + the category column
        var raw = await _db.FamilyMembers.AsNoTracking()
            .Where(fm => fm.Household != null && fm.Household.WardNo != null)
            .Select(fm => new
            {
                W = fm.Household!.WardNo!.Value,
                C = EF.Property<string>(fm, propertyName)
            })
            .ToListAsync();

        // Filter null/empty in memory
        var filtered = raw.Where(x => !string.IsNullOrEmpty(x.C)).ToList();

        // Group by (ward, category) and count
        var grouped = filtered
            .GroupBy(x => (x.W, x.C!))
            .Select(g => (WardNo: g.Key.W, Category: g.Key.Item2, Count: g.Count()))
            .ToList();

        // Determine column categories by total count
        var allCats = grouped
            .GroupBy(x => x.Category)
            .Select(g => (Cat: g.Key, Total: g.Sum(x => x.Count)))
            .OrderByDescending(x => x.Total)
            .ToList();

        var columns = topN.HasValue
            ? allCats.Take(topN.Value).Select(x => x.Cat).ToList()
            : allCats.Select(x => x.Cat).ToList();

        bool hasOther = topN.HasValue && allCats.Count > topN.Value;

        return BuildPivotTable(tableNo, title, category, description, "family_member",
            wardNos, wardNo, columns, hasOther, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 1: पारिवारिक तथा जनसंख्या विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable01Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();
        var displayWards = wardNo.HasValue
            ? wardNos.Where(w => w == wardNo.Value).ToList()
            : wardNos;

        // HH count per ward
        var hhData = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new { W = g.Key, C = g.Count() })
            .ToListAsync();
        var hhCounts = hhData.ToDictionary(x => x.W, x => x.C);

        // Population by gender per ward
        var popRaw = await FmBase()
            .Select(fm => new { W = fm.Household!.WardNo!.Value, G = fm.Gender })
            .ToListAsync();

        var popByWard = popRaw.GroupBy(x => x.W).ToDictionary(
            g => g.Key,
            g => (
                Male: g.Count(x => x.G == "पुरूष"),
                Female: g.Count(x => x.G == "महिला"),
                Other: g.Count(x => x.G != null && x.G != "पुरूष" && x.G != "महिला")
            ));

        // Household heads by gender (try common values)
        var headRaw = await FmBase()
            .Where(fm => fm.RelationToHead != null && fm.RelationToHead != "")
            .Select(fm => new { W = fm.Household!.WardNo!.Value, R = fm.RelationToHead!, G = fm.Gender })
            .ToListAsync();

        var heads = headRaw.Where(x => 
            x.R.Contains("घरमुली") || 
            x.R.Contains("परिवार प्रमुख") || 
            x.R == "स्वयं"
        ).ToList();

        var headByWard = heads.GroupBy(x => x.W).ToDictionary(
            g => g.Key,
            g => (
                Male: g.Count(x => x.G == "पुरूष"),
                Female: g.Count(x => x.G == "महिला")
            ));

        // Build
        var headers = new List<string>
        {
            "वडा नम्बर", "पुरूष घरमुली", "महिला घरमुली", "जम्मा घरपरिवार",
            "पुरूष", "महिला", "अन्य", "जम्मा जनसंख्या"
        };

        var rows = new List<TableRow>();
        long tMH = 0, tFH = 0, tHH = 0, tM = 0, tF = 0, tO = 0, tT = 0;

        foreach (var w in displayWards.OrderBy(w => w))
        {
            int hh = hhCounts.GetValueOrDefault(w);
            var pop = popByWard.GetValueOrDefault(w);
            var head = headByWard.GetValueOrDefault(w);

            int mH = head.Male, fH = head.Female;
            int m = pop.Male, f = pop.Female, o = pop.Other;

            rows.Add(new TableRow
            {
                RowLabel = $"वडा नं. {w}",
                WardNo = w,
                Cells = new List<string> { Fmt(mH), Fmt(fH), Fmt(hh), Fmt(m), Fmt(f), Fmt(o), Fmt(m + f + o) },
                RawValues = new List<double?> { mH, fH, hh, m, f, o, m + f + o }
            });

            tMH += mH; tFH += fH; tHH += hh;
            tM += m; tF += f; tO += o; tT += m + f + o;
        }

        return new TableViewModel
        {
            TableNumber = 1,
            Title = "पारिवारिक तथा जनसंख्या विवरण",
            Category = "जनसंख्या",
            Description = "घरमुली, जनसंख्या, लिङ्ग अनुसार विवरण",
            DataSource = "household + family_member",
            ColumnHeaders = headers,
            Rows = rows,
            TotalRow = new TableRow
            {
                RowLabel = "जम्मा",
                Cells = new List<string> { Fmt(tMH), Fmt(tFH), Fmt(tHH), Fmt(tM), Fmt(tF), Fmt(tO), Fmt(tT) },
                RawValues = new List<double?> { tMH, tFH, tHH, tM, tF, tO, tT }
            },
            FilteredWardNo = wardNo,
            TotalWards = wardNos.Count
        };
    }

    // ═══════════════════════════════════════════════════
    // TABLE 2: उमेर अनुसार जनसंख्या (5-year age groups)
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable02Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await FmBase()
            .Where(fm => fm.Age != null)
            .Select(fm => new { W = fm.Household!.WardNo!.Value, A = fm.Age!.Value })
            .ToListAsync();

        // Bin ages into 5-year groups and count
        var grouped = raw
            .Select(x => (x.W, Group: AgeBin(x.A)))
            .GroupBy(x => (x.W, x.Group))
            .Select(g => (WardNo: g.Key.W, Category: g.Key.Group, Count: g.Count()))
            .ToList();

        return BuildPivotTable(2, "उमेर अनुसार जनसंख्या", "जनसंख्या",
            "५ वर्षे उमेर समूह अनुसार जनसंख्या विवरण", "family_member",
            wardNos, wardNo, AgeGroupLabels, false, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 3: उमेर वर्गीकरण अनुसार जनसंख्या
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable03Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await FmBase()
            .Where(fm => fm.Age != null)
            .Select(fm => new { W = fm.Household!.WardNo!.Value, A = fm.Age!.Value })
            .ToListAsync();

        var grouped = raw
            .Select(x => (x.W, Category: AgeCategory(x.A)))
            .GroupBy(x => (x.W, x.Category))
            .Select(g => (WardNo: g.Key.W, Category: g.Key.Category, Count: g.Count()))
            .ToList();

        return BuildPivotTable(3, "उमेर वर्गीकरण अनुसार जनसंख्या", "जनसंख्या",
            "बालबालिका, युवा, वयस्क, ज्येष्ठ नागरिक वर्गीकरण", "family_member",
            wardNos, wardNo, AgeCategoryLabels, false, grouped);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 15: मृत्युको कारण अनुसार विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable15Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();
        var displayWards = wardNo.HasValue
            ? wardNos.Where(w => w == wardNo.Value).ToList()
            : wardNos;

        // Get death cause sums per ward
        var deathData = await DrBase()
            .GroupBy(dr => dr.Household!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Communicable = g.Sum(dr => dr.CauseCommunicable ?? 0),
                NonCommunicable = g.Sum(dr => dr.CauseNonCommunicable ?? 0),
                Lifestyle = g.Sum(dr => dr.CauseLifestyle ?? 0),
                Other = g.Sum(dr => dr.CauseOther ?? 0)
            })
            .ToListAsync();
        var deathDict = deathData.ToDictionary(x => x.W);

        var headers = new List<string>
        {
            "वडा नम्बर", "संक्रामक रोग", "गैर-संक्रामक रोग", "जीवनशैली", "अन्य", "जम्मा"
        };

        var rows = new List<TableRow>();
        long tComm = 0, tNonComm = 0, tLife = 0, tOther = 0, tTotal = 0;

        foreach (var w in displayWards.OrderBy(w => w))
        {
            var d = deathDict.ContainsKey(w) ? deathDict[w] : null;
            int comm = (int)(d?.Communicable ?? 0);
            int nonComm = (int)(d?.NonCommunicable ?? 0);
            int life = (int)(d?.Lifestyle ?? 0);
            int other = (int)(d?.Other ?? 0);
            int total = comm + nonComm + life + other;

            rows.Add(new TableRow
            {
                RowLabel = $"वडा नं. {w}",
                WardNo = w,
                Cells = new List<string> { Fmt(comm), Fmt(nonComm), Fmt(life), Fmt(other), Fmt(total) },
                RawValues = new List<double?> { comm, nonComm, life, other, total }
            });

            tComm += comm; tNonComm += nonComm; tLife += life; tOther += other; tTotal += total;
        }

        return new TableViewModel
        {
            TableNumber = 15,
            Title = "मृत्युको कारण अनुसार विवरण",
            Category = "शिक्षा तथा स्वास्थ्य",
            Description = "संक्रामक, गैर-संक्रामक, जीवनशैली र अन्य कारणहरू",
            DataSource = "hh_death_record",
            ColumnHeaders = headers,
            Rows = rows,
            TotalRow = new TableRow
            {
                RowLabel = "जम्मा",
                Cells = new List<string> { Fmt(tComm), Fmt(tNonComm), Fmt(tLife), Fmt(tOther), Fmt(tTotal) },
                RawValues = new List<double?> { tComm, tNonComm, tLife, tOther, tTotal }
            },
            FilteredWardNo = wardNo,
            TotalWards = wardNos.Count
        };
    }

    // ═══════════════════════════════════════════════════
    // ADDITIONAL BASE QUERY
    // ═══════════════════════════════════════════════════

    private IQueryable<HouseDetail> HdBase() =>
        _db.HouseDetails.AsNoTracking()
            .Where(hd => hd.Household != null && hd.Household.WardNo != null);

    // ═══════════════════════════════════════════════════
    // HELPER: Household categorical column table
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> HhCatTable(
        int tableNo, string title, string category, string? description,
        int? wardNo, string propertyName, int? topN)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await HhBase()
            .Select(h => new
            {
                W = h.WardNo!.Value,
                C = EF.Property<string>(h, propertyName)
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

        return BuildPivotTable(tableNo, title, category, description, "household",
            wardNos, wardNo, columns, hasOther, grouped);
    }

    // ═══════════════════════════════════════════════════
    // HELPER: House-detail categorical column table
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> HdCatTable(
        int tableNo, string title, string category, string? description,
        int? wardNo, string propertyName, int? topN)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await HdBase()
            .Select(hd => new
            {
                W = hd.Household!.WardNo!.Value,
                C = EF.Property<string>(hd, propertyName)
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

        return BuildPivotTable(tableNo, title, category, description, "house_detail",
            wardNos, wardNo, columns, hasOther, grouped);
    }

    // ═══════════════════════════════════════════════════
    // HELPER: Generic sum-table builder
    // ═══════════════════════════════════════════════════

    private TableViewModel BuildSumTable(
        int tableNo, string title, string category, string? description,
        string dataSource, List<int> allWardNos, int? filteredWardNo,
        List<string> columnLabels,
        Dictionary<int, List<double>> wardValues)
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
            TotalWards = allWardNos.Count
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

    // ═══════════════════════════════════════════════════
    // TABLE 16: बिमा गरेका घरपरिवार
    // ═══════════════════════════════════════════════════
    // → Uses FmCatTable("HasInsurance") — defined in switch above

    // ═══════════════════════════════════════════════════
    // TABLE 17: परिवारका सदस्यहरुको स्वास्थ्य
    // ═══════════════════════════════════════════════════
    // → Uses FmCatTable("HealthStatus") — defined in switch above

    // ═══════════════════════════════════════════════════
    // TABLE 20: बैंक खाता विवरण
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable20Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Bank = g.Sum(h => h.BankAccountBank),
                Coop = g.Sum(h => h.BankAccountCoop),
                Savings = g.Sum(h => h.BankAccountSavings),
                Moneylender = g.Sum(h => h.BankAccountMoneylender),
                Other = g.Sum(h => h.BankAccountOther)
            })
            .ToListAsync();

        var labels = new List<string> { "बैंक", "सहकारी", "बचत समूह", "साहु", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Bank ?? 0, x.Coop ?? 0, x.Savings ?? 0, x.Moneylender ?? 0, x.Other ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(20, "बैंक खाता विवरण", "आर्थिक अवस्था",
            "बैंक खाताको प्रकार अनुसार विवरण", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 22: ऋणको उद्देश्य अनुसार
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable22Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Business = g.Sum(h => h.LoanPurposeBusiness),
                House = g.Sum(h => h.LoanPurposeHouse),
                Medical = g.Sum(h => h.LoanPurposeMedical),
                Education = g.Sum(h => h.LoanPurposeEducation),
                Other = g.Sum(h => h.LoanPurposeOther)
            })
            .ToListAsync();

        var labels = new List<string> { "व्यापार", "घर निर्माण", "स्वास्थ्य", "शिक्षा", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Business ?? 0, x.House ?? 0, x.Medical ?? 0, x.Education ?? 0, x.Other ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(22, "ऋणको उद्देश्य अनुसार", "आर्थिक अवस्था",
            "ऋण लिएको उद्देश्य अनुसार घरपरिवार संख्या", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 35: घरपरिवारमा उपलब्ध सुविधाहरु
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable35Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Radio = g.Sum(h => h.AssetRadio),
                Bicycle = g.Sum(h => h.AssetBicycle),
                Television = g.Sum(h => h.AssetTelevision),
                Phone = g.Sum(h => h.AssetPhone),
                Computer = g.Sum(h => h.AssetComputer),
                Internet = g.Sum(h => h.AssetInternet),
                Motorcycle = g.Sum(h => h.AssetMotorcycle),
                Car = g.Sum(h => h.AssetCar),
                Refrigerator = g.Sum(h => h.AssetRefrigerator),
                Tractor = g.Sum(h => h.AssetTractor),
                BusTruck = g.Sum(h => h.AssetBusTruck),
                Tipper = g.Sum(h => h.AssetTipperTruck),
                Excavator = g.Sum(h => h.AssetExcavator),
                None = g.Sum(h => h.AssetNone)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "रेडियो", "साइकल", "टेलिभिजन", "मोबाइल", "कम्प्युटर",
            "इन्टरनेट", "मोटरसाइकल", "कार", "फ्रिज",
            "ट्र्याक्टर", "बस/ट्रक", "टिपर", "उत्खनन्यान्त्र", "कुनै पनि छैन"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Radio ?? 0, x.Bicycle ?? 0, x.Television ?? 0, x.Phone ?? 0,
                x.Computer ?? 0, x.Internet ?? 0, x.Motorcycle ?? 0, x.Car ?? 0,
                x.Refrigerator ?? 0, x.Tractor ?? 0, x.BusTruck ?? 0, x.Tipper ?? 0,
                x.Excavator ?? 0, x.None ?? 0
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(35, "घरपरिवारमा उपलब्ध सुविधाहरु", "सुविधा तथा पूर्वाधार",
            "घरपरिवारमा उपलब्ध सम्पत्ति तथा सुविधा", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 37: वार्षिक आम्दानी
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable37Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Vegetables = g.Sum(h => h.IncomeVegetables),
                Labor = g.Sum(h => h.IncomeLabor),
                Trade = g.Sum(h => h.IncomeTrade),
                Foreign = g.Sum(h => h.IncomeForeign),
                Job = g.Sum(h => h.IncomeJob),
                Other = g.Sum(h => h.IncomeOther)
            })
            .ToListAsync();

        var labels = new List<string> { "तरकारी/कृषि", "मजदुरी", "व्यापार", "विदेश", "जागिर", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Vegetables ?? 0, x.Labor ?? 0, x.Trade ?? 0,
                x.Foreign ?? 0, x.Job ?? 0, x.Other ?? 0
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(37, "वार्षिक आम्दानी", "आर्थिक अवस्था",
            "आम्दानीको स्रोत अनुसार वार्षिक आम्दानी (रु.)", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 38: वार्षिक खर्च
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable38Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Food = g.Sum(h => h.ExpenseFood),
                Clothes = g.Sum(h => h.ExpenseClothes),
                Education = g.Sum(h => h.ExpenseEducation),
                Health = g.Sum(h => h.ExpenseHealth),
                Festivals = g.Sum(h => h.ExpenseFestivals),
                Agriculture = g.Sum(h => h.ExpenseAgriculture),
                Fuel = g.Sum(h => h.ExpenseFuel),
                MobileInternet = g.Sum(h => h.ExpenseMobileInternet)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "खाना", "लुगा", "शिक्षा", "स्वास्थ्य",
            "चाडपर्व", "कृषि", "इन्धन", "मोबाइल/इन्टरनेट"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Food ?? 0, x.Clothes ?? 0, x.Education ?? 0, x.Health ?? 0,
                x.Festivals ?? 0, x.Agriculture ?? 0, x.Fuel ?? 0, x.MobileInternet ?? 0
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(38, "वार्षिक खर्च", "आर्थिक अवस्था",
            "खर्चको शीर्षक अनुसार वार्षिक खर्च (रु.)", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 40: चौपाया पशुपन्छी
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable40Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Cow = g.Sum(h => h.CowCount),
                Buffalo = g.Sum(h => h.BuffaloCount),
                OtherDairy = g.Sum(h => h.OtherDairyCount),
                Goat = g.Sum(h => h.GoatCount),
                Calf = g.Sum(h => h.CalfCount),
                Pig = g.Sum(h => h.PigCount),
                OtherMeat = g.Sum(h => h.OtherMeatCount),
                Poultry = g.Sum(h => h.PoultryCount)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "गाई", "भैँसी", "अन्य दुग्ध", "बाखा/भेडा",
            "बाच्छा", "सुँगुर", "अन्य मासु", "कुखुरा"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.Cow ?? 0, x.Buffalo ?? 0, x.OtherDairy ?? 0, x.Goat ?? 0,
                x.Calf ?? 0, x.Pig ?? 0, x.OtherMeat ?? 0, x.Poultry ?? 0
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(40, "चौपाया पशुपन्छी", "कृषि तथा पशुपालन",
            "पशुपन्छीको प्रकार अनुसार संख्या", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 42: घर संख्या
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable42Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new { W = g.Key, Count = g.Sum(h => h.HouseCount) })
            .ToListAsync();

        var labels = new List<string> { "घर संख्या" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Count ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(42, "घर संख्या", "आवास",
            "प्रति घरपरिवार घरको संख्या", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 43: घरको तल्ला (by room ranges)
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable43Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var raw = await HdBase()
            .Where(hd => hd.TotalRooms != null)
            .Select(hd => new { W = hd.Household!.WardNo!.Value, Rooms = hd.TotalRooms!.Value })
            .ToListAsync();

        var grouped = raw
            .Select(x => (x.W, Category: RoomBin(x.Rooms)))
            .GroupBy(x => (x.W, x.Category))
            .Select(g => (WardNo: g.Key.W, Category: g.Key.Category, Count: g.Count()))
            .ToList();

        var roomLabels = new List<string> { "१-२ कोठा", "३-४ कोठा", "५-६ कोठा", "७+ कोठा" };

        return BuildPivotTable(43, "घरको तल्ला", "आवास",
            "कोठा संख्या अनुसार घरको वितरण", "house_detail",
            wardNos, wardNo, roomLabels, false, grouped);
    }

    private static string RoomBin(double rooms)
    {
        if (rooms <= 2) return "१-२ कोठा";
        if (rooms <= 4) return "३-४ कोठा";
        if (rooms <= 6) return "५-६ कोठा";
        return "७+ कोठा";
    }

    // ═══════════════════════════════════════════════════
    // TABLE 44: कोठा संख्या
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable44Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HdBase()
            .GroupBy(hd => hd.Household!.WardNo!.Value)
            .Select(g => new { W = g.Key, Rooms = g.Sum(hd => hd.TotalRooms) })
            .ToListAsync();

        var labels = new List<string> { "कोठा संख्या" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Rooms ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(44, "कोठा संख्या", "आवास",
            "घरहरुमा कुल कोठा संख्या", "house_detail",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 46: भाडामा बस्ने
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable46Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HdBase()
            .GroupBy(hd => hd.Household!.WardNo!.Value)
            .Select(g => new { W = g.Key, Rented = g.Sum(hd => hd.RoomsRented) })
            .ToListAsync();

        var labels = new List<string> { "भाडामा दिइएका कोठा" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Rented ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(46, "भाडामा बस्ने", "आवास",
            "भाडामा दिइएका कोठाको संख्या", "house_detail",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 48: अन्य प्रयोजन कोठा
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable48Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HdBase()
            .GroupBy(hd => hd.Household!.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Rented = g.Sum(hd => hd.RoomsRented),
                TotalRooms = g.Sum(hd => hd.TotalRooms)
            })
            .ToListAsync();

        var labels = new List<string> { "कुल कोठा", "भाडामा दिएको" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.TotalRooms ?? 0, x.Rented ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(48, "अन्य प्रयोजन कोठा", "आवास",
            "कुल कोठा र भाडामा दिएका कोठा", "house_detail",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 56: बाल विवाह (married under 18)
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable56Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();
        var displayWards = wardNo.HasValue
            ? wardNos.Where(w => w == wardNo.Value).ToList()
            : wardNos;

        var marriedValues = new[] { "विवाहित", "वैवाहिक" };

        var raw = await FmBase()
            .Where(fm => fm.Age != null && fm.Age < 18 && fm.MaritalStatus != null)
            .Select(fm => new
            {
                W = fm.Household!.WardNo!.Value,
                G = fm.Gender ?? "अज्ञात",
                M = fm.MaritalStatus!
            })
            .ToListAsync();

        var married = raw.Where(x => marriedValues.Contains(x.M)).ToList();

        var headers = new List<string> { "वडा नम्बर", "पुरूष", "महिला", "जम्मा" };

        var rows = new List<TableRow>();
        long tM = 0, tF = 0, tT = 0;

        foreach (var w in displayWards.OrderBy(w => w))
        {
            var wardData = married.Where(x => x.W == w).ToList();
            int male = wardData.Count(x => x.G == "पुरूष");
            int female = wardData.Count(x => x.G == "महिला");
            int total = male + female;

            rows.Add(new TableRow
            {
                RowLabel = $"वडा नं. {w}",
                WardNo = w,
                Cells = new List<string> { Fmt(male), Fmt(female), Fmt(total) },
                RawValues = new List<double?> { male, female, total }
            });

            tM += male; tF += female; tT += total;
        }

        return new TableViewModel
        {
            TableNumber = 56,
            Title = "बाल विवाह",
            Category = "शिक्षा तथा स्वास्थ्य",
            Description = "१८ वर्ष मुनि विवाहित व्यक्तिहरुको संख्या",
            DataSource = "family_member",
            ColumnHeaders = headers,
            Rows = rows,
            TotalRow = new TableRow
            {
                RowLabel = "जम्मा",
                Cells = new List<string> { Fmt(tM), Fmt(tF), Fmt(tT) },
                RawValues = new List<double?> { tM, tF, tT }
            },
            FilteredWardNo = wardNo,
            TotalWards = wardNos.Count
        };
    }

    // ═══════════════════════════════════════════════════
    // TABLE 58: मासु चौपाया
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable58Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Goat = g.Sum(h => h.GoatCount),
                Calf = g.Sum(h => h.CalfCount),
                Pig = g.Sum(h => h.PigCount),
                Other = g.Sum(h => h.OtherMeatCount)
            })
            .ToListAsync();

        var labels = new List<string> { "बाखा/भेडा", "बाच्छा", "सुँगुर", "अन्य" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Goat ?? 0, x.Calf ?? 0, x.Pig ?? 0, x.Other ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(58, "मासु चौपाया", "कृषि तथा पशुपालन",
            "मासु उत्पादनका लागि पालिएका चौपाया", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 59: अन्नबाली उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable59Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                Rice = g.Sum(h => h.RiceProductionKg),
                Maize = g.Sum(h => h.MaizeProductionKg),
                Wheat = g.Sum(h => h.WheatProductionKg),
                Barley = g.Sum(h => h.BarleyProductionKg)
            })
            .ToListAsync();

        var labels = new List<string> { "धान (के.जी.)", "मकै (के.जी.)", "गहुँ (के.जी.)", "जौ (के.जी.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { x.Rice ?? 0, x.Maize ?? 0, x.Wheat ?? 0, x.Barley ?? 0 });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(59, "अन्नबाली उत्पादन", "कृषि तथा पशुपालन",
            "अन्नबाली उत्पादन कुल के.जी.", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 60: तेलहन उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable60Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                O1 = g.Sum(h => h.Oilseed1ProductionKg),
                O2 = g.Sum(h => h.Oilseed2ProductionKg),
                O3 = g.Sum(h => h.Oilseed3ProductionKg),
                O4 = g.Sum(h => h.Oilseed4ProductionKg)
            })
            .ToListAsync();

        var labels = new List<string> { "तेलहन कुल उत्पादन (के.जी.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double> { (x.O1 ?? 0) + (x.O2 ?? 0) + (x.O3 ?? 0) + (x.O4 ?? 0) });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(60, "तेलहन उत्पादन", "कृषि तथा पशुपालन",
            "तेलहन बालीको कुल उत्पादन (के.जी.)", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 61: फलफुल उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable61Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                F1 = g.Sum(h => h.Fruit1ProductionKg),
                F2 = g.Sum(h => h.Fruit2ProductionKg),
                F3 = g.Sum(h => h.Fruit3ProductionKg),
                F4 = g.Sum(h => h.Fruit4ProductionKg),
                F5 = g.Sum(h => h.Fruit5ProductionKg),
                F6 = g.Sum(h => h.Fruit6ProductionKg),
                F7 = g.Sum(h => h.Fruit7ProductionKg),
                F8 = g.Sum(h => h.Fruit8ProductionKg)
            })
            .ToListAsync();

        var labels = new List<string> { "फलफुल कुल उत्पादन (के.जी.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                (x.F1 ?? 0) + (x.F2 ?? 0) + (x.F3 ?? 0) + (x.F4 ?? 0) +
                (x.F5 ?? 0) + (x.F6 ?? 0) + (x.F7 ?? 0) + (x.F8 ?? 0)
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(61, "फलफुल उत्पादन", "कृषि तथा पशुपालन",
            "फलफुलको कुल उत्पादन (के.जी.)", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 62: दुधजन्य चौपाया
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable62Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                CowCount = g.Sum(h => h.CowCount),
                CowMilk = g.Sum(h => h.CowMilkDaily),
                BuffaloCount = g.Sum(h => h.BuffaloCount),
                BuffaloMilk = g.Sum(h => h.BuffaloMilkDaily),
                OtherCount = g.Sum(h => h.OtherDairyCount),
                OtherMilk = g.Sum(h => h.OtherDairyMilkDaily)
            })
            .ToListAsync();

        var labels = new List<string>
        {
            "गाई संख्या", "गाई दूध (लि./दिन)",
            "भैँसी संख्या", "भैँसी दूध (लि./दिन)",
            "अन्य संख्या", "अन्य दूध (लि./दिन)"
        };

        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                x.CowCount ?? 0, x.CowMilk ?? 0,
                x.BuffaloCount ?? 0, x.BuffaloMilk ?? 0,
                x.OtherCount ?? 0, x.OtherMilk ?? 0
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(62, "दुधजन्य चौपाया", "कृषि तथा पशुपालन",
            "दुधजन्य पशुको संख्या र दैनिक दूध उत्पादन", "household",
            wardNos, wardNo, labels, wardValues);
    }

    // ═══════════════════════════════════════════════════
    // TABLE 65: जडिबुटी उत्पादन
    // ═══════════════════════════════════════════════════

    private async Task<TableViewModel> GetTable65Async(int? wardNo)
    {
        var wardNos = await GetWardNumbersAsync();

        var data = await HhBase()
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new
            {
                W = g.Key,
                H1 = g.Sum(h => h.Herb1ProductionKg),
                H2 = g.Sum(h => h.Herb2ProductionKg),
                H3 = g.Sum(h => h.Herb3ProductionKg),
                H4 = g.Sum(h => h.Herb4ProductionKg),
                H5 = g.Sum(h => h.Herb5ProductionKg),
                H6 = g.Sum(h => h.Herb6ProductionKg),
                H7 = g.Sum(h => h.Herb7ProductionKg),
                H8 = g.Sum(h => h.Herb8ProductionKg),
                H9 = g.Sum(h => h.Herb9ProductionKg)
            })
            .ToListAsync();

        var labels = new List<string> { "जडिबुटी कुल उत्पादन (के.जी.)" };
        var wardValues = data.ToDictionary(
            x => x.W,
            x => new List<double>
            {
                (x.H1 ?? 0) + (x.H2 ?? 0) + (x.H3 ?? 0) + (x.H4 ?? 0) +
                (x.H5 ?? 0) + (x.H6 ?? 0) + (x.H7 ?? 0) + (x.H8 ?? 0) + (x.H9 ?? 0)
            });

        EnsureWards(wardValues, wardNos, labels.Count);

        return BuildSumTable(65, "जडिबुटी उत्पादन", "कृषि तथा पशुपालन",
            "जडिबुटीको कुल उत्पादन (के.जी.)", "household",
            wardNos, wardNo, labels, wardValues);
    }
}
