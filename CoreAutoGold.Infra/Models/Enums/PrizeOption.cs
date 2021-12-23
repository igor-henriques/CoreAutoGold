namespace CoreAutoGold.Infra.Models.Enums;

[Serializable]
[Flags]
public enum PrizeOption : byte
{
    None = 0,
    Semanal = 1,
    Diario = 2
}