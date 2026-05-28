using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Services.Kpi;

namespace QHPFH_ConceptPrototype.Services.Rules;

public sealed class OperationalRulesService
{
    private readonly PrototypeDataStore _store;
    private readonly KpiCalculationService _kpi;

    public OperationalRulesService(PrototypeDataStore store, KpiCalculationService kpi)
    {
        _store = store;
        _kpi = kpi;
    }

    public IReadOnlyList<OperationalRuleResult> EvaluateFacilityCapacity(string facilityId)
    {
        var facility = _store.GetFacilities().FirstOrDefault(x => x.Id == facilityId);
        if (facility is null)
        {
            return [CreateMissingEntityRule("Facility", facilityId, OperationalRuleCategory.Capacity, "Facility")];
        }

        var snapshot = _kpi.GetFacilitySnapshot(facilityId);
        var rules = new List<OperationalRuleResult>();
        AddCapacityRules(rules, snapshot.Capacity.OccupancyPercent, snapshot.Capacity.AvailableBeds, "Facility", facilityId: facilityId);
        AddCapacityStatusOverrides(rules, _store.GetOperationalEventsForFacility(facilityId), "Facility", facilityId: facilityId);
        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> EvaluateWardCapacity(string wardId)
    {
        var ward = _store.GetWards().FirstOrDefault(x => x.Id == wardId);
        if (ward is null)
        {
            return [CreateMissingEntityRule("Ward", wardId, OperationalRuleCategory.Capacity, "Ward")];
        }

        var snapshot = _kpi.GetWardSnapshot(wardId);
        var rules = new List<OperationalRuleResult>();
        AddCapacityRules(rules, snapshot.Capacity.OccupancyPercent, snapshot.Capacity.AvailableBeds, "Ward", facilityId: ward.FacilityId, wardId: wardId);
        AddCapacityStatusOverrides(rules, _store.GetOperationalEventsForWard(wardId), "Ward", facilityId: ward.FacilityId, wardId: wardId);
        return rules;
    }

    public string EvaluateCapacityTier(string facilityId)
    {
        var capacityOverride = _store.GetOperationalEventsForFacility(facilityId)
            .Where(x => x.IsActive && x.CapacityStatus is not null)
            .OrderByDescending(x => x.CapacityStatus)
            .FirstOrDefault();

        if (capacityOverride?.CapacityStatus is not null)
        {
            return capacityOverride.CapacityStatus.Value.ToString();
        }

        var facility = _store.GetFacilities().FirstOrDefault(x => x.Id == facilityId);
        if (facility is null)
        {
            return CapacityStatus.Tier1.ToString();
        }

        var occupancy = _kpi.GetFacilitySnapshot(facilityId).Capacity.OccupancyPercent;
        return occupancy >= 95 ? CapacityStatus.Tier3.ToString() : occupancy >= 90 ? CapacityStatus.Tier2.ToString() : CapacityStatus.Tier1.ToString();
    }

    public OperationalRuleResult CanAllocatePatientToBed(string patientId, string bedId)
    {
        var patient = _store.GetPatientById(patientId);
        var bed = _store.GetBeds().FirstOrDefault(x => x.Id == bedId);
        if (patient is null)
        {
            return CreateMissingEntityRule("Patient", patientId, OperationalRuleCategory.BedAvailability, "Bed", bedId: bedId);
        }

        if (bed is null)
        {
            return CreateMissingEntityRule("Bed", bedId, OperationalRuleCategory.BedAvailability, "Bed", patientId: patientId);
        }

        var blockingReason = GetBlockingBedReason(bed, allowOccupied: false);
        if (blockingReason is not null)
        {
            return Rule(
                "bed-allocation-blocked",
                "Bed cannot accept allocation",
                blockingReason,
                OperationalRuleCategory.BedAvailability,
                OperationalRuleSeverity.Critical,
                "Bed",
                bedId: bed.Id,
                patientId: patient.Id,
                isBlocking: true,
                isActionable: true,
                recommendedAction: "Select another open operational bed or resolve the bed status first.",
                relatedEntityType: "Bed",
                relatedEntityId: bed.Id);
        }

        if (patient.RequiresIsolation && !bed.IsIsolationCapable && bed.BedType is not BedType.Isolation and not BedType.NegativePressure)
        {
            return Rule(
                "bed-isolation-mismatch",
                "Isolation requirement mismatch",
                "Patient is infection-control flagged but the target bed is not isolation capable.",
                OperationalRuleCategory.InfectionControl,
                OperationalRuleSeverity.Warning,
                "Bed",
                bedId: bed.Id,
                patientId: patient.Id,
                isBlocking: true,
                isActionable: true,
                recommendedAction: "Use an isolation-capable bed or confirm infection-control plan before allocation.",
                relatedEntityType: "Patient",
                relatedEntityId: patient.Id);
        }

        return Rule(
            "bed-allocation-eligible",
            "Bed allocation eligible",
            bed.BedType == BedType.Transit ? "Transit bed is open and can be used as a valid allocation target." : "Bed is open for allocation based on current prototype rules.",
            OperationalRuleCategory.BedAvailability,
            OperationalRuleSeverity.Info,
            "Bed",
            bedId: bed.Id,
            patientId: patient.Id,
            relatedEntityType: "Bed",
            relatedEntityId: bed.Id);
    }

    public OperationalRuleResult CanPreAllocatePatientToBed(string patientId, string bedId)
    {
        var patient = _store.GetPatientById(patientId);
        var bed = _store.GetBeds().FirstOrDefault(x => x.Id == bedId);
        if (patient is null)
        {
            return CreateMissingEntityRule("Patient", patientId, OperationalRuleCategory.BedAvailability, "Bed", bedId: bedId);
        }

        if (bed is null)
        {
            return CreateMissingEntityRule("Bed", bedId, OperationalRuleCategory.BedAvailability, "Bed", patientId: patientId);
        }

        var blockingReason = GetBlockingBedReason(bed, allowOccupied: true);
        if (blockingReason is not null)
        {
            return Rule(
                "bed-preallocation-blocked",
                "Bed cannot accept pre-allocation",
                blockingReason,
                OperationalRuleCategory.BedAvailability,
                OperationalRuleSeverity.Critical,
                "Bed",
                bedId: bed.Id,
                patientId: patient.Id,
                isBlocking: true,
                isActionable: true,
                recommendedAction: "Choose another bed or resolve the current bed constraint before pre-allocation.",
                relatedEntityType: "Bed",
                relatedEntityId: bed.Id);
        }

        return Rule(
            "bed-preallocation-eligible",
            "Future bed pre-allocation eligible",
            bed.BedStatus == BedStatus.Occupied ? "Future allocation is allowed even though the bed is currently occupied." : "Bed can be used for future allocation planning.",
            OperationalRuleCategory.BedAvailability,
            OperationalRuleSeverity.Info,
            "Bed",
            bedId: bed.Id,
            patientId: patient.Id,
            relatedEntityType: "Bed",
            relatedEntityId: bed.Id);
    }

    public IReadOnlyList<OperationalRuleResult> EvaluateBedAvailability(string bedId)
    {
        var bed = _store.GetBeds().FirstOrDefault(x => x.Id == bedId);
        if (bed is null)
        {
            return [CreateMissingEntityRule("Bed", bedId, OperationalRuleCategory.BedAvailability, "Bed")];
        }

        var ward = _store.GetWards().FirstOrDefault(x => x.Id == bed.WardId);
        var rules = new List<OperationalRuleResult>();
        var blockingReason = GetBlockingBedReason(bed, allowOccupied: false);
        if (blockingReason is not null)
        {
            rules.Add(Rule(
                $"bed-{bed.BedStatus.ToString().ToLowerInvariant()}",
                "Bed has an allocation constraint",
                blockingReason,
                OperationalRuleCategory.BedAvailability,
                OperationalRuleSeverity.Warning,
                "Bed",
                facilityId: ward?.FacilityId,
                wardId: bed.WardId,
                bedId: bed.Id,
                isBlocking: true,
                isActionable: true,
                recommendedAction: "Resolve bed status or use future pre-allocation where clinically and operationally appropriate.",
                relatedEntityType: "Bed",
                relatedEntityId: bed.Id));
        }
        else
        {
            rules.Add(Rule(
                "bed-open-operational",
                "Bed is available",
                bed.BedType == BedType.Transit ? "Transit bed is available for short-stay flow coordination." : "Bed is open and operationally available.",
                OperationalRuleCategory.BedAvailability,
                OperationalRuleSeverity.Info,
                "Bed",
                facilityId: ward?.FacilityId,
                wardId: bed.WardId,
                bedId: bed.Id,
                relatedEntityType: "Bed",
                relatedEntityId: bed.Id));
        }

        if (bed.IsIsolationCapable || bed.BedType is BedType.Isolation or BedType.NegativePressure)
        {
            rules.Add(Rule(
                "bed-isolation-capable",
                "Isolation capable bed",
                "Bed can support infection-control allocation workflows.",
                OperationalRuleCategory.InfectionControl,
                OperationalRuleSeverity.Advisory,
                "Bed",
                facilityId: ward?.FacilityId,
                wardId: bed.WardId,
                bedId: bed.Id,
                relatedEntityType: "Bed",
                relatedEntityId: bed.Id));
        }

        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> EvaluateAllocationRequest(string allocationId)
    {
        var allocation = _store.GetAllocations().FirstOrDefault(x => x.Id == allocationId);
        if (allocation is null)
        {
            var request = _store.GetAllocationRequests().FirstOrDefault(x => x.Id == allocationId);
            return request is null
                ? [CreateMissingEntityRule("Allocation", allocationId, OperationalRuleCategory.Allocation, "Allocation")]
                : EvaluateAllocationRequestRecord(request);
        }

        var rules = new List<OperationalRuleResult>();
        if ((allocation.Priority is AllocationPriority.High or AllocationPriority.Critical) && (allocation.Status is AllocationStatus.Waiting or AllocationStatus.PendingReview))
        {
            rules.Add(Rule(
                "allocation-high-priority-waiting",
                "High-priority allocation awaiting action",
                $"{allocation.SourceType} allocation is {allocation.Priority} priority and still {allocation.Status}.",
                OperationalRuleCategory.Allocation,
                allocation.Priority == AllocationPriority.Critical ? OperationalRuleSeverity.Critical : OperationalRuleSeverity.Warning,
                "Allocation",
                facilityId: allocation.FacilityId,
                wardId: allocation.WardId,
                patientId: allocation.PatientId,
                allocationId: allocation.Id,
                isActionable: true,
                recommendedAction: "Review allocation suitability and confirm target bed plan.",
                relatedEntityType: "Allocation",
                relatedEntityId: allocation.Id));
        }

        var targetBedId = allocation.TargetBedId ?? allocation.FutureBedId;
        if (!string.IsNullOrWhiteSpace(targetBedId))
        {
            AddBedMatchRules(rules, targetBedId, allocation.PatientId, allocation.RequiresIsolation, allocation.RequiredBedType, allocation.Id, allocation.FacilityId, allocation.WardId);
        }

        if (allocation.IsFutureAllocation || allocation.IsPreAllocation || allocation.Status == AllocationStatus.PreAllocated)
        {
            rules.Add(Rule(
                "allocation-future-planning",
                "Future allocation plan exists",
                "Allocation supports pre-allocation/future-bed planning and may target a currently occupied bed.",
                OperationalRuleCategory.Allocation,
                OperationalRuleSeverity.Advisory,
                "Allocation",
                facilityId: allocation.FacilityId,
                wardId: allocation.WardId,
                patientId: allocation.PatientId,
                allocationId: allocation.Id,
                relatedEntityType: "Allocation",
                relatedEntityId: allocation.Id));
        }

        if (rules.Count == 0)
        {
            rules.Add(Rule(
                "allocation-no-active-warnings",
                "No allocation warnings",
                "No prototype allocation warnings were found for this allocation.",
                OperationalRuleCategory.Allocation,
                OperationalRuleSeverity.Info,
                "Allocation",
                facilityId: allocation.FacilityId,
                wardId: allocation.WardId,
                patientId: allocation.PatientId,
                allocationId: allocation.Id,
                relatedEntityType: "Allocation",
                relatedEntityId: allocation.Id));
        }

        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> GetAllocationWarnings(string allocationId) =>
        EvaluateAllocationRequest(allocationId).Where(x => x.Severity is OperationalRuleSeverity.Warning or OperationalRuleSeverity.Critical || x.IsBlocking).ToList();

    public IReadOnlyList<OperationalRuleResult> EvaluatePatientFlow(string patientId)
    {
        var patient = _store.GetPatientById(patientId);
        if (patient is null)
        {
            return [CreateMissingEntityRule("Patient", patientId, OperationalRuleCategory.PatientFlow, "Patient")];
        }

        var rules = new List<OperationalRuleResult>();
        var ward = _store.GetWards().FirstOrDefault(x => x.Id == patient.CurrentWardId);
        if (patient.IsDelayedDischarge)
        {
            rules.Add(Rule("patient-delayed-discharge", "Delayed discharge flag", "Patient is flagged as delayed discharge.", OperationalRuleCategory.Discharge, OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Review discharge barriers and escalation options.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        if (patient.IsOutlier)
        {
            rules.Add(Rule("patient-outlier", "Outlier patient", "Patient is outside expected ward/care stream for current flow planning.", OperationalRuleCategory.PatientFlow, OperationalRuleSeverity.Advisory, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Review repatriation or transfer plan.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        if (patient.IsInfectionControlFlagged)
        {
            rules.Add(Rule("patient-infection-control", "Infection-control flag", "Patient requires infection-control-aware bed and workflow planning.", OperationalRuleCategory.InfectionControl, OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Confirm isolation capability and infection-control plan.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        if (patient.RiskStatus is PatientRiskStatus.AtRisk or PatientRiskStatus.Critical)
        {
            rules.Add(Rule("patient-risk-status", "Patient risk status requires attention", $"Patient risk status is {patient.RiskStatus}.", OperationalRuleCategory.PatientFlow, patient.RiskStatus == PatientRiskStatus.Critical ? OperationalRuleSeverity.Critical : OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Prioritise operational review for flow and care-team coordination.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        var pendingTasks = _store.GetPatientTasks(patient.Id).Count(x => x.Status is PatientTaskStatus.Pending or PatientTaskStatus.InProgress or PatientTaskStatus.Blocked);
        var pendingResults = _store.GetPatientResults().Count(x => x.PatientId == patient.Id && x.Status is PatientResultStatus.Pending or PatientResultStatus.InProgress);
        if (pendingTasks > 0 || pendingResults > 0)
        {
            rules.Add(Rule("patient-workflow-blockers", "Patient has pending workflow items", $"Pending workflow items: {pendingTasks} task(s), {pendingResults} result(s).", OperationalRuleCategory.PatientFlow, OperationalRuleSeverity.Advisory, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Review outstanding tasks/results before transfer or discharge planning.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        if (rules.Count == 0)
        {
            rules.Add(Rule("patient-flow-no-active-warnings", "No patient flow warnings", "No prototype patient-flow warnings were found for this patient.", OperationalRuleCategory.PatientFlow, OperationalRuleSeverity.Info, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> EvaluateDischargeReadiness(string patientId)
    {
        var patient = _store.GetPatientById(patientId);
        if (patient is null)
        {
            return [CreateMissingEntityRule("Patient", patientId, OperationalRuleCategory.Discharge, "Patient")];
        }

        var ward = _store.GetWards().FirstOrDefault(x => x.Id == patient.CurrentWardId);
        var discharge = _store.GetPatientDischarge(patientId);
        var rules = new List<OperationalRuleResult>();
        if (discharge is null)
        {
            rules.Add(Rule("discharge-no-record", "No discharge workflow record", "No shared discharge workflow record is available for this patient yet.", OperationalRuleCategory.Discharge, OperationalRuleSeverity.Info, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, relatedEntityType: "Patient", relatedEntityId: patient.Id));
            return rules;
        }

        if (discharge.IsDelayed || patient.IsDelayedDischarge)
        {
            rules.Add(Rule("discharge-delayed", "Delayed discharge barrier", discharge.DelayReason ?? discharge.WaitingFor, OperationalRuleCategory.Discharge, OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Escalate discharge barrier review and update discharge progress.", relatedEntityType: "Discharge", relatedEntityId: discharge.Id));
        }

        if (discharge.DischargeProgress == DischargeProgressStatus.MedicallyReady && !string.IsNullOrWhiteSpace(discharge.WaitingFor))
        {
            rules.Add(Rule("discharge-medically-ready-blocked", "Medically ready but blocked", $"Patient is medically ready and waiting for {discharge.WaitingFor}.", OperationalRuleCategory.Discharge, OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Coordinate external barrier resolution and update estimated discharge timing.", relatedEntityType: "Discharge", relatedEntityId: discharge.Id));
        }

        var blockingTasks = _store.GetPatientTasks(patientId).Count(x => x.Status == PatientTaskStatus.Blocked);
        if (blockingTasks > 0)
        {
            rules.Add(Rule("discharge-task-blockers", "Outstanding task blockers", $"{blockingTasks} discharge-related task(s) are blocked.", OperationalRuleCategory.Discharge, OperationalRuleSeverity.Warning, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, isActionable: true, recommendedAction: "Resolve blocked tasks before confirming discharge readiness.", relatedEntityType: "Patient", relatedEntityId: patient.Id));
        }

        if (rules.Count == 0)
        {
            rules.Add(Rule("discharge-ready", "No discharge blockers", "No prototype discharge-readiness blockers were found.", OperationalRuleCategory.Discharge, OperationalRuleSeverity.Info, "Patient", facilityId: ward?.FacilityId, wardId: patient.CurrentWardId, patientId: patient.Id, relatedEntityType: "Discharge", relatedEntityId: discharge.Id));
        }

        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> GetActiveCriticalOperationalRules() =>
        _store.GetOperationalEvents()
            .Where(x => x.IsActive && x.Severity == OperationalEventSeverity.Critical)
            .Select(ToOperationalEventRule)
            .ToList();

    public IReadOnlyList<OperationalRuleResult> EvaluateOperationalEventsForFacility(string facilityId) =>
        _store.GetOperationalEventsForFacility(facilityId)
            .Where(x => x.IsActive)
            .Select(ToOperationalEventRule)
            .ToList();

    public IReadOnlyList<OperationalRuleResult> EvaluateOperationalEventsForWard(string wardId) =>
        _store.GetOperationalEventsForWard(wardId)
            .Where(x => x.IsActive)
            .Select(ToOperationalEventRule)
            .ToList();

    public bool CanPerspectiveManageBeds(string perspectiveId) =>
        _store.GetUserPerspectives().FirstOrDefault(x => x.Id == perspectiveId)?.CanManageBeds == true;

    public bool CanPerspectiveManageAllocations(string perspectiveId) =>
        _store.GetUserPerspectives().FirstOrDefault(x => x.Id == perspectiveId)?.CanManageAllocations == true;

    public bool CanPerspectiveViewReports(string perspectiveId) =>
        _store.GetUserPerspectives().FirstOrDefault(x => x.Id == perspectiveId)?.CanViewReports == true;

    public bool CanPerspectiveManageOperationalEvents(string perspectiveId) =>
        _store.GetUserPerspectives().FirstOrDefault(x => x.Id == perspectiveId)?.CanManageOperationalEvents == true;

    public IReadOnlyList<OperationalRuleResult> EvaluateScenarioActions(string scenarioId)
    {
        var scenario = _store.GetScenarioById(scenarioId);
        if (scenario is null)
        {
            return [CreateMissingEntityRule("Scenario", scenarioId, OperationalRuleCategory.Scenario, "Scenario")];
        }

        var actions = _store.GetScenarioActions(scenarioId);
        var rules = new List<OperationalRuleResult>();
        if (scenario.Status == ScenarioStatus.Active && actions.Any(x => x.IsRecommended) && !actions.Any(x => x.IsSelected))
        {
            rules.Add(Rule("scenario-recommended-action-not-selected", "Recommended scenario action not selected", "Scenario has recommended actions that have not been selected for the workshop plan.", OperationalRuleCategory.Scenario, OperationalRuleSeverity.Advisory, "Scenario", facilityId: scenario.FacilityId, wardId: scenario.WardId, isActionable: true, recommendedAction: "Review recommended scenario actions and select any agreed operational response.", relatedEntityType: "Scenario", relatedEntityId: scenario.Id));
        }

        foreach (var action in actions.Where(x => (x.Priority is AllocationPriority.High or AllocationPriority.Critical) && x.IsRecommended))
        {
            rules.Add(Rule("scenario-high-priority-action", "High-priority scenario action", action.Description, OperationalRuleCategory.Scenario, action.Priority == AllocationPriority.Critical ? OperationalRuleSeverity.Critical : OperationalRuleSeverity.Warning, "Scenario", facilityId: action.TargetFacilityId ?? scenario.FacilityId, wardId: action.TargetWardId ?? scenario.WardId, isActionable: true, recommendedAction: action.Title, relatedEntityType: "ScenarioAction", relatedEntityId: action.Id));
        }

        if (rules.Count == 0)
        {
            rules.Add(Rule("scenario-no-active-warnings", "No scenario warnings", "No prototype scenario action warnings were found.", OperationalRuleCategory.Scenario, OperationalRuleSeverity.Info, "Scenario", facilityId: scenario.FacilityId, wardId: scenario.WardId, relatedEntityType: "Scenario", relatedEntityId: scenario.Id));
        }

        return rules;
    }

    public IReadOnlyList<OperationalRuleResult> GetScenarioWarnings(string scenarioId) =>
        EvaluateScenarioActions(scenarioId).Where(x => x.Severity is OperationalRuleSeverity.Warning or OperationalRuleSeverity.Critical || x.IsActionable).ToList();

    private IReadOnlyList<OperationalRuleResult> EvaluateAllocationRequestRecord(AllocationRequestRecord request)
    {
        var rules = new List<OperationalRuleResult>();
        if ((request.Priority is AllocationPriority.High or AllocationPriority.Critical) && (request.Status is AllocationStatus.Waiting or AllocationStatus.PendingReview))
        {
            rules.Add(Rule("allocation-request-high-priority", "High-priority request awaiting review", $"Request from {request.RequestedBy} is {request.Priority} priority and still {request.Status}.", OperationalRuleCategory.Allocation, request.Priority == AllocationPriority.Critical ? OperationalRuleSeverity.Critical : OperationalRuleSeverity.Warning, "Allocation", wardId: request.PreferredWardId, allocationId: request.Id, isActionable: true, recommendedAction: "Review request and confirm preferred ward/bed suitability.", relatedEntityType: "AllocationRequest", relatedEntityId: request.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.PreferredBedId))
        {
            AddBedMatchRules(rules, request.PreferredBedId, request.PatientId, request.RequiresIsolation, request.RequiredBedType, request.Id, null, request.PreferredWardId);
        }

        if (rules.Count == 0)
        {
            rules.Add(Rule("allocation-request-no-active-warnings", "No allocation request warnings", "No prototype allocation-request warnings were found.", OperationalRuleCategory.Allocation, OperationalRuleSeverity.Info, "Allocation", wardId: request.PreferredWardId, allocationId: request.Id, relatedEntityType: "AllocationRequest", relatedEntityId: request.Id));
        }

        return rules;
    }

    private void AddCapacityRules(List<OperationalRuleResult> rules, decimal occupancy, int availableBeds, string scope, string? facilityId = null, string? wardId = null)
    {
        if (occupancy >= 95)
        {
            rules.Add(Rule("capacity-critical", "Critical capacity pressure", $"{scope} occupancy is {occupancy}%.", OperationalRuleCategory.Capacity, OperationalRuleSeverity.Critical, scope, facilityId: facilityId, wardId: wardId, isActionable: true, recommendedAction: "Escalate capacity response and review discharge/allocation options."));
        }
        else if (occupancy >= 90)
        {
            rules.Add(Rule("capacity-warning", "Capacity pressure warning", $"{scope} occupancy is {occupancy}%.", OperationalRuleCategory.Capacity, OperationalRuleSeverity.Warning, scope, facilityId: facilityId, wardId: wardId, isActionable: true, recommendedAction: "Review bed turnarounds, pending discharges, and incoming demand."));
        }
        else
        {
            rules.Add(Rule("capacity-stable", "Capacity within prototype threshold", $"{scope} occupancy is {occupancy}%.", OperationalRuleCategory.Capacity, OperationalRuleSeverity.Info, scope, facilityId: facilityId, wardId: wardId));
        }

        if (availableBeds <= 2)
        {
            rules.Add(Rule("capacity-low-open-beds", "Low open operational bed count", $"{scope} has {availableBeds} open bed(s) available.", OperationalRuleCategory.Capacity, availableBeds == 0 ? OperationalRuleSeverity.Critical : OperationalRuleSeverity.Warning, scope, facilityId: facilityId, wardId: wardId, isActionable: true, recommendedAction: "Prioritise allocation review, cleaning turnaround, and discharge barriers."));
        }
    }

    private void AddCapacityStatusOverrides(List<OperationalRuleResult> rules, IEnumerable<OperationalEventRecord> events, string scope, string? facilityId = null, string? wardId = null)
    {
        foreach (var operationalEvent in events.Where(x => x.IsActive && x.CapacityStatus is not null))
        {
            rules.Add(Rule("capacity-status-override", "Active capacity status override", $"Active operational event sets capacity status to {operationalEvent.CapacityStatus}.", OperationalRuleCategory.Capacity, MapSeverity(operationalEvent.Severity), scope, facilityId: facilityId ?? operationalEvent.FacilityId, wardId: wardId ?? operationalEvent.WardId, isActionable: operationalEvent.Severity is OperationalEventSeverity.High or OperationalEventSeverity.Critical, recommendedAction: "Follow active operational escalation and capacity coordination process.", relatedEntityType: "OperationalEvent", relatedEntityId: operationalEvent.Id));
        }
    }

    private void AddBedMatchRules(List<OperationalRuleResult> rules, string bedId, string? patientId, bool requiresIsolation, BedType? requiredBedType, string allocationId, string? facilityId, string? wardId)
    {
        var bed = _store.GetBeds().FirstOrDefault(x => x.Id == bedId);
        if (bed is null)
        {
            rules.Add(CreateMissingEntityRule("Bed", bedId, OperationalRuleCategory.Allocation, "Allocation", facilityId: facilityId, wardId: wardId, patientId: patientId, allocationId: allocationId));
            return;
        }

        if (requiresIsolation && !bed.IsIsolationCapable && bed.BedType is not BedType.Isolation and not BedType.NegativePressure)
        {
            rules.Add(Rule("allocation-isolation-mismatch", "Isolation mismatch", "Allocation requires isolation but target bed is not isolation capable.", OperationalRuleCategory.InfectionControl, OperationalRuleSeverity.Warning, "Allocation", facilityId: facilityId, wardId: wardId ?? bed.WardId, bedId: bed.Id, patientId: patientId, allocationId: allocationId, isBlocking: true, isActionable: true, recommendedAction: "Select an isolation-capable bed or confirm infection-control workaround.", relatedEntityType: "Bed", relatedEntityId: bed.Id));
        }

        if (requiredBedType is not null && bed.BedType != requiredBedType)
        {
            rules.Add(Rule("allocation-bed-type-mismatch", "Bed type mismatch", $"Allocation requests {requiredBedType} but target bed is {bed.BedType}.", OperationalRuleCategory.Allocation, OperationalRuleSeverity.Advisory, "Allocation", facilityId: facilityId, wardId: wardId ?? bed.WardId, bedId: bed.Id, patientId: patientId, allocationId: allocationId, isActionable: true, recommendedAction: "Confirm bed-type suitability before accepting allocation.", relatedEntityType: "Bed", relatedEntityId: bed.Id));
        }
    }

    private OperationalRuleResult ToOperationalEventRule(OperationalEventRecord operationalEvent)
    {
        var category = operationalEvent.Category switch
        {
            OperationalEventCategory.Staffing => OperationalRuleCategory.Staffing,
            OperationalEventCategory.InfectionControl => OperationalRuleCategory.InfectionControl,
            _ => OperationalRuleCategory.OperationalEvent
        };

        var action = operationalEvent.Category switch
        {
            OperationalEventCategory.Downtime => "Use downtime workflow and monitor recovery timeline.",
            OperationalEventCategory.Staffing => "Review staffing impact and operational capacity assumptions.",
            OperationalEventCategory.InfectionControl => "Review affected wards and isolation capacity.",
            _ => operationalEvent.RequiresAcknowledgement ? "Acknowledge and coordinate response." : "Monitor event and coordinate operational response as required."
        };

        return Rule(
            "operational-event-active",
            operationalEvent.Title,
            operationalEvent.Summary,
            category,
            MapSeverity(operationalEvent.Severity),
            operationalEvent.Scope.ToString(),
            hhsId: operationalEvent.HhsId,
            facilityId: operationalEvent.FacilityId,
            wardId: operationalEvent.WardId,
            isActionable: operationalEvent.Severity is OperationalEventSeverity.High or OperationalEventSeverity.Critical || operationalEvent.RequiresAcknowledgement,
            recommendedAction: action,
            relatedEntityType: "OperationalEvent",
            relatedEntityId: operationalEvent.Id);
    }

    private static string? GetBlockingBedReason(BedRecord bed, bool allowOccupied) => bed.BedStatus switch
    {
        BedStatus.Blocked => "Bed is blocked and cannot accept allocation until the block is resolved.",
        BedStatus.Closed => "Bed is closed and cannot accept allocation.",
        BedStatus.Maintenance => "Bed is under maintenance and cannot accept allocation.",
        BedStatus.Cleaning => "Bed is cleaning and not ready for immediate allocation.",
        BedStatus.Occupied when !allowOccupied => "Bed is occupied. Use pre-allocation if planning for a future discharge.",
        _ when !bed.IsOpenOperationally && bed.BedStatus != BedStatus.Occupied => "Bed is not currently open operationally.",
        _ => null
    };

    private static OperationalRuleSeverity MapSeverity(OperationalEventSeverity severity) => severity switch
    {
        OperationalEventSeverity.Critical => OperationalRuleSeverity.Critical,
        OperationalEventSeverity.High => OperationalRuleSeverity.Warning,
        OperationalEventSeverity.Moderate => OperationalRuleSeverity.Advisory,
        _ => OperationalRuleSeverity.Info
    };

    private static OperationalRuleResult CreateMissingEntityRule(string entityType, string entityId, OperationalRuleCategory category, string scope, string? facilityId = null, string? wardId = null, string? bedId = null, string? patientId = null, string? allocationId = null) =>
        Rule(
            $"missing-{entityType.ToLowerInvariant()}",
            $"{entityType} not found",
            $"No {entityType.ToLowerInvariant()} record exists for '{entityId}' in the shared prototype store.",
            category,
            OperationalRuleSeverity.Critical,
            scope,
            facilityId: facilityId,
            wardId: wardId,
            bedId: bedId,
            patientId: patientId,
            allocationId: allocationId,
            isBlocking: true,
            relatedEntityType: entityType,
            relatedEntityId: entityId);

    private static OperationalRuleResult Rule(
        string id,
        string title,
        string summary,
        OperationalRuleCategory category,
        OperationalRuleSeverity severity,
        string scope,
        string? hhsId = null,
        string? facilityId = null,
        string? wardId = null,
        string? bedId = null,
        string? patientId = null,
        string? allocationId = null,
        bool isBlocking = false,
        bool isActionable = false,
        string? recommendedAction = null,
        string? relatedEntityType = null,
        string? relatedEntityId = null) =>
        new(id, title, summary, category, severity, scope, hhsId, facilityId, wardId, bedId, patientId, allocationId, isBlocking, isActionable, recommendedAction, relatedEntityType, relatedEntityId);
}
