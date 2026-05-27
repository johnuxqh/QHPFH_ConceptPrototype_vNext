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

## Dark Mode and Theme Compatibility Safety

### Theme governance goals
Future changes must preserve operational readability in both light and dark mode for long-session clinical and operations usage.

### Theme rules
- Prefer shared tokens/CSS variables over one-off hard-coded component colors.
- Do not introduce hard-coded `#ffffff`/`white` light surfaces in reusable/shared components when a token can be used.
- Do not introduce hard-coded `#000000`/`black` text/icon colors in reusable/shared components; prefer inherited or tokenized theme text colors.
- Preserve clinically meaningful status colors across themes:
  - red = critical/escalation
  - amber = warning/moderate pressure
  - green = normal/BAU
  - blue = informational/contextual
- Maintain high contrast and operational scanability; avoid grey-on-grey and low-visibility controls.
- Inputs/controls (filters, selects, buttons, tables, overlays, slideouts, toasts) must remain readable in dark mode.

### Required theme validation checklist
After any styling/theme change, validate at minimum:
1. Light mode readability on key workflow screens.
2. Dark mode readability on key workflow screens.
3. Hover states remain visible in both themes.
4. Focus states remain visible in both themes.
5. Overlay/slideout/toast contrast is readable in both themes.
6. Table/grid text and separators remain legible in both themes.
7. Icons are visible in both themes.
8. Dropdowns/selects remain readable in both themes.
9. Operational severity/status cues remain easy to distinguish in both themes.

### Lightweight theme audit expectation
For small governance/stabilization tasks, perform a lightweight scan for obvious reusable hard-coded theme risks and prefer minimal, token-first corrections over broad restyling.

## Operational UX Governance Rules

### Operational UX philosophy
QH Patient Flow Hub is an operational healthcare coordination platform, situational awareness platform, and orchestration workspace.
It is not a passive reporting portal or a generic BI dashboard.

Future UX changes must prioritise:
- operational usability
- rapid scanning
- escalation awareness
- workflow efficiency
- actionability
- clinical readability
- low cognitive load

### Workflow-first principles
- Design and interactions must support decision-making, coordination, prioritisation, escalation handling, and patient flow action.
- Metrics/insights must explain operational state and drive action, not decorative display.
- Avoid vanity dashboard patterns or charting without explicit operational purpose.

### Situational awareness principles
- Users must always be able to understand where they are, the active operational scope, pressure state, escalations, and current flow status.
- Preserve continuous awareness cues across the shell and operational pages.

### Dense operational usability and readability
- Preserve high-density operational workflows for shift-based use.
- Prioritise scanability over decorative spacing.
- Avoid oversized-card or excessive whitespace patterns that reduce operational throughput.
- Keep information clinically readable, prioritised, and color-safe.

### Escalation and status semantics
- Preserve severity meaning and rapid recognition:
  - red = critical/escalation
  - amber = warning/moderate pressure
  - green = normal/BAU
  - blue = informational/contextual
- Critical states and blockers should remain visually discoverable at a glance.

### Workflow continuity rules
- Prefer contextual overlays, slideouts, and progressive drill-down to maintain workflow continuity.
- Avoid unnecessary page-hopping, modal traps, and disjointed flow interruptions.

### Operational hierarchy and interaction model
- Preserve coherent hierarchy across statewide, facility, ward, and patient-level operations.
- Avoid fragmenting the prototype into disconnected products or standalone dashboard islands.

### Realism and prototype behavior expectations
- Keep interactions clinically grounded and operationally believable.
- Avoid gimmicks, game-like behavior, or unrealistic automation concepts.

## Reusable Codex Implementation Guidance (Operational UX)
Use these standards in future implementation prompts unless explicitly overridden:
- Do not over-dashboard.
- Preserve operational workflows.
- Prioritise scanability.
- Maintain operational density.
- Avoid decorative UI.
- Keep insights actionable.
- Preserve escalation visibility.
- Keep controls workflow-adjacent, not buried behind extra navigation.
- Prefer smallest safe change over broad UX rework.

## Workshop / Prototype Mode Guidance

### Access View philosophy
Access View variations simulate operational perspectives (for example executive, command, bed manager, ward coordinator) and information emphasis.
They are prototype perspective simulations, not production authentication/authorization models.

### Experience Mode philosophy
Experience Modes simulate different operational contexts and workflow emphasis (for example concept vs standard operations expression) while preserving shared platform behavior and awareness.
They are not separate products.

### Layout Variant philosophy
Layout Variants can adjust density, emphasis, and arrangement to support operational tasks, but must preserve workflow continuity, route stability, and shared data context.
Variants represent operational presentation options, not independent application architectures.

## Shared Component Preservation Rules

### Shared component philosophy
As this prototype scales, reusable operational component discipline is mandatory.
Future work must prioritise shared components and shared interaction systems over page-specific one-off implementations.

### Reuse-first principles
- Reuse before rebuild: check for existing shared patterns/components first.
- Extend existing shared components where practical.
- Do not create one-off variants unless explicitly required by scope.
- Keep behavior and styling aligned with existing operational systems.

### Anti-duplication rules
Avoid introducing duplicate systems unless explicitly required:
- multiple KPI card systems
- multiple alert systems
- multiple slideout systems
- multiple overlay systems
- multiple patient card systems
- multiple filter systems
- multiple quick-action systems

### Interaction consistency rules
Preserve consistent interaction patterns across Bed Management, Ward Operations, Allocation Centre, Delayed Discharge, and scenario/orchestration workspaces, including:
- hover/click behavior
- overlay/slideout behavior
- quick actions
- density handling
- selection behavior
- alert handling

### Operational pattern and language preservation
Preserve shared operational terminology and avoid duplicate terms for identical concepts.
Examples include:
- Bed Management
- Ward View
- Allocation Centre
- Operational Events
- Delayed Discharge
- Open Beds
- Pending Beds
- Outliers
- EDD
- Isolation Beds

### Adaptive architecture philosophy
The platform should continue evolving toward:
- shared operational data
- adaptive operational perspectives
- shared workflows
- shared layouts
- shared interaction systems

Do not fragment into disconnected implementations.

### Controlled evolution expectations
- Avoid unnecessary rewrites of working components.
- Avoid large structural moves without clear, explicit need.
- Prefer incremental alignment and safe extension.
- Preserve stable implementation unless explicitly instructed otherwise.

### Component naming and alignment guidance
- Prefer names that reflect shared operational purpose, not page-local context.
- Avoid creating near-duplicate names for equivalent patterns.
- If creating a new shared component, document why an existing one was not extended.
- Keep naming consistent with current domain language.

## Reusable Implementation Checklist (Before Creating a New Component)
1. Does a similar shared component/pattern already exist?
2. Can an existing shared component be extended safely?
3. Does this introduce duplicate UX behavior?
4. Does this fragment operational workflows?
5. Does this preserve adaptive architecture direction?
6. Is naming aligned with shared operational language?
7. Is this the smallest safe change to satisfy the prompt?

If any answer indicates duplication/fragmentation risk, prefer extending existing shared systems.

## Lightweight Shared Inventory Placeholders (Documentation)
Track and align these shared systems before adding new variants:
- KPI cards
- overlays
- slideouts
- patient cards
- operational alerts
- filters
- activity feeds
- action panels

For each system, future work should document:
- existing implementations in use
- intended shared base pattern
- known divergences and whether intentional
- safe consolidation opportunities

## Prototype Stability and Regression Prevention Rules

### Stability objective
Protect the prototype from avoidable regressions while enabling safe, incremental evolution for workshop and clinical demonstration use.

### Smallest safe change principle (stability)
- Make the smallest safe change possible.
- Avoid unrelated refactors and broad rewrites.
- Avoid touching unrelated files.
- Avoid replacing stable systems unless explicitly required.

### Incremental merge philosophy
Prefer:
- small PRs
- incremental architecture evolution
- frequent build/check validation
- frequent deployment/path validation

Avoid:
- giant multi-system rewrites
- large untested merges
- stacked unvalidated architectural changes

### Preserve working workflows
Do not break existing:
- navigation and routes
- dark mode behavior
- overlays/slideouts
- operational workflows
- Bed Management / Ward Operations / Allocation Centre flows

unless explicitly requested.

### Shared architecture awareness
Future changes must preserve:
- shared state philosophy
- shared operational workflows
- shared component systems
- adaptive access architecture
- GitHub Pages compatibility

### Avoid silent regressions
Future changes should validate at minimum:
- routes still load
- pages still render
- overlays still open
- filters still function
- dark mode still works
- no new console errors are introduced

### Preserve operational continuity
The prototype should remain:
- operational
- clinically believable
- workflow-oriented
- stable during demonstrations

Avoid visible dead ends, broken interactions, or partially implemented UI in active workflows.

### Build and deployment validation expectations
Every future task must:
- run the available build/check command
- report pass/fail status
- explain changed files
- explain known risks/deferred work

After route/base-path/shell changes, validate GitHub Pages behavior and deep links under:
- `/QHPFH_ConceptPrototype_vNext/`

### Rollback and checkpoint philosophy
- Keep changes reviewable and rollback-friendly.
- Prefer checkpoint-style, scoped commits over monolithic changes.
- Document riskier decisions so future rollback/recovery is clear.

## Lightweight Regression Checklist Template
Use this template in future final task responses:

- Build status: PASS/FAIL
- Changed files: <list>
- Routes tested: <list>
- Pages tested: <list>
- Dark mode tested: yes/no
- Overlays/slideouts tested: yes/no
- Filters tested: yes/no
- Console errors checked: yes/no
- GitHub Pages routing/deployment path checked: yes/no
- Known risks/deferred work: <list or "none">

## High-Risk Change Guidance (Extra Caution Required)
The following change types are high risk and require extra validation depth:
- routing changes
- shell/navigation changes
- shared state changes
- filter architecture changes
- overlay/slideout system changes
- global CSS changes
- token/theme system changes
- layout wrapper changes
- session-state behavior changes

For high-risk changes, increase validation coverage and prefer incremental rollout over bundled multi-system edits.

## Session State and Demo Reset Governance Rules

### Session-state philosophy
The prototype should behave like a shared operational simulation with a common in-session state layer and cross-workspace awareness.
Within an active browser session, operational interactions should feel connected and persistent.

### Session-scoped persistence expectations
During a single browser session, operational changes should persist while:
- navigating between pages/routes
- switching access perspectives
- switching experience modes
- switching layout variants
- opening/closing overlays and slideouts

Examples of in-session writeback behavior:
- operational banner updates
- patient allocation/movement updates
- bed status updates
- activity feed additions
- workflow state transitions

### Browser refresh reset philosophy
A browser refresh should intentionally reset the prototype to seeded demo conditions:
- seeded demo data
- seeded operational state
- seeded notifications/events

This is expected for workshop simulation usage and is not a defect.

### No real persistence yet
At this stage, do not introduce:
- production databases
- server-side persistence
- authentication/session persistence infrastructure
- production-grade backend APIs

GitHub Pages compatibility and lightweight deployment remain core constraints.

### Shared operational truth rules
All operational workspaces should reflect the same in-session state.
Examples:
- patient movements in Allocation Centre should appear in Ward/Bed contexts
- operational banner changes should be visible across relevant views
- bed status changes should drive consistent KPI recalculation

Avoid disconnected, page-local state islands for shared operational concepts.

### Operational continuity expectations
Session behavior should feel:
- believable
- operationally realistic
- continuous across workflows
- coherent across perspectives

Avoid hard-refresh-like behavior during normal navigation and avoid fake isolated widgets.

### Future architecture direction (non-production)
Future implementation should move incrementally toward:
- shared `PrototypeDataStore`
- shared operational models/state
- shared event propagation
- shared derived KPI calculations
- lightweight shared session cache

without introducing a production backend in this prototype phase.

## Future Session-State Implementation Guidance
Expected future behavior standards:
- Patient movement propagation: movement in one operational workspace is reflected across related workspaces.
- Operational banner propagation: banner/notification updates appear consistently across relevant operational views.
- KPI recalculation: derived KPIs recompute from shared session state after state-changing actions.
- Overlay continuity: overlays/slideouts should preserve workflow context and read/write against shared session state.
- Access-perspective continuity: switching perspective/mode should keep the same underlying session truth while adapting presentation density/emphasis.

## Lightweight Future Architecture Placeholders (Documentation)
Use these as planned alignment targets for future implementation tasks:

### PrototypeDataStore (planned)
- In-memory/session-scoped source of operational truth for the active browser session.
- Seeded from demo data at load and reset on browser refresh.

### Shared operational state (planned)
- Common structures for beds, patients, allocations, alerts, and workflow markers.
- No per-page duplication for shared concepts.

### Shared event propagation (planned)
- Standard mechanism for publishing/observing operational state changes.
- Enables cross-workspace updates without hard-coupled page logic.

### Shared KPI calculation engine (planned)
- Derived metrics computed from shared session state, not manually duplicated per page.
- Ensures consistent KPI semantics across workspaces.

### Shared session cache (planned)
- Lightweight browser-session cache behavior only.
- Reset on refresh to seeded demo baseline.
- No production persistence guarantees.

## Accessibility and Contrast Validation Governance Rules

### Operational accessibility philosophy
Accessibility in this prototype is primarily an operational usability requirement for healthcare workflows.
It must support rapid recognition, long-session readability, fatigue reduction, and workflow safety in both light and dark environments.

### Contrast and readability expectations
Future changes must preserve readable:
- text
- icons
- tables/grids
- operational alerts
- overlays/slideouts
- filters/controls

Avoid low-contrast grey-on-grey patterns, washed-out typography, or hidden controls/icons.

### Clinically meaningful colour usage
Colour should reinforce severity and operational awareness, but should not be the only indicator.
Where practical, reinforce meaning with labels/icons/text/pattern cues in addition to colour.

### Dark mode accessibility rules
Dark mode must preserve:
- readable text and icons
- readable controls
- visible hover states
- visible focus states
- readable overlays/slideouts

Avoid black-on-dark, low-contrast greys, and washed-out action states.

### Keyboard and focus awareness
Future interaction changes should remain keyboard-aware and must not block keyboard navigation evolution.
Ensure focus visibility remains discoverable for interactive controls and maintain usable click-target sizes.

### Density with readability
Operational density is acceptable and expected, but dense screens must remain scannable, readable, and navigable.
Do not trade readability for decorative compactness.

### Accessibility-safe interaction principles
- Prefer clear hierarchy and explicit action affordances.
- Avoid subtle/low-opacity interaction states that hide operability.
- Keep operational usability ahead of decorative styling trends.

### Overlay and slideout readability expectations
Overlays/slideouts should maintain:
- readable hierarchy
- readable action labels
- visible close/escape affordances
- sufficient foreground/background contrast

## Reusable Accessibility Validation Checklist
Use this checklist for future UI-related changes:
- Text readability verified (light + dark)
- Icon visibility verified (light + dark)
- Hover visibility verified
- Focus visibility verified
- Dark mode control readability verified
- Overlay/slideout readability verified
- Table/grid readability verified
- Operational alert readability verified
- Colour dependency checked (not colour-only where practical)
- Keyboard interaction not regressed (basic tab/focus flow)
