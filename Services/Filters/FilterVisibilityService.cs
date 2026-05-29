using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Filters;
using QHPFH_ConceptPrototype.Services;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Navigation;

namespace QHPFH_ConceptPrototype.Services.Filters;

public sealed class FilterVisibilityService
{
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly NavigationStateService _navigationState;
    private readonly PrototypeDataStore _dataStore;

    public FilterVisibilityService(
        AdaptivePerspectiveEngine adaptivePerspective,
        ContextAwarenessService contextAwareness,
        NavigationStateService navigationState,
        PrototypeDataStore dataStore)
    {
        _adaptivePerspective = adaptivePerspective;
        _contextAwareness = contextAwareness;
        _navigationState = navigationState;
        _dataStore = dataStore;
    }

    public FilterVisibilityProfile GetCurrentProfile(FilterVisibilityProfile? overrideProfile = null)
    {
        if (overrideProfile is not null)
        {
            return overrideProfile;
        }

        var perspective = _adaptivePerspective.GetCurrentPerspective();
        return GetProfileForScope(
            ToFilterAccessScope(perspective?.AccessScope ?? UserAccessScope.Statewide),
            ResolveAllowedHhsValues(perspective?.AllowedHhsIds),
            ResolveAllowedFacilityValues(perspective?.AllowedFacilityIds),
            ResolveAllowedWardValues(perspective?.AllowedWardIds));
    }

    public FilterVisibilityProfile GetProfileForScope(
        FilterAccessScope accessScope,
        IReadOnlyList<string>? allowedHhsValues = null,
        IReadOnlyList<string>? allowedFacilityValues = null,
        IReadOnlyList<string>? allowedWardValues = null,
        bool showServiceStream = false)
    {
        return accessScope switch
        {
            FilterAccessScope.Hhs => FilterVisibilityProfile.Hhs(allowedHhsValues) with { ShowServiceStreamFilter = showServiceStream },
            FilterAccessScope.Facility => FilterVisibilityProfile.Facility(allowedHhsValues, allowedFacilityValues) with { ShowServiceStreamFilter = showServiceStream },
            FilterAccessScope.Ward => FilterVisibilityProfile.Ward(allowedHhsValues, allowedFacilityValues, allowedWardValues) with { ShowServiceStreamFilter = showServiceStream },
            FilterAccessScope.Custom => new(FilterAccessScope.Custom, true, true, true, showServiceStream, summaryLabel: "Custom"),
            _ => FilterVisibilityProfile.Statewide() with { ShowServiceStreamFilter = showServiceStream }
        };
    }

    public FilterVisibilityProfile CreateCustomProfile(
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
        string? summaryLabel = null) => new(
            FilterAccessScope.Custom,
            showHhsFilter,
            showFacilityFilter,
            showWardFilter,
            showServiceStreamFilter,
            lockHhsFilter,
            lockFacilityFilter,
            lockWardFilter,
            lockServiceStreamFilter,
            allowedHhsValues,
            allowedFacilityValues,
            allowedWardValues,
            allowedServiceStreamValues,
            summaryLabel);

    public string GetVisibilitySummary(FilterVisibilityProfile profile)
    {
        var visible = new[]
        {
            profile.ShowHhsFilter ? "HHS" : null,
            profile.ShowFacilityFilter ? "Facility" : null,
            profile.ShowWardFilter ? "Ward" : null,
            profile.ShowServiceStreamFilter ? "Service Stream" : null
        }.Where(x => !string.IsNullOrWhiteSpace(x));

        return string.Join(", ", visible);
    }

    public string GetWorkspaceContextLabel(FilterVisibilityProfile profile)
    {
        var nav = _navigationState.GetNavigationState();
        var context = _contextAwareness.GetCurrentLocation().SummaryText;
        return string.IsNullOrWhiteSpace(context) ? nav.CurrentWorkspaceLabel : context;
    }

    private IReadOnlyList<string> ResolveAllowedHhsValues(IReadOnlyList<string>? values) => ResolveAllowedValues(
        values,
        value => _dataStore.GetHhs().FirstOrDefault(x => IsMatch(x.Id, value) || IsMatch(x.Name, value))?.Name);

    private IReadOnlyList<string> ResolveAllowedFacilityValues(IReadOnlyList<string>? values) => ResolveAllowedValues(
        values,
        value => _dataStore.GetFacilities().FirstOrDefault(x => IsMatch(x.Id, value) || IsMatch(x.Name, value))?.Name);

    private IReadOnlyList<string> ResolveAllowedWardValues(IReadOnlyList<string>? values) => ResolveAllowedValues(
        values,
        value => _dataStore.GetWards().FirstOrDefault(x => IsMatch(x.Id, value) || IsMatch(x.WardCode, value) || IsMatch(x.Name, value))?.WardCode);

    private static IReadOnlyList<string> ResolveAllowedValues(IReadOnlyList<string>? values, Func<string, string?> resolve)
    {
        if (values is null || values.Count == 0)
        {
            return Array.Empty<string>();
        }

        return values
            .Select(value => resolve(value) ?? value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsMatch(string? left, string? right) =>
        string.Equals(left?.Trim(), right?.Trim(), StringComparison.OrdinalIgnoreCase);

    private static FilterAccessScope ToFilterAccessScope(UserAccessScope accessScope) => accessScope switch
    {
        UserAccessScope.HHS => FilterAccessScope.Hhs,
        UserAccessScope.Facility => FilterAccessScope.Facility,
        UserAccessScope.Ward => FilterAccessScope.Ward,
        _ => FilterAccessScope.Statewide
    };
}
