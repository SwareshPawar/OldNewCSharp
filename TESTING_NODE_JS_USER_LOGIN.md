# Testing Guide - Node.js User Login

## Your Specific User

**Email**: `swaresh.pawar@gmail.com`
**Password**: `Swar@123`

## Step-by-Step Testing

### 1. Start the .NET App

Press **F5** in Visual Studio or run:
```bash
dotnet run --project src/OldandNewClone.Web
```

### 2. Check Application Logs

Watch for these messages in the console:

```
info: OldandNewClone.Web.Services.DatabaseInitializer[0]
      Database initialized. Found X users. Password migration will run next.

info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Found X users that need password field migration

info: OldandNewClone.Web.Services.PasswordFieldMigration[0]
      Migrated password hash for user: ...
```

### 3. Check User Exists in MongoDB

Open a browser or use curl:

```http
GET http://localhost:5000/api/usercheck/check/swaresh.pawar@gmail.com
```

Expected response:
```json
{
  "found": true,
  "username": "...",
  "email": "swaresh.pawar@gmail.com",
  "hasPasswordField": true,
  "hasPasswordHashField": true,
  "passwordPrefix": "$2a$10$...",
  "allFields": ["_id", "username", "email", "password", "firstName", "lastName", ...]
}
```

### 4. Test Password Directly

```http
POST http://localhost:5000/api/usercheck/test-login
Content-Type: application/json

{
  "emailOrUsername": "swaresh.pawar@gmail.com",
  "password": "Swar@123"
}
```

Expected response:
```json
{
  "success": true,
  "message": "✅ Password matches!",
  "user": { ... },
  "passwordMatches": true
}
```

### 5. Test Actual Login

Navigate to: `http://localhost:5000/auth/login`

OR use API:

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "swaresh.pawar@gmail.com",
  "password": "Swar@123"
}
```

Expected response:
```json
{
  "success": true,
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "user": {
    "firstName": "...",
    "lastName": "...",
    "username": "...",
    "email": "swaresh.pawar@gmail.com"
  }
}
```

## Troubleshooting

### Issue: User Not Found

**Run this check:**
```http
GET http://localhost:5000/api/usercheck/check/swaresh.pawar@gmail.com
```

**If `found: false`:**
1. User might have been cleared by DatabaseInitializer
2. Check MongoDB directly using MongoDB Compass
3. Verify connection string in `appsettings.Development.json`

**Solution**: Re-create the user in Node.js app or .NET app

### Issue: Password Doesn't Match

**Run this test:**
```http
POST http://localhost:5000/api/usercheck/test-login
Content-Type: application/json

{
  "emailOrUsername": "swaresh.pawar@gmail.com",
  "password": "Swar@123"
}
```

**Check the response:**
- `passwordMatches`: Should be `true`
- `user.hasPasswordField`: Should be `true`
- `user.passwordPrefix`: Should start with `$2a$` or `$2b$`

**If password doesn't match:**
1. Password might be incorrect
2. Password hash might be corrupted
3. Wrong BCrypt algorithm

**Solution**: Reset password in Node.js app or use diagnostic endpoint

### Issue: "Invalid credentials" Error

**Check application logs:**

Look for:
```
info: Found user: [username] ([email]), attempting password verification
warn: Password verification failed for user: [username]. Result: Failed
```

**This means:**
- User was found ✅
- Password verification failed ❌

**Possible causes:**
1. Password hash in MongoDB doesn't match the password
2. BCrypt verification is failing
3. Password field vs PasswordHash field mismatch

**Debug steps:**

1. Check which field is being used:
```http
GET http://localhost:5000/api/passwordtest/check-user/swaresh.pawar@gmail.com
```

2. Test BCrypt directly:
```http
POST http://localhost:5000/api/passwordtest/test-password
Content-Type: application/json

{
  "usernameOrEmail": "swaresh.pawar@gmail.com",
  "password": "Swar@123"
}
```

3. Check BCrypt configuration:
```http
GET http://localhost:5000/api/passwordtest/bcrypt-info
```

### Issue: Migration Didn't Run

**Check migration status:**
```http
GET http://localhost:5000/api/passwordmigration/status
```

**If `usersNeedingMigration > 0`:**

Run manual migration:
```http
POST http://localhost:5000/api/passwordmigration/migrate
```

## Expected Behavior

### When Login Works

1. User navigates to `/auth/login`
2. Enters `swaresh.pawar@gmail.com` and `Swar@123`
3. Clicks "Login"
4. Redirected to home page ✅

### Application Logs
```
info: Found user: swaresh... (swaresh.pawar@gmail.com), attempting password verification
info: Login successful for user: swaresh...
```

## What Changed to Fix the Issue

### 1. DatabaseInitializer
- **Before**: Cleared all users with ObjectId format
- **After**: Just logs status, doesn't clear users

### 2. PasswordFieldMigration
- **Before**: Only copies `password` → `PasswordHash`
- **After**: Same, but runs AFTER DatabaseInitializer (users not cleared)

### 3. UserRepository (NEW)
- **Added**: `GetByUsernameOrEmailHybridAsync()` method
- **What it does**: 
  1. Tries to find user via UserManager (.NET users)
  2. If not found, queries MongoDB directly (Node.js users)
  3. Converts Node.js user format to ApplicationUser
  4. Copies `password` field to `PasswordHash` in memory

### 4. AuthService
- **Changed**: Uses `GetByUsernameOrEmailHybridAsync()` instead of `GetByUsernameOrEmailAsync()`
- **Effect**: Can login Node.js users without requiring migration first

## How It Works Now

### Node.js User Login Flow

1. User enters `swaresh.pawar@gmail.com` + `Swar@123`
2. AuthService calls `GetByUsernameOrEmailHybridAsync()`
3. UserRepository:
   - Tries UserManager.FindByEmail() → **NOT FOUND** (Node.js user not in Identity)
   - Queries MongoDB directly with email filter → **FOUND**
   - Reads MongoDB document:
     ```json
     {
       "_id": "...",
       "email": "swaresh.pawar@gmail.com",
       "username": "...",
       "password": "$2a$10$hash...",
       "firstName": "Swaresh",
       "lastName": "Pawar"
     }
     ```
   - Creates ApplicationUser in memory:
     ```csharp
     new ApplicationUser {
       Email = "swaresh.pawar@gmail.com",
       PasswordHash = "$2a$10$hash...", // Copied from 'password' field
       FirstName = "Swaresh",
       LastName = "Pawar"
     }
     ```
4. AuthService calls `SignInManager.CheckPasswordSignInAsync(user, "Swar@123")`
5. SignInManager uses `BCryptPasswordHasher` to verify:
   ```csharp
   BCrypt.Net.BCrypt.Verify("Swar@123", "$2a$10$hash...")
   ```
6. If verification succeeds → Login successful! ✅

## Verification Checklist

Before reporting login failure, verify:

- [ ] App is running (check console for startup messages)
- [ ] User exists in MongoDB (check via `/api/usercheck/check/email`)
- [ ] User has `password` field (check `hasPasswordField: true`)
- [ ] Password is correct (test via `/api/usercheck/test-login`)
- [ ] BCrypt is working (check `/api/passwordtest/bcrypt-info`)
- [ ] Hybrid lookup is working (check app logs for "Found Node.js user")

## Success Indicators

✅ **Migration ran successfully**:
```
info: Password field migration complete. Migrated: X, Failed: 0
```

✅ **User found via hybrid lookup**:
```
info: Found Node.js user [username], using password field
info: Found user: [username] ([email]), attempting password verification
```

✅ **Password verified**:
```
info: Login successful for user: [username]
```

✅ **Token generated**:
```json
{
  "success": true,
  "accessToken": "eyJ..."
}
```

## Still Not Working?

If you've checked everything and it still doesn't work:

1. **Check MongoDB directly** using MongoDB Compass
   - Connect to: `mongodb+srv://genericuser:Swar%40123@cluster0.ovya99h.mongodb.net/`
   - Database: `OldNewSongs`
   - Collection: `Users`
   - Find document with email: `swaresh.pawar@gmail.com`
   - Verify `password` field exists and starts with `$2a$` or `$2b$`

2. **Test password hash manually** using online BCrypt checker:
   - Go to: https://bcrypt-generator.com/
   - Paste password: `Swar@123`
   - Paste hash from MongoDB `password` field
   - Click "Verify" - should say "Match!"

3. **Enable detailed logging** in `appsettings.Development.json`:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "OldandNewClone": "Trace"
       }
     }
   }
   ```

4. **Share diagnostic info**:
   - User check result: `/api/usercheck/check/swaresh.pawar@gmail.com`
   - Password test result: `/api/usercheck/test-login`
   - Application logs (console output)
