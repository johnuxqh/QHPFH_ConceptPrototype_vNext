using QHPFH_ConceptPrototype.Data;
using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Filters;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Navigation;

namespace QHPFH_ConceptPrototype.Services.Filters;

public sealed class FilterFrameworkService
{
    public const string AllHhsLabel = DemoReferenceData.All;
    public const string AllFacilitiesLabel = DemoReferenceData.All;
    public const string AllWardsLabel = "All wards";
    public const string AllServiceStreamsLabel = "All service streams";

    private readonly PrototypeDataStore _dataStore;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly NavigationStateService _navigationState;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly Dictionary<string, FilterSelectionState> _workspaceSelections = new(StringComparer.OrdinalIgnoreCase);
    private FilterSelectionState? _currentSelection;

    public FilterFrameworkService(
        PrototypeDataStore dataStore,
        ContextAwarenessService contextAwareness,
        NavigationStateService navigationState,
        AdaptivePerspectiveEngine adaptivePerspective)
    {
        _dataStore = dataStore;
        _contextAwareness = contextAwareness;
        _navigationState = navigationState;
        _adaptivePerspective = adaptivePerspective;
    }

    public FilterSelectionState CreateDefaultSelection(string accessView = "statewide") =>
        FilterSelectionState.CreateDefault(AllHhsLabel, AllFacilitiesLabel, AllWardsLabel, AllServiceStreamsLabel, accessView);

    public FilterSelectionState GetSelection(string workspaceId, FilterSelectionState fallback) =>
        _workspaceSelections.TryGetValue(workspaceId, out var workspaceSelection)
            ? workspaceSelection
            : _currentSelection ?? fallback;

    public FilterContextRecord CreateContext(
        string workspaceId,
        FilterSelectionState selection,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        bool? canSelectHhs = null,
        bool? canSelectFacility = null,
        bool? canSelectWard = null,
        bool canSelectServiceStream = false,
        IReadOnlyCollection<string>? allowedHhs = null,
        IReadOnlyCollection<string>? allowedFacilities = null,
        IReadOnlyCollection<string>? allowedWards = null,
        IReadOnlyCollection<string>? serviceStreams = null)
    {
        var normalizedSelection = NormalizeSelection(selection, workspaceWards, allowedHhs, allowedFacilities, allowedWards, serviceStreams);

        return new FilterContextRecord(
            workspaceId,
            normalizedSelection,
            BuildHhsOptions(allowedHhs),
            BuildFacilityOptions(normalizedSelection, allowedFacilities),
            BuildWardOptions(normalizedSelection, workspaceWards, allowedWards),
            BuildServiceStreamOptions(serviceStreams),
            canSelectHhs ?? _adaptivePerspective.ShouldShowHhsFilter(),
            canSelectFacility ?? _adaptivePerspective.ShouldShowFacilityFilter(),
            canSelectWard ?? _adaptivePerspective.ShouldShowWardFilter(),
            canSelectServiceStream,
            AllHhsLabel,
            AllFacilitiesLabel,
            AllWardsLabel,
            AllServiceStreamsLabel);
    }

    public FilterSelectionState NormalizeSelection(
        FilterSelectionState selection,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        IReadOnlyCollection<string>? allowedHhs = null,
        IReadOnlyCollection<string>? allowedFacilities = null,
        IReadOnlyCollection<string>? allowedWards = null,
        IReadOnlyCollection<string>? serviceStreams = null)
    {
        var selectedHhsValues = NormalizeValues(selection.SelectedHhsValues, BuildHhsOptions(allowedHhs).Select(x => x.Value), AllHhsLabel);
        var hhsSelection = selection with { SelectedHhsValues = selectedHhsValues };

        var selectedFacilityValues = NormalizeValues(selection.SelectedFacilityValues, BuildFacilityOptions(hhsSelection, allowedFacilities).Select(x => x.Value), AllFacilitiesLabel);
        var facilitySelection = hhsSelection with { SelectedFacilityValues = selectedFacilityValues };

        var selectedWardValues = NormalizeValues(selection.SelectedWardValues, BuildWardOptions(facilitySelection, workspaceWards, allowedWards).Select(x => x.Value), AllWardsLabel);
        var selectedServiceStreamValues = NormalizeValues(selection.SelectedServiceStreamValues, BuildServiceStreamOptions(serviceStreams).Select(x => x.Value), AllServiceStreamsLabel);

        return facilitySelection with
        {
            SelectedWardValues = selectedWardValues,
            SelectedServiceStreamValues = selectedServiceStreamValues,
            AllHhsLabel = AllHhsLabel,
            AllFacilitiesLabel = AllFacilitiesLabel,
            AllWardsLabel = AllWardsLabel,
            AllServiceStreamsLabel = AllServiceStreamsLabel
        };
    }


    public FilterSelectionState ClearAll(FilterSelectionState selection) => selection with
    {
        SelectedHhsValues = Array.Empty<string>(),
        SelectedFacilityValues = Array.Empty<string>(),
        SelectedWardValues = Array.Empty<string>(),
        SelectedServiceStreamValues = Array.Empty<string>()
    };

    public FilterSelectionState SelectAllHhs(FilterSelectionState selection) => selection with { SelectedHhsValues = Array.Empty<string>() };

    public FilterSelectionState SelectAllFacilities(FilterSelectionState selection) => selection with { SelectedFacilityValues = Array.Empty<string>() };

    public FilterSelectionState SelectAllWards(FilterSelectionState selection) => selection with { SelectedWardValues = Array.Empty<string>() };

    public void ApplySelection(string workspaceId, FilterSelectionState selection, IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null)
    {
        var normalizedSelection = NormalizeSelection(selection, workspaceWards);
        _workspaceSelections[workspaceId] = normalizedSelection;
        _currentSelection = normalizedSelection;

        var navigationState = _navigationState.GetNavigationState();
        if (!string.IsNullOrWhiteSpace(navigationState.CurrentWorkspace))
        {
            _navigationState.SetCurrentWorkspace(navigationState.CurrentWorkspace);
        }

        ApplyLocationContext(normalizedSelection);
    }

    public IReadOnlyList<T> ApplyToRows<T>(IEnumerable<T> rows, FilterSelectionState selection, Func<T, string> hhsSelector, Func<T, string> facilitySelector, Func<T, string> wardSelector)
    {
        var scoped = rows;

        if (!selection.IsAllHhsSelected)
        {
            scoped = scoped.Where(row => ContainsSelection(selection.SelectedHhsValues, hhsSelector(row)));
        }

        if (!selection.IsAllFacilitiesSelected)
        {
            scoped = scoped.Where(row => ContainsSelection(selection.SelectedFacilityValues, facilitySelector(row)));
        }

        if (!selection.IsAllWardsSelected)
        {
            scoped = scoped.Where(row => ContainsSelection(selection.SelectedWardValues, wardSelector(row)));
        }

        return scoped.ToList();
    }

    public string GetContextSummary(FilterSelectionState selection) => string.Join(
        " > ",
        new[]
        {
            GetSummarySegment(selection.SelectedHhsValues, AllHhsLabel, "HHSs"),
            GetSummarySegment(selection.SelectedFacilityValues, AllFacilitiesLabel, "Facilities"),
            GetSummarySegment(selection.SelectedWardValues, AllWardsLabel, "Wards"),
            GetSummarySegment(selection.SelectedServiceStreamValues, AllServiceStreamsLabel, "Service Streams")
        }.Where(x => !string.IsNullOrWhiteSpace(x)));

    private void ApplyLocationContext(FilterSelectionState selection)
    {
        _contextAwareness.SetCurrentLocationSummary(GetContextSummary(selection));

        if (selection.SelectedWardValues.Count == 1)
        {
            var ward = ResolveWard(selection.SelectedWardValues[0], selection.SelectedFacilityValues, selection.SelectedHhsValues);
            if (ward is not null)
            {
                _contextAwareness.SetCurrentWard(ward.Id);
                return;
            }
        }

        if (selection.SelectedFacilityValues.Count == 1)
        {
            _contextAwareness.SetCurrentFacility(selection.SelectedFacilityValues[0]);
            return;
        }

        if (selection.SelectedHhsValues.Count == 1)
        {
            _contextAwareness.SetCurrentHhs(selection.SelectedHhsValues[0]);
            return;
        }

        _contextAwareness.ClearLocationContext(preserveSummary: !string.IsNullOrWhiteSpace(GetContextSummary(selection)));
    }

    private WardRecord? ResolveWard(string wardValue, IReadOnlyCollection<string> facilityValues, IReadOnlyCollection<string> hhsValues)
    {
        var wards = _dataStore.GetWards().AsEnumerable();

        if (facilityValues.Count > 0)
        {
            wards = wards.Where(x => ContainsSelection(facilityValues, x.Facility) || ContainsSelection(facilityValues, x.FacilityId));
        }

        if (hhsValues.Count > 0)
        {
            wards = wards.Where(x => ContainsSelection(hhsValues, x.Hhs));
        }

        return wards.FirstOrDefault(x => Matches(x.WardCode, wardValue) || Matches(x.Name, wardValue) || Matches(x.Id, wardValue));
    }

    private IReadOnlyList<FilterOptionRecord> BuildHhsOptions(IReadOnlyCollection<string>? allowedHhs)
    {
        var hhsNames = DemoReferenceData.HhsRecords
            .Select(x => x.Name)
            .Where(x => IsAllowed(x, allowedHhs))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(x => new FilterOptionRecord(x, x));

        return [new(AllHhsLabel, AllHhsLabel), .. hhsNames];
    }

    private IReadOnlyList<FilterOptionRecord> BuildFacilityOptions(FilterSelectionState selection, IReadOnlyCollection<string>? allowedFacilities)
    {
        var facilities = DemoReferenceData.FacilitiesByHhs
            .Where(x => selection.IsAllHhsSelected || ContainsSelection(selection.SelectedHhsValues, x.Key))
            .SelectMany(x => x.Value.Select(facility => new FilterOptionRecord(facility, facility, x.Key, facility)))
            .Where(x => IsAllowed(x.Value, allowedFacilities))
            .DistinctBy(x => x.Value, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return [new(AllFacilitiesLabel, AllFacilitiesLabel), .. facilities];
    }

    private IReadOnlyList<FilterOptionRecord> BuildWardOptions(
        FilterSelectionState selection,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards,
        IReadOnlyCollection<string>? allowedWards)
    {
        var wardSource = workspaceWards?.ToList() ?? _dataStore.GetWards()
            .Select(x => new FilterWorkspaceWardRecord(x.Hhs, x.Facility, x.WardCode, x.WardTypeLabel))
            .ToList();

        var wards = wardSource
            .Where(x => selection.IsAllHhsSelected || ContainsSelection(selection.SelectedHhsValues, x.Hhs))
            .Where(x => selection.IsAllFacilitiesSelected || ContainsSelection(selection.SelectedFacilityValues, x.Facility))
            .Where(x => IsAllowed(x.Ward, allowedWards))
            .Select(x => new FilterOptionRecord(x.Ward, x.Ward, x.Hhs, x.Facility, x.ServiceStream))
            .DistinctBy(x => x.Value, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return [new(AllWardsLabel, AllWardsLabel), .. wards];
    }

    private IReadOnlyList<FilterOptionRecord> BuildServiceStreamOptions(IReadOnlyCollection<string>? serviceStreams)
    {
        var streams = (serviceStreams ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(x => new FilterOptionRecord(x, x))
            .ToList();

        return [new(AllServiceStreamsLabel, AllServiceStreamsLabel), .. streams];
    }

    private static IReadOnlyList<string> NormalizeValues(IEnumerable<string> selectedValues, IEnumerable<string> validValues, string allLabel)
    {
        var valid = validValues.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalized = selectedValues
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Where(x => !Matches(x, allLabel) && valid.Contains(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return normalized;
    }

    private static string? GetSummarySegment(IReadOnlyList<string> values, string allLabel, string pluralLabel) => values.Count switch
    {
        0 => null,
        <= 2 => string.Join(", ", values),
        _ => $"{values.Count} {pluralLabel}"
    };

    private static bool IsAllowed(string value, IReadOnlyCollection<string>? allowedValues) =>
        allowedValues is null || allowedValues.Count == 0 || allowedValues.Any(x => Matches(x, value));

    private static bool ContainsSelection(IEnumerable<string> selection, string? value) =>
        selection.Any(x => Matches(x, value));

    private static bool Matches(string? left, string? right) =>
        string.Equals(NormalizeComparable(left), NormalizeComparable(right), StringComparison.OrdinalIgnoreCase);

    private static string NormalizeComparable(string? value) =>
        (value ?? string.Empty).Trim().Replace('’', '\'');
}
