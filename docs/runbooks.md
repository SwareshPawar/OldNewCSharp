# OldandNew Clone - Development Runbook

Quick reference guide for common development tasks.

---

## Getting Started

### Prerequisites
- Visual Studio 2026 (18.5+)
- .NET 10 SDK
- MongoDB connection (Atlas or local)
- Git

### First-Time Setup
```powershell
# Clone repository
git clone https://github.com/SwareshPawar/OldNewCSharp
cd OldNewCSharp

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run Web app
dotnet run --project src/OldandNewClone.Web

# Run MAUI app (Android)
dotnet build src/OldandNewClone.MobileDesktop -f net10.0-android -t:Run
```

---

## Environment Configuration

### Development (.env not used - use appsettings.Development.json)
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://...",
    "DatabaseName": "OldNewSongs"
  },
  "Authentication": {
    "DevelopmentBypass": true
  }
}
```

### Production (appsettings.json + Azure App Settings)
```json
{
  "MongoDbSettings": {
    "ConnectionString": "<from-azure-key-vault>",
    "DatabaseName": "OldNewSongs"
  },
  "Authentication": {
    "DevelopmentBypass": false
  }
}
```

---

## Common Tasks

### Run Web App Locally
```powershell
cd src/OldandNewClone.Web
dotnet watch run
```
Navigate to: `https://localhost:5001`

### Run MAUI App (Android)
```powershell
# Start Android emulator first (Pixel 5 API 33+)
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-android -t:Run
```

### Run MAUI App (iOS)
```powershell
# Requires Mac build host
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-ios -t:Run
```

### Run MAUI App (Windows)
```powershell
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-windows10.0.19041.0 -t:Run
```

### Run Tests
```powershell
# All tests
dotnet test

# Unit tests only
dotnet test tests/OldandNewClone.UnitTests

# Integration tests
dotnet test tests/OldandNewClone.IntegrationTests

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Add New Package
```powershell
dotnet add src/OldandNewClone.Infrastructure package <PackageName>
```

### Create Database Migration (if using EF Core later)
```powershell
dotnet ef migrations add <MigrationName> --project src/OldandNewClone.Infrastructure
```

---

## MongoDB Operations

### Import Existing Songs
```powershell
# Using mongoimport (replace with your actual file)
mongoimport --uri "mongodb+srv://..." --collection OldNewSongs --file songs.json --jsonArray
```

### Backup Database
```powershell
mongodump --uri "mongodb+srv://..." --db OldNewSongs --out ./backup
```

### Restore Database
```powershell
mongorestore --uri "mongodb+srv://..." --db OldNewSongs ./backup/OldNewSongs
```

### Create Indexes (run once)
```javascript
// In MongoDB shell or Compass
use OldNewSongs;

// Song indexes
db.OldNewSongs.createIndex({ "SongId": 1 }, { unique: true });
db.OldNewSongs.createIndex({ "Title": "text", "Lyrics": "text" });
db.OldNewSongs.createIndex({ "Category": 1 });
db.OldNewSongs.createIndex({ "Key": 1 });
db.OldNewSongs.createIndex({ "Genres": 1 });

// UserData indexes
db.UserData.createIndex({ "UserId": 1 }, { unique: true });
```

---

## Troubleshooting

### Build Fails
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### MAUI Build Fails
```powershell
# Check workloads
dotnet workload list

# Repair/install MAUI
dotnet workload install maui

# Update workloads
dotnet workload update
```

### MongoDB Connection Issues
- Verify connection string in appsettings
- Check IP whitelist in MongoDB Atlas
- Test connection with MongoDB Compass

### Authentication Not Working
- Check DevelopmentBypass setting
- Verify test user credentials
- Check cookie/JWT configuration

---

## Code Quality

### Format Code
```powershell
dotnet format
```

### Analyze Code
```powershell
dotnet build /p:RunAnalyzers=true
```

### Security Scan
```powershell
dotnet list package --vulnerable
```

---

## Deployment

### Build for Production
```powershell
# Web
dotnet publish src/OldandNewClone.Web -c Release -o ./publish/web

# MAUI Android
dotnet publish src/OldandNewClone.MobileDesktop -f net10.0-android -c Release

# MAUI iOS
dotnet publish src/OldandNewClone.MobileDesktop -f net10.0-ios -c Release
```

### Deploy Web to Azure
```powershell
# Using Azure CLI
az webapp deploy --resource-group <rg> --name <app-name> --src-path ./publish/web
```

---

## Useful Links
- [MongoDB Atlas](https://cloud.mongodb.com)
- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui)
- [Blazor Docs](https://learn.microsoft.com/aspnet/core/blazor)
- [GitHub Repo](https://github.com/SwareshPawar/OldNewCSharp)

---

**Last Updated**: 2026-04-15
