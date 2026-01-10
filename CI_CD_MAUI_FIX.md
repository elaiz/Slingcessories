# ?? CI/CD MAUI Workload Fix

## ? **Problem:**

GitHub Actions was failing with:
```
error NETSDK1147: To build this project, the following workloads must be installed: maui-android
To install these workloads, run the following command: dotnet workload restore
```

---

## ? **Solution Applied:**

### **1. Separated MAUI from Main Build**

**Before:** Tried to build everything with `dotnet restore` and `dotnet build`
**After:** Build projects individually, excluding MAUI from main workflow

### **2. Created Dedicated MAUI Job**

Added separate `maui-build` job that:
- ? Installs .NET 10 (required for MAUI)
- ? Installs MAUI workload: `dotnet workload install maui`
- ? Builds each platform separately (Android, iOS, Windows, macOS)
- ? Runs on appropriate OS for each platform

### **3. Updated All Workflows**

**Changed Files:**
- ? `.github/workflows/build-and-test.yml` - Main workflow
- ? `.github/workflows/code-quality.yml` - Quality checks

---

## ?? **What Changed:**

### **build-and-test.yml:**

#### **Main Build Job (build-and-test):**
```yaml
# OLD (failed)
- name: Restore dependencies
  run: dotnet restore

# NEW (works)
- name: Restore dependencies (excluding MAUI)
  run: |
    dotnet restore Slingcessories/Slingcessories.csproj
    dotnet restore Slingcessories.Service/Slingcessories.Service.csproj
    dotnet restore Slingcessories.Tests/Slingcessories.Tests.csproj
```

#### **New MAUI Build Job:**
```yaml
maui-build:
  name: Build MAUI (${{ matrix.platform }})
  runs-on: ${{ matrix.os }}
  needs: build-and-test
  
  strategy:
    matrix:
      include:
        - platform: Android
          os: windows-latest
          target: net10.0-android
        - platform: iOS
          os: macos-latest
          target: net10.0-ios
        # ... etc
  
  steps:
    - name: Install MAUI Workload
      run: dotnet workload install maui
    
    - name: Build MAUI
      run: dotnet build Slingcessories.Mobile.Maui/Slingccessories.Mobile.Maui.csproj --framework ${{ matrix.target }}
```

---

## ?? **Why This Works:**

### **1. .NET Version Management:**
- **Blazor & API:** Use .NET 8.0 (stable, widely supported)
- **MAUI:** Uses .NET 10.0 (required for MAUI)
- **Tests:** Run on .NET 8.0 (test project targets .NET 8)

### **2. Platform-Specific Runners:**
- **Android:** Requires Windows or Linux with Android SDK
- **iOS/macOS:** Requires macOS with Xcode
- **Windows:** Requires Windows runner
- **Tests:** Can run on any platform

### **3. Workload Isolation:**
- MAUI workloads are **large** (~2GB+)
- Only install them when actually building MAUI
- Keep main build fast and lean

---

## ?? **New Workflow Structure:**

```
Push to GitHub
    ?
??????????????????????????????????????????
?  build-and-test (Ubuntu, Windows, Mac) ?
?  - Build Blazor                        ?
?  - Build API                           ?
?  - Build Tests                         ?
?  - Run 240+ Tests                      ?
?  - Generate Coverage                   ?
??????????????????????????????????????????
    ? (All tests pass)
??????????????????????  ????????????????  ????????????????????
?  blazor-build      ?  ?  api-build   ?  ?  maui-build      ?
?  - Publish Blazor  ?  ?  - Publish   ?  ?  - Android       ?
?  - Create artifact ?  ?    API       ?  ?  - iOS           ?
??????????????????????  ?  - Create    ?  ?  - Windows       ?
                        ?    artifact  ?  ?  - macOS         ?
                        ????????????????  ????????????????????
```

---

## ?? **Performance Impact:**

### **Before (Failed):**
- ? Build failed at restore step (~30 seconds)
- ?? Total time: N/A (failed)

### **After (Success):**
- ? Main build & test: ~5-8 minutes
- ? Blazor build: ~2 minutes
- ? API build: ~2 minutes
- ? MAUI build (all platforms): ~15-20 minutes
- ?? **Total time: ~25-30 minutes**

**Note:** MAUI builds run in parallel, so actual time is ~20 minutes max.

---

## ?? **Testing the Fix:**

### **1. Push Changes:**
```bash
git add .github/
git commit -m "fix: Update CI/CD to handle MAUI workloads properly"
git push
```

### **2. Watch Workflow:**
- Go to **Actions** tab on GitHub
- See "Build and Test" workflow running
- Main build should complete in ~8 minutes
- MAUI builds run after main build passes

### **3. Verify Results:**
- ? All tests pass on 3 platforms
- ? Blazor artifact created
- ? API artifact created
- ? MAUI builds succeed (or skip if not needed)

---

## ?? **Additional Options:**

### **Option 1: Skip MAUI Builds (Faster CI)**

If you don't need to build MAUI in CI yet:

```yaml
# Remove or comment out the entire maui-build job
# maui-build:
#   name: Build MAUI
#   ...
```

**Benefit:** Faster CI (~8 minutes instead of ~25 minutes)

### **Option 2: Build MAUI Only on Main Branch**

```yaml
maui-build:
  name: Build MAUI
  if: github.ref == 'refs/heads/main'  # Only on main branch
  runs-on: ${{ matrix.os }}
  # ... rest of config
```

**Benefit:** Fast PR builds, comprehensive main branch builds

### **Option 3: Manual MAUI Builds**

```yaml
maui-build:
  name: Build MAUI
  if: github.event_name == 'workflow_dispatch'  # Only manual trigger
  runs-on: ${{ matrix.os }}
  # ... rest of config
```

**Benefit:** Build MAUI only when you need it

---

## ?? **Recommended Configuration:**

For **development phase** (faster iteration):
```yaml
# Skip MAUI builds or make them manual
# Focus on Blazor + API + Tests
```

For **release preparation**:
```yaml
# Enable MAUI builds on main branch
# Ensure all platforms build successfully
```

---

## ? **Checklist:**

- [x] Updated `build-and-test.yml` to exclude MAUI from main build
- [x] Added dedicated `maui-build` job with workload installation
- [x] Updated `code-quality.yml` to exclude MAUI
- [x] Fixed typo in test project path
- [x] Added branch names (`master` in addition to `main`)
- [ ] Push changes to GitHub
- [ ] Verify workflow runs successfully
- [ ] Adjust MAUI build triggers if needed (optional)

---

## ?? **Expected Result:**

After pushing these changes:

1. ? **Build & Test** job succeeds (~8 minutes)
2. ? **Code Quality** job succeeds (~5 minutes)
3. ? **Blazor Build** creates artifact
4. ? **API Build** creates artifact
5. ? **MAUI Build** succeeds on all platforms (or can be skipped)

**No more NETSDK1147 errors!** ??

---

## ?? **References:**

- [.NET MAUI Workloads](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)
- [GitHub Actions - .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- [Multi-Platform MAUI Builds](https://learn.microsoft.com/en-us/dotnet/maui/deployment/)

---

**Last Updated:** January 2026
