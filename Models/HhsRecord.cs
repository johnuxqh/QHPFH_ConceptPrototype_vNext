namespace QHPFH_ConceptPrototype.Models;

public sealed record HhsRecord
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string? ShortName { get; init; }
    public string? Region { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;

    // Backward compatibility for existing reference-data usage.
    public IReadOnlyList<string> Facilities { get; init; }

    public HhsRecord(string name, IReadOnlyList<string> facilities)
    {
        Id = name;
        Name = name;
        ShortName = name;
        Region = "Queensland";
        SortOrder = 0;
        Facilities = facilities;
    }

    public HhsRecord(string id, string name, string? shortName, string? region, int sortOrder, bool isActive, IReadOnlyList<string>? facilities = null)
    {
        Id = id;
        Name = name;
        ShortName = shortName;
        Region = region;
        SortOrder = sortOrder;
        IsActive = isActive;
        Facilities = facilities ?? [];
    }
}
