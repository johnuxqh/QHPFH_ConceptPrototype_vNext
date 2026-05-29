namespace QHPFH_ConceptPrototype.Models.Kpi;

public sealed record KpiCardRecord(
    string Id,
    string Title,
    string Value,
    string? Unit,
    string Subtitle,
    KpiTrendDirection Trend,
    string TrendLabel,
    KpiSeverity Severity,
    KpiCategory Category,
    string Context,
    DateTime? LastUpdated,
    bool DrilldownEnabled,
    string? ActionLabel,
    string? StatusBadge,
    string IconName,
    string ColorClass,
    string InfoText,
    bool ShowInfoIcon = true,
    string TileType = "standard",
    IReadOnlyList<KpiBreakdownRecord>? BreakdownItems = null,
    IReadOnlyList<string>? Tabs = null)
{
    public IReadOnlyList<KpiBreakdownRecord> BreakdownItems { get; } = BreakdownItems ?? Array.Empty<KpiBreakdownRecord>();
    public IReadOnlyList<string> Tabs { get; } = Tabs ?? Array.Empty<string>();
}
