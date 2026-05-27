namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientTaskRecord(
    string Id,
    string PatientId,
    string TaskType,
    string Title,
    PatientTaskStatus Status,
    PatientTaskPriority Priority,
    DateTime? DueAt,
    string? AssignedTeam);
