namespace QHPFH_ConceptPrototype.Models;

public sealed record WorkspaceDensityProfile(
    WorkspaceDensityMode Mode,
    string Label,
    string Summary,
    string WorkspaceDensityClass,
    string CardDensityClass,
    string TableDensityClass,
    string PanelDensityClass,
    string SpacingRecommendation,
    bool UseCompactCards,
    bool UseCompactTables,
    bool ReduceChrome,
    bool IncreaseOperationalDensity);
