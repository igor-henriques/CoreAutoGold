namespace CoreAutoGold.Domain.Interfaces;

public interface IAccountRepository
{
    Task<List<int>> GetAllAccountsID();
}