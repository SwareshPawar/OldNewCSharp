# Archive Notice

This file is archived as a reference baseline map.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](../CSHARP_MIGRATION_PLAN.md)
- Keep this file for source mapping reference only.
- Do not update migration progress/status in this file.

---

# Node Main Migration Baseline

This document defines the migration baseline from the Node.js source repository into this .NET repository.

## Source of Truth

- Source repo: https://github.com/SwareshPawar/OldandNew
- Source branch: `main`
- Local reference snapshot in this workspace: `refs/OldandNew-main`
- Target repo: this repository (`OldNewCSharp-1`)

## Migration Goal

Migrate all granular product behavior from the Node.js app into C#/.NET while keeping data compatibility on the shared MongoDB database.

## Primary Source File Map (Node.js)

### Backend API and Data Flow
- `refs/OldandNew-main/server.js`
  - Songs CRUD routes (`/api/songs`)
  - UserData routes (`/api/userdata`)
  - JWT middleware + admin checks
  - MongoDB collection usage
- `refs/OldandNew-main/api/index.js`
  - Alternate serverless API path(s)

### Main End-User App
- `refs/OldandNew-main/index.html`
  - 3-panel UX behavior, tabs, filters, setlists, favorites
  - song preview interactions, transpose controls, auto-scroll
  - auth UI, user data load/save patterns
- `refs/OldandNew-main/main.js`
  - split runtime logic used by the app shell
- `refs/OldandNew-main/styles.css`
  - visual language and interaction styles

### Feature Modules (Granular Behavior)
- `refs/OldandNew-main/scripts/features/songs-ui.js`
- `refs/OldandNew-main/scripts/features/song-preview-ui.js`
- `refs/OldandNew-main/scripts/features/song-crud-ui.js`
- `refs/OldandNew-main/scripts/features/setlists.js`
- `refs/OldandNew-main/scripts/features/smart-setlists.js`
- `refs/OldandNew-main/scripts/features/auth-ui.js`
- `refs/OldandNew-main/scripts/features/mobile-ui.js`
- `refs/OldandNew-main/scripts/features/admin-ui.js`
- `refs/OldandNew-main/scripts/features/password-reset.js`

### Shared/Core Utilities
- `refs/OldandNew-main/scripts/core/api-base.js`
- `refs/OldandNew-main/scripts/core/auth-client.js`
- `refs/OldandNew-main/scripts/shared/chord-normalization.js`
- `refs/OldandNew-main/scripts/shared/rhythm-set.js`

### Admin and Data Tools
- `refs/OldandNew-main/SONGSADMINCHURCHCHORDS.html` (if present in history; use current admin pages in `scripts/features/admin-ui.js` first)
- `refs/OldandNew-main/migrate-*.js`
- `refs/OldandNew-main/verify-*.js`

## Target Mapping (C#)

- API behavior: `src/OldandNewClone.Web/Controllers`
- Business logic: `src/OldandNewClone.Application/Services`
- Persistence: `src/OldandNewClone.Infrastructure/Repositories`
- Domain entities: `src/OldandNewClone.Domain/Entities`
- Web UI shell: `src/OldandNewClone.Web/Components/Pages/SongShell.razor`

## Execution Rules

- Keep MongoDB schema compatibility with Node.js fields where required.
- Preserve endpoint behavior parity before introducing design improvements.
- Migrate feature-by-feature using acceptance tests per feature module.
- Treat this file and `CSHARP_MIGRATION_PLAN.md` as migration references; canonical status remains in `CSHARP_MIGRATION_PLAN.md`.

## Immediate Next Migration Sprint

1. Baseline parity for songs list + filters + details preview.
2. Setlists parity (Global/My/Smart behavior rules).
3. Favorites + UserData sync parity.
4. Admin song CRUD parity (modal dialogs in web shell).
5. Chord transpose and auto-scroll parity checks.
6. Auth and role behavior parity verification against `server.js`.
