using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Services.Adaptive;

namespace QHPFH_ConceptPrototype.Services.Experience;

public sealed class ExperienceModeEngine : IDisposable
{
    private readonly PrototypeExperienceStateService _experienceState;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;

    public ExperienceModeEngine(
        PrototypeExperienceStateService experienceState,
        AdaptivePerspectiveEngine adaptivePerspective)
    {
        _experienceState = experienceState;
        _adaptivePerspective = adaptivePerspective;
        _experienceState.OnChange += NotifyStateChanged;
    }

    public event Action? OnChange;

    public PrototypeExperienceMode GetCurrentMode() => _experienceState.Current.ExperienceMode;

    public bool IsAwarenessMode() => GetCurrentMode() == PrototypeExperienceMode.V1AwarenessInsights;

    public bool IsCoordinatedOperationsMode() => GetCurrentMode() == PrototypeExperienceMode.V2CoordinatedOperations;

    public bool IsOperationalWorkflowMode() => GetCurrentMode() == PrototypeExperienceMode.V3OperationalWorkflow;

    public ExperienceDensityMode GetDensityMode() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => ExperienceDensityMode.Light,
        PrototypeExperienceMode.V2CoordinatedOperations => ExperienceDensityMode.Balanced,
        PrototypeExperienceMode.V3OperationalWorkflow => ExperienceDensityMode.Dense,
        _ => ExperienceDensityMode.Balanced
    };

    public string GetCardDensity() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "Summary cards",
        PrototypeExperienceMode.V2CoordinatedOperations => "Balanced cards",
        PrototypeExperienceMode.V3OperationalWorkflow => "Dense operational cards",
        _ => "Balanced cards"
    };

    public string GetOperationalDensity() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "Reduced operational density",
        PrototypeExperienceMode.V2CoordinatedOperations => "Moderate operational density",
        PrototypeExperienceMode.V3OperationalWorkflow => "High operational density",
        _ => "Moderate operational density"
    };

    public bool ShouldPrioritizeInsights() => IsAwarenessMode() || IsCoordinatedOperationsMode();

    public bool ShouldPrioritizeOperationalActions() => IsOperationalWorkflowMode();

    public bool ShouldShowAdvancedWorkflowControls() => IsOperationalWorkflowMode();

    public bool ShouldShowOperationalQuickActions() => !IsAwarenessMode() && _adaptivePerspective.ShouldShowOperationalActions();

    public bool ShouldShowWorkflowPanels() => !IsAwarenessMode() && _adaptivePerspective.ShouldShowWorkflowPanels();

    public bool ShouldShowOperationalEscalations() => !IsAwarenessMode() && _adaptivePerspective.CanManageOperationalEvents();

    public bool ShouldUseSimplifiedInteractions() => IsAwarenessMode();

    public bool ShouldShowAdvancedControls() => IsOperationalWorkflowMode();

    public bool ShouldEnableOperationalShortcuts() => IsOperationalWorkflowMode() && _adaptivePerspective.CanEditOperationalState();

    public bool ShouldShowOrchestrationPanels() => !IsAwarenessMode() && _adaptivePerspective.CanEditOperationalState();

    public bool ShouldShowAllocationCoordination() => !IsAwarenessMode() && _adaptivePerspective.ShouldShowAllocationControls();

    public bool ShouldShowOperationalTimelines() => !IsAwarenessMode() && _adaptivePerspective.ShouldShowOperationalTimeline();

    public string GetPreferredSlideoutDensity() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "Summary slideout",
        PrototypeExperienceMode.V2CoordinatedOperations => "Coordinated slideout",
        PrototypeExperienceMode.V3OperationalWorkflow => "Operational workflow slideout",
        _ => "Coordinated slideout"
    };

    public string GetPreferredPanelMode() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "Insight panels",
        PrototypeExperienceMode.V2CoordinatedOperations => _adaptivePerspective.GetRecommendedPanelMode(),
        PrototypeExperienceMode.V3OperationalWorkflow => "Workflow command panels",
        _ => _adaptivePerspective.GetRecommendedPanelMode()
    };

    public ExperienceInteractionMode GetInteractionMode() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => ExperienceInteractionMode.Simplified,
        PrototypeExperienceMode.V2CoordinatedOperations => ExperienceInteractionMode.Coordinated,
        PrototypeExperienceMode.V3OperationalWorkflow => ExperienceInteractionMode.Operational,
        _ => ExperienceInteractionMode.Coordinated
    };

    public ExperienceInformationMode GetInformationMode() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => ExperienceInformationMode.InsightFirst,
        PrototypeExperienceMode.V2CoordinatedOperations => ExperienceInformationMode.Balanced,
        PrototypeExperienceMode.V3OperationalWorkflow => ExperienceInformationMode.WorkflowFirst,
        _ => ExperienceInformationMode.Balanced
    };

    public string GetModeBadgeLabel() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "Awareness Focus",
        PrototypeExperienceMode.V2CoordinatedOperations => "Balanced Coordination",
        PrototypeExperienceMode.V3OperationalWorkflow => "Operational Workflow Mode",
        _ => "Balanced Coordination"
    };

    public string GetModeSummary() => GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "KPI-first awareness with simplified controls",
        PrototypeExperienceMode.V2CoordinatedOperations => "Balanced insight and coordinated operational workflow",
        PrototypeExperienceMode.V3OperationalWorkflow => "Workflow-first command-centre operational tooling",
        _ => "Balanced insight and coordinated operational workflow"
    };

    public ExperienceModeProfile GetModeProfile() => new(
        GetCurrentMode(),
        GetModeBadgeLabel(),
        GetModeSummary(),
        GetDensityMode(),
        GetInteractionMode(),
        GetInformationMode(),
        GetCardDensity(),
        GetOperationalDensity(),
        GetPreferredSlideoutDensity(),
        GetPreferredPanelMode());

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _experienceState.OnChange -= NotifyStateChanged;
    }
}
