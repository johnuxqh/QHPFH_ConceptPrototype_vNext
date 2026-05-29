using QHPFH_ConceptPrototype.Data;
using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Filters;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Navigation;

namespace QHPFH_ConceptPrototype.Services.Filters;

public sealed class FilterFrameworkService
{
    public const string AllHhsLabel = DemoReferenceData.All;
    public const string AllFacilitiesLabel = DemoReferenceData.All;
    public const string AllWardsLabel = "All wards";
    public const string AllServiceStreamsLabel = "All service streams";

    private const string WardValueSeparator = "||";
    private const string StatewideAccessView = "statewide";
    private const string QchAccessView = "qch-bed-manager";
    private const string QchHhs = "Children’s Health Queensland";
    private const string QchFacility = "Queensland Children's Hospital";

    private readonly PrototypeDataStore _dataStore;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly NavigationStateService _navigationState;
    private readonly FilterVisibilityService _filterVisibility;
    private readonly Dictionary<string, FilterSelectionState> _workspaceSelections = new(StringComparer.OrdinalIgnoreCase);
    private FilterSelectionState? _sharedSelection;

    public FilterFrameworkService(
        PrototypeDataStore dataStore,
        ContextAwarenessService contextAwareness,
        NavigationStateService navigationState,
        FilterVisibilityService filterVisibility)
    {
        _dataStore = dataStore;
        _contextAwareness = contextAwareness;
        _navigationState = navigationState;
        _filterVisibility = filterVisibility;
    }

    public FilterSelectionState CreateDefaultSelection(string accessView = "statewide") =>
        FilterSelectionState.CreateDefault(AllHhsLabel, AllFacilitiesLabel, AllWardsLabel, AllServiceStreamsLabel, accessView);

    public FilterSelectionState GetSelection(
        string workspaceId,
        FilterSelectionState fallback,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext) =>
        persistenceMode == FilterPersistenceMode.WorkspaceOverride
            ? GetWorkspaceSelection(workspaceId) ?? _sharedSelection ?? fallback
            : _sharedSelection ?? GetWorkspaceSelection(workspaceId) ?? fallback;

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
        IReadOnlyCollection<string>? serviceStreams = null,
        FilterVisibilityProfile? visibilityProfile = null)
    {
        var effectiveProfile = _filterVisibility.GetCurrentProfile(visibilityProfile);
        var effectiveAllowedHhs = MergeAllowedValues(allowedHhs, effectiveProfile.AllowedHhsValues);
        var effectiveAllowedFacilities = MergeAllowedValues(allowedFacilities, effectiveProfile.AllowedFacilityValues);
        var effectiveAllowedWards = MergeAllowedValues(allowedWards, effectiveProfile.AllowedWardValues);
        var normalizedSelection = NormalizeSelection(selection, workspaceWards, effectiveAllowedHhs, effectiveAllowedFacilities, effectiveAllowedWards, serviceStreams);

        return new FilterContextRecord(
            workspaceId,
            normalizedSelection,
            BuildHhsOptions(effectiveAllowedHhs),
            BuildFacilityOptions(normalizedSelection, effectiveAllowedFacilities),
            BuildWardOptions(normalizedSelection, workspaceWards, effectiveAllowedWards),
            BuildServiceStreamOptions(serviceStreams),
            canSelectHhs ?? effectiveProfile.ShowHhsFilter,
            canSelectFacility ?? effectiveProfile.ShowFacilityFilter,
            canSelectWard ?? effectiveProfile.ShowWardFilter,
            canSelectServiceStream || effectiveProfile.ShowServiceStreamFilter,
            effectiveProfile.LockHhsFilter,
            effectiveProfile.LockFacilityFilter,
            effectiveProfile.LockWardFilter,
            effectiveProfile.LockServiceStreamFilter,
            effectiveProfile,
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

        var wardOptions = BuildWardOptions(facilitySelection, workspaceWards, allowedWards);
        var selectedWardValues = NormalizeWardValues(selection.SelectedWardValues, wardOptions, AllWardsLabel);
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

    public IReadOnlyList<FilterPresetRecord> GetAvailablePresets(FilterContextRecord context)
    {
        var presets = new List<FilterPresetRecord>
        {
            new(FilterPresetType.StatewideOverview, "Statewide overview", "Return to all permitted HHS, facilities and wards."),
            new(FilterPresetType.AccessScope, "Reset to my scope", "Restore the operational scope implied by the current access view."),
            new(FilterPresetType.HighPressureWards, "High pressure wards", "Prepare for future pressure-focused filtering using the current access scope."),
            new(FilterPresetType.DelayedFlowFocus, "Delayed flow focus", "Prepare for future delayed-flow filtering using the current access scope."),
            new(FilterPresetType.EdPressureFocus, "ED pressure focus", "Prepare for future ED-pressure filtering using the current access scope."),
            new(FilterPresetType.DischargeOpportunityFocus, "Discharge opportunity focus", "Prepare for future discharge-opportunity filtering using the current access scope.")
        };

        if (context.VisibilityProfile.AllowedFacilityValues.Count == 1)
        {
            presets.Insert(2, new(FilterPresetType.MyFacility, "My facility", "Restore the facility context available to the current access view."));
        }

        if (CanApplyQchPreset(context))
        {
            presets.Insert(2, new(FilterPresetType.QchBedManager, "QCH Bed Manager", "Restore Children’s Health Queensland and Queensland Children's Hospital scope."));
        }

        return presets.Where(preset => CanApplyPreset(context, preset.Type)).ToList();
    }

    public bool CanApplyPreset(FilterContextRecord context, FilterPresetType presetType) => presetType switch
    {
        FilterPresetType.StatewideOverview => HasUnrestrictedAccessScope(context.VisibilityProfile),
        FilterPresetType.QchBedManager => CanApplyQchPreset(context),
        FilterPresetType.MyFacility => context.VisibilityProfile.AllowedFacilityValues.Count == 1,
        _ => true
    };

    public FilterSelectionState ResetSelection(
        string workspaceId,
        FilterContextRecord context,
        FilterResetMode resetMode,
        FilterSelectionState? workspaceDefault = null,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext)
    {
        var resetSelection = resetMode switch
        {
            FilterResetMode.ClearAll => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile),
            FilterResetMode.AccessScope => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile),
            FilterResetMode.WorkspaceDefault => workspaceDefault is null
                ? BuildAccessScopeSelection(context.Selection, context.VisibilityProfile)
                : ApplyAccessScopeToSelection(workspaceDefault, context.VisibilityProfile),
            _ => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile)
        };

        return ApplySelection(
            workspaceId,
            resetSelection,
            workspaceWards,
            visibilityProfile: context.VisibilityProfile,
            persistenceMode: persistenceMode);
    }

    public FilterSelectionState ResetToAccessScope(
        string workspaceId,
        FilterContextRecord context,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext) =>
        ResetSelection(workspaceId, context, FilterResetMode.AccessScope, workspaceWards: workspaceWards, persistenceMode: persistenceMode);

    public FilterSelectionState ResetToWorkspaceDefault(
        string workspaceId,
        FilterContextRecord context,
        FilterSelectionState? workspaceDefault = null,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext) =>
        ResetSelection(workspaceId, context, FilterResetMode.WorkspaceDefault, workspaceDefault, workspaceWards, persistenceMode);

    public FilterSelectionState ApplyPreset(
        string workspaceId,
        FilterContextRecord context,
        FilterPresetType presetType,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext)
    {
        if (!CanApplyPreset(context, presetType))
        {
            return ApplySelection(workspaceId, context.Selection, workspaceWards, visibilityProfile: context.VisibilityProfile, persistenceMode: persistenceMode);
        }

        var presetSelection = presetType switch
        {
            FilterPresetType.StatewideOverview => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile) with { SelectedAccessView = StatewideAccessView },
            FilterPresetType.QchBedManager => ApplyQchScope(context.Selection),
            FilterPresetType.MyFacility => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile),
            _ => BuildAccessScopeSelection(context.Selection, context.VisibilityProfile)
        };

        return ApplySelection(
            workspaceId,
            presetSelection,
            workspaceWards,
            visibilityProfile: context.VisibilityProfile,
            persistenceMode: persistenceMode);
    }

    private static FilterSelectionState BuildAccessScopeSelection(FilterSelectionState selection, FilterVisibilityProfile visibilityProfile) =>
        ApplyAccessScopeToSelection(selection, visibilityProfile) with
        {
            SelectedWardValues = Array.Empty<string>(),
            SelectedServiceStreamValues = Array.Empty<string>()
        };

    private static FilterSelectionState ApplyAccessScopeToSelection(FilterSelectionState selection, FilterVisibilityProfile visibilityProfile) => selection with
    {
        SelectedHhsValues = GetAccessScopeValues(selection.SelectedHhsValues, visibilityProfile.AllowedHhsValues, visibilityProfile.ShowHhsFilter, visibilityProfile.LockHhsFilter),
        SelectedFacilityValues = GetAccessScopeValues(selection.SelectedFacilityValues, visibilityProfile.AllowedFacilityValues, visibilityProfile.ShowFacilityFilter, visibilityProfile.LockFacilityFilter),
        SelectedWardValues = GetAccessScopeValues(selection.SelectedWardValues, visibilityProfile.AllowedWardValues, visibilityProfile.ShowWardFilter, visibilityProfile.LockWardFilter),
        SelectedServiceStreamValues = GetAccessScopeValues(selection.SelectedServiceStreamValues, visibilityProfile.AllowedServiceStreamValues, visibilityProfile.ShowServiceStreamFilter, visibilityProfile.LockServiceStreamFilter)
    };

    private static IReadOnlyList<string> GetAccessScopeValues(
        IReadOnlyList<string> currentValues,
        IReadOnlyCollection<string> allowedValues,
        bool isVisible,
        bool isLocked)
    {
        if ((isLocked || !isVisible) && allowedValues.Count == 1)
        {
            return allowedValues.ToList();
        }

        return isLocked ? currentValues : Array.Empty<string>();
    }

    private static bool HasUnrestrictedAccessScope(FilterVisibilityProfile visibilityProfile) =>
        visibilityProfile.AllowedHhsValues.Count == 0
        && visibilityProfile.AllowedFacilityValues.Count == 0
        && visibilityProfile.AllowedWardValues.Count == 0
        && visibilityProfile.AllowedServiceStreamValues.Count == 0;

    private static bool CanApplyQchPreset(FilterContextRecord context) =>
        (context.VisibilityProfile.AllowedHhsValues.Count == 0
            || context.VisibilityProfile.AllowedHhsValues.Any(value => Matches(value, QchHhs)))
        && (context.VisibilityProfile.AllowedFacilityValues.Count == 0
            || context.VisibilityProfile.AllowedFacilityValues.Any(value => Matches(value, QchFacility)));

    private static FilterSelectionState ApplyQchScope(FilterSelectionState selection) => selection with
    {
        SelectedHhsValues = [QchHhs],
        SelectedFacilityValues = [QchFacility],
        SelectedWardValues = Array.Empty<string>(),
        SelectedServiceStreamValues = Array.Empty<string>(),
        SelectedAccessView = QchAccessView
    };

    public FilterSelectionState RestoreSelection(
        string workspaceId,
        FilterSelectionState fallback,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        IReadOnlyCollection<string>? allowedHhs = null,
        IReadOnlyCollection<string>? allowedFacilities = null,
        IReadOnlyCollection<string>? allowedWards = null,
        IReadOnlyCollection<string>? serviceStreams = null,
        FilterVisibilityProfile? visibilityProfile = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext)
    {
        var restoredSelection = GetSelection(workspaceId, fallback, persistenceMode);
        return ApplySelection(
            workspaceId,
            restoredSelection,
            workspaceWards,
            allowedHhs,
            allowedFacilities,
            allowedWards,
            serviceStreams,
            visibilityProfile,
            persistenceMode);
    }

    public FilterSelectionState ApplySelection(
        string workspaceId,
        FilterSelectionState selection,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards = null,
        IReadOnlyCollection<string>? allowedHhs = null,
        IReadOnlyCollection<string>? allowedFacilities = null,
        IReadOnlyCollection<string>? allowedWards = null,
        IReadOnlyCollection<string>? serviceStreams = null,
        FilterVisibilityProfile? visibilityProfile = null,
        FilterPersistenceMode persistenceMode = FilterPersistenceMode.SharedContext)
    {
        var effectiveProfile = _filterVisibility.GetCurrentProfile(visibilityProfile);
        var effectiveAllowedHhs = MergeAllowedValues(allowedHhs, effectiveProfile.AllowedHhsValues);
        var effectiveAllowedFacilities = MergeAllowedValues(allowedFacilities, effectiveProfile.AllowedFacilityValues);
        var effectiveAllowedWards = MergeAllowedValues(allowedWards, effectiveProfile.AllowedWardValues);
        var normalizedSelection = NormalizeSelection(selection, workspaceWards, effectiveAllowedHhs, effectiveAllowedFacilities, effectiveAllowedWards, serviceStreams);
        PersistSelection(workspaceId, normalizedSelection, persistenceMode);

        var navigationState = _navigationState.GetNavigationState();
        if (!string.IsNullOrWhiteSpace(navigationState.CurrentWorkspace))
        {
            _navigationState.SetCurrentWorkspace(navigationState.CurrentWorkspace);
        }

        ApplyLocationContext(normalizedSelection, workspaceWards);
        return normalizedSelection;
    }

    private FilterSelectionState? GetWorkspaceSelection(string workspaceId) =>
        _workspaceSelections.TryGetValue(workspaceId, out var workspaceSelection)
            ? workspaceSelection
            : null;

    private void PersistSelection(string workspaceId, FilterSelectionState selection, FilterPersistenceMode persistenceMode)
    {
        _workspaceSelections[workspaceId] = selection;

        if (persistenceMode == FilterPersistenceMode.SharedContext)
        {
            _sharedSelection = selection;
        }
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
            scoped = scoped.Where(row => MatchesWardSelection(selection.SelectedWardValues, hhsSelector(row), facilitySelector(row), wardSelector(row)));
        }

        return scoped.ToList();
    }

    public string GetContextSummary(FilterSelectionState selection) =>
        GetEffectiveContextSummary(selection, workspaceWards: null);

    private string GetEffectiveContextSummary(FilterSelectionState selection, IEnumerable<FilterWorkspaceWardRecord>? workspaceWards)
    {
        var wardParents = ResolveWardParents(selection.SelectedWardValues, selection.SelectedFacilityValues, selection.SelectedHhsValues, workspaceWards);
        var effectiveFacilityValues = GetEffectiveFacilityValues(selection, wardParents);
        var effectiveHhsValues = GetEffectiveHhsValues(selection, effectiveFacilityValues, wardParents);

        return string.Join(
            " > ",
            new[]
            {
                GetSummarySegment(effectiveHhsValues, AllHhsLabel, "HHSs"),
                GetSummarySegment(effectiveFacilityValues, AllFacilitiesLabel, "Facilities"),
                GetSummarySegment(selection.SelectedWardValues, AllWardsLabel, "Wards"),
                GetSummarySegment(selection.SelectedServiceStreamValues, AllServiceStreamsLabel, "Service Streams")
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private void ApplyLocationContext(FilterSelectionState selection, IEnumerable<FilterWorkspaceWardRecord>? workspaceWards)
    {
        var wardParents = ResolveWardParents(selection.SelectedWardValues, selection.SelectedFacilityValues, selection.SelectedHhsValues, workspaceWards);
        var effectiveFacilityValues = GetEffectiveFacilityValues(selection, wardParents);
        var effectiveHhsValues = GetEffectiveHhsValues(selection, effectiveFacilityValues, wardParents);
        var summary = GetEffectiveContextSummary(selection, workspaceWards);

        var hhsId = effectiveHhsValues.Count == 1 ? effectiveHhsValues[0] : null;
        var facilityId = effectiveFacilityValues.Count == 1 ? effectiveFacilityValues[0] : null;
        var wardId = wardParents.Count == 1 ? wardParents[0].WardId : null;

        _contextAwareness.SetCurrentLocationContext(hhsId, facilityId, wardId, summary);
    }

    private IReadOnlyList<string> GetEffectiveHhsValues(
        FilterSelectionState selection,
        IReadOnlyList<string> effectiveFacilityValues,
        IReadOnlyList<ResolvedWardParent> wardParents)
    {
        var values = selection.SelectedHhsValues
            .Concat(ResolveParentHhsValues(effectiveFacilityValues))
            .Concat(wardParents.Select(x => x.Hhs))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return values;
    }

    private IReadOnlyList<string> GetEffectiveFacilityValues(FilterSelectionState selection, IReadOnlyList<ResolvedWardParent> wardParents)
    {
        var values = selection.SelectedFacilityValues
            .Concat(ResolveParentFacilityValues(wardParents))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return values;
    }

    private IReadOnlyList<string> ResolveParentHhsValues(IEnumerable<string> facilityValues)
    {
        var facilitySet = facilityValues.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (facilitySet.Count == 0)
        {
            return Array.Empty<string>();
        }

        return DemoReferenceData.FacilitiesByHhs
            .Where(x => x.Value.Any(facility => facilitySet.Contains(facility)))
            .Select(x => x.Key)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyList<string> ResolveParentFacilityValues(IEnumerable<ResolvedWardParent> wardParents) =>
        wardParents
            .Select(x => x.Facility)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private IReadOnlyList<ResolvedWardParent> ResolveWardParents(
        IReadOnlyCollection<string> wardValues,
        IReadOnlyCollection<string> facilityValues,
        IReadOnlyCollection<string> hhsValues,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards)
    {
        if (wardValues.Count == 0)
        {
            return Array.Empty<ResolvedWardParent>();
        }

        return wardValues
            .Select(wardValue => ResolveWardParent(wardValue, facilityValues, hhsValues, workspaceWards))
            .Where(x => x is not null)
            .Cast<ResolvedWardParent>()
            .DistinctBy(x => x.WardId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private ResolvedWardParent? ResolveWardParent(
        string wardValue,
        IReadOnlyCollection<string> facilityValues,
        IReadOnlyCollection<string> hhsValues,
        IEnumerable<FilterWorkspaceWardRecord>? workspaceWards)
    {
        var parsed = ParseWardValue(wardValue);
        var candidates = BuildWardParentCandidates(parsed.Ward, workspaceWards);

        if (!string.IsNullOrWhiteSpace(parsed.Hhs))
        {
            candidates = candidates.Where(x => Matches(x.Hhs, parsed.Hhs)).ToList();
        }
        else if (hhsValues.Count > 0)
        {
            candidates = candidates.Where(x => ContainsSelection(hhsValues, x.Hhs)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(parsed.Facility))
        {
            candidates = candidates.Where(x => Matches(x.Facility, parsed.Facility)).ToList();
        }
        else if (facilityValues.Count > 0)
        {
            candidates = candidates.Where(x => ContainsSelection(facilityValues, x.Facility)).ToList();
        }

        candidates = candidates
            .Where(x => Matches(x.Ward, parsed.Ward) || Matches(x.WardId, parsed.Ward))
            .DistinctBy(x => x.WardId, StringComparer.OrdinalIgnoreCase)
            .Take(2)
            .ToList();

        return candidates.Count == 1 ? candidates[0] : null;
    }

    private IReadOnlyList<ResolvedWardParent> BuildWardParentCandidates(string wardValue, IEnumerable<FilterWorkspaceWardRecord>? workspaceWards)
    {
        var workspaceCandidates = (workspaceWards ?? Array.Empty<FilterWorkspaceWardRecord>())
            .Where(x => Matches(x.Ward, wardValue))
            .Select(x => new ResolvedWardParent(
                ResolveWardId(x.Hhs, x.Facility, x.Ward) ?? BuildWardValue(x.Hhs, x.Facility, x.Ward),
                x.Hhs,
                x.Facility,
                x.Ward))
            .ToList();

        if (workspaceCandidates.Count > 0)
        {
            return workspaceCandidates;
        }

        return _dataStore.GetWards()
            .Where(x => Matches(x.WardCode, wardValue) || Matches(x.Name, wardValue) || Matches(x.Id, wardValue))
            .Select(x => new ResolvedWardParent(x.Id, x.Hhs, x.Facility, x.WardCode))
            .ToList();
    }

    private string? ResolveWardId(string hhs, string facility, string ward) => _dataStore.GetWards()
        .FirstOrDefault(x => Matches(x.Hhs, hhs)
            && Matches(x.Facility, facility)
            && (Matches(x.WardCode, ward) || Matches(x.Name, ward) || Matches(x.Id, ward)))
        ?.Id;

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

        var scopedWards = wardSource
            .Where(x => selection.IsAllHhsSelected || ContainsSelection(selection.SelectedHhsValues, x.Hhs))
            .Where(x => selection.IsAllFacilitiesSelected || ContainsSelection(selection.SelectedFacilityValues, x.Facility))
            .Where(x => IsAllowed(x.Ward, allowedWards))
            .ToList();

        var duplicateWardNames = scopedWards
            .GroupBy(x => NormalizeComparable(x.Ward), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Select(y => NormalizeComparable(y.Facility)).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
            .Select(x => x.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var wards = scopedWards
            .Select(x =>
            {
                var isDuplicate = duplicateWardNames.Contains(NormalizeComparable(x.Ward));
                var value = isDuplicate ? BuildWardValue(x.Hhs, x.Facility, x.Ward) : x.Ward;
                var label = isDuplicate ? $"{x.Ward} · {x.Facility}" : x.Ward;
                return new FilterOptionRecord(value, label, x.Hhs, x.Facility, x.ServiceStream);
            })
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
        return selectedValues
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Where(x => !Matches(x, allLabel) && valid.Contains(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyList<string> NormalizeWardValues(IEnumerable<string> selectedValues, IReadOnlyList<FilterOptionRecord> validWardOptions, string allLabel)
    {
        var validValues = validWardOptions.Select(x => x.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var optionsByLabel = validWardOptions
            .Where(x => !Matches(x.Value, allLabel))
            .GroupBy(x => NormalizeComparable(ParseWardValue(x.Value).Ward), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

        var normalized = new List<string>();
        foreach (var selectedValue in selectedValues.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
        {
            if (Matches(selectedValue, allLabel))
            {
                continue;
            }

            if (validValues.Contains(selectedValue))
            {
                normalized.Add(selectedValue);
                continue;
            }

            var parsed = ParseWardValue(selectedValue);
            if (optionsByLabel.TryGetValue(NormalizeComparable(parsed.Ward), out var matchingOptions) && matchingOptions.Count == 1)
            {
                normalized.Add(matchingOptions[0].Value);
            }
        }

        return normalized.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static string? GetSummarySegment(IReadOnlyList<string> values, string allLabel, string pluralLabel) => values.Count switch
    {
        0 => null,
        <= 2 => string.Join(", ", values.Select(x => ParseWardValue(x).Ward)),
        _ => $"{values.Count} {pluralLabel}"
    };

    private static IReadOnlyCollection<string>? MergeAllowedValues(IReadOnlyCollection<string>? explicitValues, IReadOnlyCollection<string> profileValues)
    {
        if ((explicitValues is null || explicitValues.Count == 0) && profileValues.Count == 0)
        {
            return null;
        }

        if (explicitValues is null || explicitValues.Count == 0)
        {
            return profileValues;
        }

        if (profileValues.Count == 0)
        {
            return explicitValues;
        }

        return explicitValues
            .Where(value => profileValues.Any(profileValue => Matches(profileValue, value)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsAllowed(string value, IReadOnlyCollection<string>? allowedValues) =>
        allowedValues is null || allowedValues.Count == 0 || allowedValues.Any(x => Matches(x, value));

    private static bool ContainsSelection(IEnumerable<string> selection, string? value) =>
        selection.Any(x => Matches(x, value));

    private static bool MatchesWardSelection(IEnumerable<string> selection, string hhs, string facility, string ward) =>
        selection.Any(selectedValue =>
        {
            var parsed = ParseWardValue(selectedValue);
            return Matches(parsed.Ward, ward)
                && (string.IsNullOrWhiteSpace(parsed.Facility) || Matches(parsed.Facility, facility))
                && (string.IsNullOrWhiteSpace(parsed.Hhs) || Matches(parsed.Hhs, hhs));
        });

    private static string BuildWardValue(string hhs, string facility, string ward) =>
        string.Join(WardValueSeparator, hhs, facility, ward);

    private static (string? Hhs, string? Facility, string Ward) ParseWardValue(string value)
    {
        var parts = value.Split(new[] { WardValueSeparator }, StringSplitOptions.None);
        return parts.Length == 3
            ? (parts[0], parts[1], parts[2])
            : (null, null, value);
    }

    private static bool Matches(string? left, string? right) =>
        string.Equals(NormalizeComparable(left), NormalizeComparable(right), StringComparison.OrdinalIgnoreCase);

    private static string NormalizeComparable(string? value) =>
        (value ?? string.Empty).Trim().Replace('’', '\'');

    private sealed record ResolvedWardParent(string WardId, string Hhs, string Facility, string Ward);
}
