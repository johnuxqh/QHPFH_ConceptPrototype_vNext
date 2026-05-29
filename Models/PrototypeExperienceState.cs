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
    public string SummaryText => $"{AccessViewLabel} · {ExperienceMode.ToSummaryText()} · {LayoutVariant.ToSummaryText()}";
}
