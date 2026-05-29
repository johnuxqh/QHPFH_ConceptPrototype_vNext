namespace QHPFH_ConceptPrototype.Models;

public enum ExperienceInteractionMode
{
    Simplified,
    Coordinated,
    Operational
}

public static class ExperienceInteractionModeExtensions
{
    public static string ToDisplayName(this ExperienceInteractionMode interactionMode) => interactionMode switch
    {
        ExperienceInteractionMode.Simplified => "Simplified interactions",
        ExperienceInteractionMode.Coordinated => "Coordinated interactions",
        ExperienceInteractionMode.Operational => "Operational interactions",
        _ => interactionMode.ToString()
    };
}
