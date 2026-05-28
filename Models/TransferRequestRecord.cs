namespace QHPFH_ConceptPrototype.Models;

public sealed record TransferRequestRecord(
    string Id,
    string PatientId,
    string? FromFacilityId,
    string? FromWardId,
    string? ToFacilityId,
    string? ToWardId,
    string Reason,
    AllocationPriority Priority,
    TransferReadinessStatus ReadinessStatus,
    DateTime RequestedAtUtc,
    string? Notes);
