namespace Load.Models;

public class ColumnMap
{
    public string Table { get; set; } = "";
    public string Column { get; set; } = "";
    public ColType Type { get; set; } = ColType.Text;
}