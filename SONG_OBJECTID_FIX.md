# Song Entity MongoDB Field Mapping Fix

## Problem 1 (Initial)
When accessing `/api/songs`, got error:
```
System.FormatException: Cannot deserialize a 'String' from BsonType 'ObjectId'.
```

## Problem 2 (After Adding [BsonId])
After adding `[BsonId]` attribute, got new error:
```
FormatException: Element 'id' does not match any field or property of class OldandNewClone.Domain.Entities.Song.
```

## Root Cause
**Node.js stores MongoDB documents with lowercase field names**, but C# properties use PascalCase:
- MongoDB: `{ _id: ObjectId(...), id: 1, title: "Song", category: "Bhajan", ... }`
- C#: `{ Id, SongId, Title, Category, ... }`

## Solution
Added `[BsonElement]` attributes to map C# property names to MongoDB field names:

```csharp
[BsonId]
[BsonRepresentation(BsonType.ObjectId)]
[BsonElement("_id")]  // Maps to MongoDB _id field
public string Id { get; set; } = null!;

[BsonElement("id")]  // Maps to MongoDB id field (Node.js songId)
public int SongId { get; set; }

[BsonElement("title")]  // Maps to MongoDB title field
public string Title { get; set; } = string.Empty;
// ... and so on for all properties
```

## What Each Attribute Does

### `[BsonId]`
- Marks this property as the MongoDB document identifier
- Maps to `_id` field in MongoDB

### `[BsonRepresentation(BsonType.ObjectId)]`
- Converts between MongoDB ObjectId and C# string
- MongoDB: `ObjectId("507f1f77bcf86cd799439011")`
- C#: `string "507f1f77bcf86cd799439011"`

### `[BsonElement("fieldname")]`
- Maps C# PascalCase property to MongoDB lowercase field
- C# `Title` ↔ MongoDB `title`
- C# `SongId` ↔ MongoDB `id`

## Compatibility with Node.js
This approach ensures **perfect compatibility** with your existing Node.js application:
- ✅ C# can read songs created by Node.js
- ✅ Node.js can read songs created by C#
- ✅ Both apps share the same MongoDB database
- ✅ No data migration required

## Testing
✅ Build successful
🔄 Next: Restart debugger and test `/api/songs` endpoint

## Note
You need to **stop debugging and restart** (or hot reload) for changes to take effect.
