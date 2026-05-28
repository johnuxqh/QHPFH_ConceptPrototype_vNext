namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioActionRecord(
    string Id,
    string ScenarioId,
    string Title,
    string Description,
    string ActionType,
    AllocationPriority Priority,
    string? TargetFacilityId,
    string? TargetWardId,
    bool IsRecommended,
    bool IsSelected,
    string? Notes);
