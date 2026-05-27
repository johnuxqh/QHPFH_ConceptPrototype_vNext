namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientResultRecord(
    string Id,
    string PatientId,
    string ResultType,
    string Title,
    PatientResultStatus Status,
    DateTime? RequestedAt,
    DateTime? ExpectedAt,
    string Summary);
