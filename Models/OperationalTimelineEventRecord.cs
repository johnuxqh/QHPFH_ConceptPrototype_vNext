namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalTimelineEventRecord(
    string Id,
    string EventId,
    DateTime TimestampUtc,
    string Title,
    string Description,
    string PerformedBy,
    string ActionType);
