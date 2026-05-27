namespace QHPFH_ConceptPrototype.Models;

public sealed record AllocationRecord(
    string Id,
    string PatientId,
    string Facility,
    string WardCode,
    string Priority,
    string Status,
    DateTime UpdatedAtUtc);
