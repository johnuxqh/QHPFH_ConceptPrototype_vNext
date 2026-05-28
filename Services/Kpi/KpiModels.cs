namespace QHPFH_ConceptPrototype.Services.Kpi;

public sealed record CapacitySnapshot(
    int PhysicalBeds,
    int OperationalBeds,
    int ClosedBeds,
    int BlockedBeds,
    int MaintenanceBeds,
    int CleaningBeds,
    int AvailableBeds,
    int OccupiedBeds,
    int FutureAllocatedBeds,
    int TransitBeds,
    decimal OccupancyPercent,
    decimal OperationalOccupancyPercent);

public sealed record WorkflowSnapshot(
    int TasksPending,
    int ResultsPending,
    int MedicationAlerts,
    int InfectionControlPatients,
    int DischargeBarriers,
    int AllocationReviewCounts);

public sealed record AllocationKpiSnapshot(
    int PendingAllocations,
    int TransferRequests,
    int IncomingPatients,
    int ElectiveDemand,
    int EdDemand,
    int IhtDemand);

public sealed record DelayedDischargeKpiSnapshot(
    int DelayedDischarges,
    int Outliers,
    int ReadyForDischargePatients,
    int DischargeBarrierPatients);

public sealed record OperationalPressureSnapshot(
    int ActiveOperationalEvents,
    int CriticalOperationalEvents,
    int OpenEscalations,
    int CriticalAlerts,
    int UnresolvedNotifications,
    string CapacityTier,
    bool StaffingPressureFlag,
    bool WorkloadPressureFlag);

public sealed record BedKpiSnapshot(string BedId, string WardId, string Status, bool IsOperational, bool IsOccupied, bool IsFutureAllocated);

public sealed record WardKpiSnapshot(
    string WardId,
    string FacilityId,
    string WardName,
    CapacitySnapshot Capacity,
    AllocationKpiSnapshot Allocation,
    DelayedDischargeKpiSnapshot DelayedDischarge,
    WorkflowSnapshot Workflow,
    OperationalPressureSnapshot Pressure);

public sealed record FacilityKpiSnapshot(
    string FacilityId,
    CapacitySnapshot Capacity,
    AllocationKpiSnapshot Allocation,
    DelayedDischargeKpiSnapshot DelayedDischarge,
    WorkflowSnapshot Workflow,
    OperationalPressureSnapshot Pressure,
    int AdmissionsToday,
    int DischargesToday);

public sealed record HhsKpiSnapshot(
    string HhsId,
    CapacitySnapshot Capacity,
    AllocationKpiSnapshot Allocation,
    DelayedDischargeKpiSnapshot DelayedDischarge,
    WorkflowSnapshot Workflow,
    OperationalPressureSnapshot Pressure,
    int AdmissionsToday,
    int DischargesToday);

public sealed record StatewideKpiSnapshot(
    CapacitySnapshot Capacity,
    AllocationKpiSnapshot Allocation,
    DelayedDischargeKpiSnapshot DelayedDischarge,
    WorkflowSnapshot Workflow,
    OperationalPressureSnapshot Pressure,
    int AdmissionsToday,
    int DischargesToday);
