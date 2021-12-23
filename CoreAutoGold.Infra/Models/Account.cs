namespace CoreAutoGold.Infra.Models;

public record Account
{
    public int Id { get; private init; }
    public List<GRoleData> Roles { get; private init; }

    public Account(int Id, List<GRoleData> Roles)
    {
        this.Id = Id;
        this.Roles = Roles;
    }
}
