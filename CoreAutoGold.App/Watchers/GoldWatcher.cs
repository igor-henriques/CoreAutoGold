namespace CoreAutoGold.App.Watchers;

internal class GoldWatcher : BackgroundService
{
    private readonly LogWriter _logger;
    private readonly Settings _settings;
    private readonly IAccountRepository _accountContext;
    private readonly IServerRepository _serverContext;

    public GoldWatcher(LogWriter logger, Settings settings, IAccountRepository accountContext, IServerRepository serverContext)
    {
        _logger = logger;
        _settings = settings;
        _accountContext = accountContext;
        _serverContext = serverContext;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.Write("MÓDULO DE EXECUÇÃO PRIMÁRIA INICIADO");
            _logger.Write("PRODUZIDO POR IRONSIDE - CONTATO PARA SUPORTE -> Ironside#3862");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (WeekTime() | DayTime())
                {
                    var accountIds = await _accountContext.GetAllAccountsID();

                    var accounts = await _serverContext.GetAccounts(accountIds);

                    foreach (var account in accounts)
                    {
                        var task = _settings.Requisites switch
                        {
                            PrizeRequisite.Item | PrizeRequisite.Nivel | PrizeRequisite.Cultivo => DeliveryRewardByAll(account),
                            PrizeRequisite.Item | PrizeRequisite.Cultivo => DeliveryRewardByCultivationAndItem(account),
                            PrizeRequisite.Nivel | PrizeRequisite.Cultivo => DeliveryRewardByCultivationAndLevel(account),
                            PrizeRequisite.Item | PrizeRequisite.Nivel => DeliveryRewardByItemAndLevel(account),
                            PrizeRequisite.Cultivo => DeliveryRewardByCultivation(account),
                            PrizeRequisite.Item => DeliveryRewardByItem(account),
                            PrizeRequisite.Nivel => DeliveryRewardByLevel(account),
                            _ => Task.FromResult(false),
                        };

                        await task;
                    }

                    await _serverContext.SendMessage("INFORMATIVO: O bônus periódico de cash foi entregue a todos que cumpriram o(s) requisito(s) necessário(s).");

                    _logger.Write("Recompensa entregue");
                }
                else
                {
                    _logger.Write($"{DateTime.Now.DayOfWeek} - Fora dos prazos. Entrando em descanso por 1 hora.");
                }

                await Task.Delay(TimeSpan.FromHours(1));
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }
    }
    private bool WeekTime()
        => DateTime.Now.DayOfWeek.Equals(_settings.PrizeDay) & DateTime.Now.Hour.Equals(_settings.PrizeHour) & _settings.PrizeOption.HasFlag(PrizeOption.Semanal);

    private bool DayTime()
        => DateTime.Now.Hour.Equals(_settings.PrizeHour) & _settings.PrizeOption.HasFlag(PrizeOption.Diario);

    private async Task<bool> DeliveryRewardByCultivation(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryCultivation(x, _settings.CultivationRequired.GetValueOrDefault())))
            {
                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                await _serverContext.GiveCash(currentAccount.Id, prize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByLevel(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryLevel(x, _settings.LevelRequired.GetValueOrDefault())))
            {
                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                await _serverContext.GiveCash(currentAccount.Id, prize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByItem(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryItem(x, _settings.ItemRequired.GetValueOrDefault())))
            {
                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                await _serverContext.GiveCash(currentAccount.Id, prize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByCultivationAndLevel(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryCultivation(x, _settings.CultivationRequired.GetValueOrDefault())))
            {
                int hasLevel = currentAccount.Roles.Any(x => HasNecessaryLevel(x, _settings.LevelRequired.GetValueOrDefault())) ? 1 : 0;

                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                int bonusPrize = (hasLevel * _settings.RequisiteMultiplier * prize) / 100;

                await _serverContext.GiveCash(currentAccount.Id, prize + bonusPrize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByCultivationAndItem(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryCultivation(x, _settings.CultivationRequired.GetValueOrDefault())))
            {
                int hasItem = currentAccount.Roles.Any(x => HasNecessaryItem(x, _settings.ItemRequired.GetValueOrDefault())) ? 1 : 0;

                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                int bonusPrize = (hasItem * _settings.RequisiteMultiplier * prize) / 100;

                await _serverContext.GiveCash(currentAccount.Id, prize + bonusPrize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByItemAndLevel(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryItem(x, _settings.ItemRequired.GetValueOrDefault())))
            {
                int hasLevel = currentAccount.Roles.Any(x => HasNecessaryLevel(x, _settings.LevelRequired.GetValueOrDefault())) ? 1 : 0;

                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                int bonusPrize = (hasLevel * _settings.RequisiteMultiplier * prize) / 100;

                await _serverContext.GiveCash(currentAccount.Id, prize + bonusPrize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private async Task<bool> DeliveryRewardByAll(Account currentAccount)
    {
        try
        {
            if (currentAccount.Roles.Any(x => HasNecessaryCultivation(x, _settings.CultivationRequired.GetValueOrDefault())))
            {
                int hasLevel = currentAccount.Roles.Any(x => HasNecessaryLevel(x, _settings.LevelRequired.GetValueOrDefault())) ? 1 : 0;

                int hasItem = currentAccount.Roles.Any(x => HasNecessaryItem(x, _settings.ItemRequired.GetValueOrDefault())) ? 1 : 0;

                int prize = GetPrizeOption().HasFlag(PrizeOption.Semanal) ? _settings.WeekCashAmount : _settings.DayCashAmount;

                int bonusPrize = ((hasLevel * _settings.RequisiteMultiplier * prize) + (hasItem * _settings.RequisiteMultiplier * prize)) / 100;

                await _serverContext.GiveCash(currentAccount.Id, prize + bonusPrize);

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString());
        }

        return false;
    }
    private bool HasNecessaryCultivation(GRoleData role, Cultivation cultivo)
        => Enum.Parse<Cultivation>(role.GRoleStatus.Level2.ToString()).ToInt() >= cultivo.ToInt();

    private bool HasNecessaryLevel(GRoleData role, int level)
        => role.GRoleStatus.Level >= level;

    private bool HasNecessaryItem(GRoleData role, int itemId)
        => role.GRolePocket.Items.Select(x => x.Id).Contains(itemId);

    private PrizeOption GetPrizeOption()
    {
        PrizeOption prizeOption = 0;

        if (DateTime.Now.DayOfWeek.Equals(_settings.PrizeDay) & _settings.PrizeOption.HasFlag(PrizeOption.Semanal))
            prizeOption |= PrizeOption.Semanal;

        if (DateTime.Now.Hour.Equals(_settings.PrizeHour) & _settings.PrizeOption.HasFlag(PrizeOption.Diario))
            prizeOption |= PrizeOption.Diario;

        return prizeOption;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}