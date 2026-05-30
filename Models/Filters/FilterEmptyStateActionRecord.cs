namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterEmptyStateActionRecord(
    string Label,
    FilterResetMode? ResetMode = null,
    FilterPresetType? PresetType = null);
