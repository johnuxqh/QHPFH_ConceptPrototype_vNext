namespace QHPFH_ConceptPrototype.Models;

public enum ExperienceDensityMode
{
    Light,
    Balanced,
    Dense
}

public static class ExperienceDensityModeExtensions
{
    public static string ToDisplayName(this ExperienceDensityMode densityMode) => densityMode switch
    {
        ExperienceDensityMode.Light => "Light density",
        ExperienceDensityMode.Balanced => "Balanced density",
        ExperienceDensityMode.Dense => "Dense density",
        _ => densityMode.ToString()
    };
}
