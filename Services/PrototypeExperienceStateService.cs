using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services;

public sealed class PrototypeExperienceStateService
{
    private readonly PrototypeDataStore _dataStore;

    private static readonly IReadOnlyList<PrototypeAccessViewOption> DefaultAccessViewOptions =
    [
        new("Statewide Operations", "USP-EXEC-001"),
        new("HHS Executive", "USP-HHS-001"),
        new("Facility Operations", "USP-BED-001"),
        new("Ward Clinical", "USP-WARD-001")
    ];

    public PrototypeExperienceStateService(PrototypeDataStore dataStore)
    {
        _dataStore = dataStore;
        var defaultAccessView = ResolveAccessViewOption(_dataStore.GetCurrentPerspective()?.Id)
            ?? DefaultAccessViewOptions[0];

        Current = new PrototypeExperienceState(
            defaultAccessView.Label,
            defaultAccessView.UserPerspectiveId,
            PrototypeExperienceMode.V2CoordinatedOperations,
            PrototypeLayoutVariant.VariantAStackedPanels);
    }

    public event Action? OnChange;

    public PrototypeExperienceState Current { get; private set; }

    public IReadOnlyList<PrototypeAccessViewOption> AccessViewOptions => DefaultAccessViewOptions;

    public IReadOnlyList<PrototypeExperienceMode> ExperienceModeOptions { get; } =
    [
        PrototypeExperienceMode.V1AwarenessInsights,
        PrototypeExperienceMode.V2CoordinatedOperations,
        PrototypeExperienceMode.V3OperationalWorkflow
    ];

    public IReadOnlyList<PrototypeLayoutVariant> LayoutVariantOptions { get; } =
    [
        PrototypeLayoutVariant.VariantAStackedPanels,
        PrototypeLayoutVariant.VariantBSwappablePanels,
        PrototypeLayoutVariant.VariantCCompactOperational
    ];

    public void SetAccessView(string userPerspectiveId)
    {
        var accessView = ResolveAccessViewOption(userPerspectiveId);
        if (accessView is null || Current.UserPerspectiveId == accessView.UserPerspectiveId)
        {
            return;
        }

        _dataStore.SetCurrentPerspective(accessView.UserPerspectiveId);
        Current = Current with
        {
            AccessViewLabel = accessView.Label,
            UserPerspectiveId = accessView.UserPerspectiveId
        };

        NotifyStateChanged();
    }

    public void SetExperienceMode(PrototypeExperienceMode experienceMode)
    {
        if (Current.ExperienceMode == experienceMode)
        {
            return;
        }

        Current = Current with { ExperienceMode = experienceMode };
        NotifyStateChanged();
    }

    public void SetLayoutVariant(PrototypeLayoutVariant layoutVariant)
    {
        if (Current.LayoutVariant == layoutVariant)
        {
            return;
        }

        Current = Current with { LayoutVariant = layoutVariant };
        NotifyStateChanged();
    }

    private PrototypeAccessViewOption? ResolveAccessViewOption(string? userPerspectiveId)
    {
        if (string.IsNullOrWhiteSpace(userPerspectiveId))
        {
            return null;
        }

        var knownOption = DefaultAccessViewOptions.FirstOrDefault(x => x.UserPerspectiveId == userPerspectiveId);
        if (knownOption is null)
        {
            return null;
        }

        return _dataStore.GetUserPerspectives().Any(x => x.Id == knownOption.UserPerspectiveId)
            ? knownOption
            : null;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
