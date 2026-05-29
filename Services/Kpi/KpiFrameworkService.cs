using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Context;
using QHPFH_ConceptPrototype.Models.Kpi;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Workspace;

namespace QHPFH_ConceptPrototype.Services.Kpi;

public sealed class KpiFrameworkService
{
    private readonly KpiCalculationService _kpiCalculation;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly WorkspaceDensityEngine _workspaceDensity;

    public KpiFrameworkService(
        KpiCalculationService kpiCalculation,
        ContextAwarenessService contextAwareness,
        AdaptivePerspectiveEngine adaptivePerspective,
        WorkspaceDensityEngine workspaceDensity)
    {
        _kpiCalculation = kpiCalculation;
        _contextAwareness = contextAwareness;
        _adaptivePerspective = adaptivePerspective;
        _workspaceDensity = workspaceDensity;
    }

    public KpiCardRecord CreateCard(
        string id,
        string title,
        string value,
        string subtitle,
        string iconName,
        string colorClass,
        string infoText,
        string? unit = null,
        KpiTrendDirection trend = KpiTrendDirection.Unknown,
        string? trendLabel = null,
        KpiSeverity? severity = null,
        KpiCategory? category = null,
        string? context = null,
        DateTime? lastUpdated = null,
        bool drilldownEnabled = true,
        string? actionLabel = null,
        string? statusBadge = null,
        bool showInfoIcon = true,
        string tileType = "standard",
        IReadOnlyList<KpiBreakdownRecord>? breakdownItems = null,
        IReadOnlyList<string>? tabs = null)
    {
        return new KpiCardRecord(
            id,
            title,
            value,
            unit,
            subtitle,
            trend,
            trendLabel ?? GetDefaultTrendLabel(trend),
            severity ?? InferSeverity(title, value, subtitle, colorClass),
            category ?? InferCategory(title),
            context ?? GetCurrentContextLabel(),
            lastUpdated,
            drilldownEnabled,
            actionLabel,
            statusBadge ?? InferStatusBadge(severity ?? InferSeverity(title, value, subtitle, colorClass)),
            iconName,
            colorClass,
            infoText,
            showInfoIcon,
            tileType,
            breakdownItems,
            tabs);
    }

    public IReadOnlyList<KpiCardRecord> GetStatewideAwarenessCards()
    {
        var snapshot = _kpiCalculation.GetStatewideSnapshot();
        return
        [
            CreateCard("statewide-operational-beds", "Operational Beds", snapshot.Capacity.OperationalBeds.ToString("N0"), $"{snapshot.Capacity.OperationalOccupancyPercent:0}% occupied", "open-bed-green", "kpi-green", "Derived from shared bed state across the current operational scope.", severity: InferOccupancySeverity(snapshot.Capacity.OperationalOccupancyPercent), category: KpiCategory.Capacity, statusBadge: snapshot.Pressure.CapacityTier),
            CreateCard("statewide-available-beds", "Available Beds", snapshot.Capacity.AvailableBeds.ToString("N0"), "Available now", "open-bed-default", "kpi-blue", "Derived available operational beds from shared bed records.", category: KpiCategory.Flow),
            CreateCard("statewide-pending-allocations", "Pending Allocations", snapshot.Allocation.PendingAllocations.ToString("N0"), "Awaiting allocation review", "activity-default", "kpi-orange", "Derived from shared allocation records.", severity: snapshot.Allocation.PendingAllocations > 10 ? KpiSeverity.High : KpiSeverity.Elevated, category: KpiCategory.Allocation),
            CreateCard("statewide-critical-events", "Critical Events", snapshot.Pressure.CriticalOperationalEvents.ToString("N0"), "Active critical operational events", "operation-pressure-orange", "kpi-red", "Derived from active operational event records.", severity: snapshot.Pressure.CriticalOperationalEvents > 0 ? KpiSeverity.Critical : KpiSeverity.Normal, category: KpiCategory.Pressure)
        ];
    }

    public string GetGridDensityClass() => _workspaceDensity.GetCurrentDensityMode() switch
    {
        WorkspaceDensityMode.Comfortable => "kpi-grid-density-comfortable",
        WorkspaceDensityMode.Compact => "kpi-grid-density-compact",
        _ => "kpi-grid-density-balanced"
    };

    public string GetCardDensityClass() => _workspaceDensity.GetCurrentDensityMode() switch
    {
        WorkspaceDensityMode.Comfortable => "kpi-card-density-comfortable",
        WorkspaceDensityMode.Compact => "kpi-card-density-compact",
        _ => "kpi-card-density-balanced"
    };

    public bool ShouldPrioritizeActionLabels() =>
        _adaptivePerspective.GetCurrentPerspective()?.PerspectiveType is UserPerspectiveType.BedManager or UserPerspectiveType.WardClinician or UserPerspectiveType.NUM
        || _workspaceDensity.ShouldIncreaseOperationalDensity();

    private string GetCurrentContextLabel()
    {
        var context = _contextAwareness.GetCurrentContext();
        return string.IsNullOrWhiteSpace(context.ContextSummary)
            ? "Shared operational context"
            : context.ContextSummary;
    }

    private static string GetDefaultTrendLabel(KpiTrendDirection trend) => trend switch
    {
        KpiTrendDirection.Up => "Increasing",
        KpiTrendDirection.Down => "Decreasing",
        KpiTrendDirection.Stable => "Stable",
        _ => "Trend pending"
    };

    private static KpiSeverity InferSeverity(string title, string value, string subtitle, string colorClass)
    {
        if (colorClass.Contains("red", StringComparison.OrdinalIgnoreCase)
            || subtitle.Contains("critical", StringComparison.OrdinalIgnoreCase)
            || value.Contains("Tier 3", StringComparison.OrdinalIgnoreCase))
        {
            return KpiSeverity.Critical;
        }

        if (colorClass.Contains("orange", StringComparison.OrdinalIgnoreCase)
            || subtitle.Contains("high", StringComparison.OrdinalIgnoreCase)
            || subtitle.Contains("amber", StringComparison.OrdinalIgnoreCase))
        {
            return KpiSeverity.High;
        }

        if (title.Contains("Pending", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Delayed", StringComparison.OrdinalIgnoreCase))
        {
            return KpiSeverity.Elevated;
        }

        return KpiSeverity.Normal;
    }

    private static KpiSeverity InferOccupancySeverity(decimal occupancyPercent) => occupancyPercent switch
    {
        >= 95 => KpiSeverity.Critical,
        >= 90 => KpiSeverity.High,
        >= 85 => KpiSeverity.Elevated,
        >= 75 => KpiSeverity.Watch,
        _ => KpiSeverity.Normal
    };

    private static KpiCategory InferCategory(string title)
    {
        if (title.Contains("Capacity", StringComparison.OrdinalIgnoreCase) || title.Contains("Beds", StringComparison.OrdinalIgnoreCase)) return KpiCategory.Capacity;
        if (title.Contains("Pressure", StringComparison.OrdinalIgnoreCase)) return KpiCategory.Pressure;
        if (title.Contains("Admission", StringComparison.OrdinalIgnoreCase)) return KpiCategory.Demand;
        if (title.Contains("Delayed", StringComparison.OrdinalIgnoreCase) || title.Contains("Flow", StringComparison.OrdinalIgnoreCase)) return KpiCategory.Discharge;
        if (title.Contains("Activity", StringComparison.OrdinalIgnoreCase)) return KpiCategory.Activity;
        return KpiCategory.Awareness;
    }

    private static string InferStatusBadge(KpiSeverity severity) => severity switch
    {
        KpiSeverity.Critical => "Critical",
        KpiSeverity.High => "High",
        KpiSeverity.Elevated => "Elevated",
        KpiSeverity.Watch => "Watch",
        _ => "Normal"
    };
}
