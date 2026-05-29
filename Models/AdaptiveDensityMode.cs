namespace QHPFH_ConceptPrototype.Models;

public enum AdaptiveDensityMode
{
    Comfort,
    Balanced,
    Dense,
    Command
}

public static class AdaptiveDensityModeExtensions
{
    public static string ToDisplayName(this AdaptiveDensityMode densityMode) => densityMode switch
    {
        AdaptiveDensityMode.Comfort => "Comfort density",
        AdaptiveDensityMode.Balanced => "Balanced density",
        AdaptiveDensityMode.Dense => "Dense density",
        AdaptiveDensityMode.Command => "Command density",
        _ => densityMode.ToString()
    };
}
