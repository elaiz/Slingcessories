# ? Quick Start - CI/CD Setup

## ?? 5-Minute Setup

### **Step 1: Enable GitHub Actions** (30 seconds)

1. Go to your GitHub repository
2. Click **Settings** tab
3. Click **Actions** ? **General**
4. Select **"Allow all actions and reusable workflows"**
5. Click **Save**

? **Done!** GitHub Actions is now enabled.

---

### **Step 2: Push CI/CD Files** (1 minute)

```bash
# Make sure you're on your feature branch
git status

# Add all new CI/CD files
git add .github/ scripts/ *.md

# Commit
git commit -m "feat: Add CI/CD with automated testing"

# Push
git push origin dev/Offline-Mode-Blazor
```

? **Done!** Workflows are now live.

---

### **Step 3: Watch First Run** (5 minutes)

1. Go to **Actions** tab in GitHub
2. You'll see **"Build and Test"** workflow running
3. Click on it to watch progress
4. Wait for it to complete

? **Done!** First CI run complete.

---

### **Step 4: Setup Branch Protection** (2 minutes)

1. Go to **Settings** ? **Branches**
2. Click **Add rule**
3. Branch name pattern: `main`
4. Enable:
   - ? **Require pull request reviews** (1 approval)
   - ? **Require status checks to pass**
   - Select: `Build & Test on ubuntu-latest`
5. Click **Create**

? **Done!** Main branch is now protected.

---

### **Step 5: Try Local CI Check** (1 minute)

**Windows:**
```powershell
.\scripts\ci-check.ps1
```

**Linux/macOS:**
```bash
chmod +x scripts/ci-check.sh
./scripts/ci-check.sh
```

? **Done!** You can now run CI checks locally.

---

## ?? You're Ready!

From now on:
1. **Before pushing:** Run `ci-check` script
2. **Create PRs:** Use the template (auto-filled)
3. **Wait for checks:** GitHub Actions runs automatically
4. **Merge:** Only after tests pass and review approved

---

## ?? Troubleshooting

### **Workflow not appearing?**
- Check `.github/workflows/` files are committed
- Verify GitHub Actions is enabled
- Try pushing again

### **Tests failing in CI but pass locally?**
- Check .NET version matches (`dotnet --version`)
- Clear cache: `dotnet nuget locals all --clear`
- Rebuild: `dotnet clean && dotnet build`

### **Can't run local script?**
**Windows:**
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

**Linux/macOS:**
```bash
chmod +x scripts/ci-check.sh
```

---

## ?? Full Documentation

For detailed information, see:
- **CI_CD_COMPLETE.md** - Overview and summary
- **CI_CD_SETUP.md** - Complete guide with examples
- **scripts/README.md** - Script documentation

---

## ? Checklist

- [ ] GitHub Actions enabled
- [ ] Workflows pushed to repository
- [ ] First workflow run successful
- [ ] Branch protection configured
- [ ] Local script tested
- [ ] Status badges added to README (optional)
- [ ] Team notified about new CI/CD

---

**Time to complete:** ~10 minutes
**Difficulty:** ? Easy

**You've got this!** ??
