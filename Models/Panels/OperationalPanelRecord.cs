namespace QHPFH_ConceptPrototype.Models.Panels;

public sealed record OperationalPanelRecord(
    string Id,
    string Title,
    string Subtitle,
    OperationalPanelType PanelType,
    OperationalPanelPriority Priority,
    OperationalPanelDensity Density,
    string StatusBadge,
    string Context,
    bool IsCollapsible = false,
    bool IsInitiallyExpanded = true);
