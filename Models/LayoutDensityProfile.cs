namespace QHPFH_ConceptPrototype.Models;

public sealed record LayoutDensityProfile(
    string LayoutDensity,
    string CardSpacingMode,
    string PanelSpacingMode,
    string TableDensityMode,
    bool ReduceChrome,
    bool PrioritizeWorkspaceArea);
