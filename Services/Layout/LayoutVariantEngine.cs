using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Experience;

namespace QHPFH_ConceptPrototype.Services.Layout;

public sealed class LayoutVariantEngine : IDisposable
{
    private readonly PrototypeExperienceStateService _experienceState;
    private readonly ExperienceModeEngine _experienceMode;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;

    public LayoutVariantEngine(
        PrototypeExperienceStateService experienceState,
        ExperienceModeEngine experienceMode,
        AdaptivePerspectiveEngine adaptivePerspective)
    {
        _experienceState = experienceState;
        _experienceMode = experienceMode;
        _adaptivePerspective = adaptivePerspective;
        _experienceState.OnChange += NotifyStateChanged;
    }

    public event Action? OnChange;

    public PrototypeLayoutVariant GetCurrentVariant() => _experienceState.Current.LayoutVariant;

    public bool IsStackedPanels() => GetCurrentVariant() == PrototypeLayoutVariant.VariantAStackedPanels;

    public bool IsSwappablePanels() => GetCurrentVariant() == PrototypeLayoutVariant.VariantBSwappablePanels;

    public bool IsCompactOperational() => GetCurrentVariant() == PrototypeLayoutVariant.VariantCCompactOperational;

    public LayoutPanelMode GetPanelMode() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => LayoutPanelMode.Stacked,
        PrototypeLayoutVariant.VariantBSwappablePanels => LayoutPanelMode.Swappable,
        PrototypeLayoutVariant.VariantCCompactOperational => LayoutPanelMode.CompactOperational,
        _ => LayoutPanelMode.Stacked
    };

    public bool ShouldStackPanels() => IsStackedPanels();

    public bool ShouldSwapPanels() => IsSwappablePanels();

    public bool ShouldOverlayInsights() => IsSwappablePanels() && _experienceMode.ShouldPrioritizeInsights();

    public bool ShouldUseCompactPanels() => IsCompactOperational();

    public bool ShouldCollapseSecondaryPanels() => IsCompactOperational() || (IsSwappablePanels() && _experienceMode.IsOperationalWorkflowMode());

    public LayoutDensityProfile GetLayoutDensity() => new(
        IsCompactOperational() ? "Compact operational density" : IsSwappablePanels() ? "Space-efficient density" : "Persistent panel density",
        GetCardSpacingMode(),
        GetPanelSpacingMode(),
        GetTableDensityMode(),
        ShouldReduceChrome(),
        ShouldPrioritizeWorkspaceArea());

    public string GetCardSpacingMode() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Comfort card spacing",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Balanced card spacing",
        PrototypeLayoutVariant.VariantCCompactOperational => "Compact card spacing",
        _ => "Balanced card spacing"
    };

    public string GetPanelSpacingMode() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Stacked panel spacing",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Swappable panel spacing",
        PrototypeLayoutVariant.VariantCCompactOperational => "Tight panel spacing",
        _ => "Stacked panel spacing"
    };

    public string GetTableDensityMode() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Standard rows",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Balanced rows",
        PrototypeLayoutVariant.VariantCCompactOperational => "Dense rows",
        _ => "Balanced rows"
    };

    public bool ShouldShowPanelToggle() => IsSwappablePanels();

    public bool ShouldPersistInsights() => IsStackedPanels() || (_experienceMode.IsAwarenessMode() && !IsCompactOperational());

    public bool ShouldPrioritizeWorkspaceArea() => IsCompactOperational() || _experienceMode.IsOperationalWorkflowMode();

    public bool ShouldReduceChrome() => IsCompactOperational() || _adaptivePerspective.GetDensityMode() == AdaptiveDensityMode.Command;

    public string GetLayoutVariantClass() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "layout-variant-stacked",
        PrototypeLayoutVariant.VariantBSwappablePanels => "layout-variant-swappable",
        PrototypeLayoutVariant.VariantCCompactOperational => "layout-variant-compact",
        _ => "layout-variant-stacked"
    };

    public string GetPanelVariantClass() => GetPanelMode() switch
    {
        LayoutPanelMode.Stacked => "panel-variant-stacked",
        LayoutPanelMode.Swappable => "panel-variant-swappable",
        LayoutPanelMode.CompactOperational => "panel-variant-compact",
        _ => "panel-variant-stacked"
    };

    public string GetDensityClass() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "layout-density-persistent",
        PrototypeLayoutVariant.VariantBSwappablePanels => "layout-density-balanced",
        PrototypeLayoutVariant.VariantCCompactOperational => "layout-density-compact",
        _ => "layout-density-balanced"
    };

    public string GetVariantLabel() => GetPanelMode().ToDisplayName();

    public string GetVariantSummary() => GetCurrentVariant() switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Persistent panels for safe workshop review",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Panels can swap for progressive disclosure",
        PrototypeLayoutVariant.VariantCCompactOperational => "Compact command-board treatment for scanning",
        _ => "Persistent panels for safe workshop review"
    };

    public LayoutVariantProfile GetVariantProfile() => new(
        GetCurrentVariant(),
        GetVariantLabel(),
        GetVariantSummary(),
        GetPanelMode(),
        GetLayoutDensity(),
        GetLayoutVariantClass(),
        GetPanelVariantClass(),
        GetDensityClass());

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _experienceState.OnChange -= NotifyStateChanged;
    }
}
