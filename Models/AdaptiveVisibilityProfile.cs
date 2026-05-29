namespace QHPFH_ConceptPrototype.Models;

public sealed record AdaptiveVisibilityProfile(
    bool ShowHhsFilter,
    bool ShowFacilityFilter,
    bool ShowWardFilter,
    bool ShowOperationalActions,
    bool ShowWorkflowPanels,
    bool ShowExecutiveInsights,
    bool ShowAllocationControls,
    bool ShowBedQuickActions,
    bool ShowOperationalTimeline);
