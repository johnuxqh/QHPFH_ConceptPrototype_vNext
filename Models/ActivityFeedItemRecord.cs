namespace QHPFH_ConceptPrototype.Models;

public sealed record ActivityFeedItemRecord(
    string Id,
    string Title,
    string Summary,
    ActivityFeedCategory Category,
    ActivityFeedSeverity Severity,
    ActivityFeedScope Scope,
    DateTime CreatedAtUtc,
    string CreatedBy,
    string? HhsId,
    string? FacilityId,
    string? WardId,
    string? BedId,
    string? PatientId,
    string? AllocationId,
    string? OperationalEventId,
    string? RelatedEntityType,
    string? RelatedEntityId,
    bool IsSystemGenerated,
    bool IsImportant,
    string? Notes);
