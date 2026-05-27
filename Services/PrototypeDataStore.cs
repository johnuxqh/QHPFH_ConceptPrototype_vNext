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

    public event Action? OnChange;

    public PrototypeDataStore()
    {
        ResetToSeedData();
    }

    public IReadOnlyList<HhsRecord> GetHhs() => _hhsRecords;
    public IReadOnlyList<FacilityRecord> GetFacilities() => _facilityRecords;
    public IReadOnlyList<WardRecord> GetWards() => _wardRecords;
    public IReadOnlyList<BedRecord> GetBeds() => _bedRecords;
    public IReadOnlyList<PatientRecord> GetPatients() => _patientRecords;
    public IReadOnlyList<AdmissionRecord> GetAdmissions() => _admissionRecords;
    public IReadOnlyList<AllocationRecord> GetAllocations() => _allocationRecords;
    public IReadOnlyList<OperationalEventRecord> GetOperationalEvents() => _operationalEventRecords;
    public IReadOnlyList<InformationBannerRecord> GetInformationBanners() => _informationBannerRecords;

    public bool AddInformationBanner(InformationBannerRecord banner)
    {
        if (string.IsNullOrWhiteSpace(banner.Id) || _informationBannerRecords.Any(x => x.Id == banner.Id))
        {
            return false;
        }

        _informationBannerRecords.Add(banner);
        NotifyStateChanged();
        return true;
    }

    public bool UpdateInformationBanner(InformationBannerRecord banner)
    {
        var index = _informationBannerRecords.FindIndex(x => x.Id == banner.Id);
        if (index < 0)
        {
            return false;
        }

        _informationBannerRecords[index] = banner;
        NotifyStateChanged();
        return true;
    }

    public bool RemoveInformationBanner(string bannerId)
    {
        if (string.IsNullOrWhiteSpace(bannerId))
        {
            return false;
        }

        var removed = _informationBannerRecords.RemoveAll(x => x.Id == bannerId) > 0;
        if (removed)
        {
            NotifyStateChanged();
        }

        return removed;
    }

    public bool AddOperationalEvent(OperationalEventRecord operationalEvent)
    {
        if (string.IsNullOrWhiteSpace(operationalEvent.Id) || _operationalEventRecords.Any(x => x.Id == operationalEvent.Id))
        {
            return false;
        }

        _operationalEventRecords.Add(operationalEvent);
        NotifyStateChanged();
        return true;
    }

    public bool UpdateOperationalEvent(OperationalEventRecord operationalEvent)
    {
        var index = _operationalEventRecords.FindIndex(x => x.Id == operationalEvent.Id);
        if (index < 0)
        {
            return false;
        }

        _operationalEventRecords[index] = operationalEvent;
        NotifyStateChanged();
        return true;
    }

    public bool UpdateBedStatus(string bedId, string status)
    {
        if (string.IsNullOrWhiteSpace(bedId) || string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        var index = _bedRecords.FindIndex(x => x.Id == bedId);
        if (index < 0)
        {
            return false;
        }

        var parsedStatus = Enum.TryParse<BedStatus>(status, true, out var resolvedStatus) ? resolvedStatus : BedStatus.Open;
        _bedRecords[index] = _bedRecords[index] with
        {
            BedStatus = parsedStatus,
            IsOpenOperationally = parsedStatus is BedStatus.Open or BedStatus.Occupied or BedStatus.FutureAllocated
        };
        NotifyStateChanged();
        return true;
    }

    public bool UpdateBedOperationalState(string bedId, bool isBlocked, bool isIsolation)
    {
        if (string.IsNullOrWhiteSpace(bedId))
        {
            return false;
        }

        var index = _bedRecords.FindIndex(x => x.Id == bedId);
        if (index < 0)
        {
            return false;
        }

        var nextStatus = isBlocked ? BedStatus.Blocked : _bedRecords[index].BedStatus;
        _bedRecords[index] = _bedRecords[index] with
        {
            BedStatus = nextStatus,
            IsIsolationCapable = isIsolation,
            IsSpecialistBed = _bedRecords[index].IsSpecialistBed || isIsolation,
            IsOpenOperationally = !isBlocked
        };
        NotifyStateChanged();
        return true;
    }

    public bool AssignPatientToBed(string patientId, string bedId)
    {
        if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(bedId))
        {
            return false;
        }

        var patientExists = _patientRecords.Any(x => x.Id == patientId);
        if (!patientExists)
        {
            return false;
        }

        var bedIndex = _bedRecords.FindIndex(x => x.Id == bedId);
        if (bedIndex < 0)
        {
            return false;
        }

        var bed = _bedRecords[bedIndex];
        if (bed.IsBlocked)
        {
            return false;
        }

        _bedRecords[bedIndex] = bed with
        {
            CurrentPatientId = patientId,
            BedStatus = BedStatus.Occupied,
            IsOpenOperationally = true
        };
        NotifyStateChanged();
        return true;
    }

    public bool PreAllocatePatientToFutureBed(string patientId, string facility, string wardCode, string priority)
    {
        if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(facility) || string.IsNullOrWhiteSpace(wardCode))
        {
            return false;
        }

        if (!_patientRecords.Any(x => x.Id == patientId))
        {
            return false;
        }

        var allocation = new AllocationRecord(
            Id: $"ALL-{Guid.NewGuid():N}"[..12],
            PatientId: patientId,
            Facility: facility,
            WardCode: wardCode,
            Priority: string.IsNullOrWhiteSpace(priority) ? "Routine" : priority,
            Status: "PreAllocated",
            UpdatedAtUtc: DateTime.UtcNow);

        _allocationRecords.Add(allocation);
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
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
