namespace QHPFH_ConceptPrototype.Models;

public enum ExperienceInformationMode
{
    InsightFirst,
    Balanced,
    WorkflowFirst
}

public static class ExperienceInformationModeExtensions
{
    public static string ToDisplayName(this ExperienceInformationMode informationMode) => informationMode switch
    {
        ExperienceInformationMode.InsightFirst => "Insight-first information",
        ExperienceInformationMode.Balanced => "Balanced information",
        ExperienceInformationMode.WorkflowFirst => "Workflow-first information",
        _ => informationMode.ToString()
    };
}
