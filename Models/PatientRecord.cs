namespace QHPFH_ConceptPrototype.Models;

public sealed record PatientRecord
{
    public string Id { get; init; }
    public string DisplayName { get; init; }
    public int Age { get; init; }
    public string Sex { get; init; }
    public string CurrentWardId { get; init; }
    public string CurrentWardCode => CurrentWardId;
    public string? CurrentBedId { get; init; }
    public PatientFlowStatus FlowStatus { get; init; }
    public PatientRiskStatus RiskStatus { get; init; }
    public bool IsDelayedDischarge { get; init; }
    public bool IsOutlier { get; init; }
    public bool IsInfectionControlFlagged { get; init; }
    public bool HasAllergyAlert { get; init; }
    public DateTime? EstimatedDischargeDate { get; init; }
    public int LengthOfStayDays { get; init; }
    public string? CatchmentStatus { get; init; }

    // Backward compatibility
    public string FullName => DisplayName;
    public string Status => FlowStatus.ToString();
    public bool RequiresIsolation => IsInfectionControlFlagged;
    public bool DelayedDischarge => IsDelayedDischarge;

    public PatientRecord(string id, string fullName, int age, string sex, string currentWardCode, string status, bool requiresIsolation, bool delayedDischarge)
    {
        Id = id;
        DisplayName = fullName;
        Age = age;
        Sex = sex;
        CurrentWardId = currentWardCode;
        FlowStatus = Enum.TryParse<PatientFlowStatus>(status, true, out var parsed) ? parsed : PatientFlowStatus.Admitted;
        RiskStatus = PatientRiskStatus.Stable;
        IsDelayedDischarge = delayedDischarge;
        IsInfectionControlFlagged = requiresIsolation;
    }
}
