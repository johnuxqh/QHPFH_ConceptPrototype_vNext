namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterPresetRecord(
    FilterPresetType Type,
    string Label,
    string Description);
