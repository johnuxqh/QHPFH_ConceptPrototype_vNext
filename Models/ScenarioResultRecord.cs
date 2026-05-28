namespace QHPFH_ConceptPrototype.Models;

public sealed record ScenarioResultRecord(
    string Id,
    string ScenarioId,
    string MetricName,
    decimal BaselineValue,
    decimal ProjectedValue,
    decimal DeltaValue,
    string Unit,
    NotificationSeverity Severity,
    string Summary);
