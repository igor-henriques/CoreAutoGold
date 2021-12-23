namespace CoreAutoGold.Domain.Interfaces;

public interface IServerRepository
{
    Task<List<GRoleData>> GetRolesOnAccount(int accountId);
    Task<List<Account>> GetAccounts(List<int> accountIds);
    Task<bool> GiveCash(int accountId, int cashAmount);
    Task SendMessage(string message);
}