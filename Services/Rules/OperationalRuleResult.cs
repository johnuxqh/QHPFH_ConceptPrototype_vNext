namespace QHPFH_ConceptPrototype.Services.Rules;

public sealed record OperationalRuleResult(
    string Id,
    string Title,
    string Summary,
    OperationalRuleCategory Category,
    OperationalRuleSeverity Severity,
    string Scope,
    string? HhsId,
    string? FacilityId,
    string? WardId,
    string? BedId,
    string? PatientId,
    string? AllocationId,
    bool IsBlocking,
    bool IsActionable,
    string? RecommendedAction,
    string? RelatedEntityType,
    string? RelatedEntityId);
