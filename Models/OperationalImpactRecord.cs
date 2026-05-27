namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalImpactRecord(
    string Id,
    string EventId,
    string ImpactType,
    string Description,
    string AffectedArea,
    string EstimatedDuration,
    bool IsResolved);
