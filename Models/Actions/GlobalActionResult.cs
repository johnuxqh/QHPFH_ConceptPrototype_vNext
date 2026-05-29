namespace QHPFH_ConceptPrototype.Models.Actions;

public sealed record GlobalActionResult(
    string ActionId,
    string Label,
    bool Succeeded,
    string Message,
    string WorkspaceLabel,
    string ContextSummary,
    DateTimeOffset TriggeredAt);
