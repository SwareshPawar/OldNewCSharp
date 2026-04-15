# OldandNewClone - API Reference

**Base URL (Development)**: `https://localhost:5001/api`

---

## 🏥 Health Check Endpoints

### GET /api/health
Check if the API is running.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-04-15T12:00:00Z",
  "version": "1.0.0",
  "environment": "Development"
}
```

### GET /api/health/database
Check MongoDB connection and database status.

**Response (Success):**
```json
{
  "status": "Connected",
  "database": "OldNewSongs",
  "songCount": 1234,
  "timestamp": "2026-04-15T12:00:00Z"
}
```

**Response (Error):**
```json
{
  "status": "Unhealthy",
  "error": "Connection timeout",
  "timestamp": "2026-04-15T12:00:00Z"
}
```

---

## 🎵 Songs Endpoints

### GET /api/songs
Get all songs (list view).

**Response:**
```json
[
  {
    "id": 1,
    "title": "Aap Ki Nazron Ne Samjha",
    "category": "Old",
    "key": "Dm",
    "genres": ["Old", "Sad", "Evergreen", "Hindi"]
  }
]
```

### GET /api/songs/{id}
Get a specific song with full details.

**Parameters:**
- `id` (int) - Song ID

**Response:**
```json
{
  "id": 1,
  "title": "Aap Ki Nazron Ne Samjha",
  "category": "Old",
  "key": "Dm",
  "tempo": "150",
  "time": "7/8",
  "taal": "Rupak",
  "genres": ["Old", "Sad", "Evergreen", "Hindi", "Love"],
  "lyrics": "Dm         Am             A#               C\nAapki Nazaron Ne Samjha..."
}
```

### GET /api/songs/search?q={searchTerm}
Search songs by title or lyrics.

**Query Parameters:**
- `q` (string) - Search term

**Response:**
```json
[
  {
    "id": 1,
    "title": "Aap Ki Nazron Ne Samjha",
    "category": "Old",
    "key": "Dm",
    "genres": ["Old", "Sad", "Evergreen"]
  }
]
```

### GET /api/songs/filter?category={cat}&key={key}&genres={genre1,genre2}
Filter songs by category, key, and/or genres.

**Query Parameters:**
- `category` (string, optional) - "Old" or "New"
- `key` (string, optional) - Musical key (e.g., "C", "Dm")
- `genres` (array, optional) - Genre tags

**Example:**
```
GET /api/songs/filter?category=Old&key=Dm&genres=Hindi,Love
```

**Response:**
```json
[
  {
    "id": 1,
    "title": "Aap Ki Nazron Ne Samjha",
    "category": "Old",
    "key": "Dm",
    "genres": ["Old", "Sad", "Evergreen", "Hindi", "Love"]
  }
]
```

### POST /api/songs
Create a new song (Admin only).

**Request Body:**
```json
{
  "title": "New Song",
  "category": "New",
  "key": "G",
  "tempo": "120",
  "time": "4/4",
  "taal": "Keherwa",
  "genres": ["New", "Worship"],
  "lyrics": "G D Em C\nVerse lyrics..."
}
```

**Response:**
```json
{
  "id": 1235,
  "title": "New Song",
  "category": "New",
  "key": "G",
  "tempo": "120",
  "time": "4/4",
  "taal": "Keherwa",
  "genres": ["New", "Worship"],
  "lyrics": "G D Em C\nVerse lyrics..."
}
```

### PUT /api/songs/{id}
Update an existing song (Admin only).

**Request Body:**
```json
{
  "id": 1235,
  "title": "Updated Song",
  "category": "New",
  "key": "A",
  "tempo": "130",
  "time": "4/4",
  "taal": "Keherwa",
  "genres": ["New", "Worship", "Upbeat"],
  "lyrics": "A E F#m D\nUpdated lyrics..."
}
```

**Response:**
```json
{
  "message": "Song updated"
}
```

### DELETE /api/songs/{id}
Delete a song (Admin only).

**Response:**
```json
{
  "message": "Song deleted"
}
```

---

## 👤 User Data Endpoints

### GET /api/userdata/{userId}
Get user's favorites and setlists.

**Response:**
```json
{
  "userId": "user123",
  "name": "John Doe",
  "email": "john@example.com",
  "favorites": [1, 5, 10],
  "newSetlist": [2, 6],
  "oldSetlist": [3, 7, 9]
}
```

### PUT /api/userdata/{userId}
Update user's data.

**Request Body:**
```json
{
  "favorites": [1, 5, 10, 15],
  "newSetlist": [2, 6, 8],
  "oldSetlist": [3, 7, 9, 11],
  "name": "John Doe",
  "email": "john@example.com"
}
```

**Response:**
```json
{
  "userId": "user123",
  "name": "John Doe",
  "email": "john@example.com",
  "favorites": [1, 5, 10, 15],
  "newSetlist": [2, 6, 8],
  "oldSetlist": [3, 7, 9, 11]
}
```

### POST /api/userdata/{userId}/favorites/{songId}
Toggle a song in favorites.

**Response:**
```json
{
  "success": true
}
```

### POST /api/userdata/{userId}/new-setlist/{songId}
Toggle a song in New setlist.

**Response:**
```json
{
  "success": true
}
```

### POST /api/userdata/{userId}/old-setlist/{songId}
Toggle a song in Old setlist.

**Response:**
```json
{
  "success": true
}
```

---

## 🔐 Authentication (Coming Soon)

### POST /api/auth/register
Register a new user.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "name": "User Name"
}
```

### POST /api/auth/login
Authenticate and receive a JWT token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "user123",
  "email": "user@example.com",
  "role": "User"
}
```

---

## 🎼 Transpose Service (Backend Only)

The transpose service is available programmatically but not exposed as an HTTP endpoint.  
Transposition can be done client-side or integrated into the song preview component.

**Methods Available:**
- `TransposeLyrics(string lyrics, int semitones)` - Transpose all chords in lyrics
- `TransposeChord(string chord, int semitones)` - Transpose a single chord
- `CalculateSemitones(string fromKey, string toKey)` - Get semitone distance

---

## 🔒 Authorization

Currently, all endpoints are **public** (no authentication required).

**Coming in M2:**
- `[Authorize]` - Requires any authenticated user
- `[Authorize(Roles = "Admin")]` - Requires admin role

---

## 📝 Error Responses

### 404 Not Found
```json
{
  "error": "Song not found"
}
```

### 400 Bad Request
```json
{
  "error": "Invalid request data"
}
```

### 500 Internal Server Error
```json
{
  "error": "Database connection failed"
}
```

---

## 🧪 Testing with cURL

### Get all songs
```bash
curl https://localhost:5001/api/songs
```

### Search songs
```bash
curl "https://localhost:5001/api/songs/search?q=nazron"
```

### Filter songs
```bash
curl "https://localhost:5001/api/songs/filter?category=Old&key=Dm"
```

### Create song (requires admin later)
```bash
curl -X POST https://localhost:5001/api/songs \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Song","category":"New","key":"C","tempo":"120","time":"4/4","taal":"Keherwa","genres":["Test"],"lyrics":"C G Am F"}'
```

---

## 📱 MAUI Client Usage

The same API is consumed by both Web and MAUI apps using shared services:

```csharp
// Inject ISongService in a Blazor component
@inject ISongService SongService

var songs = await SongService.GetAllSongsAsync();
var song = await SongService.GetSongByIdAsync(1);
```

---

## 🔄 API Versioning (Future)

Not yet implemented. Future versions may use:
- URL versioning: `/api/v2/songs`
- Header versioning: `Accept: application/vnd.oldandnew.v2+json`

---

**Last Updated**: 2026-04-15
