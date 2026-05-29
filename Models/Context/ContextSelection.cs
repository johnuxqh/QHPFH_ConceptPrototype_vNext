namespace QHPFH_ConceptPrototype.Models.Context;

public sealed record ContextSelection(
    string? CurrentWorkspace,
    string? CurrentPageTitle,
    string? CurrentPatientId,
    string? CurrentBedId,
    string? CurrentAllocationId,
    string? CurrentTransferId,
    string? CurrentWorkflowId);
