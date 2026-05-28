namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioAssumptionRecord(
    string Id,
    string ScenarioId,
    string Title,
    string Description,
    ScenarioConfidence Confidence,
    string? SourceLabel);
