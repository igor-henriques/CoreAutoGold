namespace CoreAutoGold.Infra.Models;

[Serializable]
public record Settings
{
    [JsonProperty("DIA PARA CASH SEMANAL")]
    public DayOfWeek PrizeDay { get; private init; }

    [JsonProperty("HORA PARA CASH DIARIO")]
    public int PrizeHour { get; private init; }

    [JsonProperty("QUANTIA DE CASH SEMANAL")]
    public int WeekCashAmount { get; private init; }

    [JsonProperty("QUANTIA DE CASH DIARIO")]
    public int DayCashAmount { get; private init; }

    [JsonProperty("MULTIPLICADOR DE REQUISITO")]
    public int RequisiteMultiplier { get; private init; }

    [JsonProperty("CULTIVO MINIMO")]
    public Cultivation? CultivationRequired { get; private init; }

    [JsonProperty("NIVEL MINIMO")]
    public int? LevelRequired { get; private init; }

    [JsonProperty("ITEM NECESSARIO")]
    public int? ItemRequired { get; private init; }

    [JsonProperty("REQUISITOS")]
    public PrizeRequisite Requisites { get; private init; }

    [JsonProperty("FREQUENCIA DE RECOMPENSA")]
    public PrizeOption PrizeOption { get; private init; }

    private readonly LogWriter _logger;

    public Settings(LogWriter logger)
    {
        _logger = logger;

        JObject jsonNodes = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("./Configurations/Settings.json"));

        this.PrizeDay = GetPrizeDay(jsonNodes);
        this.PrizeHour = jsonNodes["HORA PARA CASH DIARIO"].ToObject<int>();
        this.WeekCashAmount = jsonNodes["QUANTIA DE CASH SEMANAL"].ToObject<int>();
        this.DayCashAmount = jsonNodes["QUANTIA DE CASH DIARIO"].ToObject<int>();
        this.RequisiteMultiplier = int.Parse(jsonNodes["MULTIPLICADOR DE REQUISITO"].ToObject<string>().Replace("%", string.Empty));
        this.CultivationRequired = GetCultivation(jsonNodes);
        this.LevelRequired = jsonNodes["NIVEL MINIMO"].ToObject<int>();
        this.ItemRequired = jsonNodes["ITEM NECESSARIO"].ToObject<int>();
        this.Requisites = GetRequisites(jsonNodes);
        this.PrizeOption = GetPrizeOptions(jsonNodes);
    }
    private PrizeRequisite GetRequisites(JObject nodes)
    {
        PrizeRequisite prizeRequisite = 0;

        foreach (var package in nodes["REQUISITOS"].Children())
        {
            if (package.First.ToObject<int>() is 1)
                prizeRequisite |= Enum.Parse<PrizeRequisite>(package.First.Path.Replace("REQUISITOS.", string.Empty));
        }

        if (prizeRequisite.Equals(PrizeRequisite.None))
        {
            _logger.Write("Não foi encontrado nenhum requisito. Verifique o arquivo Settings.json.");
            Process.GetCurrentProcess().Kill();
        }

        return prizeRequisite;
    }
    private PrizeOption GetPrizeOptions(JObject nodes)
    {
        PrizeOption prizeOptions = 0;

        foreach (var package in nodes["FREQUENCIA DE RECOMPENSA"].Children())
        {
            if (package.First.ToObject<int>() is 1)
                prizeOptions |= Enum.Parse<PrizeOption>(package.First.Path.Replace("['FREQUENCIA DE RECOMPENSA'].", string.Empty));
        }

        if (prizeOptions.Equals(PrizeOption.None))
        {
            _logger.Write("Não foi encontrado nenhuma frequência de recompensa. Verifique o arquivo Settings.json.");
            Process.GetCurrentProcess().Kill();
        }

        return prizeOptions;
    }
    private Cultivation GetCultivation(JObject nodes)
    {
        var cultivation = nodes["CULTIVO MINIMO"].ToObject<int>();

        return cultivation.ToCultivation();
    }
    private DayOfWeek GetPrizeDay(JObject nodes)
    {
        string dayOfWeek = nodes["DIA PARA CASH SEMANAL"].ToObject<string>();

        return dayOfWeek
            .ToLower()
            .Trim()
            .Replace("ç", "c")
            .Replace("á", "a")
            .Replace(" ", string.Empty)
            .Replace("-feira", string.Empty) switch
        {
            "segunda" => DayOfWeek.Monday,
            "terca" => DayOfWeek.Tuesday,
            "quarta" => DayOfWeek.Wednesday,
            "quinta" => DayOfWeek.Thursday,
            "sexta" => DayOfWeek.Friday,
            "sabado" => DayOfWeek.Saturday,
            "domingo" => DayOfWeek.Sunday,
            _ => DayOfWeek.Saturday
        };
    }
}