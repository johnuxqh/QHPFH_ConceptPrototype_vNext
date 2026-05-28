namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioInputRecord(
    string Id,
    string ScenarioId,
    string InputType,
    string Label,
    decimal BaselineValue,
    decimal ScenarioValue,
    string Unit,
    string ScopeLabel,
    string? Notes);
