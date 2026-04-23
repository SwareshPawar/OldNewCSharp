# Copilot Instructions

## Project Guidelines
- The app should be designed and evolved as a Teams-style 3-panel workspace: Panel 1 is primary workspace navigation, Panel 2 is the active list context, Panel 3 is the focused content/detail view. Theme must be global across all panels and not change per selected song. Add/Edit forms should open as dialogs/modals instead of taking over a panel.
- Smart setlists use a different automated mechanism and should remain read-only (no manual add/remove), while Global/My setlists support manual management. Smart setlists should also provide a sync songs action from Panel 1.
- Setlist edit/update/delete actions should be exposed on Panel 1 as icon-only controls for each setlist item, rather than in Panel 3.
- Include stronger CSS polish as part of this phase or upcoming phases while implementing setlist workflow enhancements.

## Editing Preferences
- Prefer minimal, targeted partial edits to avoid whole-file rewrites that can unintentionally remove code.