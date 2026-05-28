using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services.Kpi;

public sealed class KpiCalculationService
{
    private readonly PrototypeDataStore _store;

    public KpiCalculationService(PrototypeDataStore store) => _store = store;

    public StatewideKpiSnapshot GetStatewideSnapshot() => BuildSnapshot(_store.GetBeds(), _store.GetPatients(), _store.GetAllocations(), _store.GetIncomingPatients(), _store.GetTransferRequests(), _store.GetPatientTasks(), _store.GetPatientResults(), _store.GetPatientAlerts(), _store.GetPatientDischarges(), _store.GetOperationalEvents(), _store.GetOperationalEscalations(), _store.GetNotifications(), _store.GetAdmissions());

    public HhsKpiSnapshot GetHhsSnapshot(string hhsId)
    {
        var facilities = _store.GetFacilities().Where(f => f.HhsId == hhsId).Select(f => f.Id).ToHashSet();
        var wards = _store.GetWards().Where(w => facilities.Contains(w.FacilityId)).Select(w => w.Id).ToHashSet();
        var snapshot = BuildSnapshot(
            _store.GetBeds().Where(b => wards.Contains(b.WardId)),
            _store.GetPatients().Where(p => wards.Contains(p.CurrentWardId)),
            _store.GetAllocations().Where(a => facilities.Contains(a.FacilityId)),
            _store.GetIncomingPatients().Where(i => i.TargetFacilityId is not null && facilities.Contains(i.TargetFacilityId)),
            _store.GetTransferRequests().Where(t => (t.ToFacilityId is not null && facilities.Contains(t.ToFacilityId)) || (t.FromFacilityId is not null && facilities.Contains(t.FromFacilityId))),
            _store.GetPatientTasks().Where(t => wards.Contains(_store.GetPatientById(t.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientResults().Where(r => wards.Contains(_store.GetPatientById(r.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientAlerts().Where(a => wards.Contains(_store.GetPatientById(a.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientDischarges().Where(d => wards.Contains(_store.GetPatientById(d.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetOperationalEvents().Where(e => e.HhsId == hhsId || (e.FacilityId is not null && facilities.Contains(e.FacilityId))),
            _store.GetOperationalEscalations().Where(e => _store.GetOperationalEvents().Any(o => o.Id == e.EventId && (o.HhsId == hhsId || (o.FacilityId is not null && facilities.Contains(o.FacilityId))))),
            _store.GetNotifications().Where(n => n.HhsId == hhsId || (n.FacilityId is not null && facilities.Contains(n.FacilityId))),
            _store.GetAdmissions().Where(a => _store.GetPatients().Any(p => p.Id == a.PatientId && wards.Contains(p.CurrentWardId))));

        return new HhsKpiSnapshot(hhsId, snapshot.Capacity, snapshot.Allocation, snapshot.DelayedDischarge, snapshot.Workflow, snapshot.Pressure, snapshot.AdmissionsToday, snapshot.DischargesToday);
    }

    public FacilityKpiSnapshot GetFacilitySnapshot(string facilityId)
    {
        var wards = _store.GetWards().Where(w => w.FacilityId == facilityId).Select(w => w.Id).ToHashSet();
        var snapshot = BuildSnapshot(
            _store.GetBeds().Where(b => wards.Contains(b.WardId)),
            _store.GetPatients().Where(p => wards.Contains(p.CurrentWardId)),
            _store.GetAllocations().Where(a => a.FacilityId == facilityId),
            _store.GetIncomingPatients().Where(i => i.TargetFacilityId == facilityId),
            _store.GetTransferRequests().Where(t => t.ToFacilityId == facilityId || t.FromFacilityId == facilityId),
            _store.GetPatientTasks().Where(t => wards.Contains(_store.GetPatientById(t.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientResults().Where(r => wards.Contains(_store.GetPatientById(r.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientAlerts().Where(a => wards.Contains(_store.GetPatientById(a.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetPatientDischarges().Where(d => wards.Contains(_store.GetPatientById(d.PatientId)?.CurrentWardId ?? string.Empty)),
            _store.GetOperationalEvents().Where(e => e.FacilityId == facilityId),
            _store.GetOperationalEscalations().Where(e => _store.GetOperationalEvents().Any(o => o.Id == e.EventId && o.FacilityId == facilityId)),
            _store.GetNotifications().Where(n => n.FacilityId == facilityId),
            _store.GetAdmissions().Where(a => _store.GetPatients().Any(p => p.Id == a.PatientId && wards.Contains(p.CurrentWardId))));

        return new FacilityKpiSnapshot(facilityId, snapshot.Capacity, snapshot.Allocation, snapshot.DelayedDischarge, snapshot.Workflow, snapshot.Pressure, snapshot.AdmissionsToday, snapshot.DischargesToday);
    }

    public WardKpiSnapshot GetWardSnapshot(string wardId)
    {
        var ward = _store.GetWards().First(w => w.Id == wardId);
        var snapshot = BuildSnapshot(
            _store.GetBeds().Where(b => b.WardId == wardId),
            _store.GetPatients().Where(p => p.CurrentWardId == wardId),
            _store.GetAllocations().Where(a => a.WardId == wardId),
            _store.GetIncomingPatients().Where(i => i.TargetWardId == wardId),
            _store.GetTransferRequests().Where(t => t.ToWardId == wardId || t.FromWardId == wardId),
            _store.GetPatientTasks().Where(t => (_store.GetPatientById(t.PatientId)?.CurrentWardId) == wardId),
            _store.GetPatientResults().Where(r => (_store.GetPatientById(r.PatientId)?.CurrentWardId) == wardId),
            _store.GetPatientAlerts().Where(a => (_store.GetPatientById(a.PatientId)?.CurrentWardId) == wardId),
            _store.GetPatientDischarges().Where(d => (_store.GetPatientById(d.PatientId)?.CurrentWardId) == wardId),
            _store.GetOperationalEvents().Where(e => e.WardId == wardId),
            _store.GetOperationalEscalations().Where(e => _store.GetOperationalEvents().Any(o => o.Id == e.EventId && o.WardId == wardId)),
            _store.GetNotifications().Where(n => n.WardId == wardId),
            _store.GetAdmissions().Where(a => (_store.GetPatientById(a.PatientId)?.CurrentWardId) == wardId));

        return new WardKpiSnapshot(wardId, ward.FacilityId, ward.Name, snapshot.Capacity, snapshot.Allocation, snapshot.DelayedDischarge, snapshot.Workflow, snapshot.Pressure);
    }

    public IReadOnlyList<BedKpiSnapshot> GetBedSnapshots(string? wardId = null)
    {
        var beds = wardId is null ? _store.GetBeds() : _store.GetBeds().Where(b => b.WardId == wardId).ToList();
        return beds.Select(b => new BedKpiSnapshot(b.Id, b.WardId, b.BedStatus.ToString(), b.IsOpenOperationally, b.BedStatus == BedStatus.Occupied, b.BedStatus == BedStatus.FutureAllocated)).ToList();
    }

    private StatewideKpiSnapshot BuildSnapshot(
        IEnumerable<BedRecord> beds,
        IEnumerable<PatientRecord> patients,
        IEnumerable<AllocationRecord> allocations,
        IEnumerable<IncomingPatientRecord> incoming,
        IEnumerable<TransferRequestRecord> transfers,
        IEnumerable<PatientTaskRecord> tasks,
        IEnumerable<PatientResultRecord> results,
        IEnumerable<PatientAlertRecord> alerts,
        IEnumerable<PatientDischargeRecord> discharges,
        IEnumerable<OperationalEventRecord> events,
        IEnumerable<OperationalEscalationRecord> escalations,
        IEnumerable<NotificationRecord> notifications,
        IEnumerable<AdmissionRecord> admissions)
    {
        var b = beds.ToList(); var p = patients.ToList(); var a = allocations.ToList(); var i = incoming.ToList(); var t = transfers.ToList();
        var pt = tasks.ToList(); var pr = results.ToList(); var pa = alerts.ToList(); var pd = discharges.ToList();
        var oe = events.ToList(); var es = escalations.ToList(); var n = notifications.ToList(); var ad = admissions.ToList();

        var physical = b.Count(x => x.IsPhysicalBed);
        var operational = b.Count(x => x.IsOpenOperationally);
        var closed = b.Count(x => x.BedStatus == BedStatus.Closed);
        var blocked = b.Count(x => x.BedStatus == BedStatus.Blocked);
        var maintenance = b.Count(x => x.BedStatus == BedStatus.Maintenance);
        var cleaning = b.Count(x => x.BedStatus == BedStatus.Cleaning);
        var occupied = b.Count(x => x.BedStatus == BedStatus.Occupied);
        var available = b.Count(x => x.BedStatus == BedStatus.Open);
        var futureAllocated = b.Count(x => x.BedStatus == BedStatus.FutureAllocated);
        var transit = b.Count(x => x.BedType == BedType.Transit);
        var occupancy = operational == 0 ? 0 : Math.Round((decimal)occupied / operational * 100, 1);
        var cap = new CapacitySnapshot(physical, operational, closed, blocked, maintenance, cleaning, available, occupied, futureAllocated, transit, occupancy, occupancy);

        var alloc = new AllocationKpiSnapshot(
            PendingAllocations: a.Count(x => x.Status is AllocationStatus.Waiting or AllocationStatus.PendingReview or AllocationStatus.PreAllocated),
            TransferRequests: t.Count,
            IncomingPatients: i.Count,
            ElectiveDemand: i.Count(x => x.SourceType == AllocationSourceType.Elective),
            EdDemand: i.Count(x => x.SourceType == AllocationSourceType.ED),
            IhtDemand: i.Count(x => x.SourceType == AllocationSourceType.IHT));

        var delayed = new DelayedDischargeKpiSnapshot(
            DelayedDischarges: p.Count(x => x.IsDelayedDischarge),
            Outliers: p.Count(x => x.IsOutlier),
            ReadyForDischargePatients: p.Count(x => x.FlowStatus == PatientFlowStatus.ReadyForDischarge),
            DischargeBarrierPatients: pd.Count(x => x.IsDelayed && !string.IsNullOrWhiteSpace(x.DelayReason)));

        var workflow = new WorkflowSnapshot(
            TasksPending: pt.Count(x => x.Status is PatientTaskStatus.Pending or PatientTaskStatus.InProgress or PatientTaskStatus.Blocked),
            ResultsPending: pr.Count(x => x.Status is PatientResultStatus.Pending or PatientResultStatus.InProgress),
            MedicationAlerts: pa.Count(x => x.AlertType == PatientAlertType.Allergy && x.IsActive),
            InfectionControlPatients: p.Count(x => x.IsInfectionControlFlagged),
            DischargeBarriers: pd.Count(x => x.IsDelayed),
            AllocationReviewCounts: a.Count(x => x.Status == AllocationStatus.PendingReview));

        var tier = oe.Where(x => x.CapacityStatus is not null).OrderByDescending(x => x.CapacityStatus).Select(x => x.CapacityStatus!.Value.ToString()).FirstOrDefault() ?? "Tier1";
        var pressure = new OperationalPressureSnapshot(
            ActiveOperationalEvents: oe.Count(x => x.IsActive),
            CriticalOperationalEvents: oe.Count(x => x.IsActive && x.Severity == OperationalEventSeverity.Critical),
            OpenEscalations: es.Count(x => !x.IsResolved),
            CriticalAlerts: pa.Count(x => x.IsActive && x.Severity == "High") + n.Count(x => x.Status == NotificationStatus.Unread && x.Severity == NotificationSeverity.Critical),
            UnresolvedNotifications: n.Count(x => x.Status is NotificationStatus.Unread or NotificationStatus.Read),
            CapacityTier: tier,
            StaffingPressureFlag: oe.Any(x => x.IsActive && x.Category == OperationalEventCategory.Staffing),
            WorkloadPressureFlag: alloc.PendingAllocations > 5 || workflow.TasksPending > 10);

        var admissionsToday = ad.Count(x => x.RequestedAtUtc.Date == DateTime.UtcNow.Date);
        var dischargesToday = p.Count(x => x.EstimatedDischargeDate?.Date == DateTime.UtcNow.Date);

        return new StatewideKpiSnapshot(cap, alloc, delayed, workflow, pressure, admissionsToday, dischargesToday);
    }
}
