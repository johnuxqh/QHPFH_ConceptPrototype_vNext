namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalEventRecord(
    string Id,
    string Scope,
    string ScopeId,
    string Category,
    string Severity,
    string Message,
    DateTime OccurredAtUtc);
