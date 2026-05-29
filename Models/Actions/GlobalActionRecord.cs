namespace QHPFH_ConceptPrototype.Models.Actions;

public sealed record GlobalActionRecord(
    string Id,
    string Label,
    GlobalActionType Type,
    GlobalActionScope Scope,
    GlobalActionStatus Status,
    string Description,
    string? WorkspaceId = null,
    string? Icon = null);
