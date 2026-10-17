namespace Load.Models;

public class JsonValidationResult
{
    public string FileName { get; set; } = "";
    public long FileSizeBytes { get; set; }
    public bool IsValid { get; set; }
    public int TotalRows { get; set; }
    public int KeysPerRow { get; set; }
    public bool IsPlainArray { get; set; }
    public bool HasConsistentKeys { get; set; }
    public int NullCountFirstRow { get; set; }
    public int NonNullCountFirstRow { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<JsonKeyInfo> KeySummary { get; set; } = new();
    public List<string> RequiredKeysFound { get; set; } = new();
    public List<string> RequiredKeysMissing { get; set; } = new();
    public List<string> SampleSubjectAreas { get; set; } = new();
}

public class JsonKeyInfo
{
    public string Key { get; set; } = "";
    public string DataType { get; set; } = "";
    public string SampleValue { get; set; } = "";
    public bool IsNull { get; set; }
}