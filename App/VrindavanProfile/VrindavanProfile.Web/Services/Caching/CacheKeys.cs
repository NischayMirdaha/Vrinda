namespace VrindavanProfile.Web.Services.Caching;

/// <summary>
/// Centralised cache-key constants and helper methods for the application.
/// All cache keys go through this class so they are easy to audit and change.
/// </summary>
public static class CacheKeys
{
    // ─── Duration presets ────────────────────────────
    /// <summary>Survey data is relatively static — cache for 30 minutes.</summary>
    public static readonly TimeSpan SurveyDataDuration = TimeSpan.FromMinutes(30);

    /// <summary>Static metadata (table lists) — cache for 1 hour.</summary>
    public static readonly TimeSpan MetadataDuration = TimeSpan.FromHours(1);

    /// <summary>Map summaries — cache for 30 minutes.</summary>
    public static readonly TimeSpan MapDuration = TimeSpan.FromMinutes(30);

    // ─── Ward Numbers ────────────────────────────────
    public const string WardNumbers = "cache:ward_numbers";

    // ─── Dashboard ───────────────────────────────────
    public static string DashboardStats(int? wardNo) =>
        wardNo.HasValue ? $"cache:dashboard:stats:ward_{wardNo.Value}" : "cache:dashboard:stats:all";

    public static string DashboardCharts(int? wardNo) =>
        wardNo.HasValue ? $"cache:dashboard:charts:ward_{wardNo.Value}" : "cache:dashboard:charts:all";

    // ─── Table Lists (static metadata) ───────────────
    public const string HouseholdTableList = "cache:tables:household_list";
    public const string InstitutionTableList = "cache:tables:institution_list";

    // ─── Individual Tables ───────────────────────────
    public static string HouseholdTable(int tableId, int? wardNo) =>
        wardNo.HasValue
            ? $"cache:tables:hh:{tableId}:ward_{wardNo.Value}"
            : $"cache:tables:hh:{tableId}:all";

    public static string InstitutionTable(int tableId, int? wardNo) =>
        wardNo.HasValue
            ? $"cache:tables:inst:{tableId}:ward_{wardNo.Value}"
            : $"cache:tables:inst:{tableId}:all";

    // ─── Map ─────────────────────────────────────────
    public const string MapTypeSummaries = "cache:map:type_summaries";

    public static string MapData(int? wardNo, string? entityType) =>
        $"cache:map:data:ward_{wardNo ?? 0}:type_{entityType ?? "all"}";

    // ─── Search ──────────────────────────────────────
    public const string SearchHouseholdTables = "cache:search:hh_tables";
    public const string SearchInstitutionTables = "cache:search:inst_tables";
}
