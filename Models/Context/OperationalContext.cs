using QHPFH_ConceptPrototype.Models;

namespace QHPFH_ConceptPrototype.Models.Context;

public sealed record OperationalContext(
    ContextLocation Location,
    UserPerspectiveRecord? CurrentPerspective,
    PrototypeExperienceMode CurrentExperienceMode,
    PrototypeLayoutVariant CurrentLayoutVariant,
    ContextSelection Selection)
{
    public string ContextSummary
    {
        get
        {
            var parts = new[]
            {
                CurrentPerspective?.DisplayName,
                Location.SummaryText,
                Selection.CurrentWorkspace
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            return string.Join(" > ", parts);
        }
    }
}
