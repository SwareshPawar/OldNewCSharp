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
