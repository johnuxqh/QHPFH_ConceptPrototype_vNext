# QH Patient Flow Hub vNext Roadmap

This roadmap tracks staged Codex implementation prompts for the vNext prototype.

> The roadmap is a working implementation tracker for staged Codex prompts and should be updated at the end of each prompt.

## Phase Tracker

| Phase | Prompt IDs | Status | Dependencies | Testing / Checkpoint Notes |
| --- | --- | --- | --- | --- |
| P0 — Governance + Safety | P0.01, P0.02, P0.03, P0.04, P0.05, P0.06, P0.07, P0.08, P0.09, P0.10 | Complete | None | Governance baseline established before architecture work. |
| P1 — Data + State Foundation | P1.01, P1.02, P1.03, P1.04, P1.05, P1.06, P1.07, P1.08, P1.09, P1.10, P1.11, P1.12, P1.13, P1.14 | Complete | P0 complete | Shared model/data scaffold, central in-memory store, and session writeback foundation are in place; shared HHS/facility/ward/bed model layer refined for future filters, access scopes, KPI calculations, and bed workflows; shared patient workflow models added for future patient slideouts, ward workflows, delayed discharge, and allocation workflows; shared allocation models added for future incoming streams, transfer coordination, future bed allocation, transit beds, and cross-page allocation workflows; shared operational event models added for future operational awareness, escalation, orchestration, downtime, and coordination workflows; shared activity feed models added for future operational audit trails, patient movement history, bed status changes, allocation activity, and escalation tracking; shared notification models added for future notification centre, toaster notifications, role-aware alerts, acknowledgement states, and cross-workspace messages; shared user perspective models added for adaptive operational experiences, access-aware workflows, operational emphasis switching, and future perspective-driven UI adaptation; shared scenario simulation models added for future what-if planning, operational modelling, capacity/demand scenarios, and orchestration workflows; shared mock API/data service layer created to wrap PrototypeDataStore and provide future page/component query and mutation access; shared demo data seeder refined for maintainable seeded hierarchy, patient, allocation, operational, notification, perspective, and scenario data; KPI calculation engine foundation added for centralized derived operational snapshots and rollups; UI wiring deferred to later P1 prompts. |
| P2 — Shell + Platform Framework | P2.01, P2.02, P2.03 | In Progress | P1 | P2.01 complete: Prototype Experience Bar created as the global workshop control layer for Access View, Experience Mode, and Layout Variant selection. P2.02 complete: Adaptive Access Perspective Engine created to centralize perspective-aware visibility, capability, operational emphasis, density, and workflow behaviour. P2.03 complete: Experience Mode Framework created to centralize progressive awareness, coordination, and workflow operational behaviours without duplicating pages. Keep routing/base-path stable while aligning shell architecture. |
| P2 — Shell + Platform Framework | P2.01, P2.02 | In Progress | P1 | P2.01 complete: Prototype Experience Bar created as the global workshop control layer for Access View, Experience Mode, and Layout Variant selection. P2.02 complete: Adaptive Access Perspective Engine created to centralize perspective-aware visibility, capability, operational emphasis, density, and workflow behaviour. Keep routing/base-path stable while aligning shell architecture. |
| P1 — Data + State Foundation | P1.01, P1.02, P1.03, P1.04, P1.05, P1.06, P1.07, P1.08, P1.09, P1.10, P1.11, P1.12, P1.13, P1.14, P1.15 | Complete | P0 complete | Shared model/data scaffold, central in-memory store, and session writeback foundation are in place; shared HHS/facility/ward/bed model layer refined for future filters, access scopes, KPI calculations, and bed workflows; shared patient workflow models added for future patient slideouts, ward workflows, delayed discharge, and allocation workflows; shared allocation models added for future incoming streams, transfer coordination, future bed allocation, transit beds, and cross-page allocation workflows; shared operational event models added for future operational awareness, escalation, orchestration, downtime, and coordination workflows; shared activity feed models added for future operational audit trails, patient movement history, bed status changes, allocation activity, and escalation tracking; shared notification models added for future notification centre, toaster notifications, role-aware alerts, acknowledgement states, and cross-workspace messages; shared user perspective models added for adaptive operational experiences, access-aware workflows, operational emphasis switching, and future perspective-driven UI adaptation; shared scenario simulation models added for future what-if planning, operational modelling, capacity/demand scenarios, and orchestration workflows; shared mock API/data service layer created to wrap PrototypeDataStore and provide future page/component query and mutation access; shared demo data seeder refined for maintainable seeded hierarchy, patient, allocation, operational, notification, perspective, and scenario data; KPI calculation engine foundation added for centralized derived operational snapshots and rollups; shared operational rules engine added for reusable capacity, bed availability, allocation, patient flow, discharge, operational event, perspective capability, and scenario rules; UI wiring deferred to later P1 prompts. |
| P2 — Shell + Platform Framework | Pending | Pending | P1 | Keep routing/base-path stable while aligning shell architecture. |
| P3 — Filter + Access Architecture | Pending | Pending | P1, P2 | Add shared filter state and access perspectives incrementally. |
| P4 — Design System + Visual Alignment | Pending | Pending | P2 | No broad redesign; align visual patterns through incremental updates only. |
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
P1.15 — Operational rules engine foundation
- Status: Complete
- Outcome: Shared `OperationalRulesService` added for reusable capacity, bed availability, allocation, patient flow, discharge, operational event, perspective capability, and scenario rules.
- Deferred UI wiring: Existing pages keep their current behaviour until later prompts connect rule results to KPI cards, insights, warnings, and workflow panels.
