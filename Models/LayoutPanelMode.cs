namespace QHPFH_ConceptPrototype.Models;

public enum LayoutPanelMode
{
    Stacked,
    Swappable,
    CompactOperational
}

public static class LayoutPanelModeExtensions
{
    public static string ToDisplayName(this LayoutPanelMode panelMode) => panelMode switch
    {
        LayoutPanelMode.Stacked => "Stacked Layout",
        LayoutPanelMode.Swappable => "Swappable Panels",
        LayoutPanelMode.CompactOperational => "Compact Operational",
        _ => panelMode.ToString()
    };
}
