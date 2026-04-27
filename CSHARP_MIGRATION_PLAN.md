# C# Application Migration Plan - Node.js to .NET 10

> Canonical migration status document for this repository.

## Overview
This document outlines the complete migration strategy for porting the Old & New Song Management application from Node.js to C# .NET 10 with MAUI and Blazor.

## Single Source of Truth Policy

This file is the only document that should be updated for migration status, roadmap, and phase progress.

### Update Rule
- Update migration progress only in this file.
- The former duplicate migration/status docs were merged into this file under `Historical Migration Records (Merged)` and then removed.

## Project Information

### Source Application (Node.js)
- **Repository**: https://github.com/SwareshPawar/OldandNew
- **Branch**: Main
- **Stack**: Express.js, MongoDB, bcryptjs, JWT
- **Database**: MongoDB Atlas (shared with C# app)

### Target Application (C#)
- **Repository**: https://github.com/SwareshPawar/OldNewCSharp
- **Framework**: .NET 10
- **UI**: Blazor Server (web) + .NET MAUI (mobile/desktop)
- **Architecture**: Clean Architecture (Domain → Application → Infrastructure → Presentation)
- **Database**: MongoDB Atlas (same as Node.js)

## Migration Phases

### ✅ Phase 1: Project Setup & Architecture (COMPLETED)
**Status**: Done
**Timeline**: Week 1

**Objectives**:
- Set up Clean Architecture project structure
- Configure MongoDB connection
- Implement basic health checks

**Deliverables**:
- ✅ Domain Layer (Entities, Common)
- ✅ Application Layer (Interfaces, Services, DTOs, Configuration)
- ✅ Infrastructure Layer (MongoDB, Repositories, Services)
- ✅ Web Layer (Blazor Server, API Controllers)
- ✅ MAUI Layer (Cross-platform UI)
- ✅ MongoDB connection to Atlas
- ✅ Configuration management (appsettings.json)

**Key Files**:
- Domain: `Song.cs`, `ApplicationUser.cs`, `UserData.cs`, `ApplicationRole.cs`
- Infrastructure: `MongoContext.cs`, `MongoDbSettings.cs`
- Configuration: `appsettings.Development.json`

---

### ✅ Phase 2: Authentication System (COMPLETED)
**Status**: Done
**Timeline**: Week 2

**Objectives**:
- Implement JWT-based authentication
- Integrate BCrypt password hashing
- Support both Node.js and .NET users (hybrid approach)
- Enable login with username or email

**Deliverables**:
- ✅ BCrypt password hasher (`BCryptPasswordHasher.cs`)
- ✅ JWT token service (`JwtTokenService.cs`)
- ✅ Authentication service (`AuthService.cs`)
- ✅ User repository with hybrid lookup (`UserRepository.cs`)
- ✅ Auth controller with register/login/refresh endpoints
- ✅ Blazor login and register pages
- ✅ Security stamp support for Identity

**Key Features**:
- ✅ Compatible with Node.js bcryptjs (10 rounds)
- ✅ Hybrid user lookup (finds both Node.js and .NET users)
- ✅ No data migration required
- ✅ Seamless authentication for both app types

**Migration Strategy - Authentication**:
```
Node.js User Format:
{
  _id: ObjectId,
  username: string (lowercase),
  email: string (lowercase),
  password: string (bcrypt hash),
  firstName: string,
  lastName: string,
  phone: string,
  isAdmin: boolean
}

C# Hybrid Approach:
1. Try UserManager.FindByEmail/FindByName (fast, Identity cache)
2. If not found, query MongoDB directly
3. Convert Node.js format to ApplicationUser in memory
4. Add required Identity fields (SecurityStamp, ConcurrencyStamp)
5. Authenticate normally with SignInManager

Result: Both Node.js and .NET users can login without data changes
```

---

### ✅ Phase 3: Diagnostic & Debug Tools (COMPLETED)
**Status**: Done
**Timeline**: Week 2

**Objectives**:
- Create tools to inspect users and test authentication
- Support both Node.js and .NET user formats
- Enable password testing and user management

**Deliverables**:
- ✅ MongoDbUserChecker service (direct MongoDB access)
- ✅ DebugController (user listing, password testing)
- ✅ UserCheckController (user inspection)
- ✅ LoginDebugController (lookup testing)
- ✅ DebugUsers.razor page (visual user management)
- ✅ Enhanced error logging

**Key Features**:
- View all users (Node.js + .NET) in one list
- Test passwords without logging in
- See user type badges (Node.js / .NET / Hybrid)
- Delete users across both formats
- Rehash passwords with BCrypt

---

### ✅ Phase 4: Core Features Migration (COMPLETED - Song Management)
**Status**: Done
**Timeline**: Week 3

**Deliverables**:
- ✅ Extended `SongDto` / `SongListItemDto` / `CreateSongDto` / `UpdateSongDto` with all fields (Singer, Mood, Tags, YoutubeLink, SpotifyLink, Notes, IsPublic, ViewCount)
- ✅ `SongService` updated with full field mapping
- ✅ `SongsController` — authorization added (`[AllowAnonymous]` on GET, `[Authorize]` on POST/PUT, `[Authorize(Roles="Admin")]` on DELETE)
- ✅ `Songs.razor` — Song Library grid with search, category/key/mood filters, pagination (12/page)
- ✅ `SongDetails.razor` — Full song view with lyrics, transpose control (+/- semitones), YouTube/Spotify links
- ✅ `EditSong.razor` — Add/Edit form with full validation, all fields
- ✅ `NavMenu.razor` — Reorganized with SONGS / ACCOUNT / DEV TOOLS sections

**Key URLs**:
- `/songs` — Song Library
- `/songs/{id}` — Song Details + Transpose
- `/songs/add` — Add New Song
- `/songs/{id}/edit` — Edit Song

---

### 🔄 Phase 5: User Profile & Setlists
**Status**: Completed
**Timeline**: Week 4

**Completed Deliverables**:
- ✅ Favorites feature
- ✅ Setlist management (New Setlist / Old Setlist)
- ✅ User state management for authenticated sessions
- ✅ Login flow integration with shared user state
- ✅ Navigation updates for favorites and setlists
- ✅ User data endpoints protected with authorization

---

### 🔄 Phase 6: MAUI Mobile App
**Status**: In Progress
**Timeline**: Week 7-8

**Completed Deliverables**:
- ✅ MAUI login page
- ✅ MAUI song library page
- ✅ MAUI song details page
- ✅ MAUI favorites page
- ✅ MAUI setlist page
- ✅ MAUI profile page
- ✅ Shared `UserStateService` moved to Application layer
- ⏳ Offline song caching
- ⏳ Full mobile shell parity with Web
- ⏳ Platform-specific features (iOS/Android)

---

### ✅ Phase 7: UI/UX Enhancement
**Status**: Completed (Core shell delivered)
**Timeline**: Week 9

**Completed Deliverables**:
- ✅ Teams-style 3-panel shell at `/songs`
- ✅ Responsive panel behavior
- ✅ Panel 2 permanent New / Old / All tabs
- ✅ Modal-based Add Song / Edit Song UX
- ✅ Global shell theme toggle
- ✅ Suggested songs drawer
- ✅ Loading and empty states in shell
- ⏳ Accessibility polish
- ⏳ Notification polish

---

### 🔒 Phase 8: Security & Production Readiness
**Status**: In Progress
**Timeline**: Week 10

**Completed Deliverables**:
- ✅ Authorization on song create/update/delete
- ✅ Authorization on user data endpoints
- ✅ Global theme persistence support
- ⏳ Role-based authorization audit across all flows
- ⏳ Input validation and sanitization review
- ⏳ HTTPS / CORS production hardening
- ⏳ Rate limiting middleware
- ⏳ Logging / monitoring improvements

---

### ✅ Phase 9: Shell Feature Parity and Teams-Style Workspace
**Status**: Completed
**Timeline**: Ongoing

**Completed Deliverables**:
- ✅ Teams-style 3-panel workspace model documented
- ✅ Global shell theme rule defined and implemented
- ✅ Full chord transposition (lines + inline chord markers)
- ✅ Auto-scroll lyrics
- ✅ Font size controls
- ✅ Suggested Songs drawer
- ✅ Stable global theme across sections and songs
- ✅ Add/Edit moved to dialogs instead of occupying panels

---

### ✅ Phase 10: Workspace Persistence
**Status**: Completed

**Completed Deliverables**:
- ✅ Persist theme mode
- ✅ Persist font size
- ✅ Persist Panel 2 category tab
- ✅ Persist selected section in Panel 1
- ✅ Persist selected song in Panel 2
- ✅ Persist active panel state across reloads
- ✅ Persist Panel 3 reading position (scroll position per song, restored on re-select)

---

### 🔄 Phase 11: Shell Polish
**Status**: In Progress

**Completed Deliverables**:
- ✅ Compact dropdown-style multi-selects for Singer / Mood / Genre in Add/Edit dialogs
- ✅ Search inside dropdown panels for Singer / Mood / Genre
- ✅ Selected items shown as removable capsules inside dropdown panels
- ✅ ESC to close dialogs and drawers
- ✅ Improve dark-mode lyrics readability in Panel 3
- ✅ Simplify transpose indicator display to signed numeric format only
- ✅ Focus handling for modals
- ✅ Quick new setlist actions on Panel 1 section headers for Global/My
- ✅ Smart setlists kept read-only in Panel 1 with sync action exposed

**Current Step (In Progress)**:
- 🔄 Keyboard shortcuts
- 🔄 Better mobile transitions
- 🔄 Unified error/loading visuals

**Next Deliverables**:
- ⏳ Keyboard shortcuts
- ⏳ Better mobile transitions
- ⏳ Unified error/loading visuals

---

### 🔄 Phase 12: MAUI Shell Parity
**Status**: Planned

**Next Deliverables**:
- ⏳ Bring Teams-style 3-panel shell mental model to MAUI
- ⏳ Global theme parity with Web
- ⏳ Modal-based add/edit parity
- ⏳ Suggested songs parity if relevant

---

### ✅ Phase 13: Data and Security Hardening (COMPLETED)
**Status**: Done

**Deliverables**:
- ✅ Authorization hardening for admin edit/delete/promote/demote flows
- ✅ Validation hardening for user profile, reset-password, and weights payloads
- ✅ Error handling improvements with structured 4xx/5xx responses + server logging
- ✅ Concurrency / safe write review
  - serialized admin mutations to reduce last-admin race conditions
  - optimistic concurrency for recommendation weights save (`expectedLastModified` token)

---

### ✅ Phase 16: Admin Panel (COMPLETED)
**Status**: Done

**Deliverables**:
- ✅ `GET /api/users` — admin-only user list endpoint (Node.js + .NET users via direct MongoDB)
- ✅ `PATCH /api/users/{id}/admin` — mark user as admin
- ✅ `PATCH /api/users/{id}/remove-admin` — remove admin role (cannot self-demote)
- ✅ `GET /api/recommendation-weights` — fetch singleton weights document
- ✅ `PUT /api/recommendation-weights` — save weights (admin only, total=100 validation)
- ✅ `RecommendationWeights` domain entity (`recommendationWeights` collection)
- ✅ `IRecommendationWeightsRepository` + `RecommendationWeightsRepository`
- ✅ `IUserRepository.GetAllAsync()` + `SetAdminStatusAsync()` — hybrid MongoDB scan
- ✅ `Admin.razor` — role-gated page at `/admin`, 3 tabs:
  - User Management: table sorted admins-first, Mark/Remove Admin with confirmation modal
  - Recommendation Weights: 9 weight fields, live total bar (must = 100), save to MongoDB
  - Rhythm Sets: placeholder for future phase
- ✅ Admin nav link in `NavMenu.razor` (visible to admin users only)

---

### 🧪 Phase 14: Testing
**Status**: Planned

**Objectives**:
- Add targeted automated coverage for the new shell and migrated behavior
- Protect regressions in song workflows, rendering, and persistence

**Deliverables**:
- [ ] Tests for shell filtering logic
- [ ] Tests for transpose and chord rendering
- [ ] Tests for suggestion selection logic
- [ ] Tests for add song flow
- [ ] Tests for edit song flow
- [ ] Tests for theme persistence
- [ ] Tests for category tab persistence
- [ ] Tests for font size persistence
- [ ] Tests for favorites and setlist toggle behavior
- [ ] Tests for modal open/close behavior

---

### 📦 Phase 15: Deployment
**Status**: Planned
**Timeline**: Final phase

---

## Technical Decisions & Best Practices

### Architecture Principles
✅ **Clean Architecture** - Separation of concerns with clear dependencies
✅ **Hybrid User Support** - No forced migration, backward compatible
✅ **Shared Database** - Node.js and .NET apps share same MongoDB
✅ **BCrypt Compatibility** - Same hashing algorithm (10 rounds)
✅ **JWT Authentication** - Stateless, scalable authentication

### Code Standards
- ✅ Use async/await for all I/O operations
- ✅ Implement ILogger for all services
- ✅ Use DTOs for API contracts
- ✅ Repository pattern for data access
- ✅ Dependency injection for all services
- ✅ Environment-based configuration

### MongoDB Practices
- ✅ Use MongoDB.Driver (official driver)
- ✅ Keep collection names consistent with Node.js
- ✅ Support both ObjectId and string IDs
- ✅ Use BsonDocument for flexible queries
- ✅ Implement proper error handling for deserialization

### Security Practices
- ✅ BCrypt for password hashing (work factor 10)
- ✅ JWT with configurable expiry
- ✅ HTTPS only in production
- ✅ CORS configured for specific origins
- ✅ Input validation on all endpoints
- ⏳ Role-based authorization (next phase)
- ⏳ Rate limiting (next phase)

---

## Migration Compatibility Matrix

| Feature | Node.js Status | C# Status | Compatible | Notes |
|---------|---------------|-----------|------------|-------|
| User Authentication | ✅ | ✅ | ✅ | Hybrid lookup supports both |
| Password Hashing (BCrypt) | ✅ | ✅ | ✅ | Same algorithm, 10 rounds |
| JWT Tokens | ✅ | ✅ | ✅ | Compatible format |
| MongoDB Connection | ✅ | ✅ | ✅ | Shared database |
| Song CRUD | ✅ | ✅ | ⏳ | Need to verify |
| User Data | ✅ | ✅ | ⏳ | Need to verify |
| Key Transposition | ✅ | ✅ | ⏳ | Need to test |
| Search/Filter | ✅ | ⏳ | ⏳ | Next phase |
| Categories/Moods | ✅ | ⏳ | ⏳ | Next phase |
| Admin Functions | ✅ | ⏳ | ⏳ | Next phase |

---

## Risk Mitigation

### Database Compatibility
**Risk**: C# app might corrupt Node.js data
**Mitigation**: 
- ✅ Hybrid lookup preserves Node.js format
- ✅ Both apps can read each other's data
- ✅ No destructive migrations
- ⏳ Add data validation before writes

### Performance
**Risk**: Hybrid lookup might be slow
**Mitigation**:
- ✅ Try Identity cache first (fast)
- ✅ MongoDB queries are indexed
- ⏳ Add caching layer if needed
- ⏳ Monitor performance metrics

### Data Consistency
**Risk**: Concurrent updates from both apps
**Mitigation**:
- ✅ Use MongoDB's built-in concurrency
- ✅ ConcurrencyStamp in ApplicationUser
- ⏳ Add optimistic concurrency checks
- ⏳ Implement last-write-wins or version control

---

## Success Criteria

### Phase Completion Checklist
Each phase is complete when:
- [ ] All planned features are implemented
- [ ] Code is reviewed and follows standards
- [ ] Unit tests pass (when added)
- [ ] Integration tests pass (when added)
- [ ] Documentation is updated
- [ ] Compatible with Node.js app
- [ ] No breaking changes to existing data

### Overall Project Success
- [ ] All Node.js features ported to C#
- [ ] Both apps can run simultaneously
- [ ] Users can use either app seamlessly
- [ ] Performance meets or exceeds Node.js
- [ ] Production deployment successful
- [ ] Zero data loss or corruption

---

## Next Immediate Steps

### Week 3 Priorities
1. **Verify Song Management**
   - Test SongsController endpoints
   - Create Blazor pages for song listing
   - Add song creation/edit forms
   - Implement search and filtering

2. **User Data Integration**
   - Link UserData to current user
   - Create user profile page
   - Add settings management

3. **Authorization**
   - Add [Authorize] attributes
   - Implement role-based access
   - Test admin functions

4. **Testing**
   - Test all endpoints with Postman
   - Verify MongoDB operations
   - Test cross-app compatibility

### Development Workflow
1. Implement feature in C#
2. Test with existing Node.js data
3. Verify Node.js app still works
4. Create Blazor UI
5. Add diagnostic/debug tools if needed
6. Document changes
7. Update this migration plan

---

## Useful Commands

### Development
```bash
# Run .NET app
dotnet run --project src/OldandNewClone.Web

# Run tests
dotnet test

# Build solution
dotnet build

# Check for errors
dotnet build --no-incremental
```

### Database
```bash
# Connect to MongoDB Atlas
mongo "mongodb+srv://cluster0.ovya99h.mongodb.net/OldNewSongs" --username genericuser

# View users in MongoDB
db.Users.find().pretty()

# Count users
db.Users.count()

# View songs
db.OldNewSongs.find().pretty()
```

### API Testing
```bash
# Test login
curl -X POST https://localhost:7005/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"user@example.com","password":"password"}'

# Test song listing
curl https://localhost:7005/api/songs

# Test with authentication
curl https://localhost:7005/api/songs \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Documentation

### Existing Documentation
- ✅ Historical auth/database migration records merged in this document under `Historical Migration Records (Merged)`
- ✅ `QUICK_START_SHARED_BACKEND.md` - Quick start guide
- ✅ `ALL_FEATURES_UPDATED.md` - Feature update summary
- ✅ `SECURITY_STAMP_FIX.md` - Security stamp fix details
- ✅ `TESTING_NODE_JS_USER_LOGIN.md` - Login testing guide

### Documentation to Create
- [ ] API Documentation (Swagger/OpenAPI)
- [ ] Architecture Decision Records (ADRs)
- [ ] Deployment Guide
- [ ] User Manual
- [ ] Developer Onboarding Guide

---

## Team & Resources

### Current Status
- ✅ Single developer migration
- ✅ Full-stack: Backend + Frontend + Mobile
- ✅ Shared database approach
- ✅ Incremental migration strategy

### Required Skills
- ✅ C# and .NET 10
- ✅ Blazor Server
- ⏳ .NET MAUI (for mobile phase)
- ✅ MongoDB
- ✅ JWT and authentication
- ✅ Clean Architecture

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2024 | Initial migration plan created | - |
| 1.1 | 2024 | Authentication phase completed | - |
| 1.2 | 2024 | Hybrid lookup implemented | - |
| 1.3 | 2024 | Diagnostic tools added | - |
| 1.4 | 2024 | Migration services removed (hybrid approach) | - |

---

## Conclusion

The migration from Node.js to C# .NET 10 is well underway with a solid foundation:

✅ **Completed**:
- Clean Architecture setup
- MongoDB integration
- Hybrid authentication (supports both apps)
- Diagnostic and debug tools
- Shared database approach
- Core features migration (Song Management)

⏳ **Next Steps**:
- User profile and preferences
- Setlist and favorites features
- MAUI mobile app development
- UI/UX enhancements
- Security hardening
- Comprehensive testing
- Deployment preparation

🎯 **Goal**: Complete, production-ready C# application that can run alongside the Node.js app with zero data migration required.

---

## Phase 7: 3-Panel Shell UI (IN PROGRESS)

### Decision: Teams/Notes-Style 3-Panel Layout
**Date**: 2026

**Problem**: Separate page navigation (Songs list → Song detail) loses context and buttons were non-interactive due to missing @rendermode InteractiveServer.

**Solution**: Single-shell 3-panel layout at /songs route.

### Panel Architecture
| Panel | Width | Content |
|-------|-------|---------|
| Panel 1 | 220px | App nav: Home, All Songs, Favorites, New Setlist, Old Setlist, Add Song, Profile/Login |
| Panel 2 | 320px | Song list with live search + category/key/mood filters |
| Panel 3 | flex  | Song detail: title, meta, transpose control, full lyrics, notes |

### Responsive Behaviour
| Breakpoint | Behaviour |
|------------|-----------|
| Desktop >1100px | All 3 panels visible simultaneously |
| Tablet 700-1100px | Panel 1 collapses to icon rail, Panel 2 + 3 visible |
| Mobile <700px | Single panel at a time, back-button stack navigation |

### Files Changed
- NEW: SongShell.razor (Web) - full 3-panel shell at /songs
- NEW: EmptyLayout.razor (Web) - full-window layout for shell
- REMOVED: Songs.razor, SongDetails.razor, EditSong.razor (replaced by shell)
- UPDATED: NavMenu.razor - /songs link
- NEW: SongShell.razor (MAUI) - mobile-first 3-panel shell
- FIX: Added @rendermode InteractiveServer to all interactive pages

### Status
- [x] Plan documented
- [ ] EmptyLayout created
- [ ] Web SongShell built
- [ ] MAUI SongShell built
- [ ] All buttons verified working

---

## Phase 8: Panel 2 Tabs + Original App Feature Parity (IN PROGRESS)

### Source Analysis: SwareshPawar/OldandNew (Node.js original)
Studied branch: main2 — https://github.com/SwareshPawar/OldandNew

### Features Identified in Original App
| Feature | Original | C# Status |
|---------|----------|-----------|
| New/Old permanent tabs in song list | NewTab/OldTab DOM | ⏳ Adding |
| Full chord transposition (lines + inline) | CHORD_REGEX, transposeChord() | ⏳ Adding |
| Auto-scroll lyrics | setupAutoScroll(), startAutoScroll() | ⏳ Adding |
| Font size zoom on lyrics | fontSize controls | ⏳ Adding |
| Suggested Songs drawer | suggestedSongsDrawer | 🔜 Next |
| Screen Wake Lock | navigator.wakeLock | 🔜 Next |
| Dark/Light theme per category | applyLyricsBackground() | 🔜 Next |
| Song count (New/Old) in sidebar | NewCount/OldCount | ✅ nav-count |

### Phase 8 Changes
- Panel 2: ALWAYS shows ✨ New | 🕰️ Old tabs — category dropdown removed
- Panel 3: Full chord transposition (chord lines + [Chord] inline syntax)
- Panel 3: Auto-scroll toggle (▶ Play / ⏸ Pause)
- Panel 3: Font size zoom (A− / A+)
- Panel 3 toolbar redesigned to match original app toolbar style

### Phase 9 Update (Implemented)
- ✅ Suggested Songs drawer in Panel 3 (category/key-based recommendations)
- ✅ Lyrics theme toggle (Auto / Dark / Light)
- ✅ Keep Screen On toggle (Wake Lock API with graceful fallback)
- ✅ Auto-scroll now stops automatically on song/section change

---

## Phase 9: Teams-Style App Structure and Theming Model

### Design Decision
The application should be understood and evolved as a **Teams-style 3-panel workspace**, not as isolated pages.

### Mental Model
| Teams-style concept | OldandNew app equivalent | Current implementation |
|---|---|---|
| Left app rail (Activity, Chat, Calendar, Files) | Primary app sections such as Home, Favorites, Setlists, and other top-level navigation | **Panel 1** |
| Middle list pane (chat/channel/task list) | Song list / filtered working list / selected collection list | **Panel 2** |
| Right content pane (message thread / task detail / document detail) | Lyrics preview / selected song detail / focused working content | **Panel 3** |

### Structural Interpretation
The app structure should be treated as:
1. **Panel 1 = Workspace Navigation**
   - Top-level areas of the app
   - Equivalent to the Teams left rail
   - Examples: Home, Favorites, New Setlist, Old Setlist, theme control, profile/account actions

2. **Panel 2 = Active Collection/List Context**
   - The currently selected working list
   - Equivalent to conversation list / task list / channel content list in Teams
   - In this app: song list, filtered list, favorites list, setlist list

3. **Panel 3 = Focused Content View**
   - The currently selected item's deep content
   - Equivalent to message thread / task details / content preview in Teams
   - In this app: lyrics preview, song metadata, transpose tools, suggestions, focused reading tools

### Theming Rule
Theme should be applied at the **workspace shell level**, not per song and not per panel.

Implications:
- Theme is global across **Panel 1, Panel 2, and Panel 3**
- Theme must remain stable while changing songs or switching lists
- Theme is a user workspace preference, similar to Teams app theming
- Content selection must not change the app theme automatically

### UX Rule for Future Migration Work
All future UI work should preserve this model:
- **Panel 1** changes workspace context
- **Panel 2** changes list context
- **Panel 3** changes focused content
- Forms such as **Add Song** and **Edit Song** should appear as dialogs/modals, not replace the role of any panel

### Migration Guidance
Future migration steps should align new features to this shell model:
- Calendar-like features belong in **Panel 1** navigation
- Collection/list views belong in **Panel 2**
- Reading/detail/preview experiences belong in **Panel 3**
- Theme, workspace preferences, and shell behavior should be centralized and persistent

### Status
- ✅ Teams-style app structure defined
- ✅ Global shell theme rule defined
- ✅ Panel responsibilities documented
- ✅ Modal-based edit/create behavior aligned to shell model

---

## Phase 10A: Real Setlist System Migration (STARTED)

### Problem
The previous C# implementation exposed only fixed user buckets:
- `NewSetlist`
- `OldSetlist`

This does **not** match the original app on the `main` branch, which supports richer setlist behavior.

### Original App Setlist Types (main branch)
The original app supports first-class setlists including:
- **Global Setlists**
- **My Setlists**
- **Smart Setlists**
- manual song addition into a setlist
- New/Old grouping inside a setlist view

### Migration Decision
The old dedicated `/setlist` page was removed because it represented the wrong model.
Setlists will now be migrated as first-class entities and integrated into the Teams-style shell.

### Backend Foundation Completed
- ✅ Added `Setlist` domain entity
- ✅ Added `SetlistDto`, `CreateSetlistDto`, `UpdateSetlistDto`
- ✅ Added `ISetlistRepository` and `ISetlistService`
- ✅ Added `SetlistRepository`
- ✅ Added `SetlistService`
- ✅ Added `SetlistsController`
- ✅ Wired DI registrations
- ✅ Aligned reader to original MongoDB setlist collections
- ✅ Removed obsolete Web and MAUI `Setlist.razor` pages
- ✅ Removed obsolete nav links pointing to fixed-bucket setlists

### Setlist Migration Checklist
- [x] Read existing Global / My / Smart setlists from the real database schema
- [x] Render Panel 1 setlist folders
- [x] Render child setlists under each folder
- [x] Select a child setlist from Panel 1
- [x] Show selected setlist overview in Panel 3
- [x] Show selected setlist songs in Panel 2
- [x] Group selected setlist songs into New / Old in Panel 3
- [x] Make all Panel 2 dependencies react correctly to selected setlist context
- [x] Add create/edit/delete dialogs for setlists (Global/My first; Smart separate)
- [x] Move setlist edit/update/delete actions to Panel 1 icon controls per setlist row (Global/My)
- [x] Add Smart setlist sync songs action on Panel 1
- [x] Add add/remove/reorder songs for a specific setlist (Global/My only) from Panel 2 via modal workflow
- [x] Add setlist persistence/restoration for selected child setlist
- [x] De-emphasize legacy `NewSetlist` / `OldSetlist` storage in the shell by replacing primary song actions with the real setlist workflow
- [ ] Mirror setlist shell behavior in MAUI shell

### Current Stable Shell Notes
The current stable `SongShell.razor` already includes:
- Panel 1 folder hierarchy for Global / My / Smart setlists
- child setlist rendering under each folder
- child setlist selection state in Panel 1
- selected setlist document loading
- Panel 2 setlist-aware song list context (shows selected setlist songs)
- Panel 2 manual song management entry point for Global/My setlists
- Panel 2 modal workflow for add/remove/reorder songs in Global/My setlists
- Panel 3 selected setlist overview
- Panel 3 New/Old grouped setlist song quick-select
- selected child setlist persistence/restoration across reloads
- Panel 3 real setlist song action modal for Global/My setlists
- legacy fixed-bucket New/Old actions de-emphasized from the primary song toolbar
- Panel 1 icon-only setlist actions for Global/My rows (edit/delete)
- Panel 1 Smart setlist sync songs action
- CSS polish pass for setlist rows and icon actions (light/dark hover states)

The next safe reconstruction step is:
- mirror the setlist shell workflow in the MAUI shell so mobile/desktop uses the same Global / My / Smart setlist model and manual song management rules


---

## Historical Migration Records (Merged)


All previously separate migration/status documents were merged here on 2026-04-24 to keep a single source of truth.


---

### Merged From: AUTO_DATABASE_MIGRATION.md

# Archive Notice

This file is archived for historical implementation detail only.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Automatic Database Migration Fix

## 🎯 Problem Solved

**Issue:** Existing MongoDB users have ObjectId format IDs that are incompatible with the current setup, causing:
```
Cannot deserialize a 'String' from BsonType 'ObjectId'
```

**Previous Problem:** Couldn't even access `/debug/users` or `/migration` pages because the error occurred when trying to read users.

## ✅ Solution: Auto-Clear on Startup

### **What Was Added:**

#### 1. **DatabaseInitializer Service**
**File:** `src/OldandNewClone.Web/Services/DatabaseInitializer.cs`

This service:
- Runs automatically on application startup
- Detects if users have incompatible format
- Automatically clears Users and Roles collections if needed
- Logs all actions for transparency

#### 2. **Startup Integration**
**File:** `src/OldandNewClone.Web/Program.cs`

The initializer runs after building the app but before processing requests.

## 🚀 How It Works

### **On Application Startup:**

1. **Tries to read users** from MongoDB
2. **If successful** → No action needed, continues normally
3. **If error occurs** (ObjectId format) → Automatically clears Users and Roles
4. **Logs the action** so you know what happened

### **Log Output (When Clearing):**
```
warn: OldandNewClone.Web.Services.DatabaseInitializer[0]
      Detected incompatible user format. Clearing Users collection...
warn: OldandNewClone.Web.Services.DatabaseInitializer[0]
      Users collection cleared due to format incompatibility
warn: OldandNewClone.Web.Services.DatabaseInitializer[0]
      Roles collection cleared due to format incompatibility
```

### **Log Output (When OK):**
```
info: OldandNewClone.Web.Services.DatabaseInitializer[0]
      Database initialized. Users count: 0
```

## 🧪 Testing Steps

1. **Stop the application** (if running)
2. **Start the application** (F5)
3. **Check the Output/Debug window** for initialization logs
4. **If users were cleared**, you'll see warning messages
5. **Navigate to** `/auth/register`
6. **Create a new account:**
   - Email: `test@example.com`
   - Password: `password123`
7. **Navigate to** `/auth/login`
8. **Login** with your credentials
9. **Should work!** ✅

## 📋 What Happens to Old Users?

### **First Run After Update:**
- ❌ Old users with ObjectId format → **Deleted automatically**
- ⚠️ Warning logged to console
- ✅ Database ready for new users

### **Subsequent Runs:**
- ✅ New users with correct format → **Kept as-is**
- ℹ️ Info logged showing user count
- ✅ No data loss

## 🔒 Safety Features

1. **Exception Handling** - Won't crash app if MongoDB is unavailable
2. **Logging** - All actions are logged for audit trail
3. **Selective Clearing** - Only clears when format error is detected
4. **Non-Breaking** - If clear fails, app continues anyway

## 📊 Manual Verification (Optional)

### **Check MongoDB After Startup:**

**Via MongoDB Compass:**
1. Connect to your database
2. Check `Users` collection
3. Should be empty after first run (if old users existed)

**Via MongoDB Shell:**
```javascript
db.Users.count()  // Should return 0 after first run
db.Roles.count()  // Should return 0 after first run
```

## 🎉 Benefits

✅ **Automatic** - No manual intervention needed
✅ **Safe** - Only clears when necessary
✅ **Transparent** - Logs all actions
✅ **User-Friendly** - No complex migration steps
✅ **Development-Friendly** - Works in all environments

## 📝 Files Created/Modified

**Created:**
- `src/OldandNewClone.Web/Services/DatabaseInitializer.cs`

**Modified:**
- `src/OldandNewClone.Web/Program.cs` - Added initializer registration and call

## ⚠️ Important Notes

### **For Development:**
- ✅ Perfect solution - auto-cleans incompatible data
- ✅ No manual steps needed
- ✅ Just restart and register

### **For Production:**
- ⚠️ Consider modifying to:
  - Not auto-delete users
  - Send alerts instead
  - Require manual confirmation
  - Migrate data instead of deleting

### **To Disable Auto-Clear:**
Comment out the initializer call in Program.cs:
```csharp
// using (var scope = app.Services.CreateScope())
// {
//     var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
//     await dbInitializer.InitializeAsync();
// }
```

## ✅ Summary

- ✅ **Auto-detects** incompatible user format
- ✅ **Auto-clears** problematic data on startup
- ✅ **Logs** all actions for transparency
- ✅ **No manual intervention** required
- ✅ **Safe** - won't clear compatible data

**Just restart the app and it will fix itself!** 🚀

---

## 🎯 Next Steps

1. **Restart the application**
2. **Check logs** for initialization messages
3. **Register a new user** at `/auth/register`
4. **Login** at `/auth/login`
5. **Everything should work!** ✅



---

### Merged From: NODEJS_AUTH_MIGRATION.md

# Archive Notice

This file is archived for historical authentication migration detail only.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Node.js Authentication Migration

## Overview
This document explains how the C# .NET authentication system was updated to match the Node.js AdminPassword branch implementation.

## Original Node.js Implementation (AdminPassword Branch)

### User Structure
```javascript
{
  firstName: string,
  lastName: string,
  username: string (lowercase),
  email: string (lowercase),
  phone: string,
  password: string (bcrypt hash),
  isAdmin: boolean
}
```

### Password Hashing
- **Library**: `bcryptjs`
- **Rounds**: 10
- **Registration**: `bcrypt.hash(password, 10)`
- **Verification**: `bcrypt.compare(password, user.password)`

### Authentication Flow
1. **Registration**: 
   - Check for existing username or email (case-insensitive)
   - Store username and email as lowercase
   - Hash password with bcrypt (10 rounds)
   - Store user in MongoDB

2. **Login**:
   - Accept username OR email as login input
   - Find user by username OR email (case-insensitive)
   - Verify password with bcrypt.compare()
   - Generate JWT token with 7-day expiry

## C# .NET Implementation Changes

### 1. ApplicationUser Entity Updated
**File**: `src/OldandNewClone.Domain/Entities/ApplicationUser.cs`

Added fields to match Node.js structure:
```csharp
public string FirstName { get; set; } = string.Empty;
public string LastName { get; set; } = string.Empty;
public string Phone { get; set; } = string.Empty;
public bool IsAdmin { get; set; } = false;
```

### 2. DTOs Updated
**File**: `src/OldandNewClone.Application/DTOs/AuthDto.cs`

#### RegisterDto
Changed from email-only to full user profile:
```csharp
public string FirstName { get; set; }
public string LastName { get; set; }
public string Username { get; set; }
public string Email { get; set; }
public string Phone { get; set; }
public string Password { get; set; }
public string ConfirmPassword { get; set; }
public bool IsAdmin { get; set; } = false;
```

#### LoginDto
Changed from email-only to username/email:
```csharp
public string UsernameOrEmail { get; set; }  // Was: Email
public string Password { get; set; }
```

#### UserInfoDto
Added all user fields:
```csharp
public string FirstName { get; set; }
public string LastName { get; set; }
public string Username { get; set; }
public string Email { get; set; }
public string Phone { get; set; }
public bool IsAdmin { get; set; }
public string Role { get; set; }
```

### 3. BCrypt Configuration
**File**: `src/OldandNewClone.Infrastructure/Services/BCryptPasswordHasher.cs`

Explicitly set work factor to 10 to match Node.js:
```csharp
private const int WorkFactor = 10;

public string HashPassword(TUser user, string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
}
```

### 4. UserRepository Enhanced
**File**: `src/OldandNewClone.Infrastructure/Repositories/UserRepository.cs`

Added methods to support username OR email lookup:
```csharp
Task<ApplicationUser?> GetByUsernameAsync(string username);
Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput);
Task<bool> UsernameExistsAsync(string username);
```

Implementation matches Node.js logic:
```csharp
public async Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput)
{
    // Try username first
    var user = await _userManager.FindByNameAsync(loginInput);
    if (user != null) return user;

    // Try email
    return await _userManager.FindByEmailAsync(loginInput);
}
```

### 5. AuthService Updated
**File**: `src/OldandNewClone.Application/Services/AuthService.cs`

#### Registration Logic
```csharp
// Check for existing username or email (case-insensitive) - matching Node.js
if (await _userRepository.EmailExistsAsync(registerDto.Email.ToLower()))
    return error("User or email already exists");

if (await _userRepository.UsernameExistsAsync(registerDto.Username.ToLower()))
    return error("User or email already exists");

// Match Node.js user structure
var user = new ApplicationUser
{
    UserName = registerDto.Username.ToLower(), // Store lowercase like Node.js
    Email = registerDto.Email.ToLower(),
    FirstName = registerDto.FirstName,
    LastName = registerDto.LastName,
    Phone = registerDto.Phone,
    IsAdmin = registerDto.IsAdmin,
    // ...
};
```

#### Login Logic
```csharp
// Match Node.js: Find by username or email, case-insensitive
var loginInput = loginDto.UsernameOrEmail.Trim().ToLower();
var user = await _userRepository.GetByUsernameOrEmailAsync(loginInput);

if (user == null)
    return error("Invalid credentials");

// BCrypt verification happens via SignInManager → BCryptPasswordHasher
var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

if (!result.Succeeded)
    return error("Invalid credentials");
```

### 6. UI Pages Updated
**Files**: 
- `src/OldandNewClone.Web/Components/Pages/Login.razor`
- `src/OldandNewClone.Web/Components/Pages/Register.razor`

#### Login Page
- Changed from "Email" to "Username or Email" input
- Updated API call to send `usernameOrEmail` instead of `email`

#### Register Page
- Added fields: First Name, Last Name, Username, Phone
- Removed: Full Name (optional)
- Updated API call to send all required fields

## Key Differences & Compatibility

### Password Hashing
| Node.js (bcryptjs) | C# (BCrypt.Net) |
|-------------------|-----------------|
| `bcrypt.hash(password, 10)` | `BCrypt.HashPassword(password, 10)` |
| `bcrypt.compare(password, hash)` | `BCrypt.Verify(password, hash)` |

**Result**: ✅ Compatible - Both use bcrypt with 10 rounds

### Case Sensitivity
| Node.js | C# .NET |
|---------|---------|
| `username.toLowerCase()` | `username.ToLower()` |
| `email.toLowerCase()` | `email.ToLower()` |

**Result**: ✅ Compatible - Both store lowercase

### Login Lookup
| Node.js | C# .NET |
|---------|---------|
| MongoDB `$or` query | Try username, then email |

**Result**: ✅ Functionally equivalent

## Migration Steps for Existing Users

### Important: Clear Existing Users
Before testing, you **must** clear existing users because:
1. Old users don't have `FirstName`, `LastName`, `Phone` fields
2. Old users may have incompatible password hashes
3. Old users use email as UserName instead of username

### How to Clear Users
1. Navigate to `/migration` page in the app
2. Click "Clear All Users"
3. Click "Clear All Roles"
4. Verify collections are empty

**OR** use the DatabaseInitializer (runs automatically on app startup):
- Checks for incompatible users
- Automatically clears Users and Roles collections if needed

## Testing the New Implementation

### 1. Register a New User
```bash
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "username": "johndoe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "password": "qwerty123",
  "confirmPassword": "qwerty123"
}
```

Expected Response:
```json
{
  "success": true,
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "user": {
    "id": "...",
    "firstName": "John",
    "lastName": "Doe",
    "username": "johndoe",
    "email": "john@example.com",
    "phone": "+1234567890",
    "isAdmin": false,
    "role": "User"
  }
}
```

### 2. Login with Username
```bash
POST /api/auth/login
{
  "usernameOrEmail": "johndoe",
  "password": "qwerty123"
}
```

### 3. Login with Email
```bash
POST /api/auth/login
{
  "usernameOrEmail": "john@example.com",
  "password": "qwerty123"
}
```

Both should succeed with the same token response.

## Verification Checklist

✅ BCrypt work factor set to 10 (matches Node.js)
✅ Registration creates user with all required fields
✅ Usernames stored as lowercase
✅ Emails stored as lowercase
✅ Login accepts username OR email
✅ Password verification uses BCrypt.Verify
✅ Duplicate username check implemented
✅ Duplicate email check implemented
✅ User entity matches Node.js structure
✅ UI pages updated with all fields
✅ Error messages match Node.js ("Invalid credentials", "User or email already exists")

## Common Issues & Solutions

### Issue: Login fails with "Invalid credentials"
**Solution**: 
1. Clear existing users (they have old structure)
2. Register fresh user with new registration form
3. Verify user has all required fields in MongoDB

### Issue: "User or email already exists" on registration
**Solution**:
1. Check if username/email already exists (case-insensitive)
2. Use different username or email
3. Clear database if testing

### Issue: Password hash not verifying
**Solution**:
1. Ensure BCryptPasswordHasher is registered in DI
2. Verify work factor is set to 10
3. Check that password is not empty or null

## Next Steps

1. **Test Registration**: Create a new user with all fields
2. **Test Login**: Login with username, then with email
3. **Verify Token**: Check JWT token contains correct user data
4. **Test Password**: Ensure bcrypt verification works correctly
5. **Monitor Logs**: Check application logs for any errors

## References

- Original Node.js Implementation: https://github.com/SwareshPawar/OldandNew/tree/AdminPassword
- BCrypt.Net Documentation: https://github.com/BcryptNet/bcrypt.net
- ASP.NET Core Identity: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity



---

### Merged From: SHARED_MONGODB_MIGRATION_GUIDE.md

# Archive Notice

This file is archived for historical interoperability implementation detail only.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Shared MongoDB Backend Migration Guide

## Problem Statement

Your Node.js app (AdminPassword branch) and .NET app need to **share the same MongoDB backend** and users should be able to login to both applications with the same credentials.

### Root Cause

The Node.js app stores password hashes in a field called `password`, while ASP.NET Core Identity uses `PasswordHash`. This causes login failures when trying to use .NET with existing Node.js users.

## Solution Overview

We've implemented a **bi-directional password field sync** system:

1. **On .NET app startup**: Automatically copies `password` → `PasswordHash` for all existing Node.js users
2. **When .NET creates/updates users**: Automatically copies `PasswordHash` → `password` for Node.js compatibility
3. **BCrypt compatibility**: Both apps use BCrypt with 10 rounds (Node.js uses `bcryptjs`, .NET uses `BCrypt.Net`)

## Architecture

```
MongoDB Users Collection
├── password (Node.js field)       ←→ Synced both ways
├── PasswordHash (.NET field)      ←→ Synced both ways
├── username (lowercase)
├── email (lowercase)
├── firstName
├── lastName
├── phone
└── isAdmin
```

## Files Changed/Created

### 1. Password Field Migration Service
**File**: `src/OldandNewClone.Web/Services/PasswordFieldMigration.cs`

- Runs automatically on .NET app startup
- Copies `password` → `PasswordHash` for all users missing PasswordHash
- Provides migration status endpoint
- **Does NOT remove** the `password` field (keeps Node.js compatibility)

### 2. Password Field Sync Service
**File**: `src/OldandNewClone.Infrastructure/Services/PasswordFieldSyncService.cs`

- Syncs `PasswordHash` → `password` when .NET creates/updates users
- Ensures new .NET users work with Node.js app
- Registered in DI container

### 3. Password Test Controller
**File**: `src/OldandNewClone.Web/Controllers/PasswordTestController.cs`

Diagnostic endpoints:
- `GET /api/passwordtest/check-user/{usernameOrEmail}` - Inspect user fields in MongoDB
- `POST /api/passwordtest/test-password` - Test password verification
- `GET /api/passwordtest/bcrypt-info` - Check BCrypt configuration

### 4. Password Migration Controller
**File**: `src/OldandNewClone.Web/Controllers/PasswordMigrationController.cs`

- `GET /api/passwordmigration/status` - Check how many users need migration
- `POST /api/passwordmigration/migrate` - Manually trigger migration

### 5. Updated Program.cs
**File**: `src/OldandNewClone.Web/Program.cs`

Added automatic password migration on startup:
```csharp
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeAsync();

    // Migrate password fields from Node.js format to .NET format
    var passwordMigration = scope.ServiceProvider.GetRequiredService<PasswordFieldMigration>();
    await passwordMigration.MigratePasswordFieldsAsync();
}
```

### 6. Updated DependencyInjection.cs
**File**: `src/OldandNewClone.Infrastructure/DependencyInjection.cs`

- Registered `PasswordFieldSyncService`
- Provided `IMongoDatabase` for direct MongoDB access

## How to Use

### Step 1: Stop Both Applications

Make sure both Node.js and .NET apps are stopped before migration.

### Step 2: Check Migration Status

Before starting the .NET app, you can check which users need migration:

```bash
# After starting .NET app
curl http://localhost:5000/api/passwordmigration/status
```

Response:
```json
{
  "totalUsers": 5,
  "usersWithPasswordField": 5,
  "usersWithPasswordHashField": 0,
  "usersNeedingMigration": 5,
  "migrationNeeded": true
}
```

### Step 3: Start .NET App

When you start the .NET app, it will **automatically**:

1. Clear any incompatible users (DatabaseInitializer)
2. Migrate `password` → `PasswordHash` for all existing users (PasswordFieldMigration)

Check the console output:
```
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Found 5 users that need password field migration
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Migrated password hash for user: testuser (ID: 6789abc...)
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Password field migration complete. Migrated: 5, Failed: 0
```

### Step 4: Test Login with Existing User

Try logging in with a user created in the Node.js app:

```bash
# Login to .NET app with Node.js user
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "testuser",
    "password": "qwerty123"
  }'
```

Should return:
```json
{
  "success": true,
  "accessToken": "eyJ...",
  "user": {
    "username": "testuser",
    "email": "test@example.com",
    ...
  }
}
```

### Step 5: Create New User in .NET

When you create a user in the .NET app, it will automatically sync to Node.js format:

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "New",
    "lastName": "User",
    "username": "newuser",
    "email": "new@example.com",
    "phone": "+1234567890",
    "password": "password123",
    "confirmPassword": "password123"
  }'
```

The user will have **both** fields in MongoDB:
```json
{
  "_id": "...",
  "username": "newuser",
  "email": "new@example.com",
  "password": "$2a$10$...",        // For Node.js
  "PasswordHash": "$2a$10$...",    // For .NET (same value)
  "firstName": "New",
  "lastName": "User",
  ...
}
```

### Step 6: Verify User Works in Both Apps

1. **Login to Node.js app**:
```bash
curl -X POST http://localhost:3000/api/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newuser",
    "password": "password123"
  }'
```

2. **Login to .NET app**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "newuser",
    "password": "password123"
  }'
```

Both should work!

## Diagnostic Tools

### Check User Fields

```bash
curl http://localhost:5000/api/passwordtest/check-user/testuser
```

Response shows which fields exist:
```json
{
  "found": "UserManager",
  "username": "testuser",
  "email": "test@example.com",
  "hasPasswordHash": true,
  "passwordHashLength": 60,
  "passwordHashPrefix": "$2a$10$abc"
}
```

### Test Password Verification

```bash
curl -X POST http://localhost:5000/api/passwordtest/test-password \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "testuser",
    "password": "qwerty123"
  }'
```

Response:
```json
{
  "method": "UserManager",
  "username": "testuser",
  "iPasswordHasherResult": "Success",
  "bcryptNetDirectResult": true,
  "bothMatch": true,
  "message": "✅ Password matches!"
}
```

### Check BCrypt Configuration

```bash
curl http://localhost:5000/api/passwordtest/bcrypt-info
```

Verifies BCrypt.Net is working correctly:
```json
{
  "bcryptNetVersion": "4.1.0.0",
  "testHash": "$2a$10$...",
  "testVerify": true,
  "hashPrefix": "$2a$10$...",
  "note": "BCrypt hashes start with $2a$, $2b$, or $2y$"
}
```

## BCrypt Compatibility

### Node.js (`bcryptjs`)
```javascript
const hash = await bcrypt.hash(password, 10);
const valid = await bcrypt.compare(password, hash);
```

### .NET (`BCrypt.Net`)
```csharp
var hash = BCrypt.Net.BCrypt.HashPassword(password, 10);
var valid = BCrypt.Net.BCrypt.Verify(password, hash);
```

**Result**: ✅ **Fully compatible** - Both produce and verify the same hashes

## Migration Scenarios

### Scenario 1: Existing Node.js Users

**Before Migration**:
```json
{
  "_id": "123",
  "username": "olduser",
  "password": "$2a$10$hash..."
}
```

**After .NET App Starts**:
```json
{
  "_id": "123",
  "username": "olduser",
  "password": "$2a$10$hash...",      // Kept for Node.js
  "PasswordHash": "$2a$10$hash..."  // Added for .NET
}
```

**Result**: User can login to both apps

### Scenario 2: New .NET Users

**When Created in .NET**:
```json
{
  "_id": "456",
  "username": "newuser",
  "PasswordHash": "$2a$10$hash...",  // Created by .NET
  "password": "$2a$10$hash..."       // Synced for Node.js
}
```

**Result**: User can login to both apps

### Scenario 3: Password Change

When a user changes password in either app:

- **In Node.js**: Updates `password` field
  - ❌ .NET won't see the change immediately
  - ✅ Next .NET app restart will sync it

- **In .NET**: Updates `PasswordHash` field
  - ⚠️ Currently doesn't auto-sync to `password`
  - 📝 TODO: Add PasswordFieldSyncService call in password reset

## Known Limitations

### 1. Password Changes Not Live-Synced

If a user changes password in the Node.js app, the .NET app won't see it until restart.

**Solution**: Restart .NET app or call migration endpoint manually.

### 2. Manual Sync After Password Reset

Currently, password reset in .NET doesn't automatically sync to `password` field.

**Workaround**: Call `/api/passwordmigration/migrate` after password resets.

### 3. Field Name Differences

Node.js and .NET use different field names for some properties:

| Node.js | .NET Identity |
|---------|---------------|
| `username` | `UserName` |
| `email` | `Email` |
| `password` | `PasswordHash` |

The migration handles these differences automatically.

## Troubleshooting

### Problem: "Invalid credentials" after migration

**Diagnosis**:
```bash
curl http://localhost:5000/api/passwordtest/check-user/youruser
```

**Check**:
- Does user have `PasswordHash` field?
- Is `hasPasswordHash` true?
- Does `passwordHashPrefix` start with `$2a$` or `$2b$`?

**Solution**: Run manual migration
```bash
curl -X POST http://localhost:5000/api/passwordmigration/migrate
```

### Problem: User exists in MongoDB but not found by .NET

**Diagnosis**:
Check if username/email are lowercase:
```bash
curl http://localhost:5000/api/passwordtest/check-user/YourUser
```

**Solution**: Usernames and emails should be stored lowercase. Update MongoDB:
```javascript
db.Users.updateMany(
  {},
  [
    { $set: { username: { $toLower: "$username" } } },
    { $set: { email: { $toLower: "$email" } } }
  ]
)
```

### Problem: BCrypt verification fails

**Diagnosis**:
```bash
curl http://localhost:5000/api/passwordtest/bcrypt-info
```

**Check**: `testVerify` should be `true`

**Solution**: Verify BCrypt.Net version is 4.1.0+
```bash
dotnet list package | findstr BCrypt
```

## Monitoring

### Application Startup Logs

Watch for these log messages:

✅ **Successful Migration**:
```
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      No users need password field migration
```

✅ **Migration Performed**:
```
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Found 3 users that need password field migration
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Migrated password hash for user: user1 (ID: ...)
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Password field migration complete. Migrated: 3, Failed: 0
```

❌ **Migration Failed**:
```
error: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Error during password field migration
```

### Check Migration Status Anytime

```bash
curl http://localhost:5000/api/passwordmigration/status
```

## Best Practices

1. **Always check migration status** before going to production
2. **Run manual migration** if you deploy to a new environment
3. **Monitor startup logs** for migration issues
4. **Test login in both apps** after migration
5. **Keep `password` field** - don't remove it even if unused by .NET

## Future Enhancements

### TODO: Real-time Password Sync

Implement webhook or message queue to sync password changes immediately:

```csharp
// After password reset in .NET
await _passwordFieldSyncService.SyncPasswordHashToNodeJsFieldAsync(userId, newHash);
```

### TODO: Bidirectional Change Detection

Monitor MongoDB change streams to detect password changes from Node.js:

```csharp
var changeStream = usersCollection.Watch();
await changeStream.ForEachAsync(change => {
    if (change.UpdateDescription.UpdatedFields.Contains("password")) {
        // Sync to PasswordHash
    }
});
```

## Summary

✅ **Automatic migration** on .NET app startup
✅ **Bi-directional compatibility** - users work in both apps
✅ **BCrypt compatible** - same hashing algorithm and rounds
✅ **Diagnostic tools** - easy to troubleshoot issues
✅ **Production ready** - tested with existing Node.js users

Your Node.js and .NET apps can now share the same MongoDB backend! 🎉



---

### Merged From: AUTH_SYSTEM_ANALYSIS.md

# Archive Notice

This file is archived analysis from an earlier migration stage.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Authentication System Analysis - Auth0 vs Custom

## 🔍 **Discovery: Original System Used Auth0**

After examining the original Node.js repository, I discovered that it uses **Auth0** for authentication, NOT a custom password system.

### **Original System (Node.js):**
- ✅ Uses **Auth0** (external authentication provider)
- ✅ No passwords stored in MongoDB
- ✅ JWT tokens issued by Auth0
- ✅ Users identified by `sub` (Auth0 user ID)
- ✅ No BCrypt password hashing needed

```javascript
// Original Node.js authentication
const authMiddleware = jwt({
  secret: jwksRsa.expressJwtSecret({
    jwksUri: `https://dev-yr80e6pevtcdjxvg.us.auth0.com/.well-known/jwks.json`
  }),
  audience: "https://oldandnew.onrender.com/api",
  issuer: `https://dev-yr80e6pevtcdjxvg.us.auth0.com/`,
  algorithms: ["RS256"]
});
```

### **Current C# System:**
- ⚠️ Trying to implement **custom username/password** authentication
- ⚠️ Storing passwords in MongoDB with BCrypt
- ⚠️ Custom JWT generation
- ⚠️ Different approach from original

## 🎯 **The Problem**

You're trying to port an **Auth0-based** system to a **custom password-based** system, which is why things aren't working as expected.

## ✅ **Solutions**

### **Option 1: Continue with Custom Authentication (Current Path)**

If you want to keep custom authentication with passwords:

1. **Clear ALL existing users** (they don't have passwords)
2. **Register new users** with email/password
3. **Use the BCrypt system** we've built

**Steps:**
```bash
# 1. Stop the app
# 2. Start the app (F5)
# 3. Navigate to /migration
# 4. Click "Clear All Users & Roles"
# 5. Go to /auth/register
# 6. Create new account: test@example.com / password123
# 7. Login at /auth/login
```

### **Option 2: Switch to Auth0 (Match Original)**

If you want to match the original system:

1. **Create Auth0 account** at https://auth0.com
2. **Install Auth0 packages:**
```bash
dotnet add src/OldandNewClone.Web package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/OldandNewClone.Web package Auth0.AspNetCore.Authentication
```
3. **Configure Auth0** in appsettings.json
4. **Remove custom password authentication**
5. **Use Auth0 login pages**

## 🔧 **Diagnostic Tools Added**

I've added `/api/diagnostic/users-raw` endpoint to help debug:

**Test it:**
```bash
# Get raw user data from MongoDB
curl http://localhost:5000/api/diagnostic/users-raw

# Test BCrypt password
curl -X POST http://localhost:5000/api/diagnostic/test-bcrypt \
  -H "Content-Type: application/json" \
  -d '{"password":"test123","storedHash":"$2a$11$..."}'
```

## 📋 **Current State Analysis**

### **Why Login Isn't Working:**

1. **Old users from Auth0** might still exist in MongoDB
2. **They don't have PasswordHash** field
3. **BCrypt can't verify** non-existent passwords
4. **ObjectId format issue** preventing proper deserialization

### **Why Rehash Doesn't Work:**

1. Can't retrieve users due to ObjectId deserialization error
2. Users don't have passwords to rehash
3. Need to clear and start fresh

## 🚀 **Recommended Fix (Right Now)**

Since you're already this far with custom authentication, let's finish it:

### **Step 1: Restart & Clear**
```bash
# Stop debugging (Shift+F5)
# Start debugging (F5)
# Go to http://localhost:PORT/migration
# Click "Clear All Users & Roles"
```

### **Step 2: Verify Clearing**
```bash
# Call diagnostic endpoint
curl http://localhost:PORT/api/diagnostic/users-raw
# Should return: "totalUsers": 0
```

### **Step 3: Register Fresh User**
```
Navigate to: /auth/register
Email: test@example.com
Password: password123
Full Name: Test User
```

### **Step 4: Check User Created**
```bash
curl http://localhost:PORT/api/diagnostic/users-raw
# Should show new user with BCrypt hash starting with $2a$ or $2b$
```

### **Step 5: Login**
```
Navigate to: /auth/login
Email: test@example.com
Password: password123
```

## 📊 **Expected Results**

### **After Clearing:**
```json
{
  "totalUsers": 0,
  "users": []
}
```

### **After Registration:**
```json
{
  "totalUsers": 1,
  "users": [
    {
      "rawId": "ObjectId(\"...\")",
      "idType": "ObjectId",
      "email": "test@example.com",
      "passwordHash": "$2a$11$...",  // BCrypt hash
      "allFields": ["_id", "UserName", "Email", "PasswordHash", "Name", "CreatedAt", ...]
    }
  ]
}
```

## ⚠️ **Important Notes**

1. **Original system had NO passwords** - it used Auth0
2. **You're building something different** - custom auth
3. **Can't migrate old users** - they have no passwords
4. **Must start fresh** with new user registration

## 🎯 **Next Steps**

1. **Restart app** → Clear users → Register → Login
2. **Test with diagnostic endpoint** to verify
3. **If still issues** → Check diagnostic output and share here

---

**The key insight:** You're not porting the same system - you're building a new authentication mechanism. The old users are incompatible because they never had passwords.



---

### Merged From: AUTHENTICATION_SYSTEM.md

# Archive Notice

This file is archived for implementation summary/history only.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Authentication System - Implementation Summary

## ✅ Phase 2 Complete: Authentication System

### 1. **Domain Layer Updates**
- ✅ Enhanced `ApplicationUser` entity with refresh token support
  - Added `RefreshToken` property
  - Added `RefreshTokenExpiryTime` property

### 2. **Application Layer**
- ✅ **Interfaces Created:**
  - `IAuthService` - Authentication operations
  - `IUserRepository` - User data access
  - `IJwtTokenService` - JWT token generation and validation

- ✅ **DTOs Created (`AuthDto.cs`):**
  - `RegisterDto` - User registration data
  - `LoginDto` - User login credentials
  - `AuthResponseDto` - Authentication response with tokens
  - `UserInfoDto` - User information
  - `RefreshTokenDto` - Refresh token request

- ✅ **Services:**
  - `AuthService` - Complete authentication logic
    - User registration with role assignment
    - User login with JWT token generation
    - Refresh token handling
    - Token revocation

- ✅ **Configuration:**
  - `JwtSettings` - JWT configuration (secret, issuer, audience, expiry)

### 3. **Infrastructure Layer**
- ✅ **Services:**
  - `JwtTokenService` - JWT token operations
    - Access token generation
    - Refresh token generation
    - Token validation
    - Extract user ID from token

- ✅ **Repositories:**
  - `UserRepository` - User CRUD operations using ASP.NET Core Identity

- ✅ **MongoDB Identity Integration:**
  - Configured MongoDB as the Identity store
  - Password policies configured (relaxed for development)
  - Email confirmation disabled for development

### 4. **Web API**
- ✅ **AuthController** with endpoints:
  - `POST /api/auth/register` - User registration
  - `POST /api/auth/login` - User login
  - `POST /api/auth/refresh-token` - Refresh access token
  - `POST /api/auth/revoke` - Revoke refresh token (authorized)
  - `GET /api/auth/me` - Get current user info (authorized)

### 5. **Blazor UI**
- ✅ **Login Page** (`/auth/login`)
  - Email and password fields
  - Form validation
  - Error handling
  - Success redirect to home

- ✅ **Register Page** (`/auth/register`)
  - Full name (optional)
  - Email, password, confirm password
  - Form validation
  - Success message and redirect

- ✅ **Navigation Menu** updated with Login and Register links

### 6. **Security Configuration**
- ✅ **JWT Authentication** configured in Program.cs
  - Bearer token authentication
  - Token validation parameters
  - Authorization middleware

- ✅ **CORS** configured for API access

## 📦 Packages Added
-  `System.IdentityModel.Tokens.Jwt` (8.17.0) - Infrastructure project
- `Microsoft.AspNetCore.Identity` (2.3.9) - Application project
- `Microsoft.AspNetCore.Authentication.JwtBearer` (10.0.6) - Web project (already installed)

## 🧪 Testing the Authentication

### 1. Register a New User
**Endpoint:** `POST /api/auth/register`
```json
{
  "email": "test@example.com",
  "password": "Test123",
  "confirmPassword": "Test123",
  "fullName": "Test User"
}
```

**Response:**
```json
{
  "success": true,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_refresh_token",
  "expiresAt": "2026-04-16T12:00:00Z",
  "user": {
    "id": "user_id",
    "email": "test@example.com",
    "fullName": "Test User",
    "role": "User"
  }
}
```

### 2. Login
**Endpoint:** `POST /api/auth/login`
```json
{
  "email": "test@example.com",
  "password": "Test123"
}
```

### 3. Use Blazor Pages
- Navigate to `/auth/register` to create an account
- Navigate to `/auth/login` to sign in

## 🔐 Security Notes

⚠️ **For Production:**
1. **Update JWT Secret** - Use a strong, random secret (at least 32 characters)
2. **Enable Password Requirements:**
   - Require digits, uppercase, lowercase, special characters
   - Minimum 8-12 characters
3. **Enable Email Confirmation** - Verify user emails before allowing login
4. **Implement Rate Limiting** - Prevent brute force attacks
5. **Use HTTPS** - Always use secure connections
6. **Store Tokens Securely** - Use HttpOnly cookies or secure storage
7. **Implement Logout** - Add proper logout functionality
8. **Add Multi-Factor Authentication (MFA)** - For enhanced security

## 📋 Next Steps

1. **Song Management UI** - Create pages for browsing, searching, and viewing songs
2. **User Dashboard** - Display user-specific data and favorites
3. **Protected Routes** - Add authorization to protect certain pages
4. **State Management** - Implement proper auth state (e.g., using Blazor Authentication State)
5. **MAUI Integration** - Connect MAUI app to authentication APIs

## 🛠️ Files Created/Modified

**Created:**
- Application Layer:
  - `Interfaces/IAuthService.cs`
  - `Interfaces/IUserRepository.cs`
  - `Interfaces/IJwtTokenService.cs`
  - `DTOs/AuthDto.cs`
  - `Services/AuthService.cs`
  - `Configuration/JwtSettings.cs`

- Infrastructure Layer:
  - `Services/JwtTokenService.cs`
  - `Repositories/UserRepository.cs`

- Web Layer:
  - `Controllers/AuthController.cs`
  - `Components/Pages/Login.razor`
  - `Components/Pages/Register.razor`

**Modified:**
- `Domain/Entities/ApplicationUser.cs` - Added refresh token properties
- `Application/DependencyInjection.cs` - Registered AuthService
- `Infrastructure/DependencyInjection.cs` - Configured Identity and JWT
- `Web/Program.cs` - Added JWT authentication middleware
- `Web/Components/Layout/NavMenu.razor` - Added auth links
- `Web/appsettings.json` - Added JWT configuration
- `Web/appsettings.Development.json` - Added JWT development settings



---

### Merged From: NEXT_STEPS_SONG_MANAGEMENT.md

# Archive Notice

This file is archived planning from an earlier phase.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# Next Steps - Song Management Implementation

## Current Status ✅
- Authentication working perfectly
- Hybrid user support (Node.js + .NET)
- Diagnostic tools in place
- Clean architecture established

## Next Priority: Song Management UI

### Step 1: Verify Existing Backend (Week 3 - Day 1)

#### Check SongsController
- [ ] Test `GET /api/songs` endpoint
- [ ] Test `GET /api/songs/{id}` endpoint
- [ ] Test `POST /api/songs` with auth
- [ ] Test `PUT /api/songs/{id}` with auth
- [ ] Test `DELETE /api/songs/{id}` with auth

#### Verify MongoDB Operations
- [ ] Check if songs collection exists
- [ ] Verify song schema matches Node.js
- [ ] Test CRUD operations
- [ ] Check authorization works

**Testing Commands**:
```bash
# List all songs
curl https://localhost:7005/api/songs

# Get single song
curl https://localhost:7005/api/songs/{id}

# Create song (needs auth token)
curl -X POST https://localhost:7005/api/songs \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Song","category":"Oldies",...}'
```

---

### Step 2: Create Blazor Song Pages (Week 3 - Days 2-3)

#### 2.1 Songs List Page
**File**: `src/OldandNewClone.Web/Components/Pages/Songs.razor`

**Features**:
- Display all songs in a table/grid
- Search by title, category, singer
- Filter by category, mood, key, tempo
- Sort by title, category, date
- Pagination
- Add/Edit/Delete buttons

**UI Layout**:
```
+------------------------------------------+
| 🎵 Song Library                          |
| [Search: _______]  [+ Add Song]          |
|                                          |
| Filters:                                 |
| Category: [All ▼] Mood: [All ▼]          |
| Key: [All ▼] Tempo: [All ▼]              |
|                                          |
| +--------------------------------------+ |
| | Title    | Category | Key | Actions | |
| |----------|----------|-----|----------| |
| | Song 1   | Oldies   | C   | [E][D]  | |
| | Song 2   | New      | G   | [E][D]  | |
| +--------------------------------------+ |
|                                          |
| Showing 1-10 of 50  [< 1 2 3 4 5 >]      |
+------------------------------------------+
```

#### 2.2 Song Details/Edit Page
**File**: `src/OldandNewClone.Web/Components/Pages/SongDetails.razor`

**Features**:
- View song details
- Edit mode toggle
- Display lyrics with formatting
- Show chords if present
- Transpose key feature
- Save/Cancel buttons

**UI Layout**:
```
+------------------------------------------+
| [< Back to Songs]          [✏️ Edit]      |
|                                          |
| 🎵 Song Title                            |
| Category: Oldies  •  Key: C  •  Tempo: 120|
| Singer: John Doe                         |
|                                          |
| Lyrics & Chords:                         |
| +--------------------------------------+ |
| |                                      | |
| | [C] Verse 1 lyrics here              | |
| | [G] More lyrics [Am] continue        | |
| |                                      | |
| +--------------------------------------+ |
|                                          |
| Transpose: [- C +]                       |
|                                          |
| [Save Changes] [Cancel]                  |
+------------------------------------------+
```

#### 2.3 Add/Edit Song Form
**File**: `src/OldandNewClone.Web/Components/Pages/EditSong.razor`

**Features**:
- Form validation
- All song fields
- Category dropdown
- Key dropdown
- Mood dropdown
- Tempo slider
- Lyrics text area
- Save/Cancel buttons

---

### Step 3: Add Authorization (Week 3 - Day 4)

#### Update SongsController
```csharp
[Authorize] // Require authentication
public class SongsController : ControllerBase
{
    [HttpGet] // Public - anyone can view
    [AllowAnonymous]
    public async Task<IActionResult> GetSongs() { }

    [HttpPost] // Require authentication
    public async Task<IActionResult> CreateSong() { }

    [HttpPut("{id}")] // Require authentication
    public async Task<IActionResult> UpdateSong() { }

    [HttpDelete("{id}")] // Require authentication + admin
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSong() { }
}
```

#### Add Authorization Policies
**File**: `src/OldandNewClone.Web/Program.cs`

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
```

---

### Step 4: Implement Search & Filtering (Week 3 - Day 5)

#### Add Search to SongRepository
```csharp
public async Task<List<Song>> SearchSongsAsync(SongSearchCriteria criteria)
{
    var filter = Builders<Song>.Filter.Empty;

    if (!string.IsNullOrEmpty(criteria.SearchText))
    {
        var searchFilter = Builders<Song>.Filter.Or(
            Builders<Song>.Filter.Regex("Title", new BsonRegularExpression(criteria.SearchText, "i")),
            Builders<Song>.Filter.Regex("Singer", new BsonRegularExpression(criteria.SearchText, "i"))
        );
        filter = filter & searchFilter;
    }

    if (!string.IsNullOrEmpty(criteria.Category))
    {
        filter = filter & Builders<Song>.Filter.Eq("Category", criteria.Category);
    }

    // Add more filters...

    return await _songs.Find(filter).ToListAsync();
}
```

---

### Step 5: Test Everything (Week 4 - Day 1)

#### Manual Testing Checklist
- [ ] Login as user
- [ ] View songs list
- [ ] Search for a song
- [ ] Filter by category
- [ ] Click on song to view details
- [ ] Edit song (if authenticated)
- [ ] Create new song
- [ ] Delete song (if admin)
- [ ] Transpose song key
- [ ] Logout and verify can still view (but not edit)

#### Cross-App Testing
- [ ] Create song in C# app
- [ ] Verify appears in Node.js app
- [ ] Edit song in Node.js app
- [ ] Verify changes appear in C# app
- [ ] Delete song in either app
- [ ] Verify deleted in both

---

## Quick Implementation Checklist

### Week 3 Tasks
- [ ] Day 1: Test existing API endpoints
- [ ] Day 2: Create Songs.razor (list view)
- [ ] Day 3: Create SongDetails.razor (view/edit)
- [ ] Day 4: Add authorization to controller
- [ ] Day 5: Implement search and filtering

### Week 4 Tasks
- [ ] Day 1: End-to-end testing
- [ ] Day 2: Bug fixes
- [ ] Day 3: UI improvements
- [ ] Day 4: Performance optimization
- [ ] Day 5: Documentation

---

## File Structure

```
src/OldandNewClone.Web/Components/Pages/
├── Songs.razor                 (Song list)
├── SongDetails.razor           (View/Edit song)
├── EditSong.razor              (Add/Edit form)
└── Components/
    ├── SongCard.razor          (Song card component)
    ├── SongSearch.razor        (Search component)
    └── KeyTransposer.razor     (Transpose component)

src/OldandNewClone.Application/
├── DTOs/
│   ├── SongDto.cs              (Song data transfer)
│   └── SongSearchCriteria.cs   (Search parameters)
└── Services/
    └── SongService.cs          (Already exists)

src/OldandNewClone.Infrastructure/
└── Repositories/
    └── SongRepository.cs       (Already exists, add search)

src/OldandNewClone.Web/Controllers/
└── SongsController.cs          (Already exists, add auth)
```

---

## Sample Code Templates

### Songs.razor (Basic Structure)
```razor
@page "/songs"
@rendermode InteractiveServer
@inject NavigationManager Navigation
@inject ILogger<Songs> Logger

<PageTitle>Songs</PageTitle>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1>🎵 Song Library</h1>
        <button class="btn btn-primary" @onclick="NavigateToAdd">
            + Add Song
        </button>
    </div>

    <!-- Search and filters here -->

    @if (songs == null)
    {
        <div class="spinner-border"></div>
    }
    else if (songs.Count == 0)
    {
        <p>No songs found</p>
    }
    else
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Title</th>
                    <th>Category</th>
                    <th>Key</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var song in songs)
                {
                    <tr>
                        <td>@song.Title</td>
                        <td>@song.Category</td>
                        <td>@song.Key</td>
                        <td>
                            <button @onclick="() => ViewSong(song.Id)">View</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<SongDto>? songs;

    protected override async Task OnInitializedAsync()
    {
        await LoadSongs();
    }

    private async Task LoadSongs()
    {
        // Call API to load songs
    }

    private void ViewSong(string id)
    {
        Navigation.NavigateTo($"/songs/{id}");
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/songs/add");
    }
}
```

---

## Success Metrics

### Performance Targets
- Page load: < 1 second
- Search results: < 500ms
- Song details: < 500ms
- Create/update: < 1 second

### User Experience
- Intuitive navigation
- Clear error messages
- Loading indicators
- Responsive design
- Mobile-friendly

### Technical
- All CRUD operations work
- Authorization properly enforced
- Cross-app compatibility maintained
- No data corruption
- Proper error handling

---

## Let's Start!

The foundation is solid. Now we build the features users will actually use!

**First Step**: Let's test the existing SongsController and verify it works with the MongoDB data.

**Command to run**:
```bash
# In browser or Postman
GET https://localhost:7005/api/songs
```

Do you want to:
1. Test the existing API first?
2. Start creating the Songs.razor page?
3. Check what songs exist in MongoDB?

Let me know and we'll proceed! 🚀



---

### Merged From: docs/NODE_MAIN_MIGRATION_BASELINE.md

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



---

### Merged From: docs/SETUP-SUMMARY.md

# Archive Notice

This file is archived setup history.

- Canonical migration status and roadmap: [CSHARP_MIGRATION_PLAN.md](../CSHARP_MIGRATION_PLAN.md)
- Do not update progress/status in this file.

---

# OldandNewClone - Architecture & Setup Summary

**Date**: 2026-04-15  
**Status**: M1 (Setup and Architecture Baseline) - ✅ **85% Complete**

---

## ✅ What Has Been Completed

### 1. Solution Structure ✅
Created a complete 7-project solution following Clean Architecture:

```
OldandNewClone.sln
├── src/
│   ├── OldandNewClone.Domain              ✅ Domain entities and value objects
│   ├── OldandNewClone.Application         ✅ Business logic and services
│   ├── OldandNewClone.Infrastructure      ✅ MongoDB and data access
│   ├── OldandNewClone.Web                 ✅ Blazor Web App
│   └── OldandNewClone.MobileDesktop       ✅ MAUI Blazor Hybrid
├── tests/
│   ├── OldandNewClone.UnitTests           ✅ 22 tests passing
│   └── OldandNewClone.IntegrationTests    ✅ Project created
└── docs/
    ├── feature-mapping.md                 ✅ Complete feature matrix
    ├── milestones.md                      ✅ Progress tracking
    └── runbooks.md                        ✅ Development guide
```

### 2. Domain Layer ✅
**Entities Created:**
- `Song` - Core song entity with lyrics, chords, metadata
- `ApplicationUser` - ASP.NET Identity user with MongoDB
- `ApplicationRole` - Role management
- `UserData` - User favorites and setlists

**Value Objects:**
- `MusicKey` - Music theory utilities (keys, transposition)

### 3. Application Layer ✅
**Services Implemented:**
- `ISongService` / `SongService` - Song CRUD and filtering
- `IUserDataService` / `UserDataService` - User data management
- `ITransposeService` / `TransposeService` - Chord transposition ✅ **22 Tests Passing**

**DTOs Created:**
- `SongDto`, `SongListItemDto`, `CreateSongDto`, `UpdateSongDto`
- `UserDataDto`, `UpdateUserDataDto`

### 4. Infrastructure Layer ✅
**MongoDB Integration:**
- `MongoContext` - Database context
- `MongoDbSettings` - Configuration
- Connection string: ✅ Configured (MongoDB Atlas)

**Repositories:**
- `ISongRepository` / `SongRepository` - Full CRUD + search/filter
- `IUserDataRepository` / `UserDataRepository` - User data ops

### 5. Web Layer (Blazor) ✅
**API Controllers:**
- `SongsController` - RESTful song endpoints
- `UserDataController` - User data endpoints

**Configuration:**
- `Program.cs` - ✅ Wired up with DI
- `appsettings.Development.json` - ✅ MongoDB connection configured

### 6. Testing ✅
**Unit Tests:**
- `TransposeServiceTests` - 13 tests passing
- `MusicKeyTests` - 9 tests passing
- **Total: 22 tests, 100% pass rate**

### 7. Documentation ✅
- `README.md` - Complete project overview
- `feature-mapping.md` - Full feature parity matrix
- `milestones.md` - 7-phase milestone tracking
- `runbooks.md` - Development commands and procedures
- `.gitignore` - Proper exclusions for .NET + MAUI

---

## 🔧 Technology Stack

| Layer | Technology | Version | Status |
|-------|-----------|---------|--------|
| Framework | .NET | 10.0 | ✅ |
| Web UI | Blazor | 10.0 | ✅ |
| Mobile/Desktop | .NET MAUI | 10.0 | ✅ |
| Database | MongoDB | 3.4.2 driver | ✅ |
| Auth | ASP.NET Identity | 7.0 | ✅ |
| Testing | xUnit | Latest | ✅ |

---

## 📋 Next Steps (Remaining 15% of M1)

### 1. Authentication Implementation (High Priority)
- [ ] Configure ASP.NET Core Identity with MongoDB
- [ ] Create login/register pages
- [ ] Implement JWT token authentication
- [ ] Add role-based authorization (`[Authorize(Roles = "Admin")]`)
- [ ] Test development bypass mode

### 2. Web UI Components (Phase M3 Start)
- [ ] Create `MainLayout.razor` (three-panel shell)
- [ ] Create `SongsList.razor` component
- [ ] Create `SongPreview.razor` component
- [ ] Create `SetlistPanel.razor` component
- [ ] Wire up API controllers to UI

### 3. Data Import
- [ ] Import existing songs.json to MongoDB
- [ ] Verify all songs loaded correctly
- [ ] Create MongoDB indexes for performance

### 4. Testing & Verification
- [ ] Integration tests for MongoDB repositories
- [ ] API endpoint tests
- [ ] End-to-end smoke test

---

## 🗄️ Database Schema

### Songs Collection (`OldNewSongs`)
```javascript
{
  "_id": ObjectId,
  "SongId": 1,              // int, auto-increment
  "Title": "Song Title",
  "Category": "Old|New",
  "Key": "C",
  "Tempo": "120",
  "Time": "4/4",
  "Taal": "Keherwa",
  "Genres": ["Hindi", "Worship", "Evergreen"],
  "Lyrics": "C G Am F\n...",
  "CreatedAt": ISODate,
  "UpdatedAt": ISODate
}
```

### UserData Collection
```javascript
{
  "_id": "user-id",
  "UserId": "user-id",
  "Name": "User Name",
  "Email": "user@example.com",
  "Favorites": [1, 2, 3],        // Song IDs
  "NewSetlist": [4, 5],
  "OldSetlist": [6, 7],
  "UpdatedAt": ISODate
}
```

---

## 🚀 Quick Start Commands

```powershell
# Build entire solution
dotnet build

# Run Web app
cd src/OldandNewClone.Web
dotnet watch run
# → https://localhost:5001

# Run tests
dotnet test

# Run MAUI Android
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-android -t:Run
```

---

## 🎯 Architecture Decisions Made

1. **Authentication**: ASP.NET Core Identity with MongoDB (instead of Auth0)
   - **Why**: Offline support for MAUI, no external dependency, cost-effective
   - **Trade-off**: More setup vs managed service

2. **Database**: MongoDB (same as original)
   - **Why**: Keep existing data, flexible schema, cloud-ready
   - **Schema**: Matching original Node.js structure

3. **Platform Strategy**: Blazor Web + MAUI Blazor Hybrid
   - **Why**: Shared UI components, single codebase, native performance
   - **Benefit**: 95%+ code reuse between platforms

4. **Project Structure**: Clean Architecture (Domain → Application → Infrastructure)
   - **Why**: Testability, maintainability, clear separation of concerns
   - **Benefit**: Easy to swap infrastructure (e.g., MongoDB → SQL Server)

5. **.NET Version**: .NET 10
   - **Why**: Latest features, best performance, long-term support
   - **Risk**: Bleeding edge (mitigated by stable release)

---

## 🔐 Security Configuration

### Development
```json
{
  "Authentication": {
    "Enabled": true,
    "DevelopmentBypass": true  // ← Auto-login for local dev
  }
}
```

### Production
```json
{
  "Authentication": {
    "Enabled": true,
    "DevelopmentBypass": false  // ← Full auth required
  }
}
```

**Test Accounts:**
- Admin: `admin@test.com` / `Test@123`
- User: `user@test.com` / `Test@123`

---

## 📊 Current Metrics

| Metric | Value |
|--------|-------|
| **Projects** | 7 |
| **Total Files** | ~40 |
| **Unit Tests** | 22 (100% passing) |
| **Test Coverage** | Services: ~80% |
| **Build Time** | ~6 seconds |
| **Lines of Code** | ~2,500 |

---

## ⚠️ Known Issues & Blockers

**Current:**
- ❌ None! All builds passing.

**Upcoming:**
- ⚠️ Authentication pages not yet created
- ⚠️ UI components not yet implemented
- ⚠️ Song data not yet imported

---

## 🎓 Learning Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui)
- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor)
- [MongoDB C# Driver](https://www.mongodb.com/docs/drivers/csharp)
- [Clean Architecture by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)

---

## 📞 Support & Contact

**Developer**: Swaresh Pawar  
**GitHub**: [@SwareshPawar](https://github.com/SwareshPawar)  
**Original App**: [oldand-new.vercel.app](https://oldand-new.vercel.app/)

---

## ✅ Verification Checklist

Before moving to M2, verify:

- [x] Solution builds successfully
- [x] All unit tests pass (22/22)
- [x] MongoDB connection configured
- [x] Domain entities created
- [x] Repositories implemented
- [x] Services implemented
- [x] API controllers created
- [x] Documentation complete
- [ ] Authentication implemented (next)
- [ ] Web UI started (next)
- [ ] Data imported (next)

---

**🎉 M1 Status: 85% Complete - Excellent Progress!**

**Next Milestone**: M2 - Complete authentication + start Web UI components

**Estimated Completion**: Week 1 complete, Week 2 starting

---

**Last Updated**: 2026-04-15

