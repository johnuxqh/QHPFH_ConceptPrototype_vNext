namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientNoteRecord(
    string Id,
    string PatientId,
    string NoteType,
    string AuthorName,
    DateTime CreatedAt,
    string Summary,
    bool IsImportant);
