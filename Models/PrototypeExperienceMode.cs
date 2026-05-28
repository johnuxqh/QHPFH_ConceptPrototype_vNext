namespace QHPFH_ConceptPrototype.Models;

public enum PrototypeExperienceMode
{
    V1AwarenessInsights,
    V2CoordinatedOperations,
    V3OperationalWorkflow
}

public static class PrototypeExperienceModeExtensions
{
    public static string ToDisplayName(this PrototypeExperienceMode mode) => mode switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "V1 — Awareness & Insights",
        PrototypeExperienceMode.V2CoordinatedOperations => "V2 — Coordinated Operations",
        PrototypeExperienceMode.V3OperationalWorkflow => "V3 — Operational Workflow",
        _ => mode.ToString()
    };

    public static string ToSummaryText(this PrototypeExperienceMode mode) => mode switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "V1 Awareness & Insights",
        PrototypeExperienceMode.V2CoordinatedOperations => "V2 Coordinated Operations",
        PrototypeExperienceMode.V3OperationalWorkflow => "V3 Operational Workflow",
        _ => mode.ToString()
    };
}
