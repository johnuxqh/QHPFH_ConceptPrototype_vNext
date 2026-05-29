namespace QHPFH_ConceptPrototype.Models.Navigation;

public sealed record NavSectionRecord(
    string Id,
    string Label,
    string WorkspaceId,
    IReadOnlyList<NavItemRecord> Items);
