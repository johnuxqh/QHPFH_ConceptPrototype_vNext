namespace QHPFH_ConceptPrototype.Models;

public sealed record AllocationRecord
{
    public string Id { get; init; }
    public string PatientId { get; init; }
    public string FacilityId { get; init; }
    public string WardId { get; init; }
    public string? TargetBedId { get; init; }
    public string? FutureBedId { get; init; }
    public AllocationSourceType SourceType { get; init; }
    public AllocationPriority Priority { get; init; }
    public AllocationStatus Status { get; init; }
    public DateTime RequestedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public string? Notes { get; init; }
    public bool IsFutureAllocation { get; init; }
    public bool IsPreAllocation { get; init; }
    public bool RequiresIsolation { get; init; }
    public BedType? RequiredBedType { get; init; }
    public string? RequiredSpecialty { get; init; }

    // Backward compatibility
    public string Facility => FacilityId;
    public string WardCode => WardId;

    public AllocationRecord(string id, string patientId, string facility, string wardCode, string priority, string status, DateTime updatedAtUtc)
    {
        Id = id;
        PatientId = patientId;
        FacilityId = facility;
        WardId = wardCode;
        Priority = Enum.TryParse<AllocationPriority>(priority, true, out var p) ? p : AllocationPriority.Routine;
        Status = Enum.TryParse<AllocationStatus>(status, true, out var s) ? s : AllocationStatus.Waiting;
        UpdatedAtUtc = updatedAtUtc;
        RequestedAtUtc = updatedAtUtc;
        SourceType = AllocationSourceType.ED;
    }
}
