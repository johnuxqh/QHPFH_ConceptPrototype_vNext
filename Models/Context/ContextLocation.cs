namespace QHPFH_ConceptPrototype.Models.Context;

public sealed record ContextLocation(
    string? CurrentHhsId,
    string? CurrentHhsName,
    string? CurrentFacilityId,
    string? CurrentFacilityName,
    string? CurrentWardId,
    string? CurrentWardName,
    ContextScope Scope,
    string? SummaryOverride = null)
{
    public string SummaryText
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(SummaryOverride))
            {
                return SummaryOverride;
            }

            var parts = new[] { CurrentHhsName, CurrentFacilityName, CurrentWardName }
                .Where(x => !string.IsNullOrWhiteSpace(x));
            var summary = string.Join(" > ", parts);
            return string.IsNullOrWhiteSpace(summary) ? Scope.ToString() : summary;
        }
    }
}
