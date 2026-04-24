# OldandNew Clone - Feature Mapping

This document maps all features from the original OldandNew application to the new C# implementation.

Source baseline: `github.com/SwareshPawar/OldandNew` branch `main`.
Local reference snapshot: `refs/OldandNew-main`.
For migration status, use `CSHARP_MIGRATION_PLAN.md`.

## Feature Status Legend
- ✅ **Completed**: Feature implemented and tested
- 🔄 **In Progress**: Currently being developed
- 📋 **Planned**: Not yet started
- ⏸️ **Blocked**: Waiting on dependencies

---

## Phase 1: Core Song Management

| Original Feature | New Feature | Status | Notes |
|-----------------|-------------|--------|-------|
| Song list view | SongsList component | 📋 | Display all songs with filters |
| Song details/lyrics | SongPreview component | 📋 | Full lyrics with chords |
| Search songs | Search API + UI | 📋 | Text search in title and lyrics |
| Filter by Category (Old/New) | Category filter | 📋 | Tab-based filtering |
| Filter by Key | Key filter dropdown | 📋 | All major and minor keys |
| Filter by Genre | Genre multi-select | 📋 | Multiple genre tags |
| Transpose chords | TransposeService | ✅ | Backend service ready |
| Prev/Next navigation | Navigation controls | 📋 | Navigate through filtered list |

---

## Phase 2: User Features

| Original Feature | New Feature | Status | Notes |
|-----------------|-------------|--------|-------|
| Favorites | UserData.Favorites | ✅ | Backend ready |
| New Setlist | UserData.NewSetlist | ✅ | Backend ready |
| Old Setlist | UserData.OldSetlist | ✅ | Backend ready |
| User authentication | ASP.NET Identity + MongoDB | 📋 | Dev bypass available |
| Cloud sync | MongoDB UserData | ✅ | Automatic with backend |
| Offline support (MAUI) | Local storage + sync | 📋 | MAUI-specific |

---

## Phase 3: Admin Features

| Original Feature | New Feature | Status | Notes |
|-----------------|-------------|--------|-------|
| Add song | Admin/SongEditor | 📋 | Form with validation |
| Edit song | Admin/SongEditor | 📋 | Inline or modal edit |
| Delete song | Delete API + confirmation | 📋 | With confirmation dialog |
| Song table view | Admin/SongTable | 📋 | Sortable, filterable table |
| Bulk operations | Admin tools | 📋 | Import/export/merge |
| Missing data report | Content quality view | 📋 | Show incomplete songs |

---

## Phase 4: UI/UX

| Original Feature | New Feature | Status | Notes |
|-----------------|-------------|--------|-------|
| Three-panel layout | MainLayout.razor | 📋 | List/Preview/Setlist |
| Responsive design | Mobile-first CSS | 📋 | Touch-friendly |
| Dark/Light theme | Theme system | 📋 | CSS variables + toggle |
| Auto-scroll lyrics | Auto-scroll service | 📋 | Configurable speed |
| Keep screen awake | MAUI WakeLock | 📋 | MAUI Essentials |
| History tracking | Recent songs list | 📋 | Per-user history |

---

## Phase 5: Advanced Features

| Original Feature | New Feature | Status | Notes |
|-----------------|-------------|--------|-------|
| Song recommendations | RecommendationService | 📋 | Based on key/genre/tempo |
| BPM tap tool | BPM calculator | 📋 | Admin utility |
| Chord detection | Auto-tag helper | 📋 | Parse lyrics for chords |
| Export setlist | Export API | 📋 | PDF/Text export |
| Import songs | Bulk import tool | 📋 | JSON/CSV import |

---

## Platform-Specific Features

### Web Only
- SEO optimization
- Share links to songs
- PWA support (optional)

### MAUI Only
- Offline-first mode
- Local database cache
- Platform-specific gestures (swipe)
- Native navigation

---

## Migration Checklist

- [ ] Import existing songs.json into MongoDB
- [ ] Verify all songs have correct schema
- [ ] Test transpose logic with sample songs
- [ ] Create test users (admin@test.com, user@test.com)
- [ ] Validate all API endpoints
- [ ] Build initial Blazor UI components
- [ ] Test Web app locally
- [ ] Test MAUI app on Android
- [ ] Test MAUI app on iOS
- [ ] Performance test with full dataset
- [ ] Accessibility audit (WCAG 2.2 AA)
- [ ] Deploy to production

---

## Out of Scope for V1

- Real-time collaborative editing
- Social features (comments, ratings)
- Audio playback integration
- Video tutorials
- Multi-language support (i18n)

---

**Last Updated**: 2026-04-15
