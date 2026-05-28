using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Services;

public sealed class PrototypeDataService
{
    private readonly PrototypeDataStore _store;

    public PrototypeDataService(PrototypeDataStore store)
    {
        _store = store;
    }

    // Hierarchy queries
    public IReadOnlyList<HhsRecord> GetHhs() => _store.GetHhs();
    public IReadOnlyList<FacilityRecord> GetFacilities() => _store.GetFacilities();
    public IReadOnlyList<FacilityRecord> GetFacilitiesByHhs(string hhsId) => _store.GetFacilities().Where(x => x.HhsId == hhsId).ToList();
    public IReadOnlyList<WardRecord> GetWards() => _store.GetWards();
    public IReadOnlyList<WardRecord> GetWardsByFacility(string facilityId) => _store.GetWards().Where(x => x.FacilityId == facilityId).ToList();
    public IReadOnlyList<BedRecord> GetBeds() => _store.GetBeds();
    public IReadOnlyList<BedRecord> GetBedsByWard(string wardId) => _store.GetBeds().Where(x => x.WardId == wardId).ToList();

    // Patient queries
    public IReadOnlyList<PatientRecord> GetPatients() => _store.GetPatients();
    public PatientRecord? GetPatientById(string patientId) => _store.GetPatientById(patientId);
    public IReadOnlyList<PatientRecord> GetPatientsByWard(string wardId) => _store.GetPatients().Where(x => x.CurrentWardId == wardId).ToList();
    public IReadOnlyList<PatientAlertRecord> GetPatientAlerts(string patientId) => _store.GetPatientAlerts(patientId);
    public IReadOnlyList<PatientTaskRecord> GetPatientTasks(string patientId) => _store.GetPatientTasks(patientId);
    public PatientDischargeRecord? GetPatientDischarge(string patientId) => _store.GetPatientDischarge(patientId);

    // Allocation queries
    public IReadOnlyList<AllocationRecord> GetAllocations() => _store.GetAllocations();
    public IReadOnlyList<AllocationRecord> GetAllocationsByWard(string wardId) => _store.GetAllocationsForWard(wardId);
    public IReadOnlyList<IncomingPatientRecord> GetIncomingPatients() => _store.GetIncomingPatients();
    public IReadOnlyList<IncomingPatientRecord> GetIncomingPatientsBySource(AllocationSourceType sourceType) => _store.GetIncomingPatientsBySource(sourceType);
    public IReadOnlyList<TransferRequestRecord> GetTransferRequests() => _store.GetTransferRequests();

    // Operational queries
    public IReadOnlyList<OperationalEventRecord> GetActiveOperationalEvents() => _store.GetActiveOperationalEvents();
    public IReadOnlyList<OperationalBannerRecord> GetOperationalBanners() => _store.GetOperationalBanners();
    public IReadOnlyList<OperationalEventRecord> GetOperationalEventsForFacility(string facilityId) => _store.GetOperationalEventsForFacility(facilityId);
    public IReadOnlyList<OperationalEventRecord> GetOperationalEventsForWard(string wardId) => _store.GetOperationalEventsForWard(wardId);

    // Notification/activity queries
    public IReadOnlyList<NotificationRecord> GetNotifications() => _store.GetNotifications();
    public IReadOnlyList<NotificationRecord> GetUnreadNotifications() => _store.GetUnreadNotifications();
    public IReadOnlyList<ActivityFeedItemRecord> GetActivityFeedItems() => _store.GetActivityFeedItems();
    public IReadOnlyList<ActivityFeedItemRecord> GetActivityFeedForWard(string wardId) => _store.GetActivityFeedForWard(wardId);
    public IReadOnlyList<ActivityFeedItemRecord> GetActivityFeedForPatient(string patientId) => _store.GetActivityFeedForPatient(patientId);

    // Scenario queries
    public IReadOnlyList<ScenarioRecord> GetScenarios() => _store.GetScenarios();
    public IReadOnlyList<ScenarioResultRecord> GetScenarioResults(string scenarioId) => _store.GetScenarioResults(scenarioId);
    public IReadOnlyList<ScenarioActionRecord> GetScenarioActions(string scenarioId) => _store.GetScenarioActions(scenarioId);

    // Mutation wrappers
    public bool UpdateBedStatus(string bedId, string status) => _store.UpdateBedStatus(bedId, status);
    public bool AssignPatientToBed(string patientId, string bedId) => _store.AssignPatientToBed(patientId, bedId);
    public bool PreAllocatePatientToBed(string allocationId, string bedId) => _store.PreAllocatePatientToBed(allocationId, bedId);
    public bool UpdateAllocationStatus(string allocationId, AllocationStatus status, string? notes = null) => _store.UpdateAllocationStatus(allocationId, status, notes);
    public bool AddInformationBanner(InformationBannerRecord banner) => _store.AddInformationBanner(banner);
    public bool AddOperationalEvent(OperationalEventRecord operationalEvent) => _store.AddOperationalEvent(operationalEvent);
    public bool AddActivityFeedItem(ActivityFeedItemRecord item) => _store.AddActivityFeedItem(item);
    public bool AddNotification(NotificationRecord notification) => _store.AddNotification(notification);
}
