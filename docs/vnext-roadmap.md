# QH Patient Flow Hub vNext Roadmap

This roadmap tracks staged Codex implementation prompts for the vNext prototype.

> The roadmap is a working implementation tracker for staged Codex prompts and should be updated at the end of each prompt.

## Phase Tracker

| Phase | Prompt IDs | Status | Dependencies | Testing / Checkpoint Notes |
| --- | --- | --- | --- | --- |
| P0 — Governance + Safety | P0.01, P0.02, P0.03, P0.04, P0.05, P0.06, P0.07, P0.08, P0.09, P0.10 | Complete | None | Governance baseline established before architecture work. |
| P1 — Data + State Foundation | P1.01, P1.02, P1.03, P1.04, P1.05, P1.06 | Complete | P0 complete | Shared model/data scaffold, central in-memory store, and session writeback foundation are in place; shared HHS/facility/ward/bed model layer refined for future filters, access scopes, KPI calculations, and bed workflows; shared patient workflow models added for future patient slideouts, ward workflows, delayed discharge, and allocation workflows; shared allocation models added for future incoming streams, transfer coordination, future bed allocation, transit beds, and cross-page allocation workflows; UI/page wiring deferred to later P1 prompts. |
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
