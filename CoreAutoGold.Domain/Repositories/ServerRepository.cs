namespace CoreAutoGold.Domain.Repositories;

public class ServerRepository : IServerRepository
{
    private readonly ServerConnection _server;
    private readonly LogWriter _logger;

    public ServerRepository(ServerConnection server, LogWriter logger)
    {
        _server = server;
        _logger = logger;

        PWGlobal.UsedPwVersion = server.PwVersion;
    }

    public Task<List<GRoleData>> GetRolesOnAccount(int accountId)
    {
        try
        {
            var response = new List<GRoleData>();

            var roles = GetUserRoles.Get(_server.gamedbd, accountId);

            foreach (var role in roles)
            {
                response.Add(GetRoleData.Get(_server.gamedbd, role.Item1));
            }

            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return default;
    }

    public async Task<List<Account>> GetAccounts(List<int> accountIds)
    {
        try
        {
            var response = new List<Account>();
            await Task.Run(async () =>
            {
                foreach (var accountId in accountIds)
                {
                    var accountRoles = await GetRolesOnAccount(accountId);

                    response.Add(new(accountId, accountRoles));
                }
            });

            return response;
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return default;
    }
    public async Task<bool> GiveCash(int accountId, int cashAmount)
    {
        try
        {
            return await Task.Run(() => DebugAddCash.Add(_server.gamedbd, accountId, cashAmount * 100));
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }

    public async Task SendMessage(string message)
    {
        try
        {
            await Task.Run(() =>
            {
                ChatBroadcast.Send(_server.gprovider, BroadcastChannel.System, message);
            });
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }
    }
}