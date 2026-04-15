# OldandNew Clone - C# Edition

A modern cross-platform song management application for church worship teams, built with Blazor and .NET MAUI.

## 🎯 Project Overview

This is a complete C# rebuild of the [OldandNew](https://github.com/SwareshPawar/OldandNew) application, providing:
- **Blazor Web App** for browser access
- **.NET MAUI Blazor Hybrid** for Android, iOS, and Windows desktop
- **MongoDB** for data storage
- **ASP.NET Core Identity** for authentication
- **Clean Architecture** for maintainability

## 🚀 Features

### User Features
- Browse and search songs with lyrics and chords
- Filter by category (Old/New), key, and genre
- Transpose chords to different keys
- Create and manage favorites
- Build custom setlists (New/Old)
- Offline support (MAUI apps)
- Light and dark themes

### Admin Features
- Add, edit, and delete songs
- Bulk import/export
- Missing data reports
- Content quality tools

## 🏗️ Architecture

```
OldandNewClone/
├── src/
│   ├── OldandNewClone.Domain          # Entities, value objects
│   ├── OldandNewClone.Application     # Business logic, services
│   ├── OldandNewClone.Infrastructure  # MongoDB, repositories
│   ├── OldandNewClone.Web             # Blazor Web App
│   └── OldandNewClone.MobileDesktop   # MAUI Blazor Hybrid
├── tests/
│   ├── OldandNewClone.UnitTests
│   └── OldandNewClone.IntegrationTests
└── docs/
    ├── feature-mapping.md             # Feature parity tracking
    ├── milestones.md                  # Project progress
    └── runbooks.md                    # Development guide
```

## 📋 Prerequisites

- **Visual Studio 2026** (18.5+) or latest
- **.NET 10 SDK**
- **MongoDB Atlas** account (or local MongoDB)
- **Android SDK** (for MAUI Android)
- **Xcode** (for MAUI iOS, Mac only)

## 🛠️ Getting Started

### 1. Clone the Repository
```powershell
git clone https://github.com/SwareshPawar/OldNewCSharp.git
cd OldNewCSharp
```

### 2. Configure MongoDB
Update `src/OldandNewClone.Web/appsettings.Development.json`:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "your-mongodb-connection-string",
    "DatabaseName": "OldNewSongs"
  }
}
```

### 3. Build and Run

#### Web App
```powershell
cd src/OldandNewClone.Web
dotnet watch run
```
Navigate to: `https://localhost:5001`

#### MAUI Android
```powershell
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-android -t:Run
```

#### MAUI Windows
```powershell
cd src/OldandNewClone.MobileDesktop
dotnet build -f net10.0-windows10.0.19041.0 -t:Run
```

## 📊 Project Status

**Current Milestone**: M1 - Setup and Architecture Baseline (🔄 In Progress)

See [milestones.md](docs/milestones.md) for detailed progress tracking.

## 📚 Documentation

- **[Feature Mapping](docs/feature-mapping.md)** - Complete feature parity matrix
- **[Milestones](docs/milestones.md)** - Sprint planning and progress
- **[Runbook](docs/runbooks.md)** - Development commands and troubleshooting

## 🧪 Testing

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🔐 Authentication

- **Development**: Uses test accounts (bypass available)
  - Admin: `admin@test.com` / `Test@123`
  - User: `user@test.com` / `Test@123`
- **Production**: Full ASP.NET Core Identity with MongoDB

## 🎨 Tech Stack

- **.NET 10** - Latest framework
- **Blazor** - Interactive web UI
- **.NET MAUI** - Cross-platform native apps
- **MongoDB** - Document database
- **ASP.NET Core Identity** - Authentication
- **xUnit** - Testing framework

## 📱 Supported Platforms

| Platform | Status |
|----------|--------|
| Web (Blazor Server) | ✅ In Development |
| Android | 📋 Planned |
| iOS | 📋 Planned |
| Windows Desktop | 📋 Planned |
| macOS | 📋 Future |

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Run tests: `dotnet test`
4. Build: `dotnet build`
5. Submit a pull request

## 📄 License

MIT License - See LICENSE file for details

## 🔗 Links

- **Original App**: [oldand-new.vercel.app](https://oldand-new.vercel.app/)
- **Original Repo**: [github.com/SwareshPawar/OldandNew](https://github.com/SwareshPawar/OldandNew)
- **Project Board**: [Coming Soon]

## 👤 Author

**Swaresh Pawar**
- GitHub: [@SwareshPawar](https://github.com/SwareshPawar)

---

**Last Updated**: 2026-04-15

