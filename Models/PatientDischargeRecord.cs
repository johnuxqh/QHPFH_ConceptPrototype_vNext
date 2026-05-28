namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientDischargeRecord(
    string Id,
    string PatientId,
    DateTime? EstimatedDischargeDate,
    DischargeProgressStatus DischargeProgress,
    string WaitingFor,
    string? BarrierCategory,
    string? Destination,
    bool IsDelayed,
    string? DelayReason);
