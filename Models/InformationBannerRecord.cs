namespace QHPFH_ConceptPrototype.Models;

public sealed record InformationBannerRecord(
    string Id,
    string Audience,
    string Severity,
    string Title,
    string Message,
    bool IsActive);
