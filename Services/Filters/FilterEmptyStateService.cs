using QHPFH_ConceptPrototype.Models.Filters;

namespace QHPFH_ConceptPrototype.Services.Filters;

public sealed class FilterEmptyStateService
{
    public FilterEmptyStateRecord? GetEmptyState(
        FilterContextRecord context,
        int filteredResultCount,
        int sourceResultCount,
        string workspaceLabel,
        string recordLabel,
        bool prototypeDataAvailable = true)
    {
        if (HasResults(filteredResultCount))
        {
            return null;
        }

        return DetermineEmptyState(context, sourceResultCount, workspaceLabel, recordLabel, prototypeDataAvailable);
    }

    public bool HasResults(int resultCount) => resultCount > 0;

    public FilterEmptyStateRecord DetermineEmptyState(
        FilterContextRecord context,
        int sourceResultCount,
        string workspaceLabel,
        string recordLabel,
        bool prototypeDataAvailable = true)
    {
        var scopeSummary = context.ContextPath;

        if (!prototypeDataAvailable)
        {
            return new(
                FilterEmptyStateType.PrototypeDataUnavailable,
                "Operational data not yet available",
                $"{workspaceLabel} data is not yet available in the prototype dataset for {scopeSummary}.",
                scopeSummary,
                GetRecoveryActions(FilterEmptyStateType.PrototypeDataUnavailable));
        }

        if (sourceResultCount == 0 || IsAccessScopeConstrained(context))
        {
            return new(
                FilterEmptyStateType.AccessScopeEmpty,
                "No operational records available within your current access scope",
                $"The current access scope does not contain matching {recordLabel} for {scopeSummary}.",
                scopeSummary,
                GetRecoveryActions(FilterEmptyStateType.AccessScopeEmpty));
        }

        if (HasActiveFilterSelection(context.Selection))
        {
            return new(
                FilterEmptyStateType.FilterConflict,
                "No matching operational data",
                $"The current filter combination returns zero {recordLabel}. Review the selected HHS, Facility, Ward, or Service Stream filters for {scopeSummary}.",
                scopeSummary,
                GetRecoveryActions(FilterEmptyStateType.FilterConflict));
        }

        return new(
            FilterEmptyStateType.ValidEmptyOperationalState,
            $"No {recordLabel} identified",
            $"The current operational scope contains no matching {recordLabel} for {scopeSummary}.",
            scopeSummary,
            GetRecoveryActions(FilterEmptyStateType.ValidEmptyOperationalState));
    }

    public IReadOnlyList<FilterEmptyStateActionRecord> GetRecoveryActions(FilterEmptyStateType type) => type switch
    {
        FilterEmptyStateType.FilterConflict =>
        [
            new("Clear filters", FilterResetMode.ClearAll),
            new("Reset scope", FilterResetMode.AccessScope),
            new("Workspace default", FilterResetMode.WorkspaceDefault)
        ],
        FilterEmptyStateType.AccessScopeEmpty =>
        [
            new("Reset scope", FilterResetMode.AccessScope),
            new("Workspace default", FilterResetMode.WorkspaceDefault)
        ],
        FilterEmptyStateType.ValidEmptyOperationalState =>
        [
            new("Clear filters", FilterResetMode.ClearAll),
            new("Reset scope", FilterResetMode.AccessScope)
        ],
        _ => Array.Empty<FilterEmptyStateActionRecord>()
    };

    private static bool HasActiveFilterSelection(FilterSelectionState selection) =>
        selection.SelectedHhsValues.Count > 0
        || selection.SelectedFacilityValues.Count > 0
        || selection.SelectedWardValues.Count > 0
        || selection.SelectedServiceStreamValues.Count > 0;

    private static bool IsAccessScopeConstrained(FilterContextRecord context) =>
        context.VisibilityProfile.AllowedHhsValues.Count > 0
        || context.VisibilityProfile.AllowedFacilityValues.Count > 0
        || context.VisibilityProfile.AllowedWardValues.Count > 0
        || context.VisibilityProfile.AllowedServiceStreamValues.Count > 0;
}
