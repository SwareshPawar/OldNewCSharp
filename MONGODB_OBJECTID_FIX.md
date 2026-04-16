# MongoDB ObjectId Serialization Fix

## 🔧 Problem Resolved

**Error:** `Cannot deserialize a 'String' from BsonType 'ObjectId'`

**Root Cause:** MongoDB was storing Identity user IDs as ObjectId (MongoDB's native type), but ASP.NET Core Identity expects string IDs.

## ✅ Solution Implemented

### 1. **Created MongoDB Serialization Configuration**
**File:** `src/OldandNewClone.Infrastructure/Configuration/MongoDbSerializationConfig.cs`

This configures MongoDB to properly serialize/deserialize ObjectIds as strings for Identity classes.

```csharp
public static class MongoDbSerializationConfig
{
    public static void Configure()
    {
        // Maps ApplicationUser.Id to handle ObjectId ↔ String conversion
        BsonClassMap.RegisterClassMap<ApplicationUser>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.Id)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
        });

        // Same for ApplicationRole
        BsonClassMap.RegisterClassMap<ApplicationRole>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.Id)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
        });
    }
}
```

### 2. **Registered Configuration in DI**
The serialization config is now called before MongoDB Identity setup in `DependencyInjection.cs`.

### 3. **Created Migration Tools**

#### **Migration Controller** (`/api/migration/*`)
- `POST /api/migration/clear-users` - Delete all users
- `POST /api/migration/clear-roles` - Delete all roles
- `GET /api/migration/check-collections` - Check database status

#### **Migration Page** (`/migration`)
- View database status (user count, role count, collections)
- Clear all users
- Clear all roles
- Guided next steps

## 🚀 How to Fix Your Current Issue

### **Quick Fix (Recommended):**

1. **Stop the application** (if running)
2. **Start the application** (F5)
3. **Navigate to** `/migration`
4. **Click** "Clear All Users & Roles" button
5. **Go to** `/auth/register`
6. **Create a new account**
7. **Login** - Should work now! ✅

### **Alternative - Manual MongoDB Cleanup:**

```javascript
// In MongoDB Compass or Shell
db.Users.deleteMany({})
db.Roles.deleteMany({})
```

Then register a new user.

## 📋 What Happens Now

### **Before (Broken):**
```json
// MongoDB stored:
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),  // ObjectId type
  "Email": "user@example.com",
  ...
}

// Identity expected:
{
  "Id": "507f1f77bcf86cd799439011",  // String type
  ...
}
```

### **After (Fixed):**
```json
// MongoDB stores ObjectId, but serializes to string:
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),  // Stored as ObjectId
  "Email": "user@example.com",
  ...
}

// Identity receives:
{
  "Id": "507f1f77bcf86cd799439011",  // Deserialized as String ✓
  ...
}
```

## 🧪 Testing Steps

1. **Clear existing data** using `/migration` page
2. **Register new user** at `/auth/register`
   - Email: `test@example.com`
   - Password: `password123`
3. **Login** at `/auth/login`
4. **Should work!** ✅

## 📝 Files Created/Modified

**Created:**
- `src/OldandNewClone.Infrastructure/Configuration/MongoDbSerializationConfig.cs`
- `src/OldandNewClone.Web/Controllers/MigrationController.cs`
- `src/OldandNewClone.Web/Components/Pages/Migration.razor`

**Modified:**
- `src/OldandNewClone.Infrastructure/DependencyInjection.cs` - Added serialization config call
- `src/OldandNewClone.Web/Components/Layout/NavMenu.razor` - Added Migration link

## ⚠️ Important Notes

1. **Existing Users Won't Work** - They have the old ObjectId format incompatible with Identity
2. **Solution:** Clear database and re-register users
3. **New Users** will work perfectly with the serialization fix
4. **Production:** Export user data before clearing, then import with new format

## 🔐 Security Note

The Migration page has dangerous operations. Consider:
- Adding authentication/authorization
- Removing it in production
- Adding confirmation dialogs
- Logging all migration actions

## ✅ Summary

- ✅ MongoDB ObjectId serialization configured
- ✅ Migration tools created
- ✅ Clear path to fix existing data
- ✅ New registrations will work correctly
- ✅ Login will work after clearing old data

**Ready to use:** Stop → Start → Visit `/migration` → Clear Users → Register → Login! 🚀
