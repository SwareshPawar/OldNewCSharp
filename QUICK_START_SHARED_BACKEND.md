# 🔑 QUICK START: Shared MongoDB Backend

## The Problem You Had

Your Node.js app stores passwords in a `password` field, but .NET uses `PasswordHash`. This caused login failures.

## The Solution

We've implemented **automatic password field migration** that runs every time your .NET app starts.

## What Happens Now

### When .NET App Starts:
1. ✅ Checks all MongoDB users
2. ✅ Copies `password` → `PasswordHash` for users that need it
3. ✅ Keeps `password` field intact for Node.js compatibility

### When You Create a User in .NET:
1. ✅ Saves to `PasswordHash` (for .NET)
2. ✅ Saves to `password` (for Node.js)
3. ✅ User works in BOTH apps

## Test It Right Now

### 1. Start Your .NET App

Watch the console for:
```
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Found X users that need password field migration
info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Password field migration complete. Migrated: X, Failed: 0
```

### 2. Login with Existing Node.js User

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "your-existing-username",
  "password": "your-existing-password"
}
```

Should work! ✅

### 3. Check Migration Status

```http
GET http://localhost:5000/api/passwordmigration/status
```

Response:
```json
{
  "totalUsers": 5,
  "usersNeedingMigration": 0,  // Should be 0 after migration
  "migrationNeeded": false
}
```

### 4. Test a User in Both Apps

**Node.js App** (port 3000):
```bash
curl -X POST http://localhost:3000/api/login \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "qwerty123"}'
```

**.NET App** (port 5000):
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail": "testuser", "password": "qwerty123"}'
```

Both should work! ✅

## Diagnostic Tools

### Check if a User is Migrated

```http
GET http://localhost:5000/api/passwordtest/check-user/username-here
```

Look for:
```json
{
  "hasPasswordHash": true,  // Must be true
  "passwordHashLength": 60   // BCrypt hash length
}
```

### Test Password Verification

```http
POST http://localhost:5000/api/passwordtest/test-password
Content-Type: application/json

{
  "usernameOrEmail": "testuser",
  "password": "qwerty123"
}
```

Look for:
```json
{
  "message": "✅ Password matches!"
}
```

## What Changed in Your Code

### New Files:
1. `src/OldandNewClone.Web/Services/PasswordFieldMigration.cs` - Auto-migration service
2. `src/OldandNewClone.Infrastructure/Services/PasswordFieldSyncService.cs` - Sync service
3. `src/OldandNewClone.Web/Controllers/PasswordTestController.cs` - Diagnostic endpoints
4. `src/OldandNewClone.Web/Controllers/PasswordMigrationController.cs` - Migration API

### Updated Files:
1. `src/OldandNewClone.Web/Program.cs` - Runs migration on startup
2. `src/OldandNewClone.Infrastructure/DependencyInjection.cs` - Registers services

## No Breaking Changes

- ✅ Existing Node.js users still work in Node.js app
- ✅ Existing Node.js users now work in .NET app
- ✅ New .NET users work in both apps
- ✅ No data loss - `password` field is never deleted

## If Something Goes Wrong

### Run Manual Migration:
```http
POST http://localhost:5000/api/passwordmigration/migrate
```

### Check Logs:
Look for errors in the console when app starts.

### Verify BCrypt:
```http
GET http://localhost:5000/api/passwordtest/bcrypt-info
```

Should show:
```json
{
  "testVerify": true  // Must be true
}
```

## That's It!

Your apps now share the same MongoDB backend and users can login to both! 🎉

For more details, see: `CSHARP_MIGRATION_PLAN.md` (single migration source of truth)
