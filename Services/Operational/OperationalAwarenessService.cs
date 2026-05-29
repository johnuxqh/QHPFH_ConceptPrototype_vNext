using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Context;
using QHPFH_ConceptPrototype.Models.Operational;
using QHPFH_ConceptPrototype.Services.Actions;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;

namespace QHPFH_ConceptPrototype.Services.Operational;

public sealed class OperationalAwarenessService : IDisposable
{
    private readonly PrototypeDataStore _dataStore;
    private readonly ContextAwarenessService _contextAwareness;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly GlobalActionService _globalActionService;
    private readonly HashSet<string> _dismissedBannerIds = new(StringComparer.OrdinalIgnoreCase);

    public OperationalAwarenessService(
        PrototypeDataStore dataStore,
        ContextAwarenessService contextAwareness,
        AdaptivePerspectiveEngine adaptivePerspective,
        GlobalActionService globalActionService)
    {
        _dataStore = dataStore;
        _contextAwareness = contextAwareness;
        _adaptivePerspective = adaptivePerspective;
        _globalActionService = globalActionService;

        _dataStore.OnChange += NotifyAwarenessChanged;
        _contextAwareness.OnContextChanged += NotifyAwarenessChanged;
        _adaptivePerspective.OnChange += NotifyAwarenessChanged;
        _globalActionService.OnChange += NotifyAwarenessChanged;
    }

    public event Action? OnChange;

    public IReadOnlyList<OperationalBannerViewModel> GetActiveBanners() =>
        GetAllActiveBannerViewModels()
            .OrderByDescending(banner => banner.IsPinned)
            .ThenByDescending(banner => banner.SortPriority)
            .ThenByDescending(banner => banner.StartsAtUtc ?? DateTime.MinValue)
            .ToList();

    public IReadOnlyList<OperationalBannerViewModel> GetBannersForCurrentContext()
    {
        var location = _contextAwareness.GetCurrentLocation();
        var canViewStatewide = _adaptivePerspective.GetCurrentPerspective()?.CanViewStatewide == true;

        return GetActiveBanners()
            .Where(banner => IsRelevantToContext(banner, location, canViewStatewide))
            .ToList();
    }

    public IReadOnlyList<OperationalBannerViewModel> GetBannersForFacility(string facilityId) =>
        GetActiveBanners()
            .Where(banner => string.Equals(GetFacilityIdForBanner(banner), facilityId, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public IReadOnlyList<OperationalBannerViewModel> GetBannersForWard(string wardId) =>
        GetActiveBanners()
            .Where(banner => string.Equals(GetWardIdForBanner(banner), wardId, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public OperationalBannerViewModel? GetHighestSeverityBanner() => GetBannersForCurrentContext().FirstOrDefault();

    public bool HasCriticalOperationalBanner() => GetBannersForCurrentContext().Any(banner => banner.IsCritical);

    public OperationalBannerViewModel? GetCapacityBannerForCurrentContext() =>
        GetBannersForCurrentContext().FirstOrDefault(banner => banner.CapacityStatus is not null);

    public void DismissBanner(string bannerId)
    {
        if (string.IsNullOrWhiteSpace(bannerId))
        {
            return;
        }

        _dismissedBannerIds.Add(bannerId);
        NotifyAwarenessChanged();
    }

    private IEnumerable<OperationalBannerViewModel> GetAllActiveBannerViewModels()
    {
        var now = DateTime.UtcNow;

        foreach (var banner in _dataStore.GetOperationalBanners().Where(banner => IsActiveWindow(banner.IsActive, banner.StartsAtUtc, banner.EndsAtUtc, now)))
        {
            if (!_dismissedBannerIds.Contains(banner.Id))
            {
                yield return FromOperationalBanner(banner);
            }
        }

        foreach (var operationalEvent in _dataStore.GetActiveOperationalEvents().Where(evt => IsActiveWindow(evt.IsActive, evt.StartsAtUtc, evt.EndsAtUtc, now)))
        {
            if (!_dismissedBannerIds.Contains(operationalEvent.Id))
            {
                yield return FromOperationalEvent(operationalEvent);
            }
        }

        foreach (var informationBanner in _dataStore.GetInformationBanners().Where(banner => banner.IsActive))
        {
            if (!_dismissedBannerIds.Contains(informationBanner.Id))
            {
                yield return FromInformationBanner(informationBanner);
            }
        }
    }

    private OperationalBannerViewModel FromOperationalBanner(OperationalBannerRecord banner) => new(
        banner.Id,
        banner.Title,
        banner.Message,
        banner.Severity,
        banner.Scope,
        banner.CapacityStatus,
        ResolveAffectedArea(banner.HhsId, banner.FacilityId, banner.WardId, banner.Scope),
        banner.IsDismissible,
        false,
        banner.IsPinned,
        banner.StartsAtUtc,
        banner.EndsAtUtc,
        "Operational banner",
        GetSortPriority(banner.Severity, banner.CapacityStatus, banner.IsPinned));

    private OperationalBannerViewModel FromOperationalEvent(OperationalEventRecord operationalEvent) => new(
        operationalEvent.Id,
        operationalEvent.Title,
        operationalEvent.Summary,
        operationalEvent.Severity,
        operationalEvent.Scope,
        operationalEvent.CapacityStatus,
        ResolveAffectedArea(operationalEvent.HhsId, operationalEvent.FacilityId, operationalEvent.WardId, operationalEvent.Scope),
        false,
        operationalEvent.RequiresAcknowledgement,
        operationalEvent.RequiresAcknowledgement,
        operationalEvent.StartsAtUtc,
        operationalEvent.EndsAtUtc,
        "Operational event",
        GetSortPriority(operationalEvent.Severity, operationalEvent.CapacityStatus, operationalEvent.RequiresAcknowledgement));

    private static OperationalBannerViewModel FromInformationBanner(InformationBannerRecord banner) => new(
        banner.Id,
        banner.Title,
        banner.Message,
        ParseSeverity(banner.Severity),
        OperationalEventScope.Statewide,
        null,
        banner.Audience,
        true,
        false,
        false,
        null,
        null,
        "Information banner",
        GetSortPriority(ParseSeverity(banner.Severity), null, false));

    private bool IsRelevantToContext(OperationalBannerViewModel banner, ContextLocation location, bool canViewStatewide)
    {
        if (banner.Scope == OperationalEventScope.Statewide || canViewStatewide && banner.IsCritical)
        {
            return true;
        }

        return banner.Scope switch
        {
            OperationalEventScope.HHS => string.IsNullOrWhiteSpace(location.CurrentHhsId) || string.Equals(GetHhsIdForBanner(banner), location.CurrentHhsId, StringComparison.OrdinalIgnoreCase),
            OperationalEventScope.Facility => string.IsNullOrWhiteSpace(location.CurrentFacilityId) || string.Equals(GetFacilityIdForBanner(banner), location.CurrentFacilityId, StringComparison.OrdinalIgnoreCase),
            OperationalEventScope.Ward => string.IsNullOrWhiteSpace(location.CurrentWardId)
                ? string.IsNullOrWhiteSpace(location.CurrentFacilityId) || string.Equals(GetFacilityIdForBanner(banner), location.CurrentFacilityId, StringComparison.OrdinalIgnoreCase)
                : string.Equals(GetWardIdForBanner(banner), location.CurrentWardId, StringComparison.OrdinalIgnoreCase),
            _ => true
        };
    }

    private string ResolveAffectedArea(string? hhsId, string? facilityId, string? wardId, OperationalEventScope scope)
    {
        var ward = string.IsNullOrWhiteSpace(wardId) ? null : _dataStore.GetWards().FirstOrDefault(x => x.Id == wardId);
        var facility = string.IsNullOrWhiteSpace(facilityId ?? ward?.FacilityId) ? null : _dataStore.GetFacilities().FirstOrDefault(x => x.Id == (facilityId ?? ward?.FacilityId));
        var hhs = string.IsNullOrWhiteSpace(hhsId ?? facility?.HhsId) ? null : _dataStore.GetHhs().FirstOrDefault(x => x.Id == (hhsId ?? facility?.HhsId));

        return scope switch
        {
            OperationalEventScope.Ward => ward is null ? "Ward context" : $"{facility?.ShortName ?? facility?.Name ?? "Facility"} · {ward.Name}",
            OperationalEventScope.Facility => facility?.Name ?? "Facility context",
            OperationalEventScope.HHS => hhs?.Name ?? "HHS context",
            _ => "Statewide"
        };
    }

    private string? GetHhsIdForBanner(OperationalBannerViewModel banner) => ResolveSourceIds(banner.Id).HhsId;

    private string? GetFacilityIdForBanner(OperationalBannerViewModel banner) => ResolveSourceIds(banner.Id).FacilityId;

    private string? GetWardIdForBanner(OperationalBannerViewModel banner) => ResolveSourceIds(banner.Id).WardId;

    private (string? HhsId, string? FacilityId, string? WardId) ResolveSourceIds(string bannerId)
    {
        var banner = _dataStore.GetOperationalBanners().FirstOrDefault(x => x.Id == bannerId);
        if (banner is not null)
        {
            return (banner.HhsId, banner.FacilityId, banner.WardId);
        }

        var operationalEvent = _dataStore.GetOperationalEvents().FirstOrDefault(x => x.Id == bannerId);
        if (operationalEvent is not null)
        {
            return (operationalEvent.HhsId, operationalEvent.FacilityId, operationalEvent.WardId);
        }

        return (null, null, null);
    }

    private static bool IsActiveWindow(bool isActive, DateTime? startsAtUtc, DateTime? endsAtUtc, DateTime now) =>
        isActive && (startsAtUtc is null || startsAtUtc <= now) && (endsAtUtc is null || endsAtUtc >= now);

    private static OperationalEventSeverity ParseSeverity(string severity) => severity.Trim().ToLowerInvariant() switch
    {
        "critical" or "red" or "incident" or "incidentmanagement" => OperationalEventSeverity.Critical,
        "high" or "warning" or "amber" => OperationalEventSeverity.High,
        "moderate" => OperationalEventSeverity.Moderate,
        _ => OperationalEventSeverity.Info
    };

    private static int GetSortPriority(OperationalEventSeverity severity, CapacityStatus? capacityStatus, bool pinned) =>
        (pinned ? 100 : 0)
        + (capacityStatus == CapacityStatus.IncidentManagement ? 40 : capacityStatus == CapacityStatus.Tier3 ? 30 : capacityStatus == CapacityStatus.Tier2 ? 20 : 0)
        + severity switch
        {
            OperationalEventSeverity.Critical => 40,
            OperationalEventSeverity.High => 30,
            OperationalEventSeverity.Moderate => 20,
            _ => 10
        };

    private void NotifyAwarenessChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _dataStore.OnChange -= NotifyAwarenessChanged;
        _contextAwareness.OnContextChanged -= NotifyAwarenessChanged;
        _adaptivePerspective.OnChange -= NotifyAwarenessChanged;
        _globalActionService.OnChange -= NotifyAwarenessChanged;
    }
}
