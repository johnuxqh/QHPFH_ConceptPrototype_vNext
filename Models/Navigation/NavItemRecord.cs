namespace QHPFH_ConceptPrototype.Models.Navigation;

public sealed record NavItemRecord(
    string Id,
    string Label,
    string? Route,
    string WorkspaceId,
    string SectionId,
    bool IsPrimary,
    IReadOnlyList<string>? VisiblePerspectiveTypes = null);
