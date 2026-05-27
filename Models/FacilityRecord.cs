namespace QHPFH_ConceptPrototype.Models;

public sealed record FacilityRecord
{
    public string Id { get; init; }
    public string HhsId { get; init; }
    public string Name { get; init; }
    public string? ShortName { get; init; }
    public FacilityType FacilityType { get; init; }
    public bool IsActive { get; init; } = true;
    public int SortOrder { get; init; }

    // Backward compatibility for existing model access.
    public string Hhs => HhsId;
    public string Facility => Name;

    public FacilityRecord(string hhs, string facility)
    {
        Id = facility;
        HhsId = hhs;
        Name = facility;
        ShortName = facility;
        FacilityType = FacilityType.Hospital;
        SortOrder = 0;
    }

    public FacilityRecord(string id, string hhsId, string name, string? shortName, FacilityType facilityType, bool isActive, int sortOrder)
    {
        Id = id;
        HhsId = hhsId;
        Name = name;
        ShortName = shortName;
        FacilityType = facilityType;
        IsActive = isActive;
        SortOrder = sortOrder;
    }
}
