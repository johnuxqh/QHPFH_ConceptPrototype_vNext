namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioRecord(
    string Id,
    string Name,
    string Description,
    ScenarioType ScenarioType,
    ScenarioStatus Status,
    DateTime CreatedAtUtc,
    string CreatedBy,
    string? HhsId,
    string? FacilityId,
    string? WardId,
    string TimeHorizonLabel,
    bool IsActive,
    bool IsPinned,
    string? Notes);
