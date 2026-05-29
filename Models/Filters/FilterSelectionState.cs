namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterSelectionState
{
    public IReadOnlyList<string> SelectedHhsValues { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SelectedFacilityValues { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SelectedWardValues { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SelectedServiceStreamValues { get; init; } = Array.Empty<string>();
    public string SelectedAccessView { get; init; } = string.Empty;
    public string AllHhsLabel { get; init; } = string.Empty;
    public string AllFacilitiesLabel { get; init; } = string.Empty;
    public string AllWardsLabel { get; init; } = string.Empty;
    public string AllServiceStreamsLabel { get; init; } = string.Empty;

    public string SelectedHhs => GetSelectionLabel(SelectedHhsValues, AllHhsLabel, "HHSs");
    public string SelectedFacility => GetSelectionLabel(SelectedFacilityValues, AllFacilitiesLabel, "Facilities");
    public string SelectedWard => GetSelectionLabel(SelectedWardValues, AllWardsLabel, "Wards");
    public string SelectedServiceStream => GetSelectionLabel(SelectedServiceStreamValues, AllServiceStreamsLabel, "Service Streams");

    public bool IsAllHhsSelected => SelectedHhsValues.Count == 0;
    public bool IsAllFacilitiesSelected => SelectedFacilityValues.Count == 0;
    public bool IsAllWardsSelected => SelectedWardValues.Count == 0;
    public bool IsAllServiceStreamsSelected => SelectedServiceStreamValues.Count == 0;

    public static FilterSelectionState CreateDefault(
        string allHhsLabel,
        string allFacilitiesLabel,
        string allWardsLabel,
        string allServiceStreamsLabel,
        string defaultAccessView) =>
        new()
        {
            AllHhsLabel = allHhsLabel,
            AllFacilitiesLabel = allFacilitiesLabel,
            AllWardsLabel = allWardsLabel,
            AllServiceStreamsLabel = allServiceStreamsLabel,
            SelectedAccessView = defaultAccessView
        };

    private static string GetSelectionLabel(IReadOnlyList<string> values, string allLabel, string pluralLabel) => values.Count switch
    {
        0 => allLabel,
        1 => values[0],
        _ => $"{values.Count} {pluralLabel} Selected"
    };
}
