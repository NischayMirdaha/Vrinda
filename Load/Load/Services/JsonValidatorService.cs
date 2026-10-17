using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Load.Models;

namespace Load.Services;

public class JsonValidatorService
{
    public async Task<JsonValidationResult> ValidateAsync(
        Stream stream, string fileName, long fileSize,
        string[] requiredKeys)
    {
        var result = new JsonValidationResult
        {
            FileName = fileName,
            FileSizeBytes = fileSize
        };

        try
        {
            stream.Position = 0;
            using var sr = new StreamReader(stream, System.Text.Encoding.UTF8);
            using var jsonReader = new JsonTextReader(sr);

            // Step 1: Check if it starts with [ (plain array)
            if (!await jsonReader.ReadAsync())
            {
                result.Errors.Add("File is empty or not readable.");
                return result;
            }

            if (jsonReader.TokenType != JsonToken.StartArray)
            {
                result.Errors.Add(
                    $"Expected plain JSON array [...] but found {jsonReader.TokenType}. " +
                    "File must start with [ not {{.");
                return result;
            }

            result.IsPlainArray = true;

            // Step 2: Read rows one by one, validate structure
            int rowIndex = 0;
            HashSet<string>? firstRowKeys = null;
            int inconsistentKeyRows = 0;
            int parseErrors = 0;

            while (true)
            {
                try
                {
                    if (!await jsonReader.ReadAsync()) break;
                    if (jsonReader.TokenType == JsonToken.EndArray) break;

                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        var row = await JObject.LoadAsync(jsonReader);
                        var keys = new HashSet<string>(row.Properties().Select(p => p.Name));

                        if (rowIndex == 0)
                        {
                            // First row — capture structure
                            firstRowKeys = keys;
                            result.KeysPerRow = keys.Count;

                            // Count nulls
                            result.NullCountFirstRow = row.Properties()
                                .Count(p => p.Value.Type == JTokenType.Null);
                            result.NonNullCountFirstRow = keys.Count - result.NullCountFirstRow;

                            // Key summary (first 50 keys)
                            foreach (var prop in row.Properties().Take(50))
                            {
                                var ki = new JsonKeyInfo
                                {
                                    Key = prop.Name,
                                    IsNull = prop.Value.Type == JTokenType.Null
                                };

                                if (!ki.IsNull)
                                {
                                    ki.DataType = prop.Value.Type switch
                                    {
                                        JTokenType.String => "string",
                                        JTokenType.Float => "float",
                                        JTokenType.Integer => "integer",
                                        JTokenType.Boolean => "boolean",
                                        _ => prop.Value.Type.ToString()
                                    };
                                    ki.SampleValue = prop.Value.ToString().Length > 60
                                        ? prop.Value.ToString()[..60] + "..."
                                        : prop.Value.ToString();
                                }
                                else
                                {
                                    ki.DataType = "null";
                                }

                                result.KeySummary.Add(ki);
                            }

                            // Check required keys
                            foreach (var rk in requiredKeys)
                            {
                                if (keys.Contains(rk))
                                    result.RequiredKeysFound.Add(rk);
                                else
                                    result.RequiredKeysMissing.Add(rk);
                            }

                            // Sample subject areas (institutional only)
                            var subjectKey = "विषयगतक्षेत्र छान्नुहोस्";
                            if (keys.Contains(subjectKey))
                            {
                                var val = row[subjectKey]?.ToString();
                                if (!string.IsNullOrEmpty(val))
                                    result.SampleSubjectAreas.Add(val);
                            }
                        }
                        else
                        {
                            // Check key consistency
                            if (keys.Count != firstRowKeys!.Count)
                            {
                                inconsistentKeyRows++;
                                if (inconsistentKeyRows <= 3)
                                {
                                    result.Warnings.Add(
                                        $"Row {rowIndex}: has {keys.Count} keys " +
                                        $"(expected {firstRowKeys.Count})");
                                }
                            }

                            // Collect more subject areas (up to 10)
                            if (result.SampleSubjectAreas.Count < 10)
                            {
                                var subjectKey = "विषयगतक्षेत्र छान्नुहोस्";
                                var val = row[subjectKey]?.ToString();
                                if (!string.IsNullOrEmpty(val) &&
                                    !result.SampleSubjectAreas.Contains(val))
                                {
                                    result.SampleSubjectAreas.Add(val);
                                }
                            }
                        }

                        rowIndex++;
                    }
                }
                catch (JsonReaderException ex)
                {
                    parseErrors++;
                    if (parseErrors <= 5)
                    {
                        result.Errors.Add(
                            $"Row {rowIndex}: JSON parse error at line {ex.LineNumber} " +
                            $"position {ex.LinePosition} — {ex.Message}");
                    }

                    // Try to recover
                    try
                    {
                        while (await jsonReader.ReadAsync())
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject ||
                                jsonReader.TokenType == JsonToken.EndArray)
                                break;
                        }
                    }
                    catch { break; }

                    rowIndex++;
                }
            }

            result.TotalRows = rowIndex;
            result.HasConsistentKeys = inconsistentKeyRows == 0;

            if (inconsistentKeyRows > 3)
            {
                result.Warnings.Add(
                    $"Total {inconsistentKeyRows} rows with inconsistent key count.");
            }

            if (parseErrors > 5)
            {
                result.Errors.Add($"Total {parseErrors} JSON parse errors found.");
            }

            // Final verdict
            result.IsValid = result.Errors.Count == 0
                && result.TotalRows > 0
                && result.IsPlainArray
                && result.RequiredKeysMissing.Count == 0;

            if (result.IsValid && parseErrors == 0)
            {
                result.Warnings.Insert(0, "✅ File is valid and ready for import.");
            }
            else if (result.IsValid && parseErrors > 0)
            {
                result.Warnings.Insert(0,
                    $"⚠️ File is mostly valid but has {parseErrors} parse error(s). " +
                    "Import will skip bad rows.");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Fatal error: {ex.Message}");
        }

        return result;
    }
}