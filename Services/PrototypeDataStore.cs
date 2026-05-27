using QHPFH_ConceptPrototype.Data;
using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services;

public sealed class PrototypeDataStore
{
    private List<HhsRecord> _hhsRecords = [];
    private List<FacilityRecord> _facilityRecords = [];
    private List<WardRecord> _wardRecords = [];
    private List<BedRecord> _bedRecords = [];
    private List<PatientRecord> _patientRecords = [];
    private List<AdmissionRecord> _admissionRecords = [];
    private List<AllocationRecord> _allocationRecords = [];
    private List<OperationalEventRecord> _operationalEventRecords = [];
    private List<InformationBannerRecord> _informationBannerRecords = [];
    private List<PatientAlertRecord> _patientAlertRecords = [];
    private List<PatientTaskRecord> _patientTaskRecords = [];
    private List<PatientResultRecord> _patientResultRecords = [];
    private List<PatientMedicationRecord> _patientMedicationRecords = [];
    private List<PatientCareTeamMemberRecord> _patientCareTeamMemberRecords = [];
    private List<PatientNoteRecord> _patientNoteRecords = [];
    private List<PatientDischargeRecord> _patientDischargeRecords = [];
    private List<IncomingPatientRecord> _incomingPatientRecords = [];
    private List<TransferRequestRecord> _transferRequestRecords = [];
    private List<AllocationRequestRecord> _allocationRequestRecords = [];

    public event Action? OnChange;

    public PrototypeDataStore() => ResetToSeedData();

    public IReadOnlyList<HhsRecord> GetHhs() => _hhsRecords;
    public IReadOnlyList<FacilityRecord> GetFacilities() => _facilityRecords;
    public IReadOnlyList<WardRecord> GetWards() => _wardRecords;
    public IReadOnlyList<BedRecord> GetBeds() => _bedRecords;
    public IReadOnlyList<PatientRecord> GetPatients() => _patientRecords;
    public PatientRecord? GetPatientById(string id) => _patientRecords.FirstOrDefault(x => x.Id == id);
    public IReadOnlyList<AdmissionRecord> GetAdmissions() => _admissionRecords;
    public IReadOnlyList<AllocationRecord> GetAllocations() => _allocationRecords;
    public IReadOnlyList<IncomingPatientRecord> GetIncomingPatients() => _incomingPatientRecords;
    public IReadOnlyList<IncomingPatientRecord> GetIncomingPatientsBySource(AllocationSourceType sourceType) => _incomingPatientRecords.Where(x => x.SourceType == sourceType).ToList();
    public IReadOnlyList<TransferRequestRecord> GetTransferRequests() => _transferRequestRecords;
    public IReadOnlyList<AllocationRequestRecord> GetAllocationRequests() => _allocationRequestRecords;
    public IReadOnlyList<AllocationRecord> GetAllocationsForWard(string wardId) => _allocationRecords.Where(x => x.WardId == wardId).ToList();
    public IReadOnlyList<AllocationRecord> GetAllocationsForPatient(string patientId) => _allocationRecords.Where(x => x.PatientId == patientId).ToList();
    public IReadOnlyList<OperationalEventRecord> GetOperationalEvents() => _operationalEventRecords;
    public IReadOnlyList<InformationBannerRecord> GetInformationBanners() => _informationBannerRecords;
    public IReadOnlyList<PatientAlertRecord> GetPatientAlerts() => _patientAlertRecords;
    public IReadOnlyList<PatientAlertRecord> GetPatientAlerts(string patientId) => _patientAlertRecords.Where(x => x.PatientId == patientId).ToList();
    public IReadOnlyList<PatientTaskRecord> GetPatientTasks() => _patientTaskRecords;
    public IReadOnlyList<PatientTaskRecord> GetPatientTasks(string patientId) => _patientTaskRecords.Where(x => x.PatientId == patientId).ToList();
    public IReadOnlyList<PatientResultRecord> GetPatientResults() => _patientResultRecords;
    public IReadOnlyList<PatientMedicationRecord> GetPatientMedications() => _patientMedicationRecords;
    public IReadOnlyList<PatientCareTeamMemberRecord> GetPatientCareTeam() => _patientCareTeamMemberRecords;
    public IReadOnlyList<PatientNoteRecord> GetPatientNotes() => _patientNoteRecords;
    public IReadOnlyList<PatientDischargeRecord> GetPatientDischarges() => _patientDischargeRecords;
    public PatientDischargeRecord? GetPatientDischarge(string patientId) => _patientDischargeRecords.FirstOrDefault(x => x.PatientId == patientId);

    public bool AddPatientNote(PatientNoteRecord note)
    {
        if (string.IsNullOrWhiteSpace(note.Id) || string.IsNullOrWhiteSpace(note.PatientId) || _patientNoteRecords.Any(x => x.Id == note.Id)) return false;
        _patientNoteRecords.Add(note);
        NotifyStateChanged();
        return true;
    }

    public bool UpdatePatientTaskStatus(string taskId, PatientTaskStatus status)
    {
        if (string.IsNullOrWhiteSpace(taskId)) return false;
        var index = _patientTaskRecords.FindIndex(x => x.Id == taskId);
        if (index < 0) return false;
        _patientTaskRecords[index] = _patientTaskRecords[index] with { Status = status };
        NotifyStateChanged();
        return true;
    }

    public bool UpdatePatientDischargeProgress(string patientId, DischargeProgressStatus progress, string? waitingFor = null, string? delayReason = null)
    {
        if (string.IsNullOrWhiteSpace(patientId)) return false;
        var index = _patientDischargeRecords.FindIndex(x => x.PatientId == patientId);
        if (index < 0) return false;
        var current = _patientDischargeRecords[index];
        _patientDischargeRecords[index] = current with
        {
            DischargeProgress = progress,
            WaitingFor = waitingFor ?? current.WaitingFor,
            DelayReason = delayReason ?? current.DelayReason,
            IsDelayed = progress == DischargeProgressStatus.WaitingForExternal || !string.IsNullOrWhiteSpace(delayReason)
        };
        NotifyStateChanged();
        return true;
    }

    public bool AddInformationBanner(InformationBannerRecord banner)
    {
        if (string.IsNullOrWhiteSpace(banner.Id) || _informationBannerRecords.Any(x => x.Id == banner.Id)) return false;
        _informationBannerRecords.Add(banner);
        NotifyStateChanged();
        return true;
    }

    public bool UpdateInformationBanner(InformationBannerRecord banner)
    {
        var index = _informationBannerRecords.FindIndex(x => x.Id == banner.Id);
        if (index < 0) return false;
        _informationBannerRecords[index] = banner;
        NotifyStateChanged();
        return true;
    }

    public bool RemoveInformationBanner(string bannerId)
    {
        if (string.IsNullOrWhiteSpace(bannerId)) return false;
        var removed = _informationBannerRecords.RemoveAll(x => x.Id == bannerId) > 0;
        if (removed) NotifyStateChanged();
        return removed;
    }

    public bool AddOperationalEvent(OperationalEventRecord operationalEvent)
    {
        if (string.IsNullOrWhiteSpace(operationalEvent.Id) || _operationalEventRecords.Any(x => x.Id == operationalEvent.Id)) return false;
        _operationalEventRecords.Add(operationalEvent);
        NotifyStateChanged();
        return true;
    }

    public bool UpdateOperationalEvent(OperationalEventRecord operationalEvent)
    {
        var index = _operationalEventRecords.FindIndex(x => x.Id == operationalEvent.Id);
        if (index < 0) return false;
        _operationalEventRecords[index] = operationalEvent;
        NotifyStateChanged();
        return true;
    }

    public bool UpdateBedStatus(string bedId, string status)
    {
        if (string.IsNullOrWhiteSpace(bedId) || string.IsNullOrWhiteSpace(status)) return false;
        var index = _bedRecords.FindIndex(x => x.Id == bedId);
        if (index < 0) return false;
        var parsedStatus = Enum.TryParse<BedStatus>(status, true, out var resolvedStatus) ? resolvedStatus : BedStatus.Open;
        _bedRecords[index] = _bedRecords[index] with { BedStatus = parsedStatus, IsOpenOperationally = parsedStatus is BedStatus.Open or BedStatus.Occupied or BedStatus.FutureAllocated };
        NotifyStateChanged();
        return true;
    }

    public bool UpdateBedOperationalState(string bedId, bool isBlocked, bool isIsolation)
    {
        if (string.IsNullOrWhiteSpace(bedId)) return false;
        var index = _bedRecords.FindIndex(x => x.Id == bedId);
        if (index < 0) return false;
        var nextStatus = isBlocked ? BedStatus.Blocked : _bedRecords[index].BedStatus;
        _bedRecords[index] = _bedRecords[index] with { BedStatus = nextStatus, IsIsolationCapable = isIsolation, IsSpecialistBed = _bedRecords[index].IsSpecialistBed || isIsolation, IsOpenOperationally = !isBlocked };
        NotifyStateChanged();
        return true;
    }

    public bool AssignPatientToBed(string patientId, string bedId)
    {
        if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(bedId)) return false;
        if (!_patientRecords.Any(x => x.Id == patientId)) return false;
        var bedIndex = _bedRecords.FindIndex(x => x.Id == bedId);
        if (bedIndex < 0) return false;
        var bed = _bedRecords[bedIndex];
        if (bed.IsBlocked) return false;
        _bedRecords[bedIndex] = bed with { CurrentPatientId = patientId, BedStatus = BedStatus.Occupied, IsOpenOperationally = true };
        NotifyStateChanged();
        return true;
    }

    public bool PreAllocatePatientToFutureBed(string patientId, string facility, string wardCode, string priority)
    {
        if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(facility) || string.IsNullOrWhiteSpace(wardCode)) return false;
        if (!_patientRecords.Any(x => x.Id == patientId)) return false;
        _allocationRecords.Add(new AllocationRecord($"ALL-{Guid.NewGuid():N}"[..12], patientId, facility, wardCode, string.IsNullOrWhiteSpace(priority) ? "Routine" : priority, "PreAllocated", DateTime.UtcNow)
        {
            SourceType = AllocationSourceType.ED,
            IsFutureAllocation = true,
            IsPreAllocation = true
        });
        NotifyStateChanged();
        return true;
    }

    public bool AddIncomingPatient(IncomingPatientRecord incomingPatient)
    {
        if (string.IsNullOrWhiteSpace(incomingPatient.Id) || _incomingPatientRecords.Any(x => x.Id == incomingPatient.Id)) return false;
        _incomingPatientRecords.Add(incomingPatient);
        NotifyStateChanged();
        return true;
    }

    public bool UpdateAllocationStatus(string allocationId, AllocationStatus status, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(allocationId)) return false;
        var index = _allocationRecords.FindIndex(x => x.Id == allocationId);
        if (index < 0) return false;
        var current = _allocationRecords[index];
        _allocationRecords[index] = current with { Status = status, UpdatedAtUtc = DateTime.UtcNow, Notes = notes ?? current.Notes };
        NotifyStateChanged();
        return true;
    }

    public bool AssignAllocationToBed(string allocationId, string bedId)
    {
        if (string.IsNullOrWhiteSpace(allocationId) || string.IsNullOrWhiteSpace(bedId)) return false;
        var index = _allocationRecords.FindIndex(x => x.Id == allocationId);
        if (index < 0) return false;
        if (!_bedRecords.Any(x => x.Id == bedId)) return false;
        var current = _allocationRecords[index];
        _allocationRecords[index] = current with { TargetBedId = bedId, Status = AllocationStatus.Allocated, UpdatedAtUtc = DateTime.UtcNow, IsPreAllocation = false };
        NotifyStateChanged();
        return true;
    }

    public bool PreAllocatePatientToBed(string allocationId, string bedId)
    {
        if (string.IsNullOrWhiteSpace(allocationId) || string.IsNullOrWhiteSpace(bedId)) return false;
        var index = _allocationRecords.FindIndex(x => x.Id == allocationId);
        if (index < 0) return false;
        if (!_bedRecords.Any(x => x.Id == bedId)) return false;
        var current = _allocationRecords[index];
        _allocationRecords[index] = current with { FutureBedId = bedId, Status = AllocationStatus.PreAllocated, IsFutureAllocation = true, IsPreAllocation = true, UpdatedAtUtc = DateTime.UtcNow };
        NotifyStateChanged();
        return true;
    }

    public bool UpdateTransferReadiness(string transferRequestId, TransferReadinessStatus readinessStatus, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(transferRequestId)) return false;
        var index = _transferRequestRecords.FindIndex(x => x.Id == transferRequestId);
        if (index < 0) return false;
        var current = _transferRequestRecords[index];
        _transferRequestRecords[index] = current with { ReadinessStatus = readinessStatus, Notes = notes ?? current.Notes };
        NotifyStateChanged();
        return true;
    }

    public void ResetToSeedData()
    {
        _hhsRecords = [..DemoDataSeed.HhsRecords];
        _facilityRecords = [..DemoDataSeed.Facilities];
        _wardRecords = [..DemoDataSeed.Wards];
        _bedRecords = [..DemoDataSeed.Beds];
        _patientRecords = [..DemoDataSeed.Patients];
        _admissionRecords = [..DemoDataSeed.Admissions];
        _allocationRecords = [..DemoDataSeed.Allocations];
        _operationalEventRecords = [..DemoDataSeed.OperationalEvents];
        _informationBannerRecords = [..DemoDataSeed.InformationBanners];
        _patientAlertRecords = [..DemoDataSeed.PatientAlerts];
        _patientTaskRecords = [..DemoDataSeed.PatientTasks];
        _patientResultRecords = [..DemoDataSeed.PatientResults];
        _patientMedicationRecords = [..DemoDataSeed.PatientMedications];
        _patientCareTeamMemberRecords = [..DemoDataSeed.PatientCareTeamMembers];
        _patientNoteRecords = [..DemoDataSeed.PatientNotes];
        _patientDischargeRecords = [..DemoDataSeed.PatientDischarges];
        _incomingPatientRecords = [..DemoDataSeed.IncomingPatients];
        _transferRequestRecords = [..DemoDataSeed.TransferRequests];
        _allocationRequestRecords = [..DemoDataSeed.AllocationRequests];
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
