namespace QHPFH_ConceptPrototype.Models;

public sealed record OperationalEventRecord
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string Summary { get; init; }
    public OperationalEventCategory Category { get; init; }
    public OperationalEventSeverity Severity { get; init; }
    public OperationalEventScope Scope { get; init; }
    public CapacityStatus? CapacityStatus { get; init; }
    public string? HhsId { get; init; }
    public string? FacilityId { get; init; }
    public string? WardId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? StartsAtUtc { get; init; }
    public DateTime? EndsAtUtc { get; init; }
    public bool IsActive { get; init; }
    public bool RequiresAcknowledgement { get; init; }
    public string? SourceSystem { get; init; }
    public string? CreatedBy { get; init; }
    public string? Notes { get; init; }

    // backward compatibility
    public string ScopeId => WardId ?? FacilityId ?? HhsId ?? "Statewide";
    public string SeverityLabel => Severity.ToString();
    public string Message => Summary;
    public DateTime OccurredAtUtc => CreatedAtUtc;

    public OperationalEventRecord(string id, string scope, string scopeId, string category, string severity, string message, DateTime occurredAtUtc)
    {
        Id = id;
        Title = category;
        Summary = message;
        Category = Enum.TryParse<OperationalEventCategory>(category.Replace(" ", string.Empty), true, out var c) ? c : OperationalEventCategory.Communication;
        Severity = Enum.TryParse<OperationalEventSeverity>(severity, true, out var s) ? s : OperationalEventSeverity.Info;
        Scope = Enum.TryParse<OperationalEventScope>(scope, true, out var sc) ? sc : OperationalEventScope.Facility;
        CreatedAtUtc = occurredAtUtc;
        StartsAtUtc = occurredAtUtc;
        IsActive = true;
        RequiresAcknowledgement = false;
        if (Scope == OperationalEventScope.Ward) WardId = scopeId;
        else if (Scope == OperationalEventScope.Facility) FacilityId = scopeId;
        else if (Scope == OperationalEventScope.HHS) HhsId = scopeId;
    }
}
