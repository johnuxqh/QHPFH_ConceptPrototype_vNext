using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Data;

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
        new("EVT-001", "Ward", "WARD-RBWH-ICU", "Cleaning", "Medium", "One bed awaiting cleaning clearance.", DateTime.UtcNow.AddMinutes(-25)),
        new("EVT-002", "Facility", "FAC-RBWH", "Pressure", "High", "ICU occupancy remains above 90%.", DateTime.UtcNow.AddMinutes(-10))
    ];

    public static IReadOnlyList<InformationBannerRecord> InformationBanners { get; } =
    [
        new("BAN-001", "All", "Info", "Operational Update", "Demo seed data scaffold enabled for staged migration.", true)
    ];
}
