namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterEmptyStateRecord(
    FilterEmptyStateType Type,
    string Title,
    string Message,
    string ScopeSummary,
    IReadOnlyList<FilterEmptyStateActionRecord> RecoveryActions);
