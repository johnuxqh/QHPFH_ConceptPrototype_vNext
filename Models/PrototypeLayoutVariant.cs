namespace QHPFH_ConceptPrototype.Models;

public enum PrototypeLayoutVariant
{
    VariantAStackedPanels,
    VariantBSwappablePanels,
    VariantCCompactOperational
}

public static class PrototypeLayoutVariantExtensions
{
    public static string ToDisplayName(this PrototypeLayoutVariant variant) => variant switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Variant A — Stacked Panels",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Variant B — Swappable Panels",
        PrototypeLayoutVariant.VariantCCompactOperational => "Variant C — Compact Operational",
        _ => variant.ToString()
    };

    public static string ToSummaryText(this PrototypeLayoutVariant variant) => variant switch
    {
        PrototypeLayoutVariant.VariantAStackedPanels => "Variant A",
        PrototypeLayoutVariant.VariantBSwappablePanels => "Variant B",
        PrototypeLayoutVariant.VariantCCompactOperational => "Variant C",
        _ => variant.ToString()
    };
}
