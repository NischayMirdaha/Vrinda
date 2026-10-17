using System.IO.Compression;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;

namespace Converty.Services;

public class ExcelToJsonService
{
    // 23MB limit in bytes (leaving 1MB safety margin under 24MB)
    private const long MaxFileBytes = 23 * 1024 * 1024;

    /// <summary>
    /// Reads the Excel stream and writes both single JSON and split JSON files (split below 24MB) per sheet to the temp output directory.
    /// Also performs column-by-column data analysis to generate database schemas and logs a complete report.
    /// Streams data row-by-row — never loads all rows into RAM at once.
    /// </summary>
    public async Task<ConversionResult> ConvertAsync(
        Stream excelStream, string outputFolder)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var sessionId = Guid.NewGuid().ToString("N");
        var sessionFolder = Path.Combine(outputFolder, sessionId);
        var singleDir = Path.Combine(sessionFolder, "single");
        var splitDir = Path.Combine(sessionFolder, "split");

        Directory.CreateDirectory(singleDir);
        Directory.CreateDirectory(splitDir);

        var sheets = new List<SheetInfo>();
        var sheetAnalyses = new List<SheetAnalysis>();

        using var reader = ExcelReaderFactory.CreateReader(excelStream, new ExcelReaderConfiguration
        {
            FallbackEncoding = Encoding.GetEncoding(1252)
        });

        int sheetIndex = 0;

        do
        {
            sheetIndex++;
            var sheetName     = string.IsNullOrWhiteSpace(reader.Name) ? $"Sheet{sheetIndex}" : reader.Name;
            var safeSheetName = SanitizeFileName(sheetName);

            var sheetInfo = new SheetInfo
            {
                Index    = sheetIndex,
                Name     = sheetName,
                SafeName = safeSheetName
            };

            var sheetAnalysis = new SheetAnalysis
            {
                SheetName = sheetName
            };

            string[] headers  = [];
            bool     firstRow = true;
            long     rowCount = 0;
            long     bufferedEmptyRows = 0;

            // ── Single JSON file ──
            var singleFileName = $"{safeSheetName}.json";
            var singleFilePath = Path.Combine(singleDir, singleFileName);
            var (singleJw, singleSw, singleCs) = await CreateJsonWriterForPathAsync(singleFilePath);
            sheetInfo.SingleFileName = singleFileName;

            // ── Split JSON files ──
            var splitFiles = new List<SplitFileInfo>();
            int partIndex = 1;
            var splitFileName = $"{safeSheetName}_{partIndex}.json";
            var splitFilePath = Path.Combine(splitDir, splitFileName);
            var (splitJw, splitSw, splitCs) = await CreateJsonWriterForPathAsync(splitFilePath);

            // Pre-allocate row cache to avoid GC churn on millions of rows
            object?[]? rowCache = null;
            ColumnAnalyzer[] columnAnalyzers = [];

            while (reader.Read())
            {
                int fieldCount = reader.FieldCount;

                if (firstRow)
                {
                    // ── Row 0 → headers with uniqueness enforcement ───────────────
                    headers = new string[fieldCount];
                    columnAnalyzers = new ColumnAnalyzer[fieldCount];
                    var headerCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < fieldCount; i++)
                    {
                        var h = reader.GetValue(i);
                        var baseHeader = (h == null || h is DBNull || string.IsNullOrWhiteSpace(h.ToString()))
                            ? $"Column_{i + 1}"
                            : h.ToString()!.Trim();

                        // Enforce uniqueness for JSON keys
                        if (headerCounts.TryGetValue(baseHeader, out int count))
                        {
                            count++;
                            headerCounts[baseHeader] = count;
                            headers[i] = $"{baseHeader}_{count}";
                        }
                        else
                        {
                            headerCounts[baseHeader] = 1;
                            headers[i] = baseHeader;
                        }

                        columnAnalyzers[i] = new ColumnAnalyzer(headers[i]);
                    }
                    
                    rowCache = new object?[fieldCount];
                    firstRow = false;
                    continue;
                }

                if (rowCache == null)
                {
                    rowCache = new object?[fieldCount];
                }

                // ── Check if row is empty and populate cache ──────────────────
                bool rowIsEmpty = true;
                for (int i = 0; i < fieldCount; i++)
                {
                    var val = reader.GetValue(i);
                    rowCache[i] = val;

                    if (val != null && val is not DBNull && !string.IsNullOrWhiteSpace(val.ToString()))
                    {
                        rowIsEmpty = false;
                    }
                }

                if (rowIsEmpty)
                {
                    // Buffer it. If it's a trailing empty row, it will be trimmed at the end.
                    // If there is another non-empty row later, these empty rows will be written.
                    bufferedEmptyRows++;
                    continue;
                }

                // ── Write any buffered empty rows (they are in the middle of data) ──
                while (bufferedEmptyRows > 0)
                {
                    // Check if current split file size exceeds limit before writing
                    if (splitCs.BytesWritten >= MaxFileBytes)
                    {
                        await CloseJsonWriterAsync(splitJw, splitSw, splitCs);
                        splitFiles.Add(new SplitFileInfo 
                        { 
                            FileName = splitFileName, 
                            FileSize = new FileInfo(splitFilePath).Length 
                        });

                        partIndex++;
                        splitFileName = $"{safeSheetName}_{partIndex}.json";
                        splitFilePath = Path.Combine(splitDir, splitFileName);
                        (splitJw, splitSw, splitCs) = await CreateJsonWriterForPathAsync(splitFilePath);
                    }

                    await WriteEmptyRowAsync(singleJw, headers);
                    await WriteEmptyRowAsync(splitJw, headers);
                    rowCount++;
                    bufferedEmptyRows--;
                }

                // ── Check if current split file size exceeds limit before writing current row ──
                if (splitCs.BytesWritten >= MaxFileBytes)
                {
                    await CloseJsonWriterAsync(splitJw, splitSw, splitCs);
                    splitFiles.Add(new SplitFileInfo 
                    { 
                        FileName = splitFileName, 
                        FileSize = new FileInfo(splitFilePath).Length 
                    });

                    partIndex++;
                    splitFileName = $"{safeSheetName}_{partIndex}.json";
                    splitFilePath = Path.Combine(splitDir, splitFileName);
                    (splitJw, splitSw, splitCs) = await CreateJsonWriterForPathAsync(splitFilePath);
                }

                // ── Observe statistics for each field on this row ─────────────
                for (int i = 0; i < fieldCount; i++)
                {
                    columnAnalyzers[i].Observe(rowCache[i]);
                }

                // ── Write current non-empty row ───────────────────────────────
                await WriteRowAsync(singleJw, headers, rowCache, fieldCount);
                await WriteRowAsync(splitJw, headers, rowCache, fieldCount);
                rowCount++;
            }

            // Close the active JSON file writers for this sheet
            await CloseJsonWriterAsync(singleJw, singleSw, singleCs);
            await CloseJsonWriterAsync(splitJw, splitSw, splitCs);

            sheetInfo.SingleFileSize = new FileInfo(singleFilePath).Length;
            
            splitFiles.Add(new SplitFileInfo 
            { 
                FileName = splitFileName, 
                FileSize = new FileInfo(splitFilePath).Length 
            });
            sheetInfo.SplitFiles = splitFiles;

            sheetInfo.RowCount    = rowCount;
            sheetInfo.ColumnCount = headers.Length;
            sheets.Add(sheetInfo);

            sheetAnalysis.TotalRows = rowCount;
            sheetAnalysis.Columns.AddRange(columnAnalyzers);
            sheetAnalyses.Add(sheetAnalysis);

        } while (reader.NextResult()); // advance to next sheet

        // ── Generate Metadata Report inside the session folder ───────────────────────────
        string metadataFileName = "excel_schema_metadata.txt";
        string metadataFilePath = Path.Combine(sessionFolder, metadataFileName);
        long metadataFileSize = 0;

        if (sheetAnalyses.Count > 0)
        {
            var report = GenerateSchemaReport(sheetAnalyses);
            await File.WriteAllTextAsync(metadataFilePath, report, Encoding.UTF8);
            metadataFileSize = new FileInfo(metadataFilePath).Length;
        }

        return new ConversionResult
        {
            SessionId        = sessionId,
            SessionFolder    = sessionFolder,
            Sheets           = sheets,
            MetadataFileName = metadataFileName,
            MetadataFileSize = metadataFileSize
        };
    }

    private static string GenerateSchemaReport(List<SheetAnalysis> analyses)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================================================");
        sb.AppendLine("                 CONVERTY EXCEL SCHEMA METADATA REPORT                  ");
        sb.AppendLine("========================================================================");
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Total Sheets: {analyses.Count}");
        sb.AppendLine();

        foreach (var sheet in analyses)
        {
            sb.AppendLine("========================================================================");
            sb.AppendLine($"SHEET: {sheet.SheetName}");
            sb.AppendLine("========================================================================");
            sb.AppendLine($"Total Rows:    {sheet.TotalRows:N0}");
            sb.AppendLine($"Total Columns: {sheet.Columns.Count}");
            sb.AppendLine();

            for (int i = 0; i < sheet.Columns.Count; i++)
            {
                var col = sheet.Columns[i];
                var inferredType = col.InferType(out string sqlServerType);
                
                double completenessPercent = sheet.TotalRows > 0 
                    ? (double)col.TotalValuesCount / sheet.TotalRows * 100 
                    : 0.0;

                sb.AppendLine("------------------------------------------------------------------------");
                sb.AppendLine($"COLUMN {i + 1}:");
                sb.AppendLine("------------------------------------------------------------------------");
                sb.AppendLine($"Header Name:   {col.Header}"); // Full, untruncated name
                sb.AppendLine($"Inferred Type: {inferredType} (Recommended SQL: {sqlServerType})");
                sb.AppendLine($"Max Length:    {col.MaxLength:N0} characters");
                sb.AppendLine($"Completeness:  {col.TotalValuesCount:N0} of {sheet.TotalRows:N0} rows ({completenessPercent:F2}%)");
                sb.AppendLine();
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static async Task<(JsonTextWriter Jw, StreamWriter Sw, CountingStream Cs)> CreateJsonWriterForPathAsync(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 131072); // 128KB buffer
        var cs = new CountingStream(fs);
        var sw = new StreamWriter(cs, new UTF8Encoding(false), 131072); // 128KB buffer
        var jw = new JsonTextWriter(sw)
        {
            Formatting  = Formatting.Indented,
            CloseOutput = false
        };

        await jw.WriteStartArrayAsync();
        return (jw, sw, cs);
    }

    private static async Task CloseJsonWriterAsync(JsonTextWriter jw, StreamWriter sw, CountingStream cs)
    {
        await jw.WriteEndArrayAsync();
        await jw.FlushAsync();
        await sw.FlushAsync();
        await cs.FlushAsync();
        await sw.DisposeAsync(); // Disposes StreamWriter -> CountingStream -> FileStream
    }

    private static async Task WriteRowAsync(JsonTextWriter jw, string[] headers, object?[] rowCache, int fieldCount)
    {
        await jw.WriteStartObjectAsync();
        for (int i = 0; i < fieldCount; i++)
        {
            var key = i < headers.Length ? headers[i] : $"Column_{i + 1}";
            var value = rowCache[i];

            await jw.WritePropertyNameAsync(key);

            if (value == null || value is DBNull)
            {
                await jw.WriteNullAsync();
            }
            else
            {
                await jw.WriteValueAsync(value);
            }
        }
        await jw.WriteEndObjectAsync();
    }

    private static async Task WriteEmptyRowAsync(JsonTextWriter jw, string[] headers)
    {
        await jw.WriteStartObjectAsync();
        for (int i = 0; i < headers.Length; i++)
        {
            await jw.WritePropertyNameAsync(headers[i]);
            await jw.WriteNullAsync();
        }
        await jw.WriteEndObjectAsync();
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid)
            name = name.Replace(c, '_');
        return name.Length > 100 ? name[..100] : name;
    }

    /// <summary>
    /// Custom wrapper stream that tracks the exact number of uncompressed bytes written
    /// </summary>
    private class CountingStream : Stream
    {
        private readonly Stream _inner;
        public long BytesWritten { get; private set; }

        public CountingStream(Stream inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;
        public override long Position 
        { 
            get => _inner.Position; 
            set => _inner.Position = value; 
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            BytesWritten += count;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            BytesWritten += count;
            return _inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            BytesWritten += buffer.Length;
            return _inner.WriteAsync(buffer, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _inner.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}

public class ConversionResult
{
    public string SessionId { get; set; } = string.Empty;
    public string SessionFolder { get; set; } = string.Empty;
    public List<SheetInfo> Sheets { get; set; } = new();
    public string MetadataFileName { get; set; } = string.Empty;
    public long MetadataFileSize { get; set; }
}

public class SplitFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class SheetInfo
{
    public int          Index          { get; set; }
    public string       Name           { get; set; } = string.Empty;
    public string       SafeName       { get; set; } = string.Empty;
    public long         RowCount       { get; set; }
    public int          ColumnCount    { get; set; }
    public string       SingleFileName { get; set; } = string.Empty;
    public long         SingleFileSize { get; set; }
    public List<SplitFileInfo> SplitFiles { get; set; } = new();
}

public class SheetAnalysis
{
    public string SheetName { get; set; } = string.Empty;
    public List<ColumnAnalyzer> Columns { get; set; } = new();
    public long TotalRows { get; set; }
}

public class ColumnAnalyzer
{
    public string Header { get; }
    public long TotalValuesCount { get; private set; }
    public long IntCount { get; private set; }
    public long DecimalCount { get; private set; }
    public long DateCount { get; private set; }
    public long BoolCount { get; private set; }
    public long StringCount { get; private set; }
    public int MaxLength { get; private set; }
    public long PhonePatternCount { get; private set; }
    public long EmailPatternCount { get; private set; }

    public ColumnAnalyzer(string header)
    {
        Header = header;
    }

    public void Observe(object? val)
    {
        if (val == null || val is DBNull) return;
        TotalValuesCount++;

        if (val is int || val is long || val is short)
        {
            IntCount++;
            var strLen = val.ToString()?.Length ?? 0;
            if (strLen > MaxLength) MaxLength = strLen;
        }
        else if (val is double || val is float || val is decimal)
        {
            DecimalCount++;
            var strLen = val.ToString()?.Length ?? 0;
            if (strLen > MaxLength) MaxLength = strLen;
        }
        else if (val is DateTime)
        {
            DateCount++;
        }
        else if (val is bool)
        {
            BoolCount++;
        }
        else
        {
            var str = val.ToString()?.Trim();
            if (string.IsNullOrEmpty(str)) return;

            StringCount++;
            if (str.Length > MaxLength) MaxLength = str.Length;

            if (long.TryParse(str, out _))
            {
                IntCount++;
            }
            else if (double.TryParse(str, out _))
            {
                DecimalCount++;
            }
            else if (DateTime.TryParse(str, out _))
            {
                DateCount++;
            }
            else if (bool.TryParse(str, out _))
            {
                BoolCount++;
            }
            else
            {
                if (IsLikelyPhone(str))
                {
                    PhonePatternCount++;
                }
                if (str.Contains('@') && str.IndexOf('.') > str.IndexOf('@'))
                {
                    EmailPatternCount++;
                }
            }
        }
    }

    private static bool IsLikelyPhone(string s)
    {
        if (s.Length < 7 || s.Length > 20) return false;
        int digitCount = 0;
        foreach (char c in s)
        {
            if (char.IsDigit(c)) digitCount++;
            else if (c != '+' && c != '-' && c != ' ' && c != '(' && c != ')') return false;
        }
        return digitCount >= 7;
    }

    public string InferType(out string sqlServerType)
    {
        if (TotalValuesCount == 0)
        {
            sqlServerType = "NVARCHAR(100)";
            return "Empty / Unknown";
        }

        if (StringCount > 0)
        {
            if (PhonePatternCount > 0 && (PhonePatternCount + IntCount) == TotalValuesCount)
            {
                sqlServerType = $"VARCHAR({Math.Max(MaxLength, 20)})";
                return "Phone Number";
            }
            if (EmailPatternCount > 0 && EmailPatternCount == StringCount)
            {
                sqlServerType = $"VARCHAR({Math.Max(MaxLength, 255)})";
                return "Email Address";
            }

            int targetLen = MaxLength <= 0 ? 100 : MaxLength;
            if (targetLen <= 50) targetLen = 50;
            else if (targetLen <= 100) targetLen = 100;
            else if (targetLen <= 255) targetLen = 255;
            else if (targetLen <= 1000) targetLen = 1000;
            else targetLen = 4000;

            sqlServerType = $"NVARCHAR({(targetLen >= 4000 ? "MAX" : targetLen.ToString())})";
            return "Text / Mixed";
        }

        if (DateCount > 0 && DateCount == TotalValuesCount)
        {
            sqlServerType = "DATETIME2";
            return "DateTime";
        }

        if (BoolCount > 0 && BoolCount == TotalValuesCount)
        {
            sqlServerType = "BIT";
            return "Boolean";
        }

        if (DecimalCount > 0)
        {
            sqlServerType = "DECIMAL(18,6)";
            return "Decimal / Double";
        }

        if (IntCount > 0)
        {
            sqlServerType = "BIGINT";
            return "Integer";
        }

        sqlServerType = "NVARCHAR(255)";
        return "Text";
    }
}
