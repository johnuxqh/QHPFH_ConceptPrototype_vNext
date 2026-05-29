namespace QHPFH_ConceptPrototype.Models.Insights;

public sealed record InsightRecord(
    string Id,
    string Title,
    string Summary,
    string DetailedDescription,
    InsightCategory Category,
    InsightSeverity Severity,
    InsightPriority Priority,
    decimal Confidence,
    string RecommendedAction,
    string ActionLabel,
    InsightActionType ActionType,
    string Context,
    string RelatedKpi,
    string TrendSignal,
    DateTime Timestamp,
    string StatusBadge,
    string IconName,
    int AffectedCount,
    IReadOnlySet<string>? RelatedEntityIds = null)
{
    public IReadOnlySet<string> RelatedEntityIds { get; } = RelatedEntityIds ?? new HashSet<string>();
}
