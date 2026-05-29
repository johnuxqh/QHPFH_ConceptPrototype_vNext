using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Context;

namespace QHPFH_ConceptPrototype.Services.Context;

public sealed class ContextAwarenessService : IDisposable
{
    private readonly PrototypeDataStore _dataStore;
    private readonly PrototypeExperienceStateService _experienceState;

    private string? _currentHhsId;
    private string? _currentFacilityId;
    private string? _currentWardId;
    private string? _locationSummaryOverride;
    private string? _currentWorkspace;
    private string? _currentPageTitle;
    private string? _currentPatientId;
    private string? _currentBedId;
    private string? _currentAllocationId;
    private string? _currentTransferId;
    private string? _currentWorkflowId;

    public ContextAwarenessService(
        PrototypeDataStore dataStore,
        PrototypeExperienceStateService experienceState)
    {
        _dataStore = dataStore;
        _experienceState = experienceState;
        _experienceState.OnChange += HandleExperienceStateChanged;
        AlignLocationToCurrentPerspective();
    }

    public event Action? OnContextChanged;

    public void SetCurrentHhs(string? hhsId)
    {
        _locationSummaryOverride = null;
        _currentHhsId = string.IsNullOrWhiteSpace(hhsId) ? null : hhsId;
        if (_currentHhsId is null)
        {
            _currentFacilityId = null;
            _currentWardId = null;
        }
        else if (_currentFacilityId is not null && _dataStore.GetFacilities().FirstOrDefault(x => x.Id == _currentFacilityId)?.HhsId != _currentHhsId)
        {
            _currentFacilityId = null;
            _currentWardId = null;
        }

        NotifyContextChanged();
    }

    public void SetCurrentFacility(string? facilityId)
    {
        _locationSummaryOverride = null;
        _currentFacilityId = string.IsNullOrWhiteSpace(facilityId) ? null : facilityId;
        var facility = _currentFacilityId is null ? null : _dataStore.GetFacilities().FirstOrDefault(x => x.Id == _currentFacilityId);
        _currentHhsId = facility?.HhsId ?? _currentHhsId;

        if (_currentFacilityId is null)
        {
            _currentWardId = null;
        }
        else if (_currentWardId is not null && _dataStore.GetWards().FirstOrDefault(x => x.Id == _currentWardId)?.FacilityId != _currentFacilityId)
        {
            _currentWardId = null;
        }

        NotifyContextChanged();
    }

    public void SetCurrentWard(string? wardId)
    {
        _locationSummaryOverride = null;
        _currentWardId = string.IsNullOrWhiteSpace(wardId) ? null : wardId;
        var ward = _currentWardId is null ? null : _dataStore.GetWards().FirstOrDefault(x => x.Id == _currentWardId);
        _currentFacilityId = ward?.FacilityId ?? _currentFacilityId;

        var facility = _currentFacilityId is null ? null : _dataStore.GetFacilities().FirstOrDefault(x => x.Id == _currentFacilityId);
        _currentHhsId = facility?.HhsId ?? _currentHhsId;

        NotifyContextChanged();
    }

    public void SetCurrentLocationSummary(string? summary)
    {
        _locationSummaryOverride = NormalizeValue(summary);
        NotifyContextChanged();
    }

    public void SetCurrentWorkspace(string? workspace)
    {
        _currentWorkspace = NormalizeValue(workspace);
        NotifyContextChanged();
    }

    public void SetCurrentPage(string? pageTitle)
    {
        _currentPageTitle = NormalizeValue(pageTitle);
        NotifyContextChanged();
    }

    public void SetCurrentPatient(string? patientId)
    {
        _currentPatientId = NormalizeValue(patientId);
        NotifyContextChanged();
    }

    public void SetCurrentBed(string? bedId)
    {
        _currentBedId = NormalizeValue(bedId);
        NotifyContextChanged();
    }

    public void SetCurrentAllocation(string? allocationId)
    {
        _currentAllocationId = NormalizeValue(allocationId);
        NotifyContextChanged();
    }

    public void SetCurrentTransfer(string? transferId)
    {
        _currentTransferId = NormalizeValue(transferId);
        NotifyContextChanged();
    }

    public void SetCurrentWorkflow(string? workflowId)
    {
        _currentWorkflowId = NormalizeValue(workflowId);
        NotifyContextChanged();
    }

    public OperationalContext GetCurrentContext() => new(
        GetCurrentLocation(),
        GetCurrentPerspective(),
        _experienceState.Current.ExperienceMode,
        _experienceState.Current.LayoutVariant,
        GetCurrentWorkspace());

    public ContextLocation GetCurrentLocation()
    {
        var hhs = _currentHhsId is null ? null : _dataStore.GetHhs().FirstOrDefault(x => x.Id == _currentHhsId);
        var facility = _currentFacilityId is null ? null : _dataStore.GetFacilities().FirstOrDefault(x => x.Id == _currentFacilityId);
        var ward = _currentWardId is null ? null : _dataStore.GetWards().FirstOrDefault(x => x.Id == _currentWardId);

        return new ContextLocation(
            _currentHhsId,
            hhs?.Name,
            _currentFacilityId,
            facility?.Name,
            _currentWardId,
            ward?.Name,
            ResolveLocationScope(),
            _locationSummaryOverride);
    }

    public ContextSelection GetCurrentWorkspace() => new(
        _currentWorkspace,
        _currentPageTitle,
        _currentPatientId,
        _currentBedId,
        _currentAllocationId,
        _currentTransferId,
        _currentWorkflowId);

    public bool HasFacilityContext() => !string.IsNullOrWhiteSpace(_currentFacilityId);

    public bool HasWardContext() => !string.IsNullOrWhiteSpace(_currentWardId);

    public bool HasOperationalContext() => HasFacilityContext() || HasWardContext() || !string.IsNullOrWhiteSpace(_currentWorkspace);

    public void ClearLocationContext(bool preserveSummary = false)
    {
        _currentHhsId = null;
        _currentFacilityId = null;
        _currentWardId = null;
        if (!preserveSummary)
        {
            _locationSummaryOverride = null;
        }
        NotifyContextChanged();
    }

    public void ResetOperationalContext()
    {
        ClearLocationContext();
        _currentWorkspace = null;
        _currentPageTitle = null;
        _currentPatientId = null;
        _currentBedId = null;
        _currentAllocationId = null;
        _currentTransferId = null;
        _currentWorkflowId = null;
        AlignLocationToCurrentPerspective();
        NotifyContextChanged();
    }

    public string GetContextSummary()
    {
        var current = GetCurrentContext();
        var accessView = _experienceState.Current.AccessViewLabel;
        var location = current.Location.SummaryText;
        return string.IsNullOrWhiteSpace(location) || location == current.Location.Scope.ToString()
            ? accessView
            : $"{accessView} > {location}";
    }

    private UserPerspectiveRecord? GetCurrentPerspective()
    {
        var perspectiveId = _experienceState.Current.UserPerspectiveId;
        return _dataStore.GetUserPerspectives().FirstOrDefault(x => x.Id == perspectiveId)
            ?? _dataStore.GetCurrentPerspective();
    }

    private void AlignLocationToCurrentPerspective()
    {
        var perspective = GetCurrentPerspective();
        if (perspective is null)
        {
            return;
        }

        _currentWardId ??= perspective.DefaultWardId ?? perspective.AllowedWardIds.FirstOrDefault();
        _currentFacilityId ??= perspective.DefaultFacilityId
            ?? (_currentWardId is null ? null : _dataStore.GetWards().FirstOrDefault(x => x.Id == _currentWardId)?.FacilityId)
            ?? perspective.AllowedFacilityIds.FirstOrDefault();
        _currentHhsId ??= (_currentFacilityId is null ? null : _dataStore.GetFacilities().FirstOrDefault(x => x.Id == _currentFacilityId)?.HhsId)
            ?? perspective.AllowedHhsIds.FirstOrDefault();
    }

    private ContextScope ResolveLocationScope()
    {
        if (!string.IsNullOrWhiteSpace(_currentWardId)) return ContextScope.Ward;
        if (!string.IsNullOrWhiteSpace(_currentFacilityId)) return ContextScope.Facility;
        if (!string.IsNullOrWhiteSpace(_currentHhsId)) return ContextScope.Hhs;
        return ContextScope.Statewide;
    }

    private static string? NormalizeValue(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private void HandleExperienceStateChanged()
    {
        AlignLocationToCurrentPerspective();
        NotifyContextChanged();
    }

    private void NotifyContextChanged() => OnContextChanged?.Invoke();

    public void Dispose()
    {
        _experienceState.OnChange -= HandleExperienceStateChanged;
    }
}
