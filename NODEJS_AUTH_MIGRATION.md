# Node.js Authentication Migration

## Overview
This document explains how the C# .NET authentication system was updated to match the Node.js AdminPassword branch implementation.

## Original Node.js Implementation (AdminPassword Branch)

### User Structure
```javascript
{
  firstName: string,
  lastName: string,
  username: string (lowercase),
  email: string (lowercase),
  phone: string,
  password: string (bcrypt hash),
  isAdmin: boolean
}
```

### Password Hashing
- **Library**: `bcryptjs`
- **Rounds**: 10
- **Registration**: `bcrypt.hash(password, 10)`
- **Verification**: `bcrypt.compare(password, user.password)`

### Authentication Flow
1. **Registration**: 
   - Check for existing username or email (case-insensitive)
   - Store username and email as lowercase
   - Hash password with bcrypt (10 rounds)
   - Store user in MongoDB

2. **Login**:
   - Accept username OR email as login input
   - Find user by username OR email (case-insensitive)
   - Verify password with bcrypt.compare()
   - Generate JWT token with 7-day expiry

## C# .NET Implementation Changes

### 1. ApplicationUser Entity Updated
**File**: `src/OldandNewClone.Domain/Entities/ApplicationUser.cs`

Added fields to match Node.js structure:
```csharp
public string FirstName { get; set; } = string.Empty;
public string LastName { get; set; } = string.Empty;
public string Phone { get; set; } = string.Empty;
public bool IsAdmin { get; set; } = false;
```

### 2. DTOs Updated
**File**: `src/OldandNewClone.Application/DTOs/AuthDto.cs`

#### RegisterDto
Changed from email-only to full user profile:
```csharp
public string FirstName { get; set; }
public string LastName { get; set; }
public string Username { get; set; }
public string Email { get; set; }
public string Phone { get; set; }
public string Password { get; set; }
public string ConfirmPassword { get; set; }
public bool IsAdmin { get; set; } = false;
```

#### LoginDto
Changed from email-only to username/email:
```csharp
public string UsernameOrEmail { get; set; }  // Was: Email
public string Password { get; set; }
```

#### UserInfoDto
Added all user fields:
```csharp
public string FirstName { get; set; }
public string LastName { get; set; }
public string Username { get; set; }
public string Email { get; set; }
public string Phone { get; set; }
public bool IsAdmin { get; set; }
public string Role { get; set; }
```

### 3. BCrypt Configuration
**File**: `src/OldandNewClone.Infrastructure/Services/BCryptPasswordHasher.cs`

Explicitly set work factor to 10 to match Node.js:
```csharp
private const int WorkFactor = 10;

public string HashPassword(TUser user, string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
}
```

### 4. UserRepository Enhanced
**File**: `src/OldandNewClone.Infrastructure/Repositories/UserRepository.cs`

Added methods to support username OR email lookup:
```csharp
Task<ApplicationUser?> GetByUsernameAsync(string username);
Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput);
Task<bool> UsernameExistsAsync(string username);
```

Implementation matches Node.js logic:
```csharp
public async Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput)
{
    // Try username first
    var user = await _userManager.FindByNameAsync(loginInput);
    if (user != null) return user;

    // Try email
    return await _userManager.FindByEmailAsync(loginInput);
}
```

### 5. AuthService Updated
**File**: `src/OldandNewClone.Application/Services/AuthService.cs`

#### Registration Logic
```csharp
// Check for existing username or email (case-insensitive) - matching Node.js
if (await _userRepository.EmailExistsAsync(registerDto.Email.ToLower()))
    return error("User or email already exists");

if (await _userRepository.UsernameExistsAsync(registerDto.Username.ToLower()))
    return error("User or email already exists");

// Match Node.js user structure
var user = new ApplicationUser
{
    UserName = registerDto.Username.ToLower(), // Store lowercase like Node.js
    Email = registerDto.Email.ToLower(),
    FirstName = registerDto.FirstName,
    LastName = registerDto.LastName,
    Phone = registerDto.Phone,
    IsAdmin = registerDto.IsAdmin,
    // ...
};
```

#### Login Logic
```csharp
// Match Node.js: Find by username or email, case-insensitive
var loginInput = loginDto.UsernameOrEmail.Trim().ToLower();
var user = await _userRepository.GetByUsernameOrEmailAsync(loginInput);

if (user == null)
    return error("Invalid credentials");

// BCrypt verification happens via SignInManager → BCryptPasswordHasher
var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

if (!result.Succeeded)
    return error("Invalid credentials");
```

### 6. UI Pages Updated
**Files**: 
- `src/OldandNewClone.Web/Components/Pages/Login.razor`
- `src/OldandNewClone.Web/Components/Pages/Register.razor`

#### Login Page
- Changed from "Email" to "Username or Email" input
- Updated API call to send `usernameOrEmail` instead of `email`

#### Register Page
- Added fields: First Name, Last Name, Username, Phone
- Removed: Full Name (optional)
- Updated API call to send all required fields

## Key Differences & Compatibility

### Password Hashing
| Node.js (bcryptjs) | C# (BCrypt.Net) |
|-------------------|-----------------|
| `bcrypt.hash(password, 10)` | `BCrypt.HashPassword(password, 10)` |
| `bcrypt.compare(password, hash)` | `BCrypt.Verify(password, hash)` |

**Result**: ✅ Compatible - Both use bcrypt with 10 rounds

### Case Sensitivity
| Node.js | C# .NET |
|---------|---------|
| `username.toLowerCase()` | `username.ToLower()` |
| `email.toLowerCase()` | `email.ToLower()` |

**Result**: ✅ Compatible - Both store lowercase

### Login Lookup
| Node.js | C# .NET |
|---------|---------|
| MongoDB `$or` query | Try username, then email |

**Result**: ✅ Functionally equivalent

## Migration Steps for Existing Users

### Important: Clear Existing Users
Before testing, you **must** clear existing users because:
1. Old users don't have `FirstName`, `LastName`, `Phone` fields
2. Old users may have incompatible password hashes
3. Old users use email as UserName instead of username

### How to Clear Users
1. Navigate to `/migration` page in the app
2. Click "Clear All Users"
3. Click "Clear All Roles"
4. Verify collections are empty

**OR** use the DatabaseInitializer (runs automatically on app startup):
- Checks for incompatible users
- Automatically clears Users and Roles collections if needed

## Testing the New Implementation

### 1. Register a New User
```bash
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "username": "johndoe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "password": "qwerty123",
  "confirmPassword": "qwerty123"
}
```

Expected Response:
```json
{
  "success": true,
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "user": {
    "id": "...",
    "firstName": "John",
    "lastName": "Doe",
    "username": "johndoe",
    "email": "john@example.com",
    "phone": "+1234567890",
    "isAdmin": false,
    "role": "User"
  }
}
```

### 2. Login with Username
```bash
POST /api/auth/login
{
  "usernameOrEmail": "johndoe",
  "password": "qwerty123"
}
```

### 3. Login with Email
```bash
POST /api/auth/login
{
  "usernameOrEmail": "john@example.com",
  "password": "qwerty123"
}
```

Both should succeed with the same token response.

## Verification Checklist

✅ BCrypt work factor set to 10 (matches Node.js)
✅ Registration creates user with all required fields
✅ Usernames stored as lowercase
✅ Emails stored as lowercase
✅ Login accepts username OR email
✅ Password verification uses BCrypt.Verify
✅ Duplicate username check implemented
✅ Duplicate email check implemented
✅ User entity matches Node.js structure
✅ UI pages updated with all fields
✅ Error messages match Node.js ("Invalid credentials", "User or email already exists")

## Common Issues & Solutions

### Issue: Login fails with "Invalid credentials"
**Solution**: 
1. Clear existing users (they have old structure)
2. Register fresh user with new registration form
3. Verify user has all required fields in MongoDB

### Issue: "User or email already exists" on registration
**Solution**:
1. Check if username/email already exists (case-insensitive)
2. Use different username or email
3. Clear database if testing

### Issue: Password hash not verifying
**Solution**:
1. Ensure BCryptPasswordHasher is registered in DI
2. Verify work factor is set to 10
3. Check that password is not empty or null

## Next Steps

1. **Test Registration**: Create a new user with all fields
2. **Test Login**: Login with username, then with email
3. **Verify Token**: Check JWT token contains correct user data
4. **Test Password**: Ensure bcrypt verification works correctly
5. **Monitor Logs**: Check application logs for any errors

## References

- Original Node.js Implementation: https://github.com/SwareshPawar/OldandNew/tree/AdminPassword
- BCrypt.Net Documentation: https://github.com/BcryptNet/bcrypt.net
- ASP.NET Core Identity: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity
