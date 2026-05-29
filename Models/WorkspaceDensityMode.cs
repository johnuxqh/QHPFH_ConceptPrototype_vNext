namespace QHPFH_ConceptPrototype.Models;

public enum WorkspaceDensityMode
{
    Comfortable,
    Balanced,
    Compact
}

public static class WorkspaceDensityModeExtensions
{
    public static string ToDisplayName(this WorkspaceDensityMode densityMode) => densityMode switch
    {
        WorkspaceDensityMode.Comfortable => "Comfortable",
        WorkspaceDensityMode.Balanced => "Balanced",
        WorkspaceDensityMode.Compact => "Compact",
        _ => densityMode.ToString()
    };
}
