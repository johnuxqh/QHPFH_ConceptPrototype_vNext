namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterContextRecord(
    string WorkspaceId,
    FilterSelectionState Selection,
    IReadOnlyList<FilterOptionRecord> HhsOptions,
    IReadOnlyList<FilterOptionRecord> FacilityOptions,
    IReadOnlyList<FilterOptionRecord> WardOptions,
    IReadOnlyList<FilterOptionRecord> ServiceStreamOptions,
    bool CanSelectHhs,
    bool CanSelectFacility,
    bool CanSelectWard,
    bool CanSelectServiceStream,
    bool IsHhsLocked,
    bool IsFacilityLocked,
    bool IsWardLocked,
    bool IsServiceStreamLocked,
    FilterVisibilityProfile VisibilityProfile,
    string AllHhsLabel,
    string AllFacilitiesLabel,
    string AllWardsLabel,
    string AllServiceStreamsLabel)
{
    public string ContextPath => string.Join(
        " → ",
        new[]
        {
            Selection.SelectedHhs == AllHhsLabel ? null : Selection.SelectedHhs,
            Selection.SelectedFacility == AllFacilitiesLabel ? null : Selection.SelectedFacility,
            Selection.SelectedWard == AllWardsLabel ? null : Selection.SelectedWard,
            Selection.SelectedServiceStream == AllServiceStreamsLabel ? null : Selection.SelectedServiceStream
        }.Where(x => !string.IsNullOrWhiteSpace(x))) switch
        {
            { Length: > 0 } path => path,
            _ => "Statewide"
        };
}

public sealed record FilterWorkspaceWardRecord(
    string Hhs,
    string Facility,
    string Ward,
    string? ServiceStream = null);
