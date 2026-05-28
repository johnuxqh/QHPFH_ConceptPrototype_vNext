namespace QHPFH_ConceptPrototype.Models;

public sealed record UserAccessContextRecord(
    string? HhsId,
    string? FacilityId,
    string? WardId,
    UserAccessScope AccessScope);
