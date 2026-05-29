using QHPFH_ConceptPrototype.Models;
using QHPFH_ConceptPrototype.Models.Actions;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Experience;
using QHPFH_ConceptPrototype.Services.Navigation;
using QHPFH_ConceptPrototype.Services.Workspace;

namespace QHPFH_ConceptPrototype.Services.Actions;

public sealed class GlobalActionService : IDisposable
{
    public const string ChatActionId = "chat";
    public const string OnCallActionId = "on-call";
    public const string ReportsActionId = "reports";
    public const string DowntimeActionId = "downtime";
    public const string SupportActionId = "support";
    public const string ContactDirectoryActionId = "contact-directory";
    public const string PrintActionId = "print";
    public const string ExportActionId = "export";
    public const string NotifyActionId = "notify";
    public const string CreateOperationalEventActionId = "create-operational-event";

    private readonly ContextAwarenessService _contextAwareness;
    private readonly NavigationStateService _navigationState;
    private readonly AdaptivePerspectiveEngine _adaptivePerspective;
    private readonly ExperienceModeEngine _experienceMode;
    private readonly WorkspaceDensityEngine _workspaceDensity;

    private static readonly IReadOnlyList<GlobalActionRecord> ActionSeed =
    [
        new(ChatActionId, "Bed Managers Chat", GlobalActionType.Chat, GlobalActionScope.Global, GlobalActionStatus.Available, "Open shared operational communications."),
        new(OnCallActionId, "On Call", GlobalActionType.OnCall, GlobalActionScope.Global, GlobalActionStatus.Available, "Show current on-call and escalation contacts."),
        new(ReportsActionId, "Reports", GlobalActionType.Reports, GlobalActionScope.Global, GlobalActionStatus.Available, "Generate operational reports and summaries."),
        new(DowntimeActionId, "Downtime", GlobalActionType.Downtime, GlobalActionScope.Global, GlobalActionStatus.Available, "Prepare printable continuity summaries for downtime events."),
        new(SupportActionId, "Support", GlobalActionType.Support, GlobalActionScope.Global, GlobalActionStatus.Available, "Open prototype support and frequently asked questions."),
        new(ContactDirectoryActionId, "Contact Directory", GlobalActionType.ContactDirectory, GlobalActionScope.Global, GlobalActionStatus.Available, "Open the facility contact directory."),
        new(PrintActionId, "Print", GlobalActionType.Print, GlobalActionScope.Workspace, GlobalActionStatus.Preview, "Prepare the current workspace for printing."),
        new(ExportActionId, "Export", GlobalActionType.Export, GlobalActionScope.Workspace, GlobalActionStatus.Preview, "Export the current workspace context."),
        new(NotifyActionId, "Notify", GlobalActionType.Notify, GlobalActionScope.OperationalContext, GlobalActionStatus.Preview, "Send a workspace-aware notification."),
        new(CreateOperationalEventActionId, "Create Operational Event", GlobalActionType.CreateOperationalEvent, GlobalActionScope.Workflow, GlobalActionStatus.Preview, "Create an operational event from the current workspace context.")
    ];

    public GlobalActionService(
        ContextAwarenessService contextAwareness,
        NavigationStateService navigationState,
        AdaptivePerspectiveEngine adaptivePerspective,
        ExperienceModeEngine experienceMode,
        WorkspaceDensityEngine workspaceDensity)
    {
        _contextAwareness = contextAwareness;
        _navigationState = navigationState;
        _adaptivePerspective = adaptivePerspective;
        _experienceMode = experienceMode;
        _workspaceDensity = workspaceDensity;

        _contextAwareness.OnContextChanged += NotifyActionsChanged;
        _navigationState.OnNavigationStateChanged += NotifyActionsChanged;
        _adaptivePerspective.OnChange += NotifyActionsChanged;
        _experienceMode.OnChange += NotifyActionsChanged;
        _workspaceDensity.OnChange += NotifyActionsChanged;
    }

    public event Action? OnChange;

    public IReadOnlyList<GlobalActionRecord> GetGlobalActions() => ActionSeed;

    public IReadOnlyList<GlobalActionRecord> GetActionsForCurrentWorkspace()
    {
        var workspaceId = _navigationState.GetNavigationState().CurrentWorkspace;
        return ActionSeed
            .Where(action => action.Status != GlobalActionStatus.Hidden)
            .Where(action => action.Scope == GlobalActionScope.Global
                || string.IsNullOrWhiteSpace(action.WorkspaceId)
                || string.Equals(action.WorkspaceId, workspaceId, StringComparison.OrdinalIgnoreCase))
            .Where(action => CanRunAction(action.Id) || action.Status == GlobalActionStatus.Preview)
            .ToList();
    }

    public IReadOnlyList<GlobalActionRecord> GetActionsForCurrentPerspective() =>
        ActionSeed
            .Where(action => action.Status != GlobalActionStatus.Hidden)
            .Where(action => CanRunAction(action.Id) || action.Status == GlobalActionStatus.Preview)
            .ToList();

    public bool CanRunAction(string actionId)
    {
        var action = FindAction(actionId);
        if (action is null || action.Status is GlobalActionStatus.Disabled or GlobalActionStatus.Hidden)
        {
            return false;
        }

        return action.Type switch
        {
            GlobalActionType.Notify or GlobalActionType.CreateOperationalEvent =>
                _adaptivePerspective.CanEditOperationalState() || _experienceMode.IsOperationalWorkflowMode(),
            _ => true
        };
    }

    public GlobalActionResult TriggerAction(string actionId)
    {
        var action = FindAction(actionId);
        var workspaceLabel = _navigationState.GetCurrentWorkspaceLabel();
        var contextSummary = _contextAwareness.GetContextSummary();

        if (action is null)
        {
            return new GlobalActionResult(
                actionId,
                "Unknown action",
                false,
                "No matching global action is registered for this prototype context.",
                workspaceLabel,
                contextSummary,
                DateTimeOffset.Now);
        }

        if (!CanRunAction(action.Id) && action.Status != GlobalActionStatus.Preview)
        {
            return new GlobalActionResult(
                action.Id,
                action.Label,
                false,
                $"{action.Label} is not available for the current perspective.",
                workspaceLabel,
                contextSummary,
                DateTimeOffset.Now);
        }

        return new GlobalActionResult(
            action.Id,
            action.Label,
            true,
            $"{action.Label} acknowledged for {workspaceLabel} in {contextSummary}.",
            workspaceLabel,
            contextSummary,
            DateTimeOffset.Now);
    }

    public string GetActionLabel(string actionId) => FindAction(actionId)?.Label ?? actionId;

    private static GlobalActionRecord? FindAction(string actionId) =>
        ActionSeed.FirstOrDefault(action => string.Equals(action.Id, actionId, StringComparison.OrdinalIgnoreCase));

    private void NotifyActionsChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _contextAwareness.OnContextChanged -= NotifyActionsChanged;
        _navigationState.OnNavigationStateChanged -= NotifyActionsChanged;
        _adaptivePerspective.OnChange -= NotifyActionsChanged;
        _experienceMode.OnChange -= NotifyActionsChanged;
        _workspaceDensity.OnChange -= NotifyActionsChanged;
    }
}
