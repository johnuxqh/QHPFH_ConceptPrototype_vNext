# QH Patient Flow Hub vNext Roadmap

This roadmap tracks staged Codex implementation prompts for the vNext prototype.

> The roadmap is a working implementation tracker for staged Codex prompts and should be updated at the end of each prompt.

## Phase Tracker

| Phase | Prompt IDs | Status | Dependencies | Testing / Checkpoint Notes |
| --- | --- | --- | --- | --- |
| P0 — Governance + Safety | P0.01, P0.02, P0.03, P0.04, P0.05, P0.06, P0.07, P0.08, P0.09, P0.10 | Complete | None | Governance baseline established before architecture work. |
| P1 — Data + State Foundation | P1.01, P1.02, P1.03, P1.04, P1.05, P1.06, P1.07, P1.08, P1.09, P1.10, P1.11, P1.12, P1.13, P1.14 | Complete | P0 complete | Shared model/data scaffold, central in-memory store, and session writeback foundation are in place; shared HHS/facility/ward/bed model layer refined for future filters, access scopes, KPI calculations, and bed workflows; shared patient workflow models added for future patient slideouts, ward workflows, delayed discharge, and allocation workflows; shared allocation models added for future incoming streams, transfer coordination, future bed allocation, transit beds, and cross-page allocation workflows; shared operational event models added for future operational awareness, escalation, orchestration, downtime, and coordination workflows; shared activity feed models added for future operational audit trails, patient movement history, bed status changes, allocation activity, and escalation tracking; shared notification models added for future notification centre, toaster notifications, role-aware alerts, acknowledgement states, and cross-workspace messages; shared user perspective models added for adaptive operational experiences, access-aware workflows, operational emphasis switching, and future perspective-driven UI adaptation; shared scenario simulation models added for future what-if planning, operational modelling, capacity/demand scenarios, and orchestration workflows; shared mock API/data service layer created to wrap PrototypeDataStore and provide future page/component query and mutation access; shared demo data seeder refined for maintainable seeded hierarchy, patient, allocation, operational, notification, perspective, and scenario data; KPI calculation engine foundation added for centralized derived operational snapshots and rollups; UI wiring deferred to later P1 prompts. |
| P2 — Shell + Platform Framework | P2.01, P2.02, P2.03, P2.04, P2.05, P2.06, P2.07, P2.08, P2.09, P2.10, P2.11, P2.12, P2.13, P2.14, P2.15 | In Progress | P1 | P2.01 complete: Prototype Experience Bar created as the global workshop control layer for Access View, Experience Mode, and Layout Variant selection. P2.02 complete: Adaptive Access Perspective Engine created to centralize perspective-aware visibility, capability, operational emphasis, density, and workflow behaviour. P2.03 complete: Experience Mode Framework created to centralize progressive awareness, coordination, and workflow operational behaviours without duplicating pages. P2.04 complete: Layout Variant Framework created to centralize stacked, swappable, and compact operational layout behaviour without duplicating pages. P2.05 complete: Shared Workspace Shell Framework created to standardize reusable operational workspace structure, adaptive shell behaviour, and future orchestration-ready workspace architecture. P2.06 complete: Persistent Context Awareness Layer created to maintain shared operational context, workspace context, and adaptive platform state across the prototype session. P2.07 complete: Unified Navigation State Engine created to centralize route, workspace, primary/secondary nav, shell, and collapse state for future standard/concept navigation alignment. P2.08 complete: Collapsible navigation behaviour added and P2.07 menu routing regression repaired. P2.09 complete: Shared Workspace Header pattern created for consistent operational workspace title, context, perspective, mode, layout, and status presentation. P2.10 complete: Workspace Density Modes created to support comfortable, balanced, and compact operational workspace density without duplicating pages. P2.11 complete: Shared Global Action Framework created to centralize reports, downtime, on-call, chat, support, print/export, and future workspace-aware actions. P2.12 complete: Operational Awareness Banner System created for shared capacity, downtime, staffing, infection-control, and operational event messaging across workspace contexts. P2.13 complete: Shared KPI Card Framework created to standardize operational KPI presentation, severity, trends, density adaptation, and perspective-aware awareness patterns across all workspace contexts. P2.14 complete: Shared Insights Card Framework created to standardize operational insight presentation, prioritization, severity interpretation, action recommendations, density adaptation, and context-aware operational sense-making across workspace contexts. P2.15 complete: Shared Operational Panel Framework created to standardize situational, insight, workflow, allocation, discharge, escalation, and review panel patterns across workspace contexts. Keep routing/base-path stable while aligning shell architecture. |
| P3 — Filter + Access Architecture | P3.01, P3.02, P3.03, P3.04, P3.05, P3.06, P3.07, P3.08, P3.09, P3.10 | Complete | P1, P2 | P3.01 complete: Universal Cascading Filter Framework created to centralize HHS, Facility, Ward, service stream, context persistence, and workspace-aware filtering across operational workspaces. P3.02 complete: Multi-Select Filtering added to the Universal Cascading Filter Framework, supporting operational multi-facility, multi-ward, and multi-HHS workflows while preserving cascading context validation. P3.03 complete: HHS → Facility → Ward cascade logic hardened across the Universal Filter Framework, including multi-select validation, All-state handling, context summary support, and QCH access-scope protection. P3.04 complete: Automatic Parent Awareness Logic implemented so facility and ward selections infer parent HHS/facility context safely across single-select, multi-select, All-state, duplicate ward, and access-scoped filtering. P3.05 complete: Role-Aware Filter Visibility implemented so users only see filters relevant to their operational access scope while preserving shared context, calculations, parent awareness, and workspace behaviour. P3.06 complete: Filter Persistence Across Pages implemented so operational context follows users between workspaces while preserving multi-select state, cascade validation, parent awareness, role-aware visibility, and access-scope restrictions. P3.07 complete: Operational Filter Chips + Breadcrumbs created to communicate active HHS, Facility, Ward, Service Stream, hidden/locked, multi-select, parent-aware, and persisted operational context across workspace pages. P3.08 complete: Filter Reset + Preset Behaviour created so operational users can clear filters, reset to access scope, reset to workspace defaults, and apply shared context presets while preserving role-aware visibility, parent awareness, persistence, and chips/breadcrumbs. P3.09 complete: Cross-Workspace Filter Synchronisation created so shared operational context changes can notify compatible workspaces while preserving persistence, access scope, role-aware visibility, parent awareness, reset/preset behaviour, and loop protection. P3.10 complete: Smart Empty-State Filter Handling created so operational users receive meaningful guidance, recovery actions, and contextual explanations when filter selections result in empty operational datasets. |
| P4 — Design System + Visual Alignment | P4.01, P4.02, P4.03, P4.04, P4.05 | In Progress | P2 | P4.01 complete: Unified vNext Token System created as the visual source of truth for colours, typography, spacing, elevation, layout sizing, status states, and dark-mode support across the Patient Flow Hub platform. P4.02 complete: Unified Typography Hierarchy created to standardise operational text roles, font sizing, weights, line heights, labels, captions, headings, and data-readable typography across shared platform components. P4.03 complete: Unified Spacing System created to standardise numeric and semantic spacing tokens for page padding, shell gaps, panels, cards, filters, chips, tables, banners, empty states, and operational density modes. P4.04 complete: Unified Card Framework created to standardise operational card containers, headers, bodies, footers, actions, density, interaction, selection, status variants, and dark-mode compatibility for shared card patterns. P4.05 complete: Unified Table Framework created to standardise operational table containers, headers, rows, cells, density, hover/selected/disabled row states, status/action cells, responsive overflow, empty states, and dark-mode compatibility for shared table patterns. No broad redesign; align visual patterns through incremental updates only. |
| P5 — Slideout + Overlay Architecture | Pending | Pending | P2, P3 | Preserve existing overlays while moving to shared architecture. |
| P6 — Patient Workflow System | Pending | Pending | P1, P3, P5 | Introduce workflow states without breaking existing operations pages. |
| P7 — Bed Management Alignment | Pending | Pending | P1, P6 | Migrate bed workflow data source progressively. |
| P8 — Ward Operations Alignment | Pending | Pending | P1, P6 | Migrate ward metrics and actions progressively. |
| P9 — Allocation Centre Alignment | Pending | Pending | P1, P6 | Migrate allocation queue and actions progressively. |
| P10 — Delayed Discharge Alignment | Pending | Pending | P1, P6 | Integrate delayed discharge models and views incrementally. |
| P11 — Scenario + Orchestration | Pending | Pending | P6, P7, P8, P9, P10 | Add scenario orchestration after core workflows are aligned. |
| P12 — Utilities + Toaster Systems | Pending | Pending | P2, P5 | Consolidate utility and toaster patterns without UX regression. |
| P13 — Validation + Cleanup | Pending | Pending | P2-P12 | Full pass for regressions, dead code, and documentation drift. |
| P14 — Final Consolidation | Pending | Pending | P13 | Final release hardening and readiness checkpoint. |


Session-level writeback foundation added to PrototypeDataStore. UI wiring deferred to later prompts.


Shared HHS/Facility/Ward/Bed model layer refined for future filters, access scopes, KPI calculations, and bed workflows.


Shared patient workflow models added for future patient slideouts, ward workflows, delayed discharge, and allocation workflows.
Shared patient workflow models support operational coordination and do not represent a full EMR.


Shared allocation models added for future incoming streams, transfer coordination, future bed allocation, transit beds, and cross-page allocation workflows.


Shared operational event models added for future operational awareness, escalation, orchestration, downtime, and coordination workflows.
Operational events are coordination/orchestration signals rather than generic dashboard notifications.


Shared activity feed models added for future operational audit trails, patient movement history, bed status changes, allocation activity, and escalation tracking.
Activity feeds are prototype operational history only and not production audit logs.


Shared notification models added for future notification centre, toaster notifications, role-aware alerts, acknowledgement states, and cross-workspace messages.
Notifications are user-facing delivery messages; operational events are operational conditions/signals.


Shared user perspective models added for adaptive operational experiences, access-aware workflows, operational emphasis switching, and future perspective-driven UI adaptation.
The platform adapts operational emphasis within shared workflows rather than creating separate applications or duplicate page structures.


Shared scenario simulation models added for future what-if planning, operational modelling, capacity/demand scenarios, and orchestration workflows.


Shared mock API/data service layer created to wrap PrototypeDataStore and provide future page/component query and mutation access.


Shared demo data seeder refined for maintainable seeded hierarchy, patient, allocation, operational, notification, perspective, and scenario data.


P1.14 — KPI calculation engine foundation
- Status: Complete
- Outcome: Centralized `KpiCalculationService` and typed KPI snapshots for statewide/HHS/facility/ward/bed/allocation/delayed discharge/pressure/workflow calculations.
- Deferred UI wiring: KPI views will migrate to service-driven snapshots in later prompts without route/layout rewrites.


P3.01 — Universal Cascading Filter Framework
- Status: Complete
- Outcome: Universal Cascading Filter Framework created to centralize HHS, Facility, Ward, service stream, context persistence, and workspace-aware filtering across operational workspaces.
- Proof-of-concept UI: Bed Management now consumes UniversalFilterBar/FilterDropdown and FilterFrameworkService while preserving the existing compact filter appearance, access view control, KPI calculations, insight filtering, and operational workflows.
- Context integration: Filter changes persist through FilterFrameworkService and update ContextAwarenessService plus NavigationStateService for workspace header context continuity.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, and future statewide workspaces will migrate to the same framework in later P3 prompts without duplicating filter logic.


P3.02 — Multi-Select Filtering
- Status: Complete
- Outcome: Multi-Select Filtering added to the Universal Cascading Filter Framework, supporting operational multi-facility, multi-ward, and multi-HHS workflows while preserving cascading context validation.
- Proof-of-concept UI: Bed Management enables multi-select HHS, Facility, and Ward filtering through UniversalFilterBar/FilterDropdown while preserving existing KPI, insight, access-view, and table workflows.
- Context integration: Multi-selection summaries are pushed into ContextAwarenessService so Workspace Header can show compact multi-area summaries without redesigning the header.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, future service-stream filters, and later access-scope enforcement can consume the same multi-select filter state without creating another framework.


P3.03 — HHS → Facility → Ward Cascade Logic
- Status: Complete
- Outcome: HHS → Facility → Ward cascade logic hardened across the Universal Filter Framework, including multi-select validation, All-state handling, context summary support, and QCH access-scope protection.
- Cascade hardening: Facility options and selections are constrained by selected HHS values, Ward options and selections are constrained by selected Facility values, and invalid downstream selections normalize back to the All state without storing literal All values.
- Ward safety: Ward values remain facility-aware where duplicate ward labels exist, and active ward context is set only when a selected ward resolves unambiguously.
- Proof-of-concept UI: Bed Management continues to consume the shared filter framework without page-local cascade logic or layout/routing changes.


P3.04 — Automatic Parent Awareness Logic
- Status: Complete
- Outcome: Automatic Parent Awareness Logic implemented so facility and ward selections infer parent HHS/facility context safely across single-select, multi-select, All-state, duplicate ward, and access-scoped filtering.
- Parent inference: Facility selections infer parent HHS context, and Ward selections infer parent Facility/HHS context without visually mutating parent filter selections.
- Context integration: ContextAwarenessService receives exact HHS, Facility, and Ward context only when resolved safely; multi-select and ambiguous selections use summary context only.
- Proof-of-concept UI: Bed Management benefits from automatic parent context while preserving existing layout, access-view behaviour, KPI cards, insights, operational banners, and routing.


P3.05 — Role-Aware Filter Visibility
- Status: Complete
- Outcome: Role-Aware Filter Visibility implemented so users only see filters relevant to their operational access scope while preserving shared context, calculations, parent awareness, and workspace behaviour.
- Visibility architecture: Shared FilterAccessScope, FilterVisibilityProfile, and FilterVisibilityService now define Statewide, HHS, Facility, Ward, and Custom filter visibility profiles with hidden/locked filter support and allowed-value hooks.
- Framework integration: FilterFrameworkService consumes visibility profiles so hidden filters continue constraining options, calculations, context summaries, and row scoping without requiring visible controls.
- Proof-of-concept UI: Bed Management uses a Statewide visibility profile for statewide mode and a QCH custom profile that hides HHS/Facility while preserving CHQ/QCH scope for calculations and context.

P3.06 — Filter Persistence Across Pages
- Status: Complete
- Outcome: Filter Persistence Across Pages implemented so operational context follows users between workspaces while preserving multi-select state, cascade validation, parent awareness, role-aware visibility, and access-scope restrictions.
- Persistence architecture: FilterFrameworkService now restores from a shared operational context by default, validates restored selections against the current workspace scope and visibility profile, and keeps a future-ready workspace override persistence mode.
- Context restoration: Bed Management asks the shared framework to restore and normalize filter state on load so KPI, insight, awareness, and header context reflect the current operational scope immediately.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, and future statewide workspaces can adopt the same restore/apply APIs without creating page-local persistence systems.

P3.07 — Operational Filter Chips + Breadcrumbs
- Status: Complete
- Outcome: Operational Filter Chips + Breadcrumbs created to communicate active HHS, Facility, Ward, Service Stream, hidden/locked, multi-select, parent-aware, and persisted operational context across workspace pages.
- Context communication: Shared OperationalFilterBreadcrumbs and OperationalFilterChips components consume FilterContextRecord directly so active scope is visible without duplicating filter state.
- Proof-of-concept UI: Bed Management renders compact breadcrumbs and passive chips near the UniversalFilterBar while preserving existing filters, access selector, KPI cards, insights, operational panels, and routing.
- Deferred behaviour: Clear/remove chip actions remain deferred until guardrails are specified for hidden, locked, persisted, and access-scoped contexts.

P3.08 — Filter Reset + Preset Behaviour
- Status: Complete
- Outcome: Filter Reset + Preset Behaviour created so operational users can clear filters, reset to access scope, reset to workspace defaults, and apply shared context presets while preserving role-aware visibility, parent awareness, persistence, and chips/breadcrumbs.
- Reset architecture: Shared FilterResetMode and FilterFrameworkService reset methods centralize Clear All, Access Scope, and Workspace Default behaviour while preserving hidden/locked allowed-value scope.
- Preset architecture: Shared FilterPresetRecord and FilterPresetType provide framework-owned preset definitions and application paths for statewide, access-scope, QCH, and future operational-focus presets.
- Proof-of-concept UI: Bed Management exposes compact reset buttons and a preset selector near UniversalFilterBar without redesigning filters, workflows, routing, KPI cards, insights, or operational panels.

P3.09 — Cross-Workspace Filter Synchronisation
- Status: Complete
- Outcome: Cross-Workspace Filter Synchronisation created so shared operational context changes can notify compatible workspaces while preserving persistence, access scope, role-aware visibility, parent awareness, reset/preset behaviour, and loop protection.
- Sync architecture: Shared FilterSyncMode, FilterSyncScope, and FilterSyncEvent models plus FilterFrameworkService events now expose normalized workspace id, selection, persistence mode, sync mode, scope, and context summary.
- Loop protection: FilterFrameworkService suppresses restore events and emits change events only when normalized selections actually change; Bed Management ignores its own emitted events and consumes shared events with suppressed re-emission.
- Deferred wiring: Ward Operations, Allocation Centre, Delayed Discharge, and future statewide workspaces can subscribe to the same shared sync event without creating page-local filter stores.

P3.10 — Smart Empty-State Filter Handling
- Status: Complete
- Outcome: Smart Empty-State Filter Handling created so operational users receive meaningful guidance, recovery actions, and contextual explanations when filter selections result in empty operational datasets.
- Empty-state architecture: Shared FilterEmptyStateType, FilterEmptyStateRecord, FilterEmptyStateActionRecord, and FilterEmptyStateService classify filter conflicts, valid empty operational states, access-scope empty states, and prototype data gaps.
- Recovery actions: Shared empty states expose reset/preset-compatible actions that reuse the P3.08 reset/preset flow instead of page-local recovery logic.
- Proof-of-concept UI: Bed Management renders a compact shared OperationalFilterEmptyState when filtered bed rows are empty while preserving KPI cards, insights, chips/breadcrumbs, banners, layout, and routing.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, future service-stream filters, and P3.03 access-scope enforcement can consume the same multi-select filter state without creating another framework.


P2.01 — Prototype Experience Bar
- Status: Complete
- Outcome: Prototype Experience Bar created as the global workshop control layer for Access View, Experience Mode, and Layout Variant selection.
- Deferred UI wiring: Page content adaptation to selected access view, experience mode, and layout variant will follow in later P2/P3 prompts without creating duplicate pages.


P2.02 — Adaptive Access Perspective Engine
- Status: Complete
- Outcome: Adaptive Access Perspective Engine created to centralize perspective-aware visibility, capability, operational emphasis, density, and workflow behaviour.
- Proof-of-concept UI: Prototype Experience Bar now shows adaptive workflow focus and perspective-aware filter visibility chips without changing page routes or duplicating pages.
- Deferred UI wiring: Filters, KPI panels, operational actions, overlays, and workflow layouts will progressively consume the engine in later prompts.


P2.03 — Experience Mode Framework
- Status: Complete
- Outcome: Experience Mode Framework created to centralize progressive awareness, coordination, and workflow operational behaviours without duplicating pages.
- Proof-of-concept UI: Prototype Experience Bar now shows subtle mode interpretation badges and operational density summaries as Experience Mode changes.
- Deferred UI wiring: Dashboards, KPI systems, slideouts, operational actions, orchestration tooling, workflow layouts, and adaptive panel systems will progressively consume the framework in later prompts.


P2.04 — Layout Variant Framework
- Status: Complete
- Outcome: Layout Variant Framework created to centralize stacked, swappable, and compact operational layout behaviour without duplicating pages.
- Proof-of-concept UI: Prototype Experience Bar now shows subtle layout interpretation badges, and the shell root receives reusable layout variant classes for future page-level adaptation.
- Deferred UI wiring: Page panel systems, situational/insights swaps, compact card treatments, and table density changes will progressively consume the framework in later prompts.


P2.05 — Shared Workspace Shell Framework
- Status: Complete
- Outcome: Shared Workspace Shell Framework created to standardize reusable operational workspace structure, adaptive shell behaviour, and future orchestration-ready workspace architecture.
- Proof-of-concept UI: Bed Management is safely wrapped in the shared Workspace Shell while preserving existing page content, filters, overlays, and workflows.
- Deferred UI wiring: Other operational pages, workspace toolbars, insight/operational region migration, side-panel orchestration, and page-level adaptive density behaviours will progressively consume the framework in later prompts.


P2.06 — Persistent Context Awareness Layer
- Status: Complete
- Outcome: Persistent Context Awareness Layer created to maintain shared operational context, workspace context, and adaptive platform state across the prototype session.
- Proof-of-concept UI: Prototype Experience Bar and Workspace Shell now expose a subtle session context summary while preserving existing page routes and workflows.
- Deferred UI wiring: HHS/facility/ward filters, active patient/bed/allocation workflows, and cross-workspace operational memory will progressively consume the context service in later prompts.


P2.07 — Unified Navigation State Engine
- Status: Complete
- Outcome: Unified Navigation State Engine created to centralize route, workspace, primary/secondary nav, shell, and collapse state for future standard/concept navigation alignment.
- Proof-of-concept wiring: Concept primary navigation, ShellSwitcher shell/collapse state, STD menu collapse state, Workspace Shell, and Context Awareness now update/read centralized navigation state without changing routes or redesigning navigation.
- Deferred UI wiring: Standard menu item routing, adaptive nav visibility, workspace-specific secondary nav, and full shared nav rendering will progressively consume the engine in later prompts.


P2.08 — Collapsible Navigation + Routing Repair
- Status: Complete
- Outcome: Collapsible navigation behaviour added and P2.07 menu routing regression repaired.
- Build repair: PrimaryNavigation markup/code-behind was normalized to remove the malformed duplicate method block that caused CI Razor compilation failures while preserving route navigation plus navigation-state updates.
- Proof-of-concept UI: Concept navigation supports a compact collapsed pill that temporarily reveals primary/secondary navigation options, routes selections correctly, and collapses back after selection.
- Deferred UI wiring: Standard shell menu route activation, adaptive navigation visibility, and full shared navigation rendering remain deferred for later prompts.


P2.09 — Shared Workspace Header Pattern
- Status: Complete
- Outcome: Shared Workspace Header pattern created for consistent operational workspace title, context, perspective, mode, layout, and status presentation.
- Proof-of-concept UI: Bed Management passes workspace status and badges through WorkspaceShell into the shared header while preserving existing page content.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, and future orchestration workspaces can adopt the shared header in later prompts without route or content rewrites.


P2.10 — Workspace Density Modes
- Status: Complete
- Outcome: Workspace Density Modes created to support comfortable, balanced, and compact operational workspace density without duplicating pages.
- Proof-of-concept UI: WorkspaceShell applies shared density classes and the Prototype Experience Bar surfaces the interpreted density profile.
- Deferred UI wiring: KPI cards, tables, panels, slideouts, workflow boards, and operational action areas can progressively consume the shared density hooks in later prompts.


P2.11 — Shared Global Action Framework
- Status: Complete
- Outcome: Shared Global Action Framework created to centralize reports, downtime, on-call, chat, support, print/export, and future workspace-aware actions.
- Proof-of-concept UI: Bottom utility navigation now sources existing utility actions from GlobalActionService while preserving chat, reports, downtime, on-call, support, and directory overlay behaviour.
- Deferred UI wiring: Print/export, notifications, and operational event creation remain placeholder action records for progressive workflow integration in later prompts.


P2.12 — Operational Awareness Banner System
- Status: Complete
- Outcome: Operational Awareness Banner System created for shared capacity, downtime, staffing, infection-control, and operational event messaging across workspace contexts.
- Proof-of-concept UI: WorkspaceShell renders OperationalBannerStack below the shared Workspace Header, sourced from OperationalAwarenessService and existing operational banner/event/information banner seed data.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, notification centre, downtime workflows, and operational event creation can progressively consume the shared banner service in later prompts.


P2.13 — Shared KPI Card Framework
- Status: Complete
- Outcome: Shared KPI Card Framework created to standardize operational KPI presentation, severity, trends, density adaptation, and perspective-aware awareness patterns across all workspace contexts.
- Proof-of-concept UI: Bed Management KPI presentation now uses KpiCardGrid/KpiCard with existing KPI values preserved and translated through KpiFrameworkService.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, statewide dashboards, facility dashboards, and orchestration KPI panels can progressively migrate to the shared KPI framework.


P2.14 — Shared Insights Card Framework
- Status: Complete
- Outcome: Shared Insights Card Framework created to standardize operational insight presentation, prioritization, severity interpretation, action recommendations, density adaptation, and context-aware operational sense-making across workspace contexts.
- Proof-of-concept UI: Bed Management Actionable Insights now render through InsightCardGrid/InsightCard while preserving the existing insight content, ward filtering, action labels, and layout flow.
- Deferred UI wiring: Ward Operations, Allocation Centre, Delayed Discharge, statewide insights, facility insights, and orchestration insights can progressively migrate to the shared insight framework.


P2.15 — Shared Operational Panel Framework
- Status: Complete
- Outcome: Shared Operational Panel Framework created to standardize situational, insight, workflow, allocation, discharge, escalation, and review panel patterns across workspace contexts.
- Proof-of-concept UI: Bed Management Actionable Insights now render inside OperationalPanel while preserving the existing InsightCardGrid content, filtering, and page layout flow.
- Deferred UI wiring: Bed Management situational panels, Ward Operations panels, Allocation Centre panels, Delayed Discharge panels, slideout content panels, and orchestration panels can progressively migrate to the shared panel framework.
- Proof-of-concept UI: Concept navigation supports a compact collapsed pill that temporarily reveals primary/secondary navigation options, routes selections correctly, and collapses back after selection.
- Deferred UI wiring: Standard shell menu route activation, adaptive navigation visibility, and full shared navigation rendering remain deferred for later prompts.
P1.15 — Operational rules engine foundation
- Status: Complete
- Outcome: Shared `OperationalRulesService` added for reusable capacity, bed availability, allocation, patient flow, discharge, operational event, perspective capability, and scenario rules.
- Deferred UI wiring: Existing pages keep their current behaviour until later prompts connect rule results to KPI cards, insights, warnings, and workflow panels.
P4.01 — Unified vNext Token System
- Status: Complete
- Outcome: Unified vNext Token System created as the visual source of truth for colours, typography, spacing, elevation, layout sizing, status states, and dark-mode support across the Patient Flow Hub platform.
- Proof-of-concept migration: WorkspaceShell, WorkspaceHeader, and UniversalFilterBar-adjacent shared filter styling now consume the unified tokens while preserving existing operational density and workflow structure.
- Deferred migration: KPI cards, tables, banners, badges, navigation internals, slideouts, and remaining page-level styles should migrate progressively in later P4 prompts without broad redesign.

P4.02 — Unified Typography Hierarchy
- Status: Complete
- Outcome: Unified Typography Hierarchy created to standardise operational text roles, font sizing, weights, line heights, labels, captions, headings, and data-readable typography across shared platform components.
- Proof-of-concept migration: WorkspaceHeader, WorkspaceShell heading context, UniversalFilterBar-adjacent labels/controls, OperationalFilterChips, OperationalFilterBreadcrumbs, and shared filter empty states now consume semantic typography roles while preserving operational density.
- Deferred migration: KPI cards, insight cards, operational panels, dense tables, slideouts, banners, and navigation internals should migrate progressively in later P4 prompts without altering workflows or information density.

P4.03 — Unified Spacing System
- Status: Complete
- Outcome: Unified Spacing System created to standardise numeric and semantic spacing tokens for page padding, shell gaps, panels, cards, filters, chips, tables, banners, empty states, and operational density modes.
- Proof-of-concept migration: WorkspaceShell, WorkspaceHeader, density spacing variants, UniversalFilterBar-adjacent spacing, OperationalFilterChips, OperationalFilterBreadcrumbs, shared filter actions, and OperationalFilterEmptyState now consume semantic spacing roles while preserving compact operational density.
- Deferred migration: KPI cards, insight cards, operational panels, dense tables, slideouts, banners, utility drawers, and navigation internals should migrate progressively in later P4 prompts without layout redesign or workflow changes.

P4.04 — Unified Card Framework
- Status: Complete
- Outcome: Unified Card Framework created to standardise operational card containers, headers, bodies, footers, actions, density, interaction, selection, status variants, and dark-mode compatibility for shared card patterns.
- Proof-of-concept migration: Shared KPI cards, shared insight cards, and shared operational panels now consume central card tokens/classes for container surface, border, radius, shadow, padding, density, hover/focus, and status accent behaviour while preserving existing data and workflows.
- Deferred migration: Page-local cards, legacy metric cards, slideout cards, utility drawer cards, navigation cards, and remaining Bed/Ward/Allocation/Delayed Discharge card variants should migrate progressively in later prompts without page redesign.

P4.05 — Unified Table Framework
- Status: Complete
- Outcome: Unified Table Framework created to standardise operational table containers, headers, rows, cells, compact density, sticky-header support, hover/selected/disabled row states, status/action cells, empty states, responsive overflow, and dark-mode compatibility for shared table patterns.
- Proof-of-concept migration: Bed Management and Ward Operations table/list views now consume central table tokens for container surface, border, radius, header styling, row borders, compact cell padding, hover states, status/action spacing, horizontal overflow, and dark-mode surfaces while preserving existing table data, sorting, filtering, and workflow actions.
- Deferred migration: Allocation Centre tables, Delayed Discharge tables, utility drawer tables, slideout tables, and remaining page-local table variants should migrate progressively in later prompts without page redesign or data hierarchy changes.
