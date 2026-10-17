using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services.Caching;
using VrindavanProfile.Web.Services.Interfaces;

namespace VrindavanProfile.Web.Services;

public class MapService : IMapService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    private readonly ILogger<MapService> _logger;

    public MapService(AppDbContext db, IMemoryCache cache, ILogger<MapService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════

    public async Task<MapViewModel> GetMapDataAsync(int? wardNo = null, string? entityType = null)
    {
        // Load ALL markers for the given ward (institution + household).
        // Entity-type filtering happens client-side in JS so the full set is serialised
        // to the view — but the summaries must match what JS would actually display.
        var instMarkers = await GetInstitutionMarkersAsync(wardNo);
        _logger.LogInformation("MapService: instMarkers count = {Count} (wardNo={W})", instMarkers.Count, wardNo);

        var hhMarkers = await GetHouseholdMarkersAsync(wardNo);
        _logger.LogInformation("MapService: hhMarkers count = {Count} (wardNo={W})", hhMarkers.Count, wardNo);

        // Merge — all markers for this ward (all entity types)
        var allWardMarkers = instMarkers.Concat(hhMarkers).ToList();

        // The markers sent to the view are the full ward-filtered set.
        // JS renderMarkers() will further filter by entity type client-side,
        // so we do NOT pre-filter by entityType here (that keeps filtering instant).
        var markers = allWardMarkers;

        // ── Summaries: derived from the SAME marker list so counts ALWAYS match pins ──
        // Group the ward-filtered markers by entity type key.  This way the badge
        // counts in the filter panel are guaranteed to equal the number of pins that
        // will appear when that filter button is clicked.
        var summaryLookup = allWardMarkers
            .GroupBy(m => m.EntityTypeKey)
            .ToDictionary(g => g.Key, g => g.Count());

        // Re-use the global (cached) type-meta list for colour/icon/label info,
        // but replace the Count with the ward-scoped value computed above.
        var globalMeta = await GetTypeSummariesAsync();
        var summaries = globalMeta
            .Select(s => new MapTypeSummary
            {
                EntityType    = s.EntityType,
                EntityTypeKey = s.EntityTypeKey,
                Color         = s.Color,
                Icon          = s.Icon,
                // Use ward-filtered count; 0 if this type has no pins in the ward.
                Count         = summaryLookup.GetValueOrDefault(s.EntityTypeKey, 0)
            })
            .OrderByDescending(s => s.EntityTypeKey == "household" ? 1 : 0)
            .ThenByDescending(s => s.Count)
            .ToList();

        var wardNumbers = await GetWardNumbersAsync();

        // Ward sidebar stats (household count + population breakdown)
        var hhCounts = await _db.Households
            .Where(h => h.WardNo != null)
            .GroupBy(h => h.WardNo!.Value)
            .Select(g => new { WardNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.WardNo, x => x.Count);

        var wardFamilyCounts = await _db.FamilyMembers
            .AsNoTracking()
            .Join(_db.Households,
                fm => fm.HouseholdId,
                h  => h.Id,
                (fm, h) => new { fm.Gender, h.WardNo })
            .Where(x => x.WardNo != null)
            .GroupBy(x => x.WardNo!.Value)
            .Select(g => new
            {
                WardNo = g.Key,
                Total  = g.Count(),
                Male   = g.Count(x => x.Gender == "पुरूष"),
                Female = g.Count(x => x.Gender == "महिला")
            })
            .ToDictionaryAsync(x => x.WardNo);

        var wardDetails = wardNumbers.Select(w => new WardInfo
        {
            WardNo         = w,
            HouseholdCount = hhCounts.GetValueOrDefault(w),
            Population     = wardFamilyCounts.GetValueOrDefault(w)?.Total ?? 0,
            MaleCount      = wardFamilyCounts.GetValueOrDefault(w)?.Male   ?? 0,
            FemaleCount    = wardFamilyCounts.GetValueOrDefault(w)?.Female ?? 0
        }).ToList();

        return new MapViewModel
        {
            Markers           = markers,
            TypeSummaries     = summaries,
            WardNumbers       = wardNumbers,
            WardDetails       = wardDetails,
            FilteredWardNo    = wardNo,
            FilteredEntityType = entityType
        };
    }

    public async Task<List<MapMarker>> GetInstitutionMarkersAsync(int? wardNo = null)
    {
        var markers = new List<MapMarker>();

        // Educational Institutions
        markers.AddRange(await BuildInstitutionMarkers(
            _db.EducationalInstitutions.AsNoTracking(),
            e => e.SurveyEntry!.WardNo,
            e => e.Name ?? e.InstitutionType ?? "शैक्षिक संस्था",
            e => e.InstitutionType ?? "",
            e => e.SurveyEntry,
            "शैक्षिक संस्था", "educational_institution",
            "#52B788", "bi-school",
            wardNo));

        // Health Institutions
        markers.AddRange(await BuildInstitutionMarkers(
            _db.HealthInstitutions.AsNoTracking(),
            h => h.SurveyEntry!.WardNo,
            h => h.Name ?? h.InstitutionType ?? "स्वास्थ्य संस्था",
            h => h.InstitutionType ?? "",
            h => h.SurveyEntry,
            "स्वास्थ्य संस्था", "health_institution",
            "#E76F51", "bi-hospital",
            wardNo));

        // Financial Institutions
        markers.AddRange(await BuildInstitutionMarkers(
            _db.FinancialInstitutions.AsNoTracking(),
            f => f.SurveyEntry!.WardNo,
            f => f.Name ?? f.InstitutionType ?? "वित्तीय संस्था",
            f => f.InstitutionType ?? "",
            f => f.SurveyEntry,
            "वित्तीय संस्था", "financial_institution",
            "#E9C46A", "bi-bank",
            wardNo));

        // Security Bodies
        markers.AddRange(await BuildInstitutionMarkers(
            _db.SecurityBodies.AsNoTracking(),
            s => s.SurveyEntry!.WardNo,
            s => s.Name ?? s.BodyType ?? "सुरक्षा निकाय",
            s => s.BodyType ?? "",
            s => s.SurveyEntry,
            "सुरक्षा निकाय", "security_body",
            "#457B9D", "bi-shield-lock",
            wardNo));

        // Hotels / Resorts
        markers.AddRange(await BuildInstitutionMarkers(
            _db.HotelResorts.AsNoTracking(),
            h => h.SurveyEntry!.WardNo,
            h => h.HotelName ?? h.HotelType ?? "होटल/रिसोर्ट",
            h => h.HotelType ?? "",
            h => h.SurveyEntry,
            "होटल/रिसोर्ट", "hotel_resort",
            "#D4A373", "bi-building",
            wardNo));

        // Bridges
        markers.AddRange(await BuildInstitutionMarkers(
            _db.Bridges.AsNoTracking(),
            b => b.SurveyEntry!.WardNo,
            b => b.BridgeType ?? "पुल/पुलेसा",
            b => b.BridgeType ?? "",
            b => b.SurveyEntry,
            "पुल/पुलेसा", "bridge",
            "#D4A373", "bi-signpost-2",
            wardNo));

        // Industries
        markers.AddRange(await BuildInstitutionMarkers(
            _db.Industries.AsNoTracking(),
            i => i.SurveyEntry!.WardNo,
            i => i.IndustryType ?? "उद्योग",
            i => i.IndustryType ?? "",
            i => i.SurveyEntry,
            "उद्योग", "industry",
            "#D4A373", "bi-gear",
            wardNo));

        // NGOs / Clubs
        markers.AddRange(await BuildInstitutionMarkers(
            _db.NgoClubGroups.AsNoTracking(),
            n => n.SurveyEntry!.WardNo,
            n => n.OrgName ?? n.OrgType ?? "गैरसरकारी संस्था",
            n => n.OrgType ?? "",
            n => n.SurveyEntry,
            "गैरसरकारी संस्था", "ngo_club_group",
            "#D4A373", "bi-people",
            wardNo));

        // Land Use
        markers.AddRange(await BuildInstitutionMarkers(
            _db.LandUses.AsNoTracking(),
            l => l.SurveyEntry!.WardNo,
            l => l.LandType ?? "भू-उपयोग",
            l => l.LandType ?? "",
            l => l.SurveyEntry,
            "भू-उपयोग", "land_use",
            "#52B788", "bi-map",
            wardNo));

        // Natural Resources
        markers.AddRange(await BuildInstitutionMarkers(
            _db.NaturalResources.AsNoTracking(),
            n => n.SurveyEntry!.WardNo,
            n => n.Name ?? n.ResourceType ?? "प्राकृतिक श्रोत",
            n => n.ResourceType ?? "",
            n => n.SurveyEntry,
            "प्राकृतिक श्रोत", "natural_resource",
            "#52B788", "bi-gem",
            wardNo));

        // Cultural Heritage
        markers.AddRange(await BuildInstitutionMarkers(
            _db.CulturalHeritages.AsNoTracking(),
            c => c.SurveyEntry!.WardNo,
            c => c.Name ?? c.HeritageType ?? "सांस्कृतिक सम्पदा",
            c => c.HeritageType ?? "",
            c => c.SurveyEntry,
            "सांस्कृतिक सम्पदा", "cultural_heritage",
            "#B5838D", "bi-palette",
            wardNo));

        // Trade / Business
        markers.AddRange(await BuildInstitutionMarkers(
            _db.TradeBusinesses.AsNoTracking(),
            t => t.SurveyEntry!.WardNo,
            t => t.BusinessName ?? t.Ownership ?? "व्यापार/व्यवसाय",
            t => t.Ownership ?? "",
            t => t.SurveyEntry,
            "व्यापार/व्यवसाय", "trade_business",
            "#D4A373", "bi-shop",
            wardNo));

        // Forests
        markers.AddRange(await BuildInstitutionMarkers(
            _db.Forests.AsNoTracking(),
            f => f.SurveyEntry!.WardNo,
            f => f.ForestName ?? f.ForestType ?? "वन",
            f => f.ForestType ?? "",
            f => f.SurveyEntry,
            "वन", "forest",
            "#52B788", "bi-tree",
            wardNo));

        // Roads
        markers.AddRange(await BuildInstitutionMarkers(
            _db.Roads.AsNoTracking(),
            r => r.SurveyEntry!.WardNo,
            r => r.RoadName ?? r.RoadType ?? "सडक",
            r => r.RoadType ?? "",
            r => r.SurveyEntry,
            "सडक", "road",
            "#D4A373", "bi-road",
            wardNo));

        // Buildings
        markers.AddRange(await BuildInstitutionMarkers(
            _db.Buildings.AsNoTracking(),
            b => b.SurveyEntry!.WardNo,
            b => b.Name ?? b.BuildingType ?? "भवन",
            b => b.BuildingType ?? "",
            b => b.SurveyEntry,
            "भवन", "building",
            "#B5838D", "bi-building",
            wardNo));

        return markers;
    }

    public async Task<List<MapMarker>> GetHouseholdMarkersAsync(int? wardNo = null)
    {
        var query = _db.Households.AsNoTracking()
            .Where(h => h.Latitude != null && h.Longitude != null
                      && h.WardNo != null);

        if (wardNo.HasValue)
            query = query.Where(h => h.WardNo == wardNo.Value);

        var data = await query
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RespondentName,
                h.Latitude,
                h.Longitude
            })
            .ToListAsync();

        return data.Select(h => new MapMarker
        {
            Id = h.Id,
            EntityType = "घरपरिवार",
            EntityTypeKey = "household",
            Name = h.SettlementName ?? h.RespondentName ?? $"घरपरिवार #{h.Id}",
            Detail = $"वडा नं. {h.WardNo}",
            WardNo = h.WardNo,
            Latitude = h.Latitude!.Value,
            Longitude = h.Longitude!.Value,
            Color = "#2D6A4F",
            Icon = "bi-house"
        }).ToList();
    }

    public async Task<List<int>> GetWardNumbersAsync()
    {
        var hhWards = _db.Households
            .Where(h => h.WardNo != null && h.Latitude != null)
            .Select(h => h.WardNo!.Value);

        var instWards = _db.SurveyEntries
            .Where(s => s.WardNo != null)
            .Select(s => s.WardNo!.Value);

        return await hhWards.Union(instWards).Distinct().OrderBy(w => w).ToListAsync();
    }

    public async Task<List<MapTypeSummary>> GetTypeSummariesAsync()
    {
        if (_cache.TryGetValue(CacheKeys.MapTypeSummaries, out List<MapTypeSummary>? cached))
            return cached!;

        var summaries = new List<MapTypeSummary>();

        // Institution type counts — grouped by entity
        summaries.Add(await BuildTypeSummary(
            _db.EducationalInstitutions.AsNoTracking(),
            "शैक्षिक संस्था", "educational_institution",
            "#52B788", "bi-school"));

        summaries.Add(await BuildTypeSummary(
            _db.HealthInstitutions.AsNoTracking(),
            "स्वास्थ्य संस्था", "health_institution",
            "#E76F51", "bi-hospital"));

        summaries.Add(await BuildTypeSummary(
            _db.FinancialInstitutions.AsNoTracking(),
            "वित्तीय संस्था", "financial_institution",
            "#E9C46A", "bi-bank"));

        summaries.Add(await BuildTypeSummary(
            _db.SecurityBodies.AsNoTracking(),
            "सुरक्षा निकाय", "security_body",
            "#457B9D", "bi-shield-lock"));

        summaries.Add(await BuildTypeSummary(
            _db.HotelResorts.AsNoTracking(),
            "होटल/रिसोर्ट", "hotel_resort",
            "#D4A373", "bi-building"));

        summaries.Add(await BuildTypeSummary(
            _db.Bridges.AsNoTracking(),
            "पुल/पुलेसा", "bridge",
            "#D4A373", "bi-signpost-2"));

        summaries.Add(await BuildTypeSummary(
            _db.Industries.AsNoTracking(),
            "उद्योग", "industry",
            "#D4A373", "bi-gear"));

        summaries.Add(await BuildTypeSummary(
            _db.NgoClubGroups.AsNoTracking(),
            "गैरसरकारी संस्था", "ngo_club_group",
            "#D4A373", "bi-people"));

        summaries.Add(await BuildTypeSummary(
            _db.LandUses.AsNoTracking(),
            "भू-उपयोग", "land_use",
            "#52B788", "bi-map"));

        summaries.Add(await BuildTypeSummary(
            _db.NaturalResources.AsNoTracking(),
            "प्राकृतिक श्रोत", "natural_resource",
            "#52B788", "bi-gem"));

        summaries.Add(await BuildTypeSummary(
            _db.CulturalHeritages.AsNoTracking(),
            "सांस्कृतिक सम्पदा", "cultural_heritage",
            "#B5838D", "bi-palette"));

        summaries.Add(await BuildTypeSummary(
            _db.TradeBusinesses.AsNoTracking(),
            "व्यापार/व्यवसाय", "trade_business",
            "#D4A373", "bi-shop"));

        summaries.Add(await BuildTypeSummary(
            _db.Forests.AsNoTracking(),
            "वन", "forest",
            "#52B788", "bi-tree"));

        summaries.Add(await BuildTypeSummary(
            _db.Roads.AsNoTracking(),
            "सडक", "road",
            "#D4A373", "bi-road"));

        summaries.Add(await BuildTypeSummary(
            _db.Buildings.AsNoTracking(),
            "भवन", "building",
            "#B5838D", "bi-building"));

        // Household count
        var hhCount = await _db.Households
            .CountAsync(h => h.Latitude != null && h.Longitude != null);
        summaries.Add(new MapTypeSummary
        {
            EntityType = "घरपरिवार",
            EntityTypeKey = "household",
            Count = hhCount,
            Color = "#2D6A4F",
            Icon = "bi-house"
        });

        // Sort: households first, then by count descending
        var result = summaries
            .OrderByDescending(s => s.EntityTypeKey == "household" ? 1 : 0)
            .ThenByDescending(s => s.Count)
            .ToList();

        _cache.Set(CacheKeys.MapTypeSummaries, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheKeys.MapDuration,
            Size = 1
        });

        return result;
    }

    // ═══════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════

    /// <summary>
    /// Generic helper to build institution markers from any entity type
    /// that has SurveyEntry navigation, Latitude, Longitude.
    /// </summary>
        private async Task<List<MapMarker>> BuildInstitutionMarkers<TEntity>(
        IQueryable<TEntity> query,
        Func<TEntity, int?> wardSel,
        Func<TEntity, string> nameSel,
        Func<TEntity, string> detailSel,
        Func<TEntity, SurveyEntry?> surveySel,
        string entityTypeNepali,
        string entityTypeKey,
        string color,
        string icon,
        int? wardNo) where TEntity : class
    {
        // Load only entities that have GPS (EF.Property is still allowed here, inside the IQueryable)
        var raw = await query
            .Where(e => EF.Property<double?>(e, "Latitude") != null
                     && EF.Property<double?>(e, "Longitude") != null)
            .ToListAsync();

        if (!raw.Any())
            return new List<MapMarker>();

        // Collect SurveyEntryIds via reflection (safe AFTER ToListAsync — no EF.Property here)
        var surveyEntryIds = new List<long>();
        foreach (var e in raw)
        {
            var prop = typeof(TEntity).GetProperty("SurveyEntryId");
            if (prop != null)
            {
                var idVal = prop.GetValue(e);
                if (idVal is long id && id > 0)
                    surveyEntryIds.Add(id);
            }
        }
        surveyEntryIds = surveyEntryIds.Distinct().ToList();

        // Lightweight projection query for WardNo only (avoids loading full SurveyEntry + timestamp cast problem)
        var wardMap = new Dictionary<long, int?>();
        if (surveyEntryIds.Any())
        {
            wardMap = await _db.SurveyEntries.AsNoTracking()
                .Where(s => surveyEntryIds.Contains(s.Id))
                .Select(s => new { s.Id, s.WardNo })
                .ToDictionaryAsync(s => s.Id, s => s.WardNo);
        }

        // Filter in-memory using the wardMap (no surveySel(e) call that would hit null SurveyEntry)
        var filtered = raw.Where(e =>
        {
            var prop = typeof(TEntity).GetProperty("SurveyEntryId");
            if (prop == null) return false;
            var idVal = prop.GetValue(e);
            if (idVal is not long seid || !wardMap.TryGetValue(seid, out var wno)) return false;
            return wno != null && (!wardNo.HasValue || wno == wardNo.Value);
        }).ToList();

        // Project using reflection for Id/Lat/Long (no EF.Property after ToListAsync)
        return filtered.Select(e =>
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            var latProp = typeof(TEntity).GetProperty("Latitude");
            var lngProp = typeof(TEntity).GetProperty("Longitude");

            long id = (idProp?.GetValue(e) as long?) ?? 0;
            double? lat = latProp?.GetValue(e) as double?;
            double? lng = lngProp?.GetValue(e) as double?;

            // Ward from our safe map (ignore the passed wardSel/surveySel for filter/projection to avoid null nav)
            int? wno = null;
            var seProp = typeof(TEntity).GetProperty("SurveyEntryId");
            if (seProp != null && seProp.GetValue(e) is long seid && wardMap.TryGetValue(seid, out var w))
                wno = w;

            return new MapMarker
            {
                Id = id,
                EntityType = entityTypeNepali,
                EntityTypeKey = entityTypeKey,
                Name = nameSel(e),
                Detail = detailSel(e),
                WardNo = wno,
                Latitude = lat ?? 0,
                Longitude = lng ?? 0,
                Color = color,
                Icon = icon
            };
        }).ToList();
    }

    /// <summary>
    /// Count entities with GPS data for the type summary.
    /// </summary>
    private async Task<MapTypeSummary> BuildTypeSummary<TEntity>(
        IQueryable<TEntity> query,
        string entityTypeNepali,
        string entityTypeKey,
        string color,
        string icon) where TEntity : class
    {
        var count = await query
            .CountAsync(e => EF.Property<double?>(e, "Latitude") != null
                          && EF.Property<double?>(e, "Longitude") != null);

        return new MapTypeSummary
        {
            EntityType = entityTypeNepali,
            EntityTypeKey = entityTypeKey,
            Count = count,
            Color = color,
            Icon = icon
        };
    }
}
