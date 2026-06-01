using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using QHPFH_ConceptPrototype.Models.Navigation;
using QHPFH_ConceptPrototype.Services.Context;

namespace QHPFH_ConceptPrototype.Services.Navigation;

public sealed class NavigationStateService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly ContextAwarenessService _contextAwareness;

    private string _currentRoute = string.Empty;
    private string _currentWorkspace = "home";
    private string _currentPrimaryNavId = "home";
    private string _currentSecondaryNavId = string.Empty;
    private NavigationShellMode _currentShellMode = NavigationShellMode.Concept;
    private bool _isPrimaryNavCollapsed;
    private bool _isSecondaryNavCollapsed;

    private static readonly IReadOnlyDictionary<string, RouteNavigationDefinition> RouteDefinitions =
        new Dictionary<string, RouteNavigationDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            [string.Empty] = new("home", "home", string.Empty, "My Hub", "My Hub", string.Empty),
            ["bed-and-ward-hub"] = new("bed", "bed-ward", "Bed and Ward Hub", "Bed & Ward", "Bed and Ward Hub", "Bed and Ward Hub"),
            ["bed-management"] = new("bed", "bed-management", "Bed Management", "Bed Management", "Bed Management", "Bed Management"),
            ["ward-operations"] = new("bed", "ward-operations", "Ward View", "Ward Operations", "Ward Operations", "Ward View"),
            ["allocation-centre"] = new("bed", "allocation-centre", "Allocation Centre", "Allocation Centre", "Allocation Centre", "Allocation Centre")
        };

    public NavigationStateService(
        NavigationManager navigationManager,
        ContextAwarenessService contextAwareness)
    {
        _navigationManager = navigationManager;
        _contextAwareness = contextAwareness;
        _navigationManager.LocationChanged += HandleLocationChanged;
        SetCurrentRoute(GetRelativeRoute(_navigationManager.Uri), notify: false);
    }

    public event Action? OnNavigationStateChanged;

    public NavigationStateSnapshot GetNavigationState() => new(
        _currentRoute,
        _currentWorkspace,
        _currentPrimaryNavId,
        _currentSecondaryNavId,
        _currentShellMode,
        _isPrimaryNavCollapsed,
        _isSecondaryNavCollapsed,
        GetCurrentNavLabel(),
        GetCurrentWorkspaceLabel(),
        GetCurrentSectionLabel());

    public void SetCurrentRoute(string route) => SetCurrentRoute(route, notify: true);

    public void SetCurrentWorkspace(string workspaceId)
    {
        _currentWorkspace = NormalizeId(workspaceId);
        _contextAwareness.SetCurrentWorkspace(GetCurrentWorkspaceLabel());
        NotifyNavigationChanged();
    }

    public void SetPrimaryNav(string navId)
    {
        _currentPrimaryNavId = NormalizeId(navId);
        NotifyNavigationChanged();
    }

    public void SetSecondaryNav(string navId)
    {
        _currentSecondaryNavId = NormalizeId(navId);
        NotifyNavigationChanged();
    }

    public void SetShellMode(NavigationShellMode shellMode)
    {
        _currentShellMode = shellMode;
        NotifyNavigationChanged();
    }

    public void SetPrimaryNavCollapsed(bool collapsed)
    {
        _isPrimaryNavCollapsed = collapsed;
        NotifyNavigationChanged();
    }

    public void TogglePrimaryNavCollapsed() => SetPrimaryNavCollapsed(!_isPrimaryNavCollapsed);

    public void SetSecondaryNavCollapsed(bool collapsed)
    {
        _isSecondaryNavCollapsed = collapsed;
        NotifyNavigationChanged();
    }

    public void ToggleSecondaryNavCollapsed() => SetSecondaryNavCollapsed(!_isSecondaryNavCollapsed);

    public string GetCurrentWorkspaceLabel() => ResolveDefinition().WorkspaceLabel;

    public string GetCurrentSectionLabel() => ResolveDefinition().SectionLabel;

    public string GetCurrentNavLabel() => ResolveDefinition().NavLabel;

    public bool IsCurrentRoute(string route) => string.Equals(_currentRoute, NormalizeRoute(route), StringComparison.OrdinalIgnoreCase);

    public bool IsWorkspaceActive(string workspaceId) => string.Equals(_currentWorkspace, NormalizeId(workspaceId), StringComparison.OrdinalIgnoreCase);

    public string? ResolveRoute(string primaryNavId, string secondaryNavId)
    {
        if (primaryNavId is "home" or "myhub")
        {
            return string.Empty;
        }

        if (primaryNavId != "bed")
        {
            return null;
        }

        return secondaryNavId switch
        {
            "Bed and Ward Hub" or "bed-ward" => "bed-and-ward-hub",
            "Bed Management" or "bed-management" => "bed-management",
            "Ward View" or "Ward Operations" or "ward-operations" => "ward-operations",
            "Allocation Centre" or "allocation-centre" => "allocation-centre",
            _ => null
        };
    }

    private void SetCurrentRoute(string route, bool notify)
    {
        _currentRoute = NormalizeRoute(route);
        var definition = ResolveDefinition();
        _currentPrimaryNavId = definition.PrimaryNavId;
        _currentSecondaryNavId = definition.SecondaryNavId;
        _currentWorkspace = definition.WorkspaceId;
        _contextAwareness.SetCurrentWorkspace(definition.WorkspaceLabel);
        _contextAwareness.SetCurrentPage(definition.NavLabel);

        if (notify)
        {
            NotifyNavigationChanged();
        }
    }

    private RouteNavigationDefinition ResolveDefinition() =>
        RouteDefinitions.TryGetValue(_currentRoute, out var definition)
            ? definition
            : new("home", "home", string.Empty, "My Hub", "My Hub", string.Empty);

    private string GetRelativeRoute(string uri) => NormalizeRoute(_navigationManager.ToBaseRelativePath(uri));

    private static string NormalizeRoute(string? route) => (route ?? string.Empty).Split('?', '#')[0].Trim('/').ToLowerInvariant();

    private static string NormalizeId(string? value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e) => SetCurrentRoute(GetRelativeRoute(e.Location));

    private void NotifyNavigationChanged() => OnNavigationStateChanged?.Invoke();

    public void Dispose()
    {
        _navigationManager.LocationChanged -= HandleLocationChanged;
    }

    private sealed record RouteNavigationDefinition(
        string PrimaryNavId,
        string WorkspaceId,
        string SecondaryNavId,
        string NavLabel,
        string WorkspaceLabel,
        string SectionLabel);
}
