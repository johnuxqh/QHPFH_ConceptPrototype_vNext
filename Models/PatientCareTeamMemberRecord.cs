namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientCareTeamMemberRecord(
    string Id,
    string PatientId,
    string Name,
    string Role,
    string Team,
    string? ContactLabel,
    bool IsPrimary);
