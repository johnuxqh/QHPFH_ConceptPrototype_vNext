namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalBannerRecord(
    string Id,
    string Title,
    string Message,
    OperationalEventSeverity Severity,
    OperationalEventScope Scope,
    CapacityStatus? CapacityStatus,
    string? HhsId,
    string? FacilityId,
    string? WardId,
    bool IsDismissible,
    bool IsPinned,
    bool IsActive,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc);
