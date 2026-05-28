namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientAlertRecord(
    string Id,
    string PatientId,
    PatientAlertType AlertType,
    string Severity,
    string Label,
    string Description,
    bool IsActive);
