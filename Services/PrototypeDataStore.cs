using QHPFH_ConceptPrototype.Data;
using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services;

public sealed class PrototypeDataStore
{
    private readonly List<HhsRecord> _hhsRecords;
    private readonly List<FacilityRecord> _facilityRecords;
    private readonly List<WardRecord> _wardRecords;
    private readonly List<BedRecord> _bedRecords;
    private readonly List<PatientRecord> _patientRecords;
    private readonly List<AdmissionRecord> _admissionRecords;
    private readonly List<AllocationRecord> _allocationRecords;
    private readonly List<OperationalEventRecord> _operationalEventRecords;
    private readonly List<InformationBannerRecord> _informationBannerRecords;

    public event Action? OnChange;

    public PrototypeDataStore()
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

    private void NotifyStateChanged() => OnChange?.Invoke();
}
