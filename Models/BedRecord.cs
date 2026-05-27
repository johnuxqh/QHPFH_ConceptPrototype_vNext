namespace QHPFH_ConceptPrototype.Models;

public sealed record BedRecord(
    string Id,
    string WardCode,
    string BedLabel,
    string Status,
    bool IsIsolation,
    bool IsBlocked,
    string? AssignedPatientId);
