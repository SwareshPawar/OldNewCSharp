# OldandNew Clone - Milestones

Track progress across all development phases.

---

## Status Legend
- ❌ **Not Started**
- 🔄 **In Progress**
- ✅ **Done**
- 🚫 **Blocked**

---

## M1: Setup and Architecture Baseline
**Target**: Week 1  
**Status**: 🔄 **In Progress**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Create solution structure | - | ✅ | 7 projects created |
| Add project references | - | ✅ | Dependencies wired |
| Install NuGet packages | - | ✅ | MongoDB, Identity, etc. |
| Configure MongoDB connection | - | ✅ | Connection string added |
| Create domain entities | - | ✅ | Song, User, UserData |
| Create repository interfaces | - | ✅ | ISongRepo, IUserDataRepo |
| Implement repositories | - | ✅ | MongoDB repositories |
| Create application services | - | ✅ | Song, UserData, Transpose |
| Wire up DI in Web project | - | ✅ | Program.cs configured |
| Create API controllers | - | ✅ | Songs, UserData endpoints |
| Configure authentication | - | 📋 | Identity setup needed |
| Create appsettings configs | - | ✅ | Dev settings ready |
| Verify build succeeds | - | 📋 | Pending |
| Document architecture | - | ✅ | feature-mapping.md created |

**Blockers**: None  
**Next**: Complete authentication setup, verify build

---

## M2: Shared Core Services
**Target**: Week 2  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Implement TransposeService tests | - | 📋 | xUnit tests |
| Create RecommendationService | - | 📋 | Scoring algorithm |
| Create SongQueryService | - | 📋 | Advanced filtering |
| Add validation layer | - | 📋 | FluentValidation |
| Create DTO mappers | - | 📋 | AutoMapper or manual |
| Add logging infrastructure | - | 📋 | Serilog recommended |
| Health checks (MongoDB) | - | 📋 | /health endpoint |
| Unit tests for services | - | 📋 | 80%+ coverage target |

**Blockers**: None  
**Dependencies**: M1 complete

---

## M3: Web Feature Parity
**Target**: Weeks 3-4  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Create MainLayout component | - | 📋 | Three-panel shell |
| Create SongsList component | - | 📋 | List with filters |
| Create SongPreview component | - | 📋 | Lyrics with chords |
| Create SetlistPanel component | - | 📋 | Favorites + setlists |
| Implement category filter | - | 📋 | Old/New tabs |
| Implement key filter | - | 📋 | Dropdown |
| Implement genre filter | - | 📋 | Multi-select |
| Implement search | - | 📋 | Debounced input |
| Implement transpose UI | - | 📋 | +/- semitone buttons |
| Implement prev/next navigation | - | 📋 | Keyboard support |
| Create Settings page | - | 📋 | Theme, auto-scroll, etc. |
| Add theme toggle | - | 📋 | Light/dark mode |
| Make responsive | - | 📋 | Mobile, tablet, desktop |
| E2E tests (Playwright) | - | 📋 | Critical user journeys |

**Blockers**: None  
**Dependencies**: M2 complete

---

## M4: Mobile and Desktop Parity (MAUI)
**Target**: Week 5  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Configure MAUI project | - | 📋 | Platform targets |
| Reference shared components | - | 📋 | Razor Class Library |
| Platform-specific handlers | - | 📋 | Android, iOS, Windows |
| Offline storage (SQLite) | - | 📋 | Local cache |
| Sync service | - | 📋 | Background sync |
| Keep-screen-awake | - | 📋 | MAUI Essentials |
| Swipe gestures | - | 📋 | Touch navigation |
| Test on Android emulator | - | 📋 | API 33+ |
| Test on iOS simulator | - | 📋 | iOS 15+ |
| Test on Windows desktop | - | 📋 | Windows 11 |
| Optimize for phone layout | - | 📋 | Single-column |
| Optimize for tablet layout | - | 📋 | Two-column |

**Blockers**: None  
**Dependencies**: M3 complete

---

## M5: Admin and Bulk Tools
**Target**: Week 6  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Create AdminDashboard page | - | 📋 | Overview metrics |
| Create SongEditor component | - | 📋 | Add/edit form |
| Create SongTable component | - | 📋 | Sortable, filterable |
| Implement role-based auth | - | 📋 | Admin role check |
| Bulk import tool | - | 📋 | JSON/CSV upload |
| Bulk export tool | - | 📋 | Download all songs |
| BPM tap utility | - | 📋 | Manual tempo entry |
| Missing data report | - | 📋 | Validation dashboard |
| Auto-tag helper | - | 📋 | Parse chords |
| Merge duplicates tool | - | 📋 | Fuzzy matching |

**Blockers**: None  
**Dependencies**: M4 complete

---

## M6: QA, Accessibility, and Performance
**Target**: Week 7  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Integration tests | - | 📋 | API + data layer |
| UI smoke tests (Web) | - | 📋 | Playwright |
| UI smoke tests (MAUI) | - | 📋 | UITest |
| WCAG 2.2 AA audit | - | 📋 | Axe DevTools |
| Keyboard navigation test | - | 📋 | All features |
| Screen reader test | - | 📋 | NVDA/JAWS |
| Contrast ratio check | - | 📋 | 4.5:1 minimum |
| Focus order validation | - | 📋 | Logical flow |
| Large dataset test | - | 📋 | 10,000+ songs |
| Filter performance test | - | 📋 | < 150ms target |
| Virtualization for lists | - | 📋 | Blazor Virtualize |
| Add caching layer | - | 📋 | In-memory cache |
| Debounce search input | - | 📋 | 300ms delay |

**Blockers**: None  
**Dependencies**: M5 complete

---

## M7: Release and Monitoring
**Target**: Week 8  
**Status**: ❌ **Not Started**

| Task | Owner | Status | Notes |
|------|-------|--------|-------|
| Create CI/CD pipeline | - | 📋 | GitHub Actions |
| Deploy Web to Azure | - | 📋 | App Service |
| Publish MAUI to Google Play | - | 📋 | Internal test track |
| Publish MAUI to App Store | - | 📋 | TestFlight |
| Add Application Insights | - | 📋 | Telemetry |
| Add error tracking | - | 📋 | Sentry or similar |
| Create runbook | - | 📋 | Deployment steps |
| Create rollback plan | - | 📋 | Restore procedure |
| UAT with pilot users | - | 📋 | 5-10 testers |
| Fix critical bugs | - | 📋 | P0/P1 issues |
| Public release (Web) | - | 📋 | Production deploy |
| Public release (MAUI) | - | 📋 | Store submission |

**Blockers**: None  
**Dependencies**: M6 complete

---

## Weekly Tracking Template

### Week 1 (2026-04-15 to 2026-04-21)
**Milestone**: M1  
**Planned Items**:
- Solution setup ✅
- Domain entities ✅
- Repositories ✅
- API controllers ✅
- Authentication setup 📋

**Completed Items**:
- Created 7-project solution
- Configured MongoDB connection
- Implemented core repositories
- Created API endpoints

**Blockers**: None

**Decisions**:
- Use ASP.NET Identity with MongoDB instead of Auth0
- Support dev bypass for local development
- Use .NET 10 for latest features

**Risks for Next Week**:
- Authentication implementation complexity
- First build verification

---

**Last Updated**: 2026-04-15
