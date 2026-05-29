using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Experience;
using QHPFH_ConceptPrototype.Services.Layout;

namespace QHPFH_ConceptPrototype.Services.Workspace;

public sealed class WorkspaceDensityEngine : IDisposable
{
    private readonly PrototypeExperienceStateService _experienceState;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly ExperienceModeEngine _experienceMode;
    private readonly LayoutVariantEngine _layoutVariant;

    public WorkspaceDensityEngine(
        PrototypeExperienceStateService experienceState,
        AdaptivePerspectiveEngine adaptivePerspective,
        ExperienceModeEngine experienceMode,
        LayoutVariantEngine layoutVariant)
    {
        _experienceState = experienceState;
        _adaptivePerspective = adaptivePerspective;
        _experienceMode = experienceMode;
        _layoutVariant = layoutVariant;
        _experienceState.OnChange += NotifyDensityChanged;
    }

    public event Action? OnChange;

    public WorkspaceDensityMode GetCurrentDensityMode()
    {
        var perspective = _adaptivePerspective.GetCurrentPerspective();

        if (_layoutVariant.IsCompactOperational())
        {
            return WorkspaceDensityMode.Compact;
        }

        if (_experienceMode.IsOperationalWorkflowMode())
        {
            return WorkspaceDensityMode.Compact;
        }

        if (perspective?.PerspectiveType == UserPerspectiveType.Executive && _experienceMode.IsAwarenessMode())
        {
            return WorkspaceDensityMode.Comfortable;
        }

        if (_experienceMode.IsAwarenessMode())
        {
            return WorkspaceDensityMode.Comfortable;
        }

        if (perspective?.PerspectiveType is UserPerspectiveType.BedManager or UserPerspectiveType.OperationsCentre
            && !_experienceMode.IsAwarenessMode())
        {
            return WorkspaceDensityMode.Balanced;
        }

        if (perspective?.PerspectiveType is UserPerspectiveType.WardClinician or UserPerspectiveType.NUM
            && _experienceMode.IsOperationalWorkflowMode())
        {
            return WorkspaceDensityMode.Compact;
        }

        return WorkspaceDensityMode.Balanced;
    }

    public WorkspaceDensityProfile GetDensityProfile()
    {
        var mode = GetCurrentDensityMode();
        return new WorkspaceDensityProfile(
            mode,
            GetDensityLabel(mode),
            GetDensitySummary(mode),
            GetWorkspaceDensityClass(mode),
            GetCardDensityClass(mode),
            GetTableDensityClass(mode),
            GetPanelDensityClass(mode),
            GetSpacingRecommendation(mode),
            ShouldUseCompactCards(),
            ShouldUseCompactTables(),
            ShouldReduceChrome(),
            ShouldIncreaseOperationalDensity());
    }

    public string GetWorkspaceDensityClass() => GetWorkspaceDensityClass(GetCurrentDensityMode());

    public string GetCardDensityClass() => GetCardDensityClass(GetCurrentDensityMode());

    public string GetTableDensityClass() => GetTableDensityClass(GetCurrentDensityMode());

    public string GetPanelDensityClass() => GetPanelDensityClass(GetCurrentDensityMode());

    public bool ShouldUseCompactCards() => GetCurrentDensityMode() == WorkspaceDensityMode.Compact;

    public bool ShouldUseCompactTables() => GetCurrentDensityMode() == WorkspaceDensityMode.Compact;

    public bool ShouldReduceChrome() => GetCurrentDensityMode() == WorkspaceDensityMode.Compact || _layoutVariant.ShouldReduceChrome();

    public bool ShouldIncreaseOperationalDensity() =>
        GetCurrentDensityMode() == WorkspaceDensityMode.Compact
        || _experienceMode.IsOperationalWorkflowMode()
        || _adaptivePerspective.GetDensityMode() is AdaptiveDensityMode.Dense or AdaptiveDensityMode.Command;

    private static string GetWorkspaceDensityClass(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "workspace-density-comfortable",
        WorkspaceDensityMode.Compact => "workspace-density-compact",
        _ => "workspace-density-balanced"
    };

    private static string GetCardDensityClass(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "workspace-card-density-comfortable",
        WorkspaceDensityMode.Compact => "workspace-card-density-compact",
        _ => "workspace-card-density-balanced"
    };

    private static string GetTableDensityClass(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "workspace-table-density-comfortable",
        WorkspaceDensityMode.Compact => "workspace-table-density-compact",
        _ => "workspace-table-density-balanced"
    };

    private static string GetPanelDensityClass(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "workspace-panel-density-comfortable",
        WorkspaceDensityMode.Compact => "workspace-panel-density-compact",
        _ => "workspace-panel-density-balanced"
    };

    private static string GetDensityLabel(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "Comfortable density",
        WorkspaceDensityMode.Compact => "Compact density",
        _ => "Balanced density"
    };

    private static string GetDensitySummary(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "Awareness-friendly spacing with lower visible control density",
        WorkspaceDensityMode.Compact => "Command-centre spacing with more operational context visible",
        _ => "Balanced operational review spacing for workshop readability"
    };

    private static string GetSpacingRecommendation(WorkspaceDensityMode mode) => mode switch
    {
        WorkspaceDensityMode.Comfortable => "Wider workspace gaps and softer header spacing",
        WorkspaceDensityMode.Compact => "Reduced chrome, tighter panel gaps, and compact chip spacing",
        _ => "Moderate workspace gaps and balanced panel spacing"
    };

    private void NotifyDensityChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _experienceState.OnChange -= NotifyDensityChanged;
    }
}
