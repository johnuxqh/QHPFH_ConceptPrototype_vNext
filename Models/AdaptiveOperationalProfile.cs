namespace QHPFH_ConceptPrototype.Models;

public sealed record AdaptiveOperationalProfile(
    AdaptiveDensityMode DensityMode,
    AdaptiveWorkflowEmphasis WorkflowEmphasis,
    string RecommendedPanelMode,
    string RecommendedDefaultTab,
    string RecommendedOperationalFocus);
