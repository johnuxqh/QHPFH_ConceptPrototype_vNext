namespace QHPFH_ConceptPrototype.Models;

public enum AdaptiveWorkflowEmphasis
{
    Awareness,
    ExecutiveInsights,
    Coordination,
    BedOrchestration,
    WardWorkflow,
    AllocationFlow,
    DischargeBarriers,
    Reporting
}

public static class AdaptiveWorkflowEmphasisExtensions
{
    public static string ToDisplayName(this AdaptiveWorkflowEmphasis emphasis) => emphasis switch
    {
        AdaptiveWorkflowEmphasis.Awareness => "Awareness Focus",
        AdaptiveWorkflowEmphasis.ExecutiveInsights => "Awareness Focus",
        AdaptiveWorkflowEmphasis.Coordination => "Coordination Focus",
        AdaptiveWorkflowEmphasis.BedOrchestration => "Bed Orchestration Focus",
        AdaptiveWorkflowEmphasis.WardWorkflow => "Workflow Focus",
        AdaptiveWorkflowEmphasis.AllocationFlow => "Allocation Flow Focus",
        AdaptiveWorkflowEmphasis.DischargeBarriers => "Discharge Barriers Focus",
        AdaptiveWorkflowEmphasis.Reporting => "Reporting Focus",
        _ => emphasis.ToString()
    };
}
