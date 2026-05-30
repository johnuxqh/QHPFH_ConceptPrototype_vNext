namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterSyncEvent(
    string WorkspaceId,
    FilterSelectionState Selection,
    FilterPersistenceMode PersistenceMode,
    FilterSyncMode SyncMode,
    FilterSyncScope SyncScope,
    string ContextSummary);
