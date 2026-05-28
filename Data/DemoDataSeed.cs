using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Data;

/// <summary>
/// Canonical seed entry point for all prototype in-memory demo datasets.
/// Keep PrototypeDataStore seeding wired to this class only.
/// </summary>
public static partial class DemoDataSeed;
public static class DemoDataSeed
{
    public static IReadOnlyList<HhsRecord> HhsRecords { get; } =
    [
        new("HHS-MN", "Metro North", "MN", "SE QLD", 1, true, ["Royal Brisbane and Women's Hospital", "The Prince Charles Hospital"]),
        new("HHS-MS", "Metro South", "MS", "SE QLD", 2, true, ["Princess Alexandra Hospital"])
    ];

    public static IReadOnlyList<FacilityRecord> Facilities { get; } =
    [
        new("FAC-RBWH", "HHS-MN", "Royal Brisbane and Women's Hospital", "RBWH", FacilityType.Hospital, true, 1),
        new("FAC-TPCH", "HHS-MN", "The Prince Charles Hospital", "TPCH", FacilityType.Hospital, true, 2),
        new("FAC-PAH", "HHS-MS", "Princess Alexandra Hospital", "PAH", FacilityType.Hospital, true, 1)
    ];

    public static IReadOnlyList<WardRecord> Wards { get; } =
    [
        new WardRecord("Metro North", "Royal Brisbane and Women's Hospital", "SUR", "Surgical", "Surgical operational unit", "Surgical", 32, 30, 27, 3, 90.0m, "High", "Platform matches activity", 4, 3, 1, 5, 4, 1, 1, 2, 1, "08:45") with { Id = "WARD-RBWH-SUR", FacilityId = "FAC-RBWH", SortOrder = 1, IsOperational = true, IsActive = true },
        new WardRecord("Metro North", "Royal Brisbane and Women's Hospital", "ICU", "Intensive Care", "Critical care unit", "CriticalCare", 16, 14, 13, 1, 92.8m, "High", "Acuity demand", 1, 2, 0, 1, 1, 0, 1, 2, 0, "08:41") with { Id = "WARD-RBWH-ICU", FacilityId = "FAC-RBWH", SortOrder = 2, IsOperational = true, IsActive = true },
        new WardRecord("Metro South", "Princess Alexandra Hospital", "GEN", "General Medicine", "General medicine operational unit", "Medical", 28, 26, 23, 3, 88.5m, "Medium", "Insufficient staffing", 3, 2, 1, 4, 3, 2, 1, 2, -1, "08:40") with { Id = "WARD-PAH-GEN", FacilityId = "FAC-PAH", SortOrder = 1, IsOperational = true, IsActive = true },
        new WardRecord("Metro North", "The Prince Charles Hospital", "MAT", "Maternity Overflow", "Ward retained with no active patient data", "Maternity", 10, 0, 0, 0, 0m, "Low", "Temporarily non-operational", 0, 0, 0, 0, 0, 0, 0, 10, 0, "08:30") with { Id = "WARD-TPCH-MAT", FacilityId = "FAC-TPCH", SortOrder = 1, IsOperational = false, IsActive = true }
    ];

    public static IReadOnlyList<BedRecord> Beds { get; } =
    [
        new BedRecord("BED-SUR-01", "WARD-RBWH-SUR", "SUR-01", "Occupied", false, false, "PAT-001") with { BedType = BedType.Standard, SortOrder = 1 },
        new BedRecord("BED-SUR-02", "WARD-RBWH-SUR", "SUR-02", "Open", false, false, null) with { BedType = BedType.Standard, SortOrder = 2 },
        new BedRecord("BED-ICU-01", "WARD-RBWH-ICU", "ICU-01", "Occupied", true, false, "PAT-002") with { BedType = BedType.Isolation, IsSpecialistBed = true, IsIsolationCapable = true, IsNegativePressure = true, SortOrder = 1 },
        new BedRecord("BED-ICU-02", "WARD-RBWH-ICU", "ICU-02", "Cleaning", false, false, null) with { BedType = BedType.CriticalCare, SortOrder = 2 },
        new BedRecord("BED-GEN-01", "WARD-PAH-GEN", "GEN-01", "Maintenance", false, false, null) with { BedType = BedType.Standard, MaintenanceReason = "Hydraulic rail service", IsOpenOperationally = false, SortOrder = 1 },
        new BedRecord("BED-GEN-02", "WARD-PAH-GEN", "GEN-02", "Blocked", false, true, null) with { ClosureReason = "Staffing constraint", IsOpenOperationally = false, SortOrder = 2 },
        new BedRecord("BED-GEN-03", "WARD-PAH-GEN", "GEN-03", "FutureAllocated", false, false, null) with { FutureAllocatedPatientId = "PAT-003", BedType = BedType.Transit, SortOrder = 3 }
    ];

    public static IReadOnlyList<PatientRecord> Patients { get; } =
    [
        new PatientRecord("PAT-001", "Clark Kent", 38, "M", "WARD-RBWH-SUR", "Admitted", false, false) with { CurrentBedId = "BED-SUR-01", RiskStatus = PatientRiskStatus.Stable, LengthOfStayDays = 2, CatchmentStatus = "In-Catchment" },
        new PatientRecord("PAT-002", "Diana Prince", 34, "F", "WARD-RBWH-ICU", "Admitted", true, false) with { CurrentBedId = "BED-ICU-01", RiskStatus = PatientRiskStatus.AtRisk, IsInfectionControlFlagged = true, HasAllergyAlert = true, LengthOfStayDays = 4 },
        new PatientRecord("PAT-003", "Bruce Wayne", 45, "M", "WARD-PAH-GEN", "Queued", false, true) with { FlowStatus = PatientFlowStatus.PreAllocated, IsOutlier = true, IsDelayedDischarge = true, EstimatedDischargeDate = DateTime.UtcNow.Date.AddDays(2), LengthOfStayDays = 9 },
        new PatientRecord("PAT-004", "Pepper Potts", 41, "F", "WARD-PAH-GEN", "Allocated", false, false) with { FlowStatus = PatientFlowStatus.Allocated, RiskStatus = PatientRiskStatus.Watch, LengthOfStayDays = 1 },
        new PatientRecord("PAT-005", "Johnny Storm", 29, "M", "WARD-RBWH-SUR", "ReadyForDischarge", false, false) with { FlowStatus = PatientFlowStatus.ReadyForDischarge, RiskStatus = PatientRiskStatus.Stable, EstimatedDischargeDate = DateTime.UtcNow.Date, LengthOfStayDays = 3 }
    ];

    public static IReadOnlyList<PatientAlertRecord> PatientAlerts { get; } =
    [
        new("ALR-001", "PAT-002", PatientAlertType.Allergy, "High", "Allergy Alert", "Known allergy requires medication verification.", true),
        new("ALR-002", "PAT-002", PatientAlertType.InfectionControl, "Medium", "Infection Control", "Isolation precautions required.", true),
        new("ALR-003", "PAT-003", PatientAlertType.FallsRisk, "Medium", "Falls Risk", "Mobility support recommended during transfers.", true)
    ];

    public static IReadOnlyList<PatientTaskRecord> PatientTasks { get; } =
    [
        new("TSK-001", "PAT-003", "Discharge", "Discharge summary", PatientTaskStatus.InProgress, PatientTaskPriority.High, DateTime.UtcNow.AddHours(6), "Ward Coordination"),
        new("TSK-002", "PAT-004", "Pathology", "Pathology review", PatientTaskStatus.Pending, PatientTaskPriority.Medium, DateTime.UtcNow.AddHours(3), "Medical Team"),
        new("TSK-003", "PAT-002", "Radiology", "CT follow-up", PatientTaskStatus.Pending, PatientTaskPriority.High, DateTime.UtcNow.AddHours(4), "ICU Team")
    ];

    public static IReadOnlyList<PatientResultRecord> PatientResults { get; } =
    [
        new("RST-001", "PAT-004", "Pathology", "Blood panel", PatientResultStatus.Pending, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(2), "Awaiting lab validation."),
        new("RST-002", "PAT-002", "Radiology", "CT chest", PatientResultStatus.InProgress, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), "Imaging in progress.")
    ];

    public static IReadOnlyList<PatientMedicationRecord> PatientMedications { get; } =
    [
        new("MED-001", "PAT-001", "Cefazolin", "IV", "8 hourly", "Active", DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(-6), "Post-op prophylaxis."),
        new("MED-002", "PAT-002", "Heparin", "Subcutaneous", "Daily", "Active", DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(-19), "Monitor platelets.")
    ];

    public static IReadOnlyList<PatientCareTeamMemberRecord> PatientCareTeamMembers { get; } =
    [
        new("CTM-001", "PAT-003", "Stephen Strange", "Consultant", "General Medicine", "Ext 4102", true),
        new("CTM-002", "PAT-003", "Natasha Romanoff", "NUM", "Ward Coordination", "Ext 2250", false),
        new("CTM-003", "PAT-002", "Jean Grey", "Infection Control CNC", "ICU", "Ext 3371", true)
    ];

    public static IReadOnlyList<PatientNoteRecord> PatientNotes { get; } =
    [
        new("NTE-001", "PAT-003", "Progress", "Stephen Strange", DateTime.UtcNow.AddHours(-4), "Awaiting community package confirmation before discharge.", true),
        new("NTE-002", "PAT-004", "Result", "Pepper Potts", DateTime.UtcNow.AddHours(-1), "Pathology pending; maintain current observation plan.", false)
    ];

    public static IReadOnlyList<PatientDischargeRecord> PatientDischarges { get; } =
    [
        new("DSC-001", "PAT-003", DateTime.UtcNow.Date.AddDays(2), DischargeProgressStatus.WaitingForExternal, "Community support package", "External Services", "Home with supports", true, "Awaiting package approval"),
        new("DSC-002", "PAT-005", DateTime.UtcNow.Date, DischargeProgressStatus.MedicallyReady, "Transport booking", null, "Home", false, null)
    ];

public static IReadOnlyList<AdmissionRecord> Admissions { get; } =
    [
        new("ADM-001", "PAT-003", "ED", "Queued", DateTime.UtcNow.AddMinutes(-35), DateTime.UtcNow.AddMinutes(20))
    ];

    public static IReadOnlyList<AllocationRecord> Allocations { get; } =
    [
        new AllocationRecord("ALL-001", "PAT-003", "FAC-PAH", "WARD-PAH-GEN", "High", "PreAllocated", DateTime.UtcNow.AddMinutes(-12)) with
        {
            SourceType = AllocationSourceType.ED,
            TargetBedId = "BED-GEN-03",
            FutureBedId = "BED-GEN-03",
            IsFutureAllocation = true,
            IsPreAllocation = true,
            RequiresIsolation = true,
            RequiredBedType = BedType.Transit,
            RequiredSpecialty = "General Medicine",
            Notes = "Pre-allocated to currently occupied transit-capable bed."
        }
    ];

    public static IReadOnlyList<IncomingPatientRecord> IncomingPatients { get; } =
    [
        new("INC-ED-001", "PAT-003", AllocationSourceType.ED, "RBWH ED", "FAC-PAH", "WARD-PAH-GEN", BedType.Transit, "General Medicine", true, "Isolation required", AllocationPriority.Critical, AllocationStatus.Waiting, DateTime.UtcNow.AddMinutes(45), "Escalated from ED hold."),
        new("INC-IHT-001", "PAT-004", AllocationSourceType.IHT, "Metro North IHT", "FAC-RBWH", "WARD-RBWH-SUR", BedType.Standard, "Surgical", false, null, AllocationPriority.Medium, AllocationStatus.PendingReview, DateTime.UtcNow.AddHours(2), "Inter-hospital transfer request."),
        new("INC-ELE-001", "PAT-005", AllocationSourceType.Elective, "Elective Theatre List", "FAC-RBWH", "WARD-RBWH-SUR", BedType.Standard, "Surgical", false, null, AllocationPriority.Routine, AllocationStatus.Waiting, DateTime.UtcNow.AddHours(6), "Elective admission planning."),
        new("INC-ADD-001", "PAT-001", AllocationSourceType.AddOn, "Add-On Queue", "FAC-RBWH", "WARD-RBWH-ICU", BedType.CriticalCare, "Critical Care", false, null, AllocationPriority.High, AllocationStatus.PendingReview, DateTime.UtcNow.AddHours(1), "Late add-on bed request."),
        new("INC-TRN-001", "PAT-002", AllocationSourceType.Transit, "Transit Bay", "FAC-PAH", "WARD-PAH-GEN", BedType.Transit, "General Medicine", true, "Contact precautions", AllocationPriority.High, AllocationStatus.InTransit, DateTime.UtcNow.AddMinutes(30), "Transit bed utilization example.")
    ];

    public static IReadOnlyList<TransferRequestRecord> TransferRequests { get; } =
    [
        new("TRF-001", "PAT-002", "FAC-RBWH", "WARD-RBWH-ICU", "FAC-PAH", "WARD-PAH-GEN", "Step-down from ICU", AllocationPriority.High, TransferReadinessStatus.PendingClinicalClearance, DateTime.UtcNow.AddMinutes(-25), "Ward transfer stream example."),
        new("TRF-002", "PAT-001", "FAC-RBWH", "WARD-RBWH-SUR", "FAC-RBWH", "WARD-RBWH-SUR", "Transit discharge lounge", AllocationPriority.Medium, TransferReadinessStatus.TransportBooked, DateTime.UtcNow.AddMinutes(-10), "Transit-related transfer example.")
    ];

    public static IReadOnlyList<AllocationRequestRecord> AllocationRequests { get; } =
    [
        new("ARQ-001", "INC-ED-001", "PAT-003", "Barbara Gordon", DateTime.UtcNow.AddMinutes(-20), "WARD-PAH-GEN", "BED-GEN-03", BedType.Transit, "General Medicine", true, AllocationPriority.Critical, AllocationStatus.PreAllocated, "Future bed pre-allocation request."),
        new("ARQ-002", "INC-IHT-001", "PAT-004", "Peggy Carter", DateTime.UtcNow.AddMinutes(-15), "WARD-RBWH-SUR", null, BedType.Standard, "Surgical", false, AllocationPriority.Medium, AllocationStatus.PendingReview, "IHT stream request.")
    ];

    public static IReadOnlyList<OperationalEventRecord> OperationalEvents { get; } =
    [
        new OperationalEventRecord("EVT-001", "Facility", "FAC-RBWH", "Capacity", "Moderate", "Facility operating at Tier 2 capacity.", DateTime.UtcNow.AddHours(-6)) with { Title = "Capacity Tier 2", Scope = OperationalEventScope.Facility, CapacityStatus = CapacityStatus.Tier2, IsActive = true, RequiresAcknowledgement = true, CreatedBy = "Barbara Gordon", SourceSystem = "OpsHub" },
        new OperationalEventRecord("EVT-002", "Facility", "FAC-PAH", "Capacity", "High", "Escalated to Tier 3 due to access block.", DateTime.UtcNow.AddHours(-2)) with { Title = "Capacity Tier 3 Escalation", Scope = OperationalEventScope.Facility, CapacityStatus = CapacityStatus.Tier3, IsActive = true, RequiresAcknowledgement = true, CreatedBy = "Peggy Carter", SourceSystem = "OpsHub" },
        new OperationalEventRecord("EVT-003", "Facility", "FAC-RBWH", "Infrastructure", "Info", "Planned fire alarm testing 14:00-14:30.", DateTime.UtcNow.AddHours(-12)) with { Title = "Planned Fire Alarm Testing", StartsAtUtc = DateTime.UtcNow.AddHours(2), EndsAtUtc = DateTime.UtcNow.AddHours(2.5), IsActive = true, CreatedBy = "James Gordon" },
        new OperationalEventRecord("EVT-004", "Facility", "FAC-RBWH", "Downtime", "Critical", "Bed tracking module downtime declared.", DateTime.UtcNow.AddMinutes(-40)) with { Title = "System Downtime", IsActive = true, RequiresAcknowledgement = true, SourceSystem = "JARVIS Ops", CreatedBy = "Maria Hill" },
        new OperationalEventRecord("EVT-005", "Ward", "WARD-PAH-GEN", "Staffing", "High", "Ward staffing reduced by two RNs for evening shift.", DateTime.UtcNow.AddHours(-1)) with { Title = "Staffing Impact", IsActive = true, CreatedBy = "Nick Fury" },
        new OperationalEventRecord("EVT-006", "Ward", "WARD-RBWH-ICU", "InfectionControl", "High", "Infection control escalation in ICU.", DateTime.UtcNow.AddHours(-3)) with { Title = "Infection Control Escalation", IsActive = false, EndsAtUtc = DateTime.UtcNow.AddMinutes(-20), CreatedBy = "Jean Grey" }
    ];

    public static IReadOnlyList<OperationalBannerRecord> OperationalBanners { get; } =
    [
        new("OBN-001", "Ward Advisory", "Isolation workflow escalation active.", OperationalEventSeverity.High, OperationalEventScope.Ward, null, null, "FAC-RBWH", "WARD-RBWH-ICU", true, true, true, DateTime.UtcNow.AddHours(-3), null),
        new("OBN-002", "Facility Capacity", "Princess Alexandra Hospital operating at Tier 3 capacity.", OperationalEventSeverity.Critical, OperationalEventScope.Facility, CapacityStatus.Tier3, null, "FAC-PAH", null, false, true, true, DateTime.UtcNow.AddHours(-2), null)
    ];

    public static IReadOnlyList<OperationalTimelineEventRecord> OperationalTimelineEvents { get; } =
    [
        new("OTL-001", "EVT-002", DateTime.UtcNow.AddHours(-2), "Capacity escalated", "Capacity moved to Tier 3.", "Peggy Carter", "Escalation"),
        new("OTL-002", "EVT-004", DateTime.UtcNow.AddMinutes(-35), "Downtime declared", "Downtime protocol activated.", "Maria Hill", "Downtime")
    ];

    public static IReadOnlyList<OperationalEscalationRecord> OperationalEscalations { get; } =
    [
        new("OES-001", "EVT-002", "Tier3", DateTime.UtcNow.AddHours(-2), "Peggy Carter", "Access block threshold exceeded", false, null)
    ];

    public static IReadOnlyList<OperationalImpactRecord> OperationalImpacts { get; } =
    [
        new("OIM-001", "EVT-002", "CapacityLoss", "12 beds unavailable due to access block.", "Facility", "4 hours", false),
        new("OIM-002", "EVT-005", "Staffing", "Reduced elective throughput expected.", "Ward", "8 hours", false)
    ];



    public static IReadOnlyList<ActivityFeedItemRecord> ActivityFeedItems { get; } =
    [
        new("ACT-001", "Bed status changed", "Bed SUR-02 marked Cleaning.", ActivityFeedCategory.BedStatus, ActivityFeedSeverity.Warning, ActivityFeedScope.Bed, DateTime.UtcNow.AddMinutes(-55), "Barbara Gordon", null, "FAC-RBWH", "WARD-RBWH-SUR", "BED-SUR-02", null, null, null, "BedRecord", "BED-SUR-02", true, false, null),
        new("ACT-002", "Patient pre-allocated", "Patient PAT-003 pre-allocated to Bed GEN-03.", ActivityFeedCategory.Allocation, ActivityFeedSeverity.Info, ActivityFeedScope.Allocation, DateTime.UtcNow.AddMinutes(-42), "Peggy Carter", null, "FAC-PAH", "WARD-PAH-GEN", "BED-GEN-03", "PAT-003", "ALL-001", null, "AllocationRecord", "ALL-001", false, true, "Future bed planning action."),
        new("ACT-003", "Operational banner updated", "Facility capacity banner updated to Tier 3.", ActivityFeedCategory.Banner, ActivityFeedSeverity.Critical, ActivityFeedScope.Facility, DateTime.UtcNow.AddMinutes(-35), "Maria Hill", null, "FAC-PAH", null, null, null, null, "EVT-002", "OperationalBannerRecord", "OBN-002", true, true, null),
        new("ACT-004", "Delayed discharge activity", "Community package pending for delayed discharge patient.", ActivityFeedCategory.Discharge, ActivityFeedSeverity.Warning, ActivityFeedScope.Patient, DateTime.UtcNow.AddMinutes(-28), "Stephen Strange", null, "FAC-PAH", "WARD-PAH-GEN", null, "PAT-003", null, null, "PatientDischargeRecord", "DSC-001", false, true, null),
        new("ACT-005", "Transfer readiness updated", "Transfer request moved to Pending Clinical Clearance.", ActivityFeedCategory.Allocation, ActivityFeedSeverity.Info, ActivityFeedScope.Allocation, DateTime.UtcNow.AddMinutes(-22), "Natasha Romanoff", null, "FAC-RBWH", "WARD-RBWH-ICU", null, "PAT-002", null, null, "TransferRequestRecord", "TRF-001", false, false, null),
        new("ACT-006", "Downtime protocol generated", "Downtime workflow initiated for bed tracking module.", ActivityFeedCategory.Downtime, ActivityFeedSeverity.Critical, ActivityFeedScope.Facility, DateTime.UtcNow.AddMinutes(-16), "Nick Fury", null, "FAC-RBWH", null, null, null, null, "EVT-004", "OperationalEventRecord", "EVT-004", true, true, null),
        new("ACT-007", "Operational event escalated", "Capacity event escalated to Tier 3 coordination level.", ActivityFeedCategory.OperationalEvent, ActivityFeedSeverity.Critical, ActivityFeedScope.OperationalEvent, DateTime.UtcNow.AddMinutes(-10), "Jean Grey", null, "FAC-PAH", null, null, null, null, "EVT-002", "OperationalEscalationRecord", "OES-001", false, true, null)
    ];

    

    public static IReadOnlyList<NotificationRecord> Notifications { get; } =
    [
        new("NTF-001", "System update", "PrototypeDataStore seed refreshed for session.", NotificationType.System, NotificationSeverity.Info, NotificationStatus.Read, NotificationAudienceScope.All, DateTime.UtcNow.AddHours(-8), null, DateTime.UtcNow.AddHours(-7), null, false, null, null, null, null, null, null, null, null, "ACT-001", null, null, "OpsHub", "Barbara Gordon", false, true),
        new("NTF-002", "Capacity escalation", "Facility capacity moved to Tier 3.", NotificationType.Operational, NotificationSeverity.Critical, NotificationStatus.Unread, NotificationAudienceScope.Facility, DateTime.UtcNow.AddHours(-2), null, null, null, true, null, null, "FAC-PAH", null, null, null, null, "EVT-002", "ACT-007", "Open event", "/allocation-centre", "OpsHub", "Peggy Carter", true, false),
        new("NTF-003", "Allocation update", "New IHT request awaiting review.", NotificationType.Allocation, NotificationSeverity.Warning, NotificationStatus.Unread, NotificationAudienceScope.Facility, DateTime.UtcNow.AddMinutes(-70), null, null, null, false, null, null, "FAC-RBWH", "WARD-RBWH-SUR", "PAT-004", null, "ALL-001", null, "ACT-005", "Review", "/allocation-centre", "OpsHub", "Natasha Romanoff", false, true),
        new("NTF-004", "Discharge reminder", "Delayed discharge patient has pending external package.", NotificationType.Discharge, NotificationSeverity.Warning, NotificationStatus.Unread, NotificationAudienceScope.Ward, DateTime.UtcNow.AddMinutes(-40), null, null, null, false, null, null, "FAC-PAH", "WARD-PAH-GEN", "PAT-003", null, null, null, "ACT-004", "View patient", "/ward-operations", "OpsHub", "Stephen Strange", false, true),
        new("NTF-005", "Downtime report", "Downtime pack generated for bed tracking outage.", NotificationType.Downtime, NotificationSeverity.Critical, NotificationStatus.Acknowledged, NotificationAudienceScope.Facility, DateTime.UtcNow.AddMinutes(-30), DateTime.UtcNow.AddHours(4), DateTime.UtcNow.AddMinutes(-25), DateTime.UtcNow.AddMinutes(-20), true, null, null, "FAC-RBWH", null, null, null, null, "EVT-004", "ACT-006", "Open downtime", "/bed-management", "JARVIS Ops", "Maria Hill", true, false),
        new("NTF-006", "Ward staffing impact", "Ward staffing impact added for evening shift.", NotificationType.Operational, NotificationSeverity.Warning, NotificationStatus.Unread, NotificationAudienceScope.Ward, DateTime.UtcNow.AddMinutes(-15), null, null, null, false, null, null, "FAC-PAH", "WARD-PAH-GEN", null, null, null, "EVT-005", null, null, null, "OpsHub", "Nick Fury", false, true)
    ];

    

    public static IReadOnlyList<UserPerspectiveRecord> UserPerspectives { get; } =
    [
        new("USP-EXEC-001", "Statewide Executive", "Bruce Wayne", UserPerspectiveType.Executive, UserAccessScope.Statewide, UserWorkflowFocus.Awareness, UserOperationalMode.InsightsOnly, null, null, ["HHS-MN","HHS-MS"], ["FAC-RBWH","FAC-TPCH","FAC-PAH"], [], true, true, true, true, false, false, true, true, true, true, "Comfort", "Executive", "Bruce Wayne", null, "#2F5D8A"),
        new("USP-HHS-001", "HHS Coordinator", "Barbara Gordon", UserPerspectiveType.HHSCoordinator, UserAccessScope.HHS, UserWorkflowFocus.Coordination, UserOperationalMode.HybridOperational, null, null, ["HHS-MN"], ["FAC-RBWH","FAC-TPCH"], ["WARD-RBWH-SUR","WARD-RBWH-ICU","WARD-TPCH-MAT"], false, true, true, true, true, true, true, true, true, false, "Balanced", "Standard", "Barbara Gordon", null, "#3B7A57"),
        new("USP-BED-001", "Facility Bed Manager", "Peggy Carter", UserPerspectiveType.BedManager, UserAccessScope.Facility, UserWorkflowFocus.Orchestration, UserOperationalMode.OperationalCommand, "FAC-PAH", null, ["HHS-MS"], ["FAC-PAH"], ["WARD-PAH-GEN"], false, false, true, true, true, true, true, true, true, false, "Dense", "Operations", "Peggy Carter", null, "#B35C1E"),
        new("USP-WARD-001", "Ward Clinician", "Jean Grey", UserPerspectiveType.WardClinician, UserAccessScope.Ward, UserWorkflowFocus.Workflow, UserOperationalMode.HybridOperational, "FAC-RBWH", "WARD-RBWH-ICU", ["HHS-MN"], ["FAC-RBWH"], ["WARD-RBWH-ICU"], false, false, false, true, false, true, false, true, false, false, "Dense", "Clinical", "Jean Grey", null, "#7A3E9D"),
        new("USP-ALC-001", "Allocation Coordinator", "Natasha Romanoff", UserPerspectiveType.AllocationCoordinator, UserAccessScope.Facility, UserWorkflowFocus.Coordination, UserOperationalMode.OperationalCommand, "FAC-RBWH", null, ["HHS-MN"], ["FAC-RBWH"], ["WARD-RBWH-SUR","WARD-RBWH-ICU"], false, false, true, true, true, true, true, true, true, false, "Dense", "Allocation", "Natasha Romanoff", null, "#A33A3A"),
        new("USP-DDC-001", "Delayed Discharge Coordinator", "Stephen Strange", UserPerspectiveType.DelayedDischargeCoordinator, UserAccessScope.Facility, UserWorkflowFocus.Reporting, UserOperationalMode.HybridOperational, "FAC-PAH", null, ["HHS-MS"], ["FAC-PAH"], ["WARD-PAH-GEN"], false, false, true, true, false, false, true, true, true, false, "Balanced", "DelayedDischarge", "Stephen Strange", null, "#4F6D3A")
    ];

    

    public static IReadOnlyList<ScenarioRecord> Scenarios { get; } =
    [
        new("SCN-001", "ED Demand Surge", "Model ED presentations increasing by 15% over next shift.", ScenarioType.Demand, ScenarioStatus.Active, DateTime.UtcNow.AddHours(-5), "Reed Richards", "HHS-MN", "FAC-RBWH", null, "Next 12 hours", true, true, null),
        new("SCN-002", "Bed Closure Staffing", "Model closure of 10 beds due to staffing constraints.", ScenarioType.Staffing, ScenarioStatus.Reviewed, DateTime.UtcNow.AddHours(-4), "Barbara Gordon", "HHS-MS", "FAC-PAH", null, "Next 24 hours", true, false, null),
        new("SCN-003", "Delayed Discharge Improvement", "Model expedited discharge barrier review reducing delayed discharges.", ScenarioType.Discharge, ScenarioStatus.Active, DateTime.UtcNow.AddHours(-3), "Maria Hill", "HHS-MS", "FAC-PAH", "WARD-PAH-GEN", "Next 24 hours", true, false, null),
        new("SCN-004", "Transit Bed Expansion", "Model opening 4 transit beds for access block mitigation.", ScenarioType.Capacity, ScenarioStatus.Draft, DateTime.UtcNow.AddHours(-2), "Peggy Carter", "HHS-MN", "FAC-RBWH", null, "Tomorrow AM", true, false, null)
    ];

    public static IReadOnlyList<ScenarioInputRecord> ScenarioInputs { get; } =
    [
        new("SCI-001", "SCN-001", "Demand", "ED presentations", 120, 138, "patients/day", "Facility", null),
        new("SCI-002", "SCN-002", "Capacity", "Open beds", 56, 46, "beds", "Facility", "Staffing reduction"),
        new("SCI-003", "SCN-003", "Discharge", "Delayed discharge patients", 18, 13, "patients", "Ward", null),
        new("SCI-004", "SCN-004", "Capacity", "Transit beds", 2, 6, "beds", "Facility", null)
    ];

    public static IReadOnlyList<ScenarioAssumptionRecord> ScenarioAssumptions { get; } =
    [
        new("SCA-001", "SCN-001", "Ambulance surge sustained", "Assumes sustained ambulance arrivals over 12 hours.", ScenarioConfidence.Medium, "Ops workshop"),
        new("SCA-002", "SCN-002", "Agency coverage unavailable", "Assumes no backfill for two RN vacancies.", ScenarioConfidence.High, "Roster planning"),
        new("SCA-003", "SCN-003", "Community package turnaround", "Assumes package approvals improve by one day.", ScenarioConfidence.Medium, "Discharge team"),
        new("SCA-004", "SCN-004", "Transit conversion feasible", "Assumes two treatment spaces can convert to transit beds.", ScenarioConfidence.Low, "Site walkthrough")
    ];

    public static IReadOnlyList<ScenarioResultRecord> ScenarioResults { get; } =
    [
        new("SCR-001", "SCN-001", "ED holds", 18, 31, 13, "patients", NotificationSeverity.Critical, "ED holds increase materially under surge."),
        new("SCR-002", "SCN-002", "Available beds", 12, 4, -8, "beds", NotificationSeverity.Warning, "Capacity cushion narrows significantly."),
        new("SCR-003", "SCN-003", "Delayed discharge pressure", 18, 13, -5, "patients", NotificationSeverity.Success, "Pressure improves with barrier reduction."),
        new("SCR-004", "SCN-004", "Occupancy", 94, 90, -4, "%", NotificationSeverity.Info, "Transit expansion eases occupancy pressure.")
    ];

    public static IReadOnlyList<ScenarioImpactRecord> ScenarioImpacts { get; } =
    [
        new("SCP-001", "SCN-001", ScenarioImpactType.DemandIncrease, OperationalEventScope.Facility, null, "FAC-RBWH", null, NotificationSeverity.Critical, "ED access block risk", "Increased ED queue and admission delays.", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(12)),
        new("SCP-002", "SCN-002", ScenarioImpactType.BedClosure, OperationalEventScope.Facility, null, "FAC-PAH", null, NotificationSeverity.Warning, "Ward closure impact", "Ten beds unavailable due to staffing.", DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(24)),
        new("SCP-003", "SCN-003", ScenarioImpactType.Improvement, OperationalEventScope.Ward, null, "FAC-PAH", "WARD-PAH-GEN", NotificationSeverity.Success, "Discharge improvement", "Reduced delayed discharge cohort.", DateTime.UtcNow.AddHours(4), DateTime.UtcNow.AddHours(24)),
        new("SCP-004", "SCN-004", ScenarioImpactType.CapacityRisk, OperationalEventScope.Facility, null, "FAC-RBWH", null, NotificationSeverity.Info, "Transit bed mitigation", "Improved short-stay throughput.", DateTime.UtcNow.AddHours(8), DateTime.UtcNow.AddHours(24))
    ];

    public static IReadOnlyList<ScenarioActionRecord> ScenarioActions { get; } =
    [
        new("SCT-001", "SCN-001", "Activate Tier 3 escalation", "Activate command coordination for ED surge.", "Escalation", AllocationPriority.Critical, "FAC-RBWH", null, true, true, null),
        new("SCT-002", "SCN-002", "Reduce elective intake", "Temporarily reduce electives while staffing constrained.", "FlowControl", AllocationPriority.High, "FAC-PAH", null, true, false, null),
        new("SCT-003", "SCN-003", "Prioritise barrier review", "Expedite social/discharge barrier huddles.", "Discharge", AllocationPriority.Medium, "FAC-PAH", "WARD-PAH-GEN", true, true, null),
        new("SCT-004", "SCN-004", "Open transit beds", "Convert nominated spaces to transit beds.", "Capacity", AllocationPriority.High, "FAC-RBWH", null, true, false, null)
    ];

    public static IReadOnlyList<InformationBannerRecord> InformationBanners { get; } =
    [
        new("BAN-001", "All", "Info", "Operational Update", "Demo seed data scaffold enabled for staged migration.", true)
    ];
}
