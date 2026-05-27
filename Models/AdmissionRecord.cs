namespace QHPFH_ConceptPrototype.Models;

public sealed record AdmissionRecord(
    string Id,
    string PatientId,
    string Source,
    string Status,
    DateTime RequestedAtUtc,
    DateTime? EstimatedArrivalAtUtc);
