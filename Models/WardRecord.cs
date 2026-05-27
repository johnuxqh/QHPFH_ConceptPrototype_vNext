namespace QHPFH_ConceptPrototype.Models;

public sealed record WardRecord
{
    public string Id { get; init; }
    public string FacilityId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public WardType WardType { get; init; }
    public int PhysicalBedCount { get; init; }
    public bool IsOperational { get; init; } = true;
    public bool IsActive { get; init; } = true;
    public int SortOrder { get; init; }

    // Existing properties used by current prototype pages.
    public string Hhs { get; init; }
    public string Facility { get; init; }
    public string WardCode { get; init; }
    public string WardName => Name;
    public string WardDescription => Description;
    public string WardTypeLabel => WardType.ToString();
    public int PhysicalBeds => PhysicalBedCount;
    public int OpenBeds { get; init; }
    public int OccupiedBeds { get; init; }
    public int AvailableBeds { get; init; }
    public decimal OccupancyPercent { get; init; }
    public string PressureStatus { get; init; }
    public string BedReductionReason { get; init; }
    public int ElectiveAdmissionsToday { get; init; }
    public int EdAddOnsToday { get; init; }
    public int HithReturnsToday { get; init; }
    public int PredictedDischargesToday { get; init; }
    public int ActualDischargesToday { get; init; }
    public int DelayedDischargeCount { get; init; }
    public int CleaningAwaiting { get; init; }
    public int NonOperationalBeds { get; init; }
    public int CapacityDelta24h { get; init; }
    public string LastUpdated { get; init; }

    public WardRecord(string hhs, string facility, string wardCode, string wardName, string wardDescription, string wardType, int physicalBeds, int openBeds, int occupiedBeds, int availableBeds, decimal occupancyPercent, string pressureStatus, string bedReductionReason, int electiveAdmissionsToday, int edAddOnsToday, int hithReturnsToday, int predictedDischargesToday, int actualDischargesToday, int delayedDischargeCount, int cleaningAwaiting, int nonOperationalBeds, int capacityDelta24h, string lastUpdated)
    {
        Id = $"{facility}-{wardCode}";
        FacilityId = facility;
        Name = wardName;
        Description = wardDescription;
        WardType = Enum.TryParse<WardType>(wardType.Replace(" ", string.Empty), true, out var wt) ? wt : WardType.Specialty;
        PhysicalBedCount = physicalBeds;
        SortOrder = 0;
        Hhs = hhs;
        Facility = facility;
        WardCode = wardCode;
        OpenBeds = openBeds;
        OccupiedBeds = occupiedBeds;
        AvailableBeds = availableBeds;
        OccupancyPercent = occupancyPercent;
        PressureStatus = pressureStatus;
        BedReductionReason = bedReductionReason;
        ElectiveAdmissionsToday = electiveAdmissionsToday;
        EdAddOnsToday = edAddOnsToday;
        HithReturnsToday = hithReturnsToday;
        PredictedDischargesToday = predictedDischargesToday;
        ActualDischargesToday = actualDischargesToday;
        DelayedDischargeCount = delayedDischargeCount;
        CleaningAwaiting = cleaningAwaiting;
        NonOperationalBeds = nonOperationalBeds;
        CapacityDelta24h = capacityDelta24h;
        LastUpdated = lastUpdated;
    }
}
