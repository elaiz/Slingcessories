# ?? Slingcessories CI/CD Local Setup Script (PowerShell)
# This script helps you run CI/CD checks locally before pushing

$ErrorActionPreference = "Stop"

Write-Host "?? Slingcessories - Local CI/CD Check" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "? .NET SDK not found. Please install .NET 8.0 or higher." -ForegroundColor Red
    exit 1
}

$dotnetVersion = dotnet --version
Write-Host "? .NET SDK found: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Step 1: Clean
Write-Host "?? Step 1/6: Cleaning solution..." -ForegroundColor Yellow
dotnet clean | Out-Null
Write-Host "? Clean complete" -ForegroundColor Green
Write-Host ""

# Step 2: Restore
Write-Host "?? Step 2/6: Restoring NuGet packages..." -ForegroundColor Yellow
try {
    dotnet restore
    Write-Host "? Restore complete" -ForegroundColor Green
} catch {
    Write-Host "? Restore failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Build
Write-Host "?? Step 3/6: Building solution..." -ForegroundColor Yellow
try {
    dotnet build --configuration Release --no-restore
    Write-Host "? Build successful" -ForegroundColor Green
} catch {
    Write-Host "? Build failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 4: Run Tests
Write-Host "?? Step 4/6: Running unit tests..." -ForegroundColor Yellow
try {
    dotnet test --configuration Release --no-build --verbosity normal `
        --logger "trx;LogFileName=test-results.trx" `
        --collect:"XPlat Code Coverage" `
        --results-directory ./TestResults
    Write-Host "? All tests passed" -ForegroundColor Green
} catch {
    Write-Host "? Tests failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 5: Code Coverage Report
Write-Host "?? Step 5/6: Generating code coverage report..." -ForegroundColor Yellow
if (Get-Command reportgenerator -ErrorAction SilentlyContinue) {
    try {
        reportgenerator `
            -reports:"./TestResults/**/coverage.cobertura.xml" `
            -targetdir:"./coverage-report" `
            -reporttypes:"Html;TextSummary" | Out-Null
        
        if (Test-Path "./coverage-report/Summary.txt") {
            Write-Host "? Coverage report generated" -ForegroundColor Green
            Write-Host ""
            Write-Host "?? Coverage Summary:" -ForegroundColor Cyan
            Get-Content "./coverage-report/Summary.txt" | Select-String "Line coverage:"
            Write-Host ""
            Write-Host "?? Full report: ./coverage-report/index.html" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "??  Could not generate coverage report" -ForegroundColor Yellow
    }
} else {
    Write-Host "??  ReportGenerator not installed. Skipping coverage report." -ForegroundColor Yellow
    Write-Host "   Install with: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Yellow
}
Write-Host ""

# Step 6: Check for Security Vulnerabilities
Write-Host "?? Step 6/6: Checking for security vulnerabilities..." -ForegroundColor Yellow
$vulnerableOutput = dotnet list package --vulnerable --include-transitive 2>&1 | Out-String
if ($vulnerableOutput -match "has the following vulnerable packages") {
    Write-Host "? Vulnerable packages detected!" -ForegroundColor Red
    Write-Host $vulnerableOutput
    exit 1
} else {
    Write-Host "? No vulnerable packages detected" -ForegroundColor Green
}
Write-Host ""

# Summary
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "?? All checks passed!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your code is ready to push! ??" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. git add ."
Write-Host "  2. git commit -m 'Your message'"
Write-Host "  3. git push"
Write-Host ""
