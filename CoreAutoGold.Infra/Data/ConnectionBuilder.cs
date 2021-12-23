namespace CoreAutoGold.Infra.Data;

public class ConnectionBuilder
{
    public static string GetConnectionString()
    {
        DatabaseConnection data = JsonConvert.DeserializeObject<DatabaseConnection>(File.ReadAllText("./Configurations/Database.json"));

        return data.ToString();
    }
}