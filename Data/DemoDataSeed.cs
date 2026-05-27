using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Data;

public static class DemoDataSeed
{
    public static IReadOnlyList<HhsRecord> HhsRecords { get; } =
    [
        new("Metro North", ["Royal Brisbane and Women's Hospital"]),
        new("Metro South", ["Princess Alexandra Hospital"])
    ];

    public static IReadOnlyList<FacilityRecord> Facilities { get; } =
    [
        new("Metro North", "Royal Brisbane and Women's Hospital"),
        new("Metro South", "Princess Alexandra Hospital")
    ];

    public static IReadOnlyList<WardRecord> Wards { get; } =
    [
        new("Metro North", "Royal Brisbane and Women's Hospital", "SUR", "Surgical", "Surgical operational unit", "Surgical", 32, 30, 27, 3, 90.0m, "High", "Platform matches activity", 4, 3, 1, 5, 4, 1, 1, 2, 1, "08:45"),
        new("Metro South", "Princess Alexandra Hospital", "GEN", "General Medicine", "General medicine operational unit", "Medical", 28, 26, 23, 3, 88.5m, "Medium", "Insufficient staffing", 3, 2, 1, 4, 3, 2, 1, 2, -1, "08:40")
    ];

    public static IReadOnlyList<BedRecord> Beds { get; } =
    [
        new("BED-SUR-01", "SUR", "SUR-01", "Occupied", false, false, "PAT-001"),
        new("BED-SUR-02", "SUR", "SUR-02", "Open", false, false, null),
        new("BED-GEN-01", "GEN", "GEN-01", "Occupied", true, false, "PAT-002")
    ];

    public static IReadOnlyList<PatientRecord> Patients { get; } =
    [
        new("PAT-001", "Clark Kent", 38, "M", "SUR", "Admitted", false, false),
        new("PAT-002", "Diana Prince", 34, "F", "GEN", "Admitted", true, false),
        new("PAT-003", "Bruce Wayne", 45, "M", "GEN", "Queued", false, true)
    ];

    public static IReadOnlyList<OperationalEventRecord> OperationalEvents { get; } =
    [
        new("EVT-001", "Ward", "SUR", "Cleaning", "Medium", "One bed awaiting cleaning clearance.", DateTime.UtcNow.AddMinutes(-25)),
        new("EVT-002", "Facility", "Royal Brisbane and Women's Hospital", "Pressure", "High", "Surgical occupancy remains above 90%.", DateTime.UtcNow.AddMinutes(-10))
    ];

    public static IReadOnlyList<InformationBannerRecord> InformationBanners { get; } =
    [
        new("BAN-001", "All", "Info", "Operational Update", "Demo seed data scaffold enabled for staged migration.", true)
    ];
}
