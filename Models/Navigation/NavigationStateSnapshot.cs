namespace QHPFH_ConceptPrototype.Models.Navigation;

public sealed record NavigationStateSnapshot(
    string CurrentRoute,
    string CurrentWorkspace,
    string CurrentPrimaryNavId,
    string CurrentSecondaryNavId,
    NavigationShellMode CurrentShellMode,
    bool IsPrimaryNavCollapsed,
    bool IsSecondaryNavCollapsed,
    string CurrentNavLabel,
    string CurrentWorkspaceLabel,
    string CurrentSectionLabel);
