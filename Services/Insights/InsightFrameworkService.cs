using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Insights;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Kpi;
using QHPFH_ConceptPrototype.Services.Workspace;

namespace QHPFH_ConceptPrototype.Services.Insights;

public sealed class InsightFrameworkService
{
    private readonly KpiCalculationService _kpiCalculation;
    private readonly KpiFrameworkService _kpiFramework;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly WorkspaceDensityEngine _workspaceDensity;
    private readonly PrototypeDataStore _dataStore;

    public InsightFrameworkService(
        KpiCalculationService kpiCalculation,
        KpiFrameworkService kpiFramework,
        ContextAwarenessService contextAwareness,
        AdaptivePerspectiveEngine adaptivePerspective,
        WorkspaceDensityEngine workspaceDensity,
        PrototypeDataStore dataStore)
    {
        _kpiCalculation = kpiCalculation;
        _kpiFramework = kpiFramework;
        _contextAwareness = contextAwareness;
        _adaptivePerspective = adaptivePerspective;
        _workspaceDensity = workspaceDensity;
        _dataStore = dataStore;
    }

    public InsightRecord CreateInsight(
        string id,
        string title,
        string summary,
        string detailedDescription,
        InsightCategory category,
        InsightSeverity severity,
        InsightPriority? priority = null,
        decimal confidence = 0.82m,
        string recommendedAction = "Review operational context",
        string actionLabel = "Review",
        InsightActionType actionType = InsightActionType.ViewContext,
        string? context = null,
        string relatedKpi = "Operational state",
        string trendSignal = "Current signal",
        DateTime? timestamp = null,
        string? statusBadge = null,
        string iconName = "info",
        int affectedCount = 0,
        IReadOnlySet<string>? relatedEntityIds = null)
    {
        return new InsightRecord(
            id,
            title,
            summary,
            detailedDescription,
            category,
            severity,
            priority ?? InferPriority(severity, affectedCount),
            confidence,
            recommendedAction,
            actionLabel,
            actionType,
            context ?? GetCurrentContextLabel(),
            relatedKpi,
            trendSignal,
            timestamp ?? DateTime.Now,
            statusBadge ?? InferStatusBadge(severity, affectedCount),
            iconName,
            affectedCount,
            relatedEntityIds);
    }

    public IReadOnlyList<InsightRecord> GetStatewideOperationalInsights()
    {
        var snapshot = _kpiCalculation.GetStatewideSnapshot();
        var activeEvents = _dataStore.GetActiveOperationalEvents();
        var insights = new List<InsightRecord>();

        if (snapshot.Capacity.OperationalOccupancyPercent >= 90)
        {
            insights.Add(CreateInsight(
                "statewide-capacity-pressure",
                "Capacity pressure is building",
                $"Statewide operational occupancy is {snapshot.Capacity.OperationalOccupancyPercent:0}% with {snapshot.Capacity.AvailableBeds:N0} available beds.",
                "Capacity and available-bed signals indicate constrained system flexibility. Review discharge opportunities and escalation pathways before demand increases.",
                InsightCategory.Capacity,
                snapshot.Capacity.OperationalOccupancyPercent >= 95 ? InsightSeverity.Critical : InsightSeverity.High,
                relatedKpi: "Operational Beds",
                trendSignal: "Occupancy pressure",
                actionLabel: "Review capacity",
                actionType: InsightActionType.ReviewWorkflow,
                affectedCount: snapshot.Capacity.AvailableBeds));
        }

        if (snapshot.Allocation.PendingAllocations > 0)
        {
            insights.Add(CreateInsight(
                "statewide-allocation-queue",
                "Allocation queue needs coordination",
                $"{snapshot.Allocation.PendingAllocations:N0} allocations are waiting for review across the current operational scope.",
                "Pending allocations may reduce bed matching speed. Coordinate allocation reviews against available beds and incoming demand.",
                InsightCategory.Allocation,
                snapshot.Allocation.PendingAllocations > 10 ? InsightSeverity.High : InsightSeverity.Elevated,
                relatedKpi: "Pending Allocations",
                trendSignal: "Queue pressure",
                actionLabel: "Review queue",
                actionType: InsightActionType.Coordinate,
                affectedCount: snapshot.Allocation.PendingAllocations));
        }

        foreach (var operationalEvent in activeEvents.Where(x => x.Severity is OperationalEventSeverity.High or OperationalEventSeverity.Critical).Take(3))
        {
            insights.Add(CreateInsight(
                $"event-{operationalEvent.Id}",
                operationalEvent.Title,
                operationalEvent.Summary,
                operationalEvent.Notes ?? operationalEvent.Summary,
                InsightCategory.OperationalRisk,
                operationalEvent.Severity == OperationalEventSeverity.Critical ? InsightSeverity.Critical : InsightSeverity.High,
                relatedKpi: "Operational Events",
                trendSignal: operationalEvent.Category.ToString(),
                actionLabel: "Open event",
                actionType: InsightActionType.OpenDetails,
                statusBadge: operationalEvent.Severity.ToString(),
                affectedCount: 1));
        }

        return PrioritizeInsights(insights);
    }

    public IReadOnlyList<InsightRecord> PrioritizeInsights(IEnumerable<InsightRecord> insights) =>
        insights
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Severity)
            .ThenByDescending(x => x.AffectedCount)
            .ThenByDescending(x => x.Timestamp)
            .ToList();

    public string GetGridDensityClass() => _workspaceDensity.GetCurrentDensityMode() switch
    {
        WorkspaceDensityMode.Comfortable => "insight-grid-density-comfortable",
        WorkspaceDensityMode.Compact => "insight-grid-density-compact",
        _ => "insight-grid-density-balanced"
    };

    public string GetCardDensityClass() => _workspaceDensity.GetCurrentDensityMode() switch
    {
        WorkspaceDensityMode.Comfortable => "insight-card-density-comfortable",
        WorkspaceDensityMode.Compact => "insight-card-density-compact",
        _ => "insight-card-density-balanced"
    };

    public bool ShouldShowDetailedInsightText() => !_workspaceDensity.ShouldUseCompactCards();

    public bool ShouldPrioritizeActions() =>
        _adaptivePerspective.ShouldShowOperationalActions()
        || _workspaceDensity.ShouldIncreaseOperationalDensity();

    public string GetPerspectiveFocusLabel() =>
        _adaptivePerspective.GetRecommendedOperationalFocus();

    public string GetKpiAwarenessLabel() =>
        _kpiFramework.ShouldPrioritizeActionLabels() ? "Action-ready KPI signal" : "Awareness KPI signal";

    private string GetCurrentContextLabel()
    {
        var context = _contextAwareness.GetCurrentContext();
        return string.IsNullOrWhiteSpace(context.ContextSummary)
            ? "Shared operational context"
            : context.ContextSummary;
    }

    private static InsightPriority InferPriority(InsightSeverity severity, int affectedCount) => severity switch
    {
        InsightSeverity.Critical => InsightPriority.Critical,
        InsightSeverity.High => InsightPriority.HighPriority,
        InsightSeverity.Elevated => InsightPriority.AttentionRequired,
        InsightSeverity.Watch => InsightPriority.Watch,
        _ when affectedCount > 0 => InsightPriority.Watch,
        _ => InsightPriority.Informational
    };

    private static string InferStatusBadge(InsightSeverity severity, int affectedCount) => severity switch
    {
        InsightSeverity.Critical => "Critical",
        InsightSeverity.High => "High priority",
        InsightSeverity.Elevated => "Attention",
        InsightSeverity.Watch => "Watch",
        _ when affectedCount > 0 => "Signal",
        _ => "Information"
    };
}
