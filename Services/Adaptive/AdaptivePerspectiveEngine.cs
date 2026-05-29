using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services.Adaptive;

public sealed class AdaptivePerspectiveEngine : IDisposable
{
    private readonly PrototypeExperienceStateService _experienceState;
    private readonly PrototypeDataStore _dataStore;

    public AdaptivePerspectiveEngine(
        PrototypeExperienceStateService experienceState,
        PrototypeDataStore dataStore)
    {
        _experienceState = experienceState;
        _dataStore = dataStore;
        _experienceState.OnChange += NotifyStateChanged;
    }

    public event Action? OnChange;

    public UserPerspectiveRecord? GetCurrentPerspective()
    {
        var currentPerspectiveId = _experienceState.Current.UserPerspectiveId;
        return _dataStore.GetUserPerspectives().FirstOrDefault(x => x.Id == currentPerspectiveId)
            ?? _dataStore.GetCurrentPerspective();
    }

    public PrototypeExperienceMode GetCurrentExperienceMode() => _experienceState.Current.ExperienceMode;

    public PrototypeLayoutVariant GetCurrentLayoutVariant() => _experienceState.Current.LayoutVariant;

    public bool IsAwarenessMode() => GetCurrentExperienceMode() == PrototypeExperienceMode.V1AwarenessInsights;

    public bool IsHybridOperationalMode() => GetCurrentExperienceMode() == PrototypeExperienceMode.V2CoordinatedOperations;

    public bool IsWorkflowMode() => GetCurrentExperienceMode() == PrototypeExperienceMode.V3OperationalWorkflow;

    public bool CanManageBeds() => GetCurrentPerspective()?.CanManageBeds == true && !IsAwarenessMode();

    public bool CanManageAllocations() => GetCurrentPerspective()?.CanManageAllocations == true && !IsAwarenessMode();

    public bool CanManageOperationalEvents() => GetCurrentPerspective()?.CanManageOperationalEvents == true && !IsAwarenessMode();

    public bool CanViewExecutiveInsights() =>
        GetCurrentPerspective()?.CanAccessExecutiveViews == true
        || GetCurrentPerspective()?.PerspectiveType == UserPerspectiveType.Executive;

    public bool CanViewReports() => GetCurrentPerspective()?.CanViewReports == true;

    public bool CanViewPatientDetails()
    {
        var perspective = GetCurrentPerspective();
        return perspective is not null
            && perspective.PerspectiveType != UserPerspectiveType.Executive
            && (perspective.CanViewWard || perspective.CanManageBeds || perspective.CanManageAllocations);
    }

    public bool CanEditOperationalState()
    {
        var perspective = GetCurrentPerspective();
        return !IsAwarenessMode()
            && perspective is not null
            && (perspective.CanManageBeds
                || perspective.CanManageAllocations
                || perspective.CanManageOperationalEvents);
    }

    public bool ShouldShowHhsFilter() => GetCurrentPerspective()?.AccessScope == UserAccessScope.Statewide;

    public bool ShouldShowFacilityFilter()
    {
        var scope = GetCurrentPerspective()?.AccessScope;
        return scope is UserAccessScope.Statewide or UserAccessScope.HHS or UserAccessScope.Facility;
    }

    public bool ShouldShowWardFilter()
    {
        var scope = GetCurrentPerspective()?.AccessScope;
        return scope is UserAccessScope.Statewide or UserAccessScope.HHS or UserAccessScope.Facility or UserAccessScope.Ward;
    }

    public bool ShouldShowOperationalActions() => CanEditOperationalState() || IsWorkflowMode();

    public bool ShouldShowWorkflowPanels() => !IsAwarenessMode() || GetCurrentPerspective()?.WorkflowFocus != UserWorkflowFocus.Awareness;

    public bool ShouldShowExecutiveInsights() => CanViewExecutiveInsights() || IsAwarenessMode();

    public bool ShouldShowAllocationControls() => CanManageAllocations() || GetCurrentPerspective()?.PerspectiveType == UserPerspectiveType.AllocationCoordinator;

    public bool ShouldShowBedQuickActions() => CanManageBeds() || GetCurrentPerspective()?.PerspectiveType is UserPerspectiveType.BedManager or UserPerspectiveType.NUM or UserPerspectiveType.WardClinician;

    public bool ShouldShowOperationalTimeline() => !IsAwarenessMode() && GetCurrentPerspective()?.CanManageOperationalEvents == true;

    public AdaptiveDensityMode GetDensityMode()
    {
        if (IsAwarenessMode())
        {
            return AdaptiveDensityMode.Comfort;
        }

        var perspective = GetCurrentPerspective();
        if (perspective?.OperationalMode == UserOperationalMode.OperationalCommand || IsWorkflowMode())
        {
            return AdaptiveDensityMode.Command;
        }

        return perspective?.PreferredDensityMode switch
        {
            "Comfort" => AdaptiveDensityMode.Comfort,
            "Dense" => AdaptiveDensityMode.Dense,
            _ => AdaptiveDensityMode.Balanced
        };
    }

    public AdaptiveWorkflowEmphasis GetWorkflowEmphasis()
    {
        return GetCurrentPerspective()?.PerspectiveType switch
        {
            UserPerspectiveType.Executive => AdaptiveWorkflowEmphasis.ExecutiveInsights,
            UserPerspectiveType.BedManager or UserPerspectiveType.OperationsCentre => AdaptiveWorkflowEmphasis.BedOrchestration,
            UserPerspectiveType.WardClinician or UserPerspectiveType.NUM => AdaptiveWorkflowEmphasis.WardWorkflow,
            UserPerspectiveType.AllocationCoordinator => AdaptiveWorkflowEmphasis.AllocationFlow,
            UserPerspectiveType.DelayedDischargeCoordinator => AdaptiveWorkflowEmphasis.DischargeBarriers,
            UserPerspectiveType.ReadOnlyAnalyst => AdaptiveWorkflowEmphasis.Reporting,
            UserPerspectiveType.HHSCoordinator or UserPerspectiveType.FacilityCoordinator => AdaptiveWorkflowEmphasis.Coordination,
            _ => AdaptiveWorkflowEmphasis.Awareness
        };
    }

    public string GetRecommendedPanelMode() => GetCurrentLayoutVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Stacked panels",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Swappable panels",
        PrototypeLayoutVariant.VariantCCompactOperational => "Compact operational panels",
        _ => "Standard panels"
    };

    public string GetRecommendedDefaultTab() => GetWorkflowEmphasis() switch
    {
        AdaptiveWorkflowEmphasis.ExecutiveInsights => "Insights",
        AdaptiveWorkflowEmphasis.BedOrchestration => "Bed state",
        AdaptiveWorkflowEmphasis.WardWorkflow => "Patient tasks",
        AdaptiveWorkflowEmphasis.AllocationFlow => "Incoming patients",
        AdaptiveWorkflowEmphasis.DischargeBarriers => "Barriers",
        AdaptiveWorkflowEmphasis.Reporting => "Reports",
        _ => "Overview"
    };

    public string GetRecommendedOperationalFocus() => GetWorkflowEmphasis() switch
    {
        AdaptiveWorkflowEmphasis.ExecutiveInsights => "Statewide awareness, trend visibility, and exception monitoring",
        AdaptiveWorkflowEmphasis.BedOrchestration => "Bed readiness, allocation pressure, and operational actions",
        AdaptiveWorkflowEmphasis.WardWorkflow => "Patient movement, ward tasks, and clinical workflow readiness",
        AdaptiveWorkflowEmphasis.AllocationFlow => "Incoming demand, pre-allocation, and transfer coordination",
        AdaptiveWorkflowEmphasis.DischargeBarriers => "Discharge barriers, escalation tasks, and flow constraints",
        AdaptiveWorkflowEmphasis.Reporting => "Reporting, review, and non-editing operational awareness",
        AdaptiveWorkflowEmphasis.Coordination => "Cross-area coordination, facility pressure, and escalations",
        _ => "Operational awareness and shared situational truth"
    };

    public AdaptiveVisibilityProfile GetVisibilityProfile() => new(
        ShouldShowHhsFilter(),
        ShouldShowFacilityFilter(),
        ShouldShowWardFilter(),
        ShouldShowOperationalActions(),
        ShouldShowWorkflowPanels(),
        ShouldShowExecutiveInsights(),
        ShouldShowAllocationControls(),
        ShouldShowBedQuickActions(),
        ShouldShowOperationalTimeline());

    public AdaptiveOperationalProfile GetOperationalProfile() => new(
        GetDensityMode(),
        GetWorkflowEmphasis(),
        GetRecommendedPanelMode(),
        GetRecommendedDefaultTab(),
        GetRecommendedOperationalFocus());

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _experienceState.OnChange -= NotifyStateChanged;
    }
}
