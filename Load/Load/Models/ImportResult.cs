namespace Load.Models;

public class ImportResult
{
    public string ImportType { get; set; } = "";
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public int SkippedCount { get; set; }
    public int FilesProcessed { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> UnmappedKeys { get; set; } = new();
    public Dictionary<string, int> SubjectAreaCounts { get; set; } = new();

    public Dictionary<string, long> TableRowCounts { get; set; } = new();
}