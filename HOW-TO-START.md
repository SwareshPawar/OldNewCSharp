# 🚀 How to Start the Project

## Quick Start (Visual Studio 2026)

### Option 1: Using Visual Studio (Recommended)

1. **Open the Solution**
   - Open `OldandNewClone.sln` in Visual Studio 2026

2. **Set Startup Project**
   - In **Solution Explorer**, right-click on `OldandNewClone.Web`
   - Select **"Set as Startup Project"**
   - OR: Click the dropdown next to the green ▶ button and select `OldandNewClone.Web`

3. **Run the Project**
   - Press **F5** or click the green ▶ **"OldandNewClone.Web"** button
   - Visual Studio will:
     - Build the solution
     - Start the web server
     - Automatically open your browser to `https://localhost:7005`

4. **What You'll See**
   - ✅ Landing page showing project status
   - ✅ Links to test the API
   - ✅ Interactive API testing dashboard
   - ✅ Documentation links

---

### Option 2: Using Command Line

```powershell
# Navigate to the Web project
cd src/OldandNewClone.Web

# Run the app (auto-opens browser)
dotnet run

# OR use watch mode (hot reload)
dotnet watch run
```

**URL:** `https://localhost:7005` or `http://localhost:5265`

---

## 🎯 What to Do First

### 1. Test the Home Page
- Should see: "✅ Project Status: Running!"
- Check: Architecture, Features, Testing status

### 2. Click "Open API Test Dashboard"
- URL: `/api-test`
- Click buttons to test:
  - ✅ Health endpoint
  - ✅ Database connection
  - ✅ Songs API (will be empty until data imported)

### 3. Test API Endpoints Directly
Open in new tabs:
- `https://localhost:7005/api/health` - API status
- `https://localhost:7005/api/health/database` - MongoDB status
- `https://localhost:7005/api/songs` - Songs list (empty for now)

---

## ⚙️ Configuration Check

### Verify MongoDB Connection

1. **Open:** `src/OldandNewClone.Web/appsettings.Development.json`

2. **Check:** Your connection string is configured:
   ```json
   {
     "MongoDbSettings": {
       "ConnectionString": "mongodb+srv://genericuser:Swar%40123@cluster0.ovya99h.mongodb.net/OldNewSongs?retryWrites=true&w=majority&appName=Cluster0",
       "DatabaseName": "OldNewSongs"
     }
   }
   ```

3. **Test:** Click "Test /api/health/database" in the API Test page
   - ✅ Should show: `"status": "Connected"`
   - ❌ If error: Check MongoDB Atlas IP whitelist or connection string

---

## 📱 URLs to Bookmark

| Page | URL | Description |
|------|-----|-------------|
| **Home** | `https://localhost:7005` | Landing page / dashboard |
| **API Test** | `https://localhost:7005/api-test` | Interactive API tester |
| **Health** | `https://localhost:7005/api/health` | API health check |
| **Database** | `https://localhost:7005/api/health/database` | MongoDB status |
| **Songs API** | `https://localhost:7005/api/songs` | Songs endpoint |

---

## 🔧 Troubleshooting

### "Connection refused" or "Cannot connect to MongoDB"
**Solution:**
1. Check MongoDB Atlas IP whitelist (add your IP or `0.0.0.0/0` for testing)
2. Verify connection string in `appsettings.Development.json`
3. Test connection with MongoDB Compass

### "The project doesn't run"
**Solution:**
```powershell
# Clean and rebuild
dotnet clean
dotnet build
dotnet run --project src/OldandNewClone.Web
```

### "Browser doesn't open automatically"
**Solution:**
- Manually open: `https://localhost:7005`
- Check `src/OldandNewClone.Web/Properties/launchSettings.json`
- Verify `"launchBrowser": true`

### "Port already in use"
**Solution:**
1. Change port in `launchSettings.json`:
   ```json
   "applicationUrl": "https://localhost:8001;http://localhost:8000"
   ```
2. Or kill existing process:
   ```powershell
   Get-Process -Name "OldandNewClone.Web" | Stop-Process
   ```

---

## 🎨 Auto-Open Browser Configuration

The browser should auto-open because of this configuration:

**File:** `src/OldandNewClone.Web/Properties/launchSettings.json`
```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "launchBrowser": true,  // ← Auto-opens browser
      "applicationUrl": "https://localhost:7005;http://localhost:5265"
    }
  }
}
```

If it doesn't auto-open, check this file and ensure `"launchBrowser": true`.

---

## 📊 Expected Console Output

When you run the app, you should see:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7005
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5265
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

Then your browser should automatically open to `https://localhost:7005`.

---

## 🎯 Next Actions After Starting

1. ✅ **Verify Home Page Loads** - See status dashboard
2. ✅ **Click "API Test Dashboard"** - Test all endpoints
3. ✅ **Test MongoDB Connection** - Click "Test /api/health/database"
4. 📋 **Import Songs Data** - Run mongoimport (see docs/runbooks.md)
5. 📋 **Start Building UI** - Create song list/preview components

---

## 💡 Quick Tips

- **Hot Reload:** Use `dotnet watch run` for auto-rebuild on file changes
- **Stop Server:** Press `Ctrl+C` in terminal or click Stop in Visual Studio
- **View Logs:** Check the Output window in Visual Studio
- **Debug:** Set breakpoints and press F5 to debug

---

## ✅ Success Checklist

After starting the project, you should be able to:
- [ ] Browser auto-opens to `https://localhost:7005`
- [ ] See "✅ Project Status: Running!" on home page
- [ ] Click "Open API Test Dashboard" button
- [ ] Test `/api/health` endpoint (returns JSON)
- [ ] Test `/api/health/database` endpoint (shows MongoDB connection)
- [ ] Test `/api/songs` endpoint (returns `[]` until data imported)

---

**🎉 If all checks pass, your project is running perfectly!**

**Need Help?** Check `docs/runbooks.md` for more commands and troubleshooting.
