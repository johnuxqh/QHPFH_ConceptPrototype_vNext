namespace QHPFH_ConceptPrototype.Models.Filters;

public sealed record FilterVisibilityProfile
{
    public FilterVisibilityProfile(
        FilterAccessScope accessScope,
        bool showHhsFilter,
        bool showFacilityFilter,
        bool showWardFilter,
        bool showServiceStreamFilter,
        bool lockHhsFilter = false,
        bool lockFacilityFilter = false,
        bool lockWardFilter = false,
        bool lockServiceStreamFilter = false,
        IReadOnlyList<string>? allowedHhsValues = null,
        IReadOnlyList<string>? allowedFacilityValues = null,
        IReadOnlyList<string>? allowedWardValues = null,
        IReadOnlyList<string>? allowedServiceStreamValues = null,
        string? summaryLabel = null)
    {
        AccessScope = accessScope;
        ShowHhsFilter = showHhsFilter;
        ShowFacilityFilter = showFacilityFilter;
        ShowWardFilter = showWardFilter;
        ShowServiceStreamFilter = showServiceStreamFilter;
        LockHhsFilter = lockHhsFilter;
        LockFacilityFilter = lockFacilityFilter;
        LockWardFilter = lockWardFilter;
        LockServiceStreamFilter = lockServiceStreamFilter;
        AllowedHhsValues = allowedHhsValues ?? Array.Empty<string>();
        AllowedFacilityValues = allowedFacilityValues ?? Array.Empty<string>();
        AllowedWardValues = allowedWardValues ?? Array.Empty<string>();
        AllowedServiceStreamValues = allowedServiceStreamValues ?? Array.Empty<string>();
        SummaryLabel = summaryLabel;
    }

    public FilterAccessScope AccessScope { get; init; }
    public bool ShowHhsFilter { get; init; }
    public bool ShowFacilityFilter { get; init; }
    public bool ShowWardFilter { get; init; }
    public bool ShowServiceStreamFilter { get; init; }
    public bool LockHhsFilter { get; init; }
    public bool LockFacilityFilter { get; init; }
    public bool LockWardFilter { get; init; }
    public bool LockServiceStreamFilter { get; init; }
    public IReadOnlyList<string> AllowedHhsValues { get; init; }
    public IReadOnlyList<string> AllowedFacilityValues { get; init; }
    public IReadOnlyList<string> AllowedWardValues { get; init; }
    public IReadOnlyList<string> AllowedServiceStreamValues { get; init; }
    public string? SummaryLabel { get; init; }

    public static FilterVisibilityProfile Statewide() => new(
        FilterAccessScope.Statewide,
        showHhsFilter: true,
        showFacilityFilter: true,
        showWardFilter: true,
        showServiceStreamFilter: false,
        summaryLabel: "Statewide");

    public static FilterVisibilityProfile Hhs(IReadOnlyList<string>? allowedHhsValues = null) => new(
        FilterAccessScope.Hhs,
        showHhsFilter: false,
        showFacilityFilter: true,
        showWardFilter: true,
        showServiceStreamFilter: false,
        lockHhsFilter: true,
        allowedHhsValues: allowedHhsValues,
        summaryLabel: "HHS");

    public static FilterVisibilityProfile Facility(IReadOnlyList<string>? allowedHhsValues = null, IReadOnlyList<string>? allowedFacilityValues = null) => new(
        FilterAccessScope.Facility,
        showHhsFilter: false,
        showFacilityFilter: false,
        showWardFilter: true,
        showServiceStreamFilter: false,
        lockHhsFilter: true,
        lockFacilityFilter: true,
        allowedHhsValues: allowedHhsValues,
        allowedFacilityValues: allowedFacilityValues,
        summaryLabel: "Facility");

    public static FilterVisibilityProfile Ward(IReadOnlyList<string>? allowedHhsValues = null, IReadOnlyList<string>? allowedFacilityValues = null, IReadOnlyList<string>? allowedWardValues = null) => new(
        FilterAccessScope.Ward,
        showHhsFilter: false,
        showFacilityFilter: false,
        showWardFilter: false,
        showServiceStreamFilter: false,
        lockHhsFilter: true,
        lockFacilityFilter: true,
        allowedHhsValues: allowedHhsValues,
        allowedFacilityValues: allowedFacilityValues,
        allowedWardValues: allowedWardValues,
        summaryLabel: "Ward");
}
