namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioImpactRecord(
    string Id,
    string ScenarioId,
    ScenarioImpactType ImpactType,
    OperationalEventScope Scope,
    string? HhsId,
    string? FacilityId,
    string? WardId,
    NotificationSeverity Severity,
    string Title,
    string Description,
    DateTime? EstimatedStartUtc,
    DateTime? EstimatedEndUtc);
