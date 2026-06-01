namespace QHPFH_ConceptPrototype.Models;

public sealed record PrototypeAccessViewOption(
    string Label,
    string UserPerspectiveId);

public sealed record PrototypeExperienceState(
    string AccessViewLabel,
    string UserPerspectiveId,
    PrototypeExperienceMode ExperienceMode,
    PrototypeLayoutVariant LayoutVariant)
{
    public string RoleLabel => AccessViewLabel;
    public string ScopeLabel => UserPerspectiveId switch
    {
        "USP-EXEC-001" => "Queensland",
        "USP-HHS-001" => "HHS",
        "USP-BED-001" => "Facility",
        "USP-WARD-001" => "Ward",
        _ => "Operational"
    };

    public string SummaryText => $"{RoleLabel} · {ScopeLabel}";
}
