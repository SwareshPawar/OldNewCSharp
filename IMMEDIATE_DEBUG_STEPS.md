# IMMEDIATE DEBUG STEPS

## The Problem
You're getting "An error occurred during login" when clicking the login button.

## Quick Debug Steps

### Step 1: Start the App
Press **F5** in Visual Studio to start debugging.

### Step 2: Test User Lookup First
**Before trying to login**, open a browser and go to:

```
http://localhost:5000/api/logindebug/test-lookup/swaresh.pawar@gmail.com
```

This will show you:
- Whether the user is found via standard lookup
- Whether the user is found via hybrid lookup (MongoDB direct)
- User details including password hash

### Step 3: Check the Response

**Expected Success Response:**
```json
{
  "input": "swaresh.pawar@gmail.com",
  "normalizedInput": "swaresh.pawar@gmail.com",
  "standardLookup": null,  // OK if null (user not in Identity yet)
  "hybridLookup": {
    "found": true,
    "userName": "...",
    "email": "swaresh.pawar@gmail.com",
    "hasPasswordHash": true,
    "passwordHashPrefix": "$2a$10$..."
  }
}
```

**If you see an error:**
```json
{
  "error": "...",
  "type": "...",
  "stackTrace": "...",
  "innerError": "..."
}
```

This tells you EXACTLY what's wrong!

### Step 4: Check Visual Studio Output Window

1. Go to **View** → **Output**
2. Select **"Debug"** from the dropdown
3. Look for log messages:

```
info: OldandNewClone.Infrastructure.Repositories.UserRepository[0]
      Searching MongoDB for user with login input: swaresh.pawar@gmail.com

info: OldandNewClone.Infrastructure.Repositories.UserRepository[0]
      Found Node.js user in MongoDB for: swaresh.pawar@gmail.com
```

### Step 5: Test Actual Login

Now navigate to:
```
http://localhost:5000/auth/login
```

Enter:
- Username or Email: `swaresh.pawar@gmail.com`
- Password: `Swar@123`

Click "Login"

### Step 6: Check Error Message

The error message now shows the ACTUAL exception:

Instead of generic "An error occurred", you'll see:
```
An error occurred: [Actual error message here]
```

### Step 7: Check Application Logs

In Visual Studio Output window, look for:

```
error: OldandNewClone.Application.Services.AuthService[0]
      Error during user login for input: swaresh.pawar@gmail.com. Exception: [Type], Message: [Message]
```

## Common Errors and Solutions

### Error: "MongoDB database is null!"

**Cause**: IMongoDatabase not being injected correctly

**Solution**:
1. Check `appsettings.Development.json` has MongoDB connection string
2. Restart the app
3. Check DependencyInjection.cs registration

### Error: "Cannot deserialize..."

**Cause**: User has ObjectId format that Identity can't read

**Solution**:
- The hybrid lookup should handle this by querying MongoDB directly
- Check if `FindNodeJsUserAsync` is being called

### Error: "User not found"

**Check**:
1. Is user in MongoDB?
2. Is email/username correct?
3. Is email/username lowercase in MongoDB?

**Test**: Run the `/api/usercheck/check/swaresh.pawar@gmail.com` endpoint

### Error: "Password verification failed"

**Cause**: Password hash format issue

**Test**:
```
POST http://localhost:5000/api/usercheck/test-login
{
  "emailOrUsername": "swaresh.pawar@gmail.com",
  "password": "Swar@123"
}
```

Should show: `"passwordMatches": true`

## Debug Checklist

Before reporting the issue, check:

- [ ] App is running (no build errors)
- [ ] MongoDB connection string is correct
- [ ] Test lookup endpoint works: `/api/logindebug/test-lookup/swaresh.pawar@gmail.com`
- [ ] User is found in hybrid lookup
- [ ] User has password hash
- [ ] Visual Studio Output shows debug logs
- [ ] Actual error message from login page (not just "An error occurred")

## Get the Actual Error

1. **Start app with F5**
2. **Open browser console** (F12)
3. **Navigate to login page**
4. **Enter credentials and click Login**
5. **Check both**:
   - Error message on the page (now shows actual exception)
   - Visual Studio Output window (shows detailed logs)

## Share This Info

If still not working, share:
1. Response from `/api/logindebug/test-lookup/swaresh.pawar@gmail.com`
2. Error message from login page
3. Logs from Visual Studio Output window
4. Any error in browser console (F12)

This will show exactly what's failing!
