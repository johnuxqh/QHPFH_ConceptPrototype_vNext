namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientMedicationRecord(
    string Id,
    string PatientId,
    string MedicationName,
    string Route,
    string Frequency,
    string Status,
    DateTime? DueAt,
    DateTime? LastGivenAt,
    string? Notes);
