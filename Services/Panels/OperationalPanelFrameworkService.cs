using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Panels;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Experience;
using QHPFH_ConceptPrototype.Services.Insights;
using QHPFH_ConceptPrototype.Services.Kpi;
using QHPFH_ConceptPrototype.Services.Layout;
using QHPFH_ConceptPrototype.Services.Operational;
using QHPFH_ConceptPrototype.Services.Workspace;

namespace QHPFH_ConceptPrototype.Services.Panels;

public sealed class OperationalPanelFrameworkService
{
    private readonly WorkspaceDensityEngine _workspaceDensity;
    private readonly LayoutVariantEngine _layoutVariant;
    private readonly ExperienceModeEngine _experienceMode;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly KpiFrameworkService _kpiFramework;
    private readonly InsightFrameworkService _insightFramework;
    private readonly OperationalAwarenessService _operationalAwareness;

    public OperationalPanelFrameworkService(
        WorkspaceDensityEngine workspaceDensity,
        LayoutVariantEngine layoutVariant,
        ExperienceModeEngine experienceMode,
        AdaptivePerspectiveEngine adaptivePerspective,
        ContextAwarenessService contextAwareness,
        KpiFrameworkService kpiFramework,
        InsightFrameworkService insightFramework,
        OperationalAwarenessService operationalAwareness)
    {
        _workspaceDensity = workspaceDensity;
        _layoutVariant = layoutVariant;
        _experienceMode = experienceMode;
        _adaptivePerspective = adaptivePerspective;
        _contextAwareness = contextAwareness;
        _kpiFramework = kpiFramework;
        _insightFramework = insightFramework;
        _operationalAwareness = operationalAwareness;
    }

    public OperationalPanelDensity GetCurrentDensity() => _workspaceDensity.GetCurrentDensityMode() switch
    {
        WorkspaceDensityMode.Comfortable => OperationalPanelDensity.Comfortable,
        WorkspaceDensityMode.Compact => OperationalPanelDensity.Compact,
        _ => OperationalPanelDensity.Balanced
    };

    public OperationalPanelRecord CreatePanelRecord(
        string id,
        string title,
        string subtitle,
        OperationalPanelType panelType,
        OperationalPanelPriority priority = OperationalPanelPriority.Standard,
        string? statusBadge = null,
        string? context = null,
        bool isCollapsible = false,
        bool isInitiallyExpanded = true)
    {
        return new OperationalPanelRecord(
            id,
            title,
            subtitle,
            panelType,
            priority,
            GetCurrentDensity(),
            statusBadge ?? GetDefaultStatusBadge(priority),
            context ?? GetCurrentContextLabel(),
            isCollapsible,
            isInitiallyExpanded);
    }

    public string GetPanelClass(OperationalPanelType panelType, OperationalPanelPriority priority) => string.Join(
        " ",
        new[]
        {
            "operational-panel",
            GetPanelTypeClass(panelType),
            GetPanelPriorityClass(priority),
            GetPanelDensityClass(),
            GetLayoutVariantClass(),
            GetExperienceClass(),
            ShouldPrioritizeOperationalActions() ? "operational-panel--actions-prioritized" : null,
            ShouldReduceChrome() ? "operational-panel--reduced-chrome" : null
        }.Where(x => !string.IsNullOrWhiteSpace(x)));

    public string GetGridClass() => string.Join(
        " ",
        new[]
        {
            "operational-panel-grid",
            GetPanelDensityClass(),
            GetLayoutVariantClass()
        });

    public string GetPanelDensityClass() => GetCurrentDensity() switch
    {
        OperationalPanelDensity.Comfortable => "operational-panel--comfortable",
        OperationalPanelDensity.Compact => "operational-panel--compact",
        _ => "operational-panel--balanced"
    };

    public string GetPanelTypeClass(OperationalPanelType panelType) => $"operational-panel--{ToKebabCase(panelType.ToString())}";

    public string GetPanelPriorityClass(OperationalPanelPriority priority) => $"operational-panel-priority--{priority.ToString().ToLowerInvariant()}";

    public string GetLayoutVariantClass() => _layoutVariant.GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "operational-panel-layout--stacked",
        PrototypeLayoutVariant.VariantBSwappablePanels => "operational-panel-layout--swappable",
        PrototypeLayoutVariant.VariantCCompactOperational => "operational-panel-layout--compact",
        _ => "operational-panel-layout--stacked"
    };

    public string GetExperienceClass() => _experienceMode.GetCurrentMode() switch
    {
        PrototypeExperienceMode.V1AwarenessInsights => "operational-panel-experience--awareness",
        PrototypeExperienceMode.V2CoordinatedOperations => "operational-panel-experience--coordination",
        PrototypeExperienceMode.V3OperationalWorkflow => "operational-panel-experience--workflow",
        _ => "operational-panel-experience--coordination"
    };

    public bool ShouldUseCompactPanels() => GetCurrentDensity() == OperationalPanelDensity.Compact || _layoutVariant.ShouldUseCompactPanels();

    public bool ShouldReduceChrome() => _workspaceDensity.ShouldReduceChrome() || _layoutVariant.ShouldReduceChrome();

    public bool ShouldPrioritizeOperationalActions() =>
        _experienceMode.ShouldPrioritizeOperationalActions()
        || _adaptivePerspective.ShouldShowOperationalActions()
        || _insightFramework.ShouldPrioritizeActions()
        || _kpiFramework.ShouldPrioritizeActionLabels();

    public bool ShouldSurfaceAwarenessMessaging() =>
        _experienceMode.ShouldPrioritizeInsights()
        || _adaptivePerspective.ShouldShowExecutiveInsights()
        || _operationalAwareness.HasCriticalOperationalBanner();

    private string GetCurrentContextLabel()
    {
        var context = _contextAwareness.GetCurrentContext();
        return string.IsNullOrWhiteSpace(context.ContextSummary)
            ? "Shared operational context"
            : context.ContextSummary;
    }

    private static string GetDefaultStatusBadge(OperationalPanelPriority priority) => priority switch
    {
        OperationalPanelPriority.Critical => "Critical",
        OperationalPanelPriority.High => "High priority",
        OperationalPanelPriority.Attention => "Attention",
        OperationalPanelPriority.Watch => "Watch",
        _ => "Active"
    };

    private static string ToKebabCase(string value) => string.Concat(value.Select((character, index) =>
        index > 0 && char.IsUpper(character)
            ? $"-{char.ToLowerInvariant(character)}"
            : char.ToLowerInvariant(character).ToString()));
}
