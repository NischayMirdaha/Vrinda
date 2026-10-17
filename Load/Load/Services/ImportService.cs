using Npgsql;
using Newtonsoft.Json;
using NpgsqlTypes;
using Newtonsoft.Json.Linq;
using Load.Models;

namespace Load.Services;

/// <summary>
/// Universal import service for all survey types.
/// - Streams JSON row-by-row (handles 400MB+ files)
/// - Recovers from parse errors (skips bad rows)
/// - Deduplicates columns (no "column specified more than once" errors)
/// - Routes institutional rows by विषयगतक्षेत्र
/// - Links child tables to parent via _submission__id
/// </summary>
public class ImportService
{
    private readonly string _connectionString;
    private readonly ILogger<ImportService> _logger;

    public ImportService(string connectionString, ILogger<ImportService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    // ================================================================
    // IMPORT INSTITUTIONAL — parent + child routing by subject area
    // ================================================================
    public async Task<ImportResult> ImportInstitutionalAsync(
        Stream jsonStream,
        Dictionary<string, ColumnMap> mapping,
        List<(string Keyword, string Table)> subjectKeywords)
    {
        var result = new ImportResult { ImportType = "Institutional" };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var unmappedKeys = new HashSet<string>();
        var subjectCounts = new Dictionary<string, int>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        int rowIndex = 0;
        await foreach (var row in StreamJsonRows(jsonStream, result))
        {
            using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Step 1: Insert survey_entry
                long seId = await InsertRow(conn, row, "survey_entry", 0,
                    mapping, unmappedKeys);

                // Step 2: Route to child table
                var subjectToken = row["विषयगतक्षेत्र छान्नुहोस्"];
                if (subjectToken != null && subjectToken.Type != JTokenType.Null)
                {
                    var subject = subjectToken.ToString().Trim();
                    var childTable = FindChildTable(subject, subjectKeywords);

                    if (childTable != null)
                    {
                        await InsertRow(conn, row, childTable, seId,
                            mapping, unmappedKeys);
                        subjectCounts[subject] =
                            subjectCounts.GetValueOrDefault(subject) + 1;
                    }
                    else
                    {
                        result.Warnings.Add($"Row {rowIndex}: Unknown subject '{subject}'");
                    }
                }

                await tx.CommitAsync();
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                result.ErrorCount++;
                if (result.Errors.Count < 100)
                    result.Errors.Add($"Row {rowIndex}: {ex.Message}");
            }

            rowIndex++;
            if (rowIndex % 500 == 0)
                _logger.LogInformation("Institutional: {N} rows", rowIndex);
        }

        sw.Stop();
        result.TotalRows = rowIndex;
        result.Duration = sw.Elapsed;
        result.UnmappedKeys = unmappedKeys.OrderBy(k => k).ToList();
        result.SubjectAreaCounts = subjectCounts;
        return result;
    }

    // ================================================================
    // IMPORT PARENT TABLE — household (no routing, direct insert)
    // ================================================================
    public async Task<ImportResult> ImportParentAsync(
        Stream jsonStream, string tableName,
        Dictionary<string, ColumnMap> mapping)
    {
        var result = new ImportResult { ImportType = tableName };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var unmappedKeys = new HashSet<string>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        int rowIndex = 0;
        await foreach (var row in StreamJsonRows(jsonStream, result))
        {
            try
            {
                await InsertRow(conn, row, tableName, 0,
                    mapping, unmappedKeys);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                if (result.Errors.Count < 100)
                    result.Errors.Add($"Row {rowIndex}: {ex.Message}");
            }

            rowIndex++;
            if (rowIndex % 1000 == 0)
                _logger.LogInformation("{Table}: {N} rows", tableName, rowIndex);
        }

        sw.Stop();
        result.TotalRows = rowIndex;
        result.Duration = sw.Elapsed;
        result.UnmappedKeys = unmappedKeys.OrderBy(k => k).ToList();
        return result;
    }

    // ================================================================
    // IMPORT CHILD TABLE — family, house_detail, death
    // Links to parent via _submission__id → parent.odk_id
    // ================================================================
    public async Task<ImportResult> ImportChildAsync(
        Stream jsonStream, string childTable, string parentTable,
        Dictionary<string, ColumnMap> mapping)
    {
        var result = new ImportResult { ImportType = childTable };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var unmappedKeys = new HashSet<string>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Build lookup: parent odk_id → parent.id
        var parentLookup = new Dictionary<long, long>();
        var lookupSql = $"SELECT id, odk_id FROM {parentTable} WHERE odk_id IS NOT NULL";
        await using (var cmd = new NpgsqlCommand(lookupSql, conn))
        {
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var dbId = rdr.GetInt64(0);
                var odkId = rdr.GetInt64(1);
                parentLookup[odkId] = dbId;
            }
        }
        _logger.LogInformation("Loaded {Count} {Table} lookups",
            parentLookup.Count, parentTable);

        // FK column name
        var fkColumn = parentTable == "household" ? "household_id" : "survey_entry_id";

        int rowIndex = 0;
        await foreach (var row in StreamJsonRows(jsonStream, result))
        {
            try
            {
                // Find parent ID
                long parentId = 0;
                var subIdToken = row["_submission__id"];
                if (subIdToken != null && subIdToken.Type != JTokenType.Null)
                {
                    var subId = (long)(double)subIdToken;
                    parentLookup.TryGetValue(subId, out parentId);
                }

                await InsertRow(conn, row, childTable, parentId,
                    mapping, unmappedKeys, fkColumn);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                if (result.Errors.Count < 100)
                    result.Errors.Add($"Row {rowIndex}: {ex.Message}");
            }

            rowIndex++;
            if (rowIndex % 5000 == 0)
                _logger.LogInformation("{Table}: {N} rows", childTable, rowIndex);
        }

        sw.Stop();
        result.TotalRows = rowIndex;
        result.Duration = sw.Elapsed;
        result.UnmappedKeys = unmappedKeys.OrderBy(k => k).ToList();
        return result;
    }

    // ================================================================
    // IMPORT MULTIPLE FILES — sequential parts of same table
    // ================================================================
    public async Task<ImportResult> ImportMultipleAsync(
        IFormFileCollection files, string mode, string tableName,
        Dictionary<string, ColumnMap> mapping,
        List<(string Keyword, string Table)>? subjectKeywords = null,
        string? parentTable = null)
    {
        var combined = new ImportResult { ImportType = $"{tableName} ({files.Count} files)" };
        var sw = System.Diagnostics.Stopwatch.StartNew();

        foreach (var file in files.OrderBy(f => f.FileName))
        {
            _logger.LogInformation("Processing file: {Name} ({Size} MB)",
                file.FileName, file.Length / 1024 / 1024);

            using var stream = file.OpenReadStream();

            ImportResult fileResult;
            if (mode == "institutional")
            {
                fileResult = await ImportInstitutionalAsync(
                    stream, mapping, subjectKeywords!);
            }
            else if (mode == "parent")
            {
                fileResult = await ImportParentAsync(stream, tableName, mapping);
            }
            else // child
            {
                fileResult = await ImportChildAsync(
                    stream, tableName, parentTable!, mapping);
            }

            // Merge results
            combined.SuccessCount += fileResult.SuccessCount;
            combined.ErrorCount += fileResult.ErrorCount;
            combined.SkippedCount += fileResult.SkippedCount;
            combined.TotalRows += fileResult.TotalRows;
            combined.FilesProcessed++;

            foreach (var e in fileResult.Errors)
            {
                if (combined.Errors.Count < 100)
                    combined.Errors.Add($"[{file.FileName}] {e}");
            }
            foreach (var w in fileResult.Warnings)
                combined.Warnings.Add($"[{file.FileName}] {w}");
            foreach (var k in fileResult.UnmappedKeys)
                combined.UnmappedKeys.Add(k);
            foreach (var kvp in fileResult.SubjectAreaCounts)
            {
                combined.SubjectAreaCounts[kvp.Key] =
                    combined.SubjectAreaCounts.GetValueOrDefault(kvp.Key) + kvp.Value;
            }
        }

        sw.Stop();
        combined.Duration = sw.Elapsed;
        combined.UnmappedKeys = combined.UnmappedKeys.Distinct().OrderBy(k => k).ToList();
        return combined;
    }

    // ================================================================
    // STREAM JSON ROWS — memory-efficient, error-recovering
    // ================================================================
    private async IAsyncEnumerable<JObject> StreamJsonRows(
        Stream stream, ImportResult result)
    {
        stream.Position = 0;
        using var sr = new StreamReader(stream, System.Text.Encoding.UTF8);
        using var jsonReader = new JsonTextReader(sr);

        // Navigate to array start
        while (await jsonReader.ReadAsync())
        {
            if (jsonReader.TokenType == JsonToken.StartArray)
                break;
        }

        bool keepReading = true;
        while (keepReading)
        {
            JObject? row = null;
            try
            {
                if (!await jsonReader.ReadAsync())
                    break;

                if (jsonReader.TokenType == JsonToken.EndArray)
                    break;

                if (jsonReader.TokenType == JsonToken.StartObject)
                {
                    row = await JObject.LoadAsync(jsonReader);
                }
            }
            catch (JsonReaderException ex)
            {
                result.SkippedCount++;
                if (result.Errors.Count < 100)
                    result.Errors.Add($"JSON parse error: {ex.Message}");

                // Recovery: skip to next object
                try
                {
                    while (await jsonReader.ReadAsync())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject ||
                            jsonReader.TokenType == JsonToken.EndArray)
                        {
                            if (jsonReader.TokenType == JsonToken.EndArray)
                                keepReading = false;
                            break;
                        }
                    }
                }
                catch
                {
                    keepReading = false;
                }
                continue;
            }

            if (row != null)
                yield return row;
        }
    }

    // ================================================================
    // FIND CHILD TABLE — contains matching for subject area
    // ================================================================
    private string? FindChildTable(string subject,
        List<(string Keyword, string Table)> keywords)
    {
        foreach (var (keyword, table) in keywords)
        {
            if (subject.Contains(keyword)) return table;
        }
        return null;
    }
        // ================================================================
    // INSERT ROW — deduplicated columns, handles all types
    // ================================================================
    private async Task<long> InsertRow(
        NpgsqlConnection conn, JObject row, string tableName,
        long parentId, Dictionary<string, ColumnMap> mapping,
        HashSet<string> unmappedKeys, string fkColumn = "survey_entry_id")
    {
        // Dictionary deduplicates columns — last value wins
        var columnValues = new Dictionary<string, (string ParamName, NpgsqlParameter Param)>();
        int pi = 0;

        // Add FK if child table
        if (parentId > 0)
        {
            columnValues[$"\"{fkColumn}\""] = ($"@p{pi}",
                new NpgsqlParameter($"@p{pi}", parentId));
            pi++;
        }

        foreach (var prop in row.Properties())
        {
            var key = prop.Name;

            if (mapping.TryGetValue(key, out var map) && map.Table == tableName)
            {
                if (map.Type == ColType.Ignore) continue;

                var val = ConvertValue(prop.Value, map.Type);
                if (val != null)
                {
                    var colName = $"\"{map.Column}\"";
                    var param = new NpgsqlParameter($"@p{pi}", val);
                    columnValues[colName] = ($"@p{pi}", param);
                    pi++;
                }
            }
            else if (!mapping.ContainsKey(key))
            {
                unmappedKeys.Add(key);
            }
        }

        string sql;
        if (columnValues.Count == 0)
        {
            sql = $"INSERT INTO {tableName} DEFAULT VALUES RETURNING id";
        }
        else
        {
            var cols = string.Join(", ", columnValues.Keys);
            var parms = string.Join(", ", columnValues.Values.Select(v => v.ParamName));
            sql = $"INSERT INTO {tableName} ({cols}) VALUES ({parms}) RETURNING id";
        }

        await using var cmd = new NpgsqlCommand(sql, conn);
        foreach (var cv in columnValues.Values)
            cmd.Parameters.Add(cv.Param);

        return (long)(await cmd.ExecuteScalarAsync())!;
    }

    // ================================================================
    // CONVERT VALUE — JSON token → .NET type → PostgreSQL
    // ================================================================
    private object? ConvertValue(JToken token, ColType type)
    {
        if (token.Type == JTokenType.Null) return null;

        return type switch
        {
            ColType.Text => token.Type switch
            {
                JTokenType.String => token.ToString(),
                JTokenType.Float => ((double)token).ToString(),
                JTokenType.Integer => ((long)token).ToString(),
                _ => token.ToString()
            },

            ColType.Integer => token.Type switch
            {
                JTokenType.Float => (int)(double)token,
                JTokenType.Integer => (int)(long)token,
                JTokenType.String => int.TryParse(token.ToString(), out var i) ? i : (object?)null,
                _ => null
            },

            ColType.BigInt => token.Type switch
            {
                JTokenType.Float => (long)(double)token,
                JTokenType.Integer => (long)token,
                JTokenType.String => long.TryParse(token.ToString(), out var l) ? l : (object?)null,
                _ => null
            },

            ColType.Double => token.Type switch
            {
                JTokenType.Float => (double)token,
                JTokenType.Integer => (double)(long)token,
                JTokenType.String => double.TryParse(token.ToString(), out var d) ? d : (object?)null,
                _ => null
            },

            ColType.SmallInt => token.Type switch
            {
                JTokenType.Float => (short)(double)token,
                JTokenType.Integer => (short)(long)token,
                JTokenType.String => short.TryParse(token.ToString(), out var s) ? s : (object?)null,
                _ => null
            },

            ColType.Timestamp => token.Type == JTokenType.String 
    ? token.ToString() 
    : null,

            ColType.Ignore => null,
            _ => null
        };
    }
    // ================================================================
    // GET TABLE ROW COUNTS — after import
    // ================================================================
    public async Task<Dictionary<string, long>> GetTableRowCountsAsync()
    {
        var counts = new Dictionary<string, long>();
        var tables = new[]
        {
            "survey_entry", "educational_institution", "health_institution",
            "building", "security_body", "financial_institution",
            "hotel_resort", "bridge", "industry", "ngo_club_group",
            "land_use", "natural_resource", "cultural_heritage",
            "trade_business", "forest", "road", "ward_statistics",
            "household", "family_member", "house_detail", "hh_death_record"
        };

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        foreach (var table in tables)
        {
            try
            {
                await using var cmd = new NpgsqlCommand(
                    $"SELECT COUNT(*) FROM {table}", conn);
                var count = (long)(await cmd.ExecuteScalarAsync())!;
                if (count > 0)
                    counts[table] = count;
            }
            catch { }
        }

        return counts;
    }
} // End of ImportService class