namespace QHPFH_ConceptPrototype.Models;

public sealed record AllocationRequestRecord(
    string Id,
    string? IncomingPatientId,
    string? PatientId,
    string RequestedBy,
    DateTime RequestedAtUtc,
    string? PreferredWardId,
    string? PreferredBedId,
    BedType? RequiredBedType,
    string? RequiredSpecialty,
    bool RequiresIsolation,
    AllocationPriority Priority,
    AllocationStatus Status,
    string? Notes);
