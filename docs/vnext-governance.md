# QH Patient Flow Hub vNext Governance (Codex Safety Rules)

## Purpose
This document defines the lightweight governance and safety checkpoints for all future Codex changes to the QH Patient Flow Hub vNext prototype.

Primary objective: keep changes safe, small, reviewable, and non-disruptive to stakeholder demo stability.

## Non-Negotiable Stability Rules

### 1) Preserve Routing
Do not break, remove, or rename existing public routes unless explicitly requested.
Current routes that must continue to work include:
- `/`
- `/bed-management`
- `/ward-operations`
- `/allocation-centre`
- any current Bed & Ward routes
- any current Delayed Discharge routes
- Home/MyHub routes
- Connected Apps routes (if present)

### 2) Preserve GitHub Pages Compatibility
Do not break base path behavior or deployment behavior.
The app must continue to run under the current GitHub Pages repository path.

### 3) Preserve Existing Screens
Do not remove current screen content or UI sections unless explicitly requested.
Do not replace existing pages with placeholders.

### 4) Preserve Current Visual Layout
No broad visual redesigns.
Do not change spacing, card structure, navigation, typography, or color systems unless explicitly requested.

### 5) Preserve Operational Capability
Do not remove or degrade existing operational concepts/features, including:
- filters
- navigation modes
- toaster utilities
- dark mode
- slideouts/panels (if present)
- allocation concepts
- bed/ward views
- patient-related concepts
- reports/downtime/chat/support concepts

### 6) Preserve Data Safety
Demo data must remain fictional and privacy-safe.
Use comic-book civilian names only.
Do not introduce realistic/random real-world patient or staff names.

### 7) Smallest Safe Change Principle
For every task:
- implement the smallest safe change that satisfies the request
- avoid unrelated refactors
- avoid large rewrites
- avoid touching unnecessary files
- clearly explain every changed file

### 8) Build Validation Is Required
Before completing any task:
1. Run the available build/check command.
2. Confirm pass/fail status.
3. If build fails, fix before finishing unless genuinely blocked.
4. Report any remaining issues clearly.

### 9) Regression Checklist Is Required
Every final Codex response must include a short checklist stating:
- build status
- routes affected
- pages affected
- components affected
- known risks
- manual test steps

## GitHub Pages Base Path and Routing Safety

Current expected GitHub Pages base path:
- `/QHPFH_ConceptPrototype_vNext/`

Required protections:
- Keep `<base href="/QHPFH_ConceptPrototype_vNext/" />` aligned with current repo deployment path unless explicitly changing hosting strategy.
- Do not casually change base href, because it can break CSS/script loading and internal client-side routing on GitHub Pages.
- Keep or maintain a deep-link fallback strategy (for example `wwwroot/404.html`) so direct URLs continue to load the Blazor app.
- Remove or update stale references to old repo paths (for example `/QHPFH_ConceptPrototype/`) unless a deliberate fallback is documented in the change.
- Prefer framework-safe internal navigation (relative/component routing) over hard-coded absolute URLs.

Required deep-route smoke tests after any routing/base-path change:
1. Open `/QHPFH_ConceptPrototype_vNext/`
2. Open `/QHPFH_ConceptPrototype_vNext/bed-management`
3. Open `/QHPFH_ConceptPrototype_vNext/ward-operations`
4. Open `/QHPFH_ConceptPrototype_vNext/allocation-centre`
5. Confirm app shell and page content load as expected from direct URL entry.

## Expected Codex Workflow (Every Task)
1. Read the prompt and identify explicit constraints.
2. Identify the minimum file set required.
3. Make the smallest safe change only.
4. Re-check for accidental route/layout/feature changes.
5. Run build/check.
6. Provide concise change summary + regression checklist.
7. Preserve existing behavior unless prompt explicitly instructs otherwise.

## Default Behavior Preservation Rule
Unless explicitly instructed otherwise in a prompt, preserve all existing:
- behavior
- routes
- shell/navigation structure
- major page content
- operating concepts and demo flows

## Quick Validation Checklist Template
Use this at the end of each task:

- Build status: PASS/FAIL
- Routes affected: <list or "none">
- Pages affected: <list or "none">
- Components affected: <list or "none">
- Known risks: <list or "none">
- Route fallback file changed: yes/no (and which file)
- Base href changed: yes/no (and why)
- Manual test steps:
  1. Launch app
  2. Visit key routes (`/`, `/bed-management`, `/ward-operations`, `/allocation-centre`)
  3. Verify expected screen content is still present
  4. Verify shell/navigation mode behavior
  5. Verify deep links under `/QHPFH_ConceptPrototype_vNext/` load correctly

## Scope
This governance file applies to all future Codex work in this repository unless superseded by explicit user/developer/system instructions.
