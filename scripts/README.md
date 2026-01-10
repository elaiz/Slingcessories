# ??? CI/CD Scripts

This directory contains scripts to help with continuous integration and deployment.

## ?? Available Scripts

### **1. ci-check.sh** (Linux/macOS)
Bash script that runs all CI checks locally before pushing code.

**Usage:**
```bash
chmod +x scripts/ci-check.sh
./scripts/ci-check.sh
```

### **2. ci-check.ps1** (Windows)
PowerShell script that runs all CI checks locally before pushing code.

**Usage:**
```powershell
.\scripts\ci-check.ps1
```

---

## ? What These Scripts Do

1. **Clean** - Removes build artifacts
2. **Restore** - Downloads NuGet packages
3. **Build** - Compiles the solution in Release mode
4. **Test** - Runs all 240+ unit tests
5. **Coverage** - Generates code coverage report
6. **Security** - Checks for vulnerable dependencies

---

## ?? When to Run

### **Before Every Push:**
```bash
# Run the script
./scripts/ci-check.sh  # or ci-check.ps1 on Windows

# If all checks pass:
git add .
git commit -m "Your message"
git push
```

### **Before Creating a PR:**
Run the script to ensure your code will pass CI checks.

### **After Pulling Latest Changes:**
Verify nothing is broken after merging upstream changes.

---

## ?? Understanding the Output

### **? Success (Green):**
All checks passed. Your code is ready to push!

### **? Failure (Red):**
One or more checks failed. Fix the issues before pushing.

### **?? Warning (Yellow):**
Non-critical issues detected. Review and decide if action is needed.

---

## ?? Prerequisites

### **Required:**
- .NET 8.0 SDK or higher
- Git

### **Optional (for full coverage reporting):**
```bash
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool
```

---

## ?? Troubleshooting

### **"Permission denied" (Linux/macOS):**
```bash
chmod +x scripts/ci-check.sh
```

### **"Execution policy" error (Windows):**
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

### **Script fails but tests pass in IDE:**
1. Clear NuGet cache: `dotnet nuget locals all --clear`
2. Clean and rebuild: `dotnet clean && dotnet restore && dotnet build`
3. Close and reopen your IDE

---

## ?? Code Coverage

After running the script, view the coverage report:

**Linux/macOS:**
```bash
open coverage-report/index.html
```

**Windows:**
```powershell
start coverage-report/index.html
```

---

## ?? Integration with Git Hooks

### **Option 1: Pre-commit Hook** (Run on every commit)

Create `.git/hooks/pre-commit`:

```bash
#!/bin/bash
./scripts/ci-check.sh
```

Make it executable:
```bash
chmod +x .git/hooks/pre-commit
```

### **Option 2: Pre-push Hook** (Run before pushing)

Create `.git/hooks/pre-push`:

```bash
#!/bin/bash
./scripts/ci-check.sh
```

Make it executable:
```bash
chmod +x .git/hooks/pre-push
```

---

## ?? Tips

### **Speed Up Checks:**
If you only want to run tests without coverage:
```bash
dotnet test --no-build --verbosity minimal
```

### **Run Specific Tests:**
```bash
# Blazor tests only
dotnet test --filter "FullyQualifiedName~Blazor"

# Specific test class
dotnet test --filter "FullyQualifiedName~OfflineDataServiceTests"
```

### **Watch Mode (for TDD):**
```bash
dotnet watch test
```

---

## ?? Additional Resources

- [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Git Hooks Documentation](https://git-scm.com/book/en/v2/Customizing-Git-Git-Hooks)

---

## ?? Contributing

When adding new scripts:
1. Test on all platforms (Windows, Linux, macOS)
2. Add error handling
3. Provide clear output messages
4. Update this README

---

**Last Updated:** January 2026
