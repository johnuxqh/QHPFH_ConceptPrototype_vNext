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
        new("Metro North", "Royal Brisbane and Women's Hospital", "SUR", "Surgical", "Surgical operational unit", "Surgical", 32, 30, 27, 3, 90.0m, "High", "Platform matches activity", 4, 3, 1, 5, 4, 1, 1, 2, 1, "08:45") with { Id = "WARD-RBWH-SUR", FacilityId = "FAC-RBWH", SortOrder = 1, IsOperational = true, IsActive = true },
        new("Metro North", "Royal Brisbane and Women's Hospital", "ICU", "Intensive Care", "Critical care unit", "CriticalCare", 16, 14, 13, 1, 92.8m, "High", "Acuity demand", 1, 2, 0, 1, 1, 0, 1, 2, 0, "08:41") with { Id = "WARD-RBWH-ICU", FacilityId = "FAC-RBWH", SortOrder = 2, IsOperational = true, IsActive = true },
        new("Metro South", "Princess Alexandra Hospital", "GEN", "General Medicine", "General medicine operational unit", "Medical", 28, 26, 23, 3, 88.5m, "Medium", "Insufficient staffing", 3, 2, 1, 4, 3, 2, 1, 2, -1, "08:40") with { Id = "WARD-PAH-GEN", FacilityId = "FAC-PAH", SortOrder = 1, IsOperational = true, IsActive = true },
        new("Metro North", "The Prince Charles Hospital", "MAT", "Maternity Overflow", "Ward retained with no active patient data", "Maternity", 10, 0, 0, 0, 0m, "Low", "Temporarily non-operational", 0, 0, 0, 0, 0, 0, 0, 10, 0, "08:30") with { Id = "WARD-TPCH-MAT", FacilityId = "FAC-TPCH", SortOrder = 1, IsOperational = false, IsActive = true }
    ];

    public static IReadOnlyList<BedRecord> Beds { get; } =
    [
        new("BED-SUR-01", "WARD-RBWH-SUR", "SUR-01", "Occupied", false, false, "PAT-001") with { BedType = BedType.Standard, SortOrder = 1 },
        new("BED-SUR-02", "WARD-RBWH-SUR", "SUR-02", "Open", false, false, null) with { BedType = BedType.Standard, SortOrder = 2 },
        new("BED-ICU-01", "WARD-RBWH-ICU", "ICU-01", "Occupied", true, false, "PAT-002") with { BedType = BedType.Isolation, IsSpecialistBed = true, IsIsolationCapable = true, IsNegativePressure = true, SortOrder = 1 },
        new("BED-ICU-02", "WARD-RBWH-ICU", "ICU-02", "Cleaning", false, false, null) with { BedType = BedType.CriticalCare, SortOrder = 2 },
        new("BED-GEN-01", "WARD-PAH-GEN", "GEN-01", "Maintenance", false, false, null) with { BedType = BedType.Standard, MaintenanceReason = "Hydraulic rail service", IsOpenOperationally = false, SortOrder = 1 },
        new("BED-GEN-02", "WARD-PAH-GEN", "GEN-02", "Blocked", false, true, null) with { ClosureReason = "Staffing constraint", IsOpenOperationally = false, SortOrder = 2 },
        new("BED-GEN-03", "WARD-PAH-GEN", "GEN-03", "FutureAllocated", false, false, null) with { FutureAllocatedPatientId = "PAT-003", BedType = BedType.Transit, SortOrder = 3 }
    ];

    public static IReadOnlyList<PatientRecord> Patients { get; } =
    [
        new("PAT-001", "Clark Kent", 38, "M", "SUR", "Admitted", false, false),
        new("PAT-002", "Diana Prince", 34, "F", "ICU", "Admitted", true, false),
        new("PAT-003", "Bruce Wayne", 45, "M", "GEN", "Queued", false, true)
    ];

    public static IReadOnlyList<AdmissionRecord> Admissions { get; } =
    [
        new("ADM-001", "PAT-003", "ED", "Queued", DateTime.UtcNow.AddMinutes(-35), DateTime.UtcNow.AddMinutes(20))
    ];

    public static IReadOnlyList<AllocationRecord> Allocations { get; } =
    [
        new("ALL-001", "PAT-003", "Princess Alexandra Hospital", "GEN", "High", "Pending", DateTime.UtcNow.AddMinutes(-12))
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
