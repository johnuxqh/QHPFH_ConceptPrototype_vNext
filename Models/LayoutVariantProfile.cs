namespace QHPFH_ConceptPrototype.Models;

public sealed record LayoutVariantProfile(
    PrototypeLayoutVariant Variant,
    string Label,
    string Summary,
    LayoutPanelMode PanelMode,
    LayoutDensityProfile DensityProfile,
    string LayoutVariantClass,
    string PanelVariantClass,
    string DensityClass);
