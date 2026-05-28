namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalEscalationRecord(
    string Id,
    string EventId,
    string EscalationLevel,
    DateTime EscalatedAtUtc,
    string EscalatedBy,
    string Reason,
    bool IsResolved,
    DateTime? ResolvedAtUtc);
