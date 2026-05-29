namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterSelectionState(
    string SelectedHhs,
    string SelectedFacility,
    string SelectedWard,
    string SelectedServiceStream,
    string SelectedAccessView)
{
    public static FilterSelectionState CreateDefault(
        string allHhsLabel,
        string allFacilitiesLabel,
        string allWardsLabel,
        string allServiceStreamsLabel,
        string defaultAccessView) =>
        new(allHhsLabel, allFacilitiesLabel, allWardsLabel, allServiceStreamsLabel, defaultAccessView);
}
