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
