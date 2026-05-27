namespace QHPFH_ConceptPrototype.Models;

public sealed record IncomingPatientRecord(
    string Id,
    string PatientId,
    AllocationSourceType SourceType,
    string SourceLocation,
    string? TargetFacilityId,
    string? TargetWardId,
    BedType? RequestedBedType,
    string? RequiredSpecialty,
    bool RequiresIsolation,
    string? InfectionControlStatus,
    AllocationPriority Priority,
    AllocationStatus Status,
    DateTime? ExpectedArrivalAtUtc,
    string? Notes);
