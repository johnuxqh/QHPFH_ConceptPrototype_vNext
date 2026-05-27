namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientRecord(
    string Id,
    string FullName,
    int Age,
    string Sex,
    string CurrentWardCode,
    string Status,
    bool RequiresIsolation,
    bool DelayedDischarge);
