namespace CoreAutoGold.Domain.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly LogWriter _logger;
    private readonly MySqlConnection context;

    public AccountRepository(LogWriter logger, ContextBuilder context)
    {
        _logger = logger;
        this.context = context.Get();
    }

    public async Task<List<int>> GetAllAccountsID()
    {
        try
        {
            return await context.QueryAsync<int>("SELECT ID FROM users") as List<int>;
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return default;
    }
}