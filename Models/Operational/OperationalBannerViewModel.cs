using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Models.Operational;

public sealed record OperationalBannerViewModel(
    string Id,
    string Title,
    string Message,
    OperationalEventSeverity Severity,
    OperationalEventScope Scope,
    CapacityStatus? CapacityStatus,
    string AffectedArea,
    bool IsDismissible,
    bool RequiresAcknowledgement,
    bool IsPinned,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    string SourceType,
    int SortPriority)
{
    public bool IsCritical => Severity == OperationalEventSeverity.Critical || CapacityStatus == QHPFH_ConceptPrototype.Models.CapacityStatus.IncidentManagement;
}
