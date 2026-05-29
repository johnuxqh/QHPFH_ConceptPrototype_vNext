using QHPFH_ConceptPrototype.Data;
using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Filters;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Navigation;
using QHPFH_ConceptPrototype.Services.Adaptive;

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
        var selectedHhs = NormalizeOption(selection.SelectedHhs, AllHhsLabel);
        var hhsValues = BuildHhsOptions(allowedHhs).Select(x => x.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!hhsValues.Contains(selectedHhs))
        {
            selectedHhs = AllHhsLabel;
        }

        var selectedFacility = NormalizeOption(selection.SelectedFacility, AllFacilitiesLabel);
        var facilityValues = BuildFacilityOptions(selection with { SelectedHhs = selectedHhs }, allowedFacilities).Select(x => x.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!facilityValues.Contains(selectedFacility))
        {
            selectedFacility = AllFacilitiesLabel;
        }

        var selectedWard = NormalizeOption(selection.SelectedWard, AllWardsLabel);
        var wardValues = BuildWardOptions(selection with { SelectedHhs = selectedHhs, SelectedFacility = selectedFacility }, workspaceWards, allowedWards).Select(x => x.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!wardValues.Contains(selectedWard))
        {
            selectedWard = AllWardsLabel;
        }

        var selectedServiceStream = NormalizeOption(selection.SelectedServiceStream, AllServiceStreamsLabel);
        var streamValues = BuildServiceStreamOptions(serviceStreams).Select(x => x.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!streamValues.Contains(selectedServiceStream))
        {
            selectedServiceStream = AllServiceStreamsLabel;
        }

        return selection with
        {
            SelectedHhs = selectedHhs,
            SelectedFacility = selectedFacility,
            SelectedWard = selectedWard,
            SelectedServiceStream = selectedServiceStream
        };
    }

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

        if (selection.SelectedHhs != AllHhsLabel)
        {
            scoped = scoped.Where(row => Matches(hhsSelector(row), selection.SelectedHhs));
        }

        if (selection.SelectedFacility != AllFacilitiesLabel)
        {
            scoped = scoped.Where(row => Matches(facilitySelector(row), selection.SelectedFacility));
        }

        if (selection.SelectedWard != AllWardsLabel)
        {
            scoped = scoped.Where(row => Matches(wardSelector(row), selection.SelectedWard));
        }

        return scoped.ToList();
    }

    private void ApplyLocationContext(FilterSelectionState selection)
    {
        if (selection.SelectedWard != AllWardsLabel)
        {
            var ward = ResolveWard(selection.SelectedWard, selection.SelectedFacility, selection.SelectedHhs);
            if (ward is not null)
            {
                _contextAwareness.SetCurrentWard(ward.Id);
                return;
            }
        }

        if (selection.SelectedFacility != AllFacilitiesLabel)
        {
            _contextAwareness.SetCurrentFacility(selection.SelectedFacility);
            return;
        }

        if (selection.SelectedHhs != AllHhsLabel)
        {
            _contextAwareness.SetCurrentHhs(selection.SelectedHhs);
            return;
        }

        _contextAwareness.ClearLocationContext();
    }

    private WardRecord? ResolveWard(string wardValue, string facilityValue, string hhsValue)
    {
        var wards = _dataStore.GetWards().AsEnumerable();

        if (facilityValue != AllFacilitiesLabel)
        {
            wards = wards.Where(x => Matches(x.Facility, facilityValue) || Matches(x.FacilityId, facilityValue));
        }

        if (hhsValue != AllHhsLabel)
        {
            wards = wards.Where(x => Matches(x.Hhs, hhsValue));
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
            .Where(x => selection.SelectedHhs == AllHhsLabel || Matches(x.Key, selection.SelectedHhs))
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
            .Where(x => selection.SelectedHhs == AllHhsLabel || Matches(x.Hhs, selection.SelectedHhs))
            .Where(x => selection.SelectedFacility == AllFacilitiesLabel || Matches(x.Facility, selection.SelectedFacility))
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

    private static bool IsAllowed(string value, IReadOnlyCollection<string>? allowedValues) =>
        allowedValues is null || allowedValues.Count == 0 || allowedValues.Any(x => Matches(x, value));

    private static bool Matches(string? left, string? right) =>
        string.Equals(NormalizeComparable(left), NormalizeComparable(right), StringComparison.OrdinalIgnoreCase);

    private static string NormalizeComparable(string? value) =>
        (value ?? string.Empty).Trim().Replace('’', '\'');

    private static string NormalizeOption(string? value, string allLabel) =>
        string.IsNullOrWhiteSpace(value) ? allLabel : value.Trim();
}
