namespace CureAutoGold.Infra.Data;

public record DatabaseConnection
{
    public string HOST { get; set; }
    public string DB { get; set; }
    public string USER { get; set; }
    public int PORT { get; set; }
    public string PASSWORD { get; set; }

    public override string ToString()
    {
        return $"Server={HOST};Port={PORT};Database={DB};Uid={USER};Pwd={PASSWORD};ConvertZeroDateTime=True;SslMode=none";
    }
}