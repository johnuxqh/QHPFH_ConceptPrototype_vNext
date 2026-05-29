namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterOptionRecord(
    string Value,
    string Label,
    string? Hhs = null,
    string? Facility = null,
    string? ServiceStream = null);
