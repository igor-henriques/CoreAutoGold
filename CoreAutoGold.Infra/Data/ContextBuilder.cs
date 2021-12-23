namespace CoreAutoGold.Infra.Data;

public class ContextBuilder
{
    private readonly MySqlConnection context;

    public ContextBuilder()
    {
        context = new MySqlConnection(ConnectionBuilder.GetConnectionString());
    }

    public MySqlConnection Get() => context!;
}