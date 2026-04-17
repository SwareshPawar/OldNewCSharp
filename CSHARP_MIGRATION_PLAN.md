# C# Application Migration Plan - Node.js to .NET 10

## Overview
This document outlines the complete migration strategy for porting the Old & New Song Management application from Node.js to C# .NET 10 with MAUI and Blazor.

## Project Information

### Source Application (Node.js)
- **Repository**: https://github.com/SwareshPawar/OldandNew
- **Branch**: AdminPassword (custom authentication)
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

### 🔄 Phase 10: Workspace Persistence
**Status**: In Progress
**Timeline**: Next

**Completed Deliverables**:
- ✅ Persist theme mode
- ✅ Persist font size
- ✅ Persist Panel 2 category tab
- ✅ Persist selected section in Panel 1
- ✅ Persist selected song in Panel 2
- ✅ Persist active panel state across reloads

**Next Deliverables**:
- ⏳ Persist Panel 3 reading position if useful

---

### 🔄 Phase 11: Shell Polish
**Status**: Planned

**Next Deliverables**:
- ⏳ ESC to close dialogs and drawers
- ⏳ Focus handling for modals
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

### 🔄 Phase 13: Data and Security Hardening
**Status**: Planned

**Next Deliverables**:
- ⏳ Authorization review for edit/delete flows
- ⏳ Validation review for add/edit payloads
- ⏳ Error handling improvements
- ⏳ Concurrency / safe write review

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
- ✅ `NODEJS_AUTH_MIGRATION.md` - Authentication migration details
- ✅ `SHARED_MONGODB_MIGRATION_GUIDE.md` - Database sharing guide
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
Setlists will now be migrated as first-class entities and later integrated into the Teams-style shell.

### Backend Foundation Completed
- ✅ Added `Setlist` domain entity
- ✅ Added `SetlistDto`, `CreateSetlistDto`, `UpdateSetlistDto`
- ✅ Added `ISetlistRepository` and `ISetlistService`
- ✅ Added `SetlistRepository`
- ✅ Added `SetlistService`
- ✅ Added `SetlistsController`
- ✅ Wired DI registrations
- ✅ Added MongoDB `Setlists` collection access
- ✅ Removed obsolete Web and MAUI `Setlist.razor` pages
- ✅ Removed obsolete nav links pointing to fixed-bucket setlists

### Next Implementation Steps
- ⏳ Add Shell UI for Global / My / Smart setlists
- ⏳ Add create/edit/delete dialogs for setlists
- ⏳ Add add/remove song to specific setlist
- ⏳ Add New/Old grouping within setlist detail view
- ⏳ Keep Favorites separate from setlists
- ⏳ De-emphasize legacy `NewSetlist` / `OldSetlist` storage after migration path is complete

---

## Phase 10B: Setlist Folder Hierarchy in Panel 1 (STARTED)

### Requirement
Setlists must follow a **folder hierarchy** in the Teams-style shell.

### Panel 1 Setlist Structure
Panel 1 should not treat `Global Setlists`, `My Setlists`, and `Smart Setlists` as individual setlists.
Instead, they are **folder/group nodes** that contain user-created setlists under them.

Structure:
- **Global Setlists** (folder)
  - Setlist A
  - Setlist B
- **My Setlists** (folder)
  - Setlist C
  - Setlist D
- **Smart Setlists** (folder)
  - Setlist E
  - Setlist F

### UX Interpretation
- The folder nodes live in **Panel 1**
- The created setlists also appear in **Panel 1** as nested child items under the folder
- Panel 1 therefore contains both:
  - top-level workspace sections
  - setlist folders
  - actual setlist entries beneath each folder

### Implementation Rule
- `Global Setlists`, `My Setlists`, and `Smart Setlists` are not leaf items
- They are expandable/collapsible containers
- The actual setlist documents loaded from MongoDB are rendered beneath them
- This mirrors the original application more closely than the earlier fixed-bucket model

### Status
- ✅ Requirement documented
- ✅ Reader aligned to original MongoDB setlist collections
- ⏳ Panel 1 nested folder rendering
- ⏳ Setlist selection behavior
- ⏳ Panel 2 / Panel 3 integration for selected setlist
