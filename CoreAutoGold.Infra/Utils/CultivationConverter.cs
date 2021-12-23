namespace CoreAutoGold.Infra.Utils;

public static class CultivationConverter
{
    public static int ToInt(this Cultivation cultivation)
    {
        return cultivation switch
        {
            Cultivation.Of09 => 9,
            Cultivation.Of19 => 19,
            Cultivation.Of29 => 29,
            Cultivation.Of39 => 39,
            Cultivation.Of49 => 49,
            Cultivation.Of59 => 59,
            Cultivation.Of69 => 69,
            Cultivation.Of79 => 79,
            var localCultivation when localCultivation.HasFlag(Cultivation.OfEvil1) | localCultivation.HasFlag(Cultivation.OfGod1) => 89,
            var localCultivation when localCultivation.HasFlag(Cultivation.OfEvil2) | localCultivation.HasFlag(Cultivation.OfGod2) => 99,
            var localCultivation when localCultivation.HasFlag(Cultivation.OfEvil3) | localCultivation.HasFlag(Cultivation.OfGod3) => 101,
            Cultivation.None => 0,
            _ => 0
        };
    }

    public static Cultivation ToCultivation(this int cultivation)
    {
        return cultivation switch
        {
            9 => Cultivation.Of09,
            19 => Cultivation.Of19,
            29 => Cultivation.Of29,
            39 => Cultivation.Of39,
            49 => Cultivation.Of49,
            59 => Cultivation.Of59,
            69 => Cultivation.Of69,
            79 => Cultivation.Of79,
            89 => Cultivation.OfGod1 | Cultivation.OfEvil1,
            99 => Cultivation.OfGod2 | Cultivation.OfEvil2,
            101 => Cultivation.OfGod3 | Cultivation.OfEvil3,
            _ => Cultivation.None
        };
    }
}