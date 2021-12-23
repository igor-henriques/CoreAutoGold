namespace CoreAutoGold.Infra.Models.Enums;

[Serializable]
[Flags]
public enum PrizeRequisite : byte
{
    None = 0,

    [JsonProperty("CULTIVO")]
    Cultivo = 1,

    [JsonProperty("NIVEL")]
    Nivel = 2,

    [JsonProperty("ITEM")]
    Item = 4
}