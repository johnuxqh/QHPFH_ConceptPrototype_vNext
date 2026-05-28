namespace QHPFH_ConceptPrototype.Models;

public sealed record BedRecord
{
    public string Id { get; init; }
    public string WardId { get; init; }
    public string BedLabel { get; init; }
    public string BedNumber => BedLabel;
    public BedType BedType { get; init; } = BedType.Standard;
    public BedStatus BedStatus { get; init; } = BedStatus.Open;
    public bool IsPhysicalBed { get; init; } = true;
    public bool IsOpenOperationally { get; init; } = true;
    public bool IsSpecialistBed { get; init; }
    public bool IsIsolationCapable { get; init; }
    public bool IsNegativePressure { get; init; }
    public bool IsPositivePressure { get; init; }
    public string? CurrentPatientId { get; init; }
    public string? FutureAllocatedPatientId { get; init; }
    public string? MaintenanceReason { get; init; }
    public string? ClosureReason { get; init; }
    public int SortOrder { get; init; }

    // Backward compatibility for current store logic.
    public string WardCode => WardId;
    public string Status => BedStatus.ToString();
    public bool IsIsolation => IsIsolationCapable;
    public bool IsBlocked => BedStatus == BedStatus.Blocked;
    public string? AssignedPatientId => CurrentPatientId;

    public BedRecord(string id, string wardCode, string bedLabel, string status, bool isIsolation, bool isBlocked, string? assignedPatientId)
    {
        Id = id;
        WardId = wardCode;
        BedLabel = bedLabel;
        IsIsolationCapable = isIsolation;
        BedStatus = isBlocked ? BedStatus.Blocked : Enum.TryParse<BedStatus>(status, true, out var parsed) ? parsed : BedStatus.Open;
        IsOpenOperationally = BedStatus is BedStatus.Open or BedStatus.Occupied or BedStatus.FutureAllocated;
        IsSpecialistBed = IsIsolationCapable;
        CurrentPatientId = assignedPatientId;
    }
}
