#!/bin/bash

# ?? Slingcessories CI/CD Local Setup Script
# This script helps you run CI/CD checks locally before pushing

set -e  # Exit on error

echo "?? Slingcessories - Local CI/CD Check"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}? .NET SDK not found. Please install .NET 8.0 or higher.${NC}"
    exit 1
fi

echo -e "${GREEN}? .NET SDK found: $(dotnet --version)${NC}"
echo ""

# Step 1: Clean
echo "?? Step 1/6: Cleaning solution..."
dotnet clean > /dev/null 2>&1
echo -e "${GREEN}? Clean complete${NC}"
echo ""

# Step 2: Restore
echo "?? Step 2/6: Restoring NuGet packages..."
if dotnet restore; then
    echo -e "${GREEN}? Restore complete${NC}"
else
    echo -e "${RED}? Restore failed${NC}"
    exit 1
fi
echo ""

# Step 3: Build
echo "?? Step 3/6: Building solution..."
if dotnet build --configuration Release --no-restore; then
    echo -e "${GREEN}? Build successful${NC}"
else
    echo -e "${RED}? Build failed${NC}"
    exit 1
fi
echo ""

# Step 4: Run Tests
echo "?? Step 4/6: Running unit tests..."
if dotnet test --configuration Release --no-build --verbosity normal \
    --logger "trx;LogFileName=test-results.trx" \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults; then
    echo -e "${GREEN}? All tests passed${NC}"
else
    echo -e "${RED}? Tests failed${NC}"
    exit 1
fi
echo ""

# Step 5: Code Coverage Report
echo "?? Step 5/6: Generating code coverage report..."
if command -v reportgenerator &> /dev/null; then
    reportgenerator \
        -reports:"./TestResults/**/coverage.cobertura.xml" \
        -targetdir:"./coverage-report" \
        -reporttypes:"Html;TextSummary" > /dev/null 2>&1
    
    if [ -f "./coverage-report/Summary.txt" ]; then
        echo -e "${GREEN}? Coverage report generated${NC}"
        echo ""
        echo "?? Coverage Summary:"
        cat ./coverage-report/Summary.txt | grep "Line coverage:"
        echo ""
        echo "?? Full report: ./coverage-report/index.html"
    fi
else
    echo -e "${YELLOW}??  ReportGenerator not installed. Skipping coverage report.${NC}"
    echo -e "   Install with: dotnet tool install -g dotnet-reportgenerator-globaltool"
fi
echo ""

# Step 6: Check for Security Vulnerabilities
echo "?? Step 6/6: Checking for security vulnerabilities..."
if dotnet list package --vulnerable --include-transitive 2>&1 | grep -q "has the following vulnerable packages"; then
    echo -e "${RED}? Vulnerable packages detected!${NC}"
    dotnet list package --vulnerable --include-transitive
    exit 1
else
    echo -e "${GREEN}? No vulnerable packages detected${NC}"
fi
echo ""

# Summary
echo "======================================"
echo -e "${GREEN}?? All checks passed!${NC}"
echo "======================================"
echo ""
echo "Your code is ready to push! ??"
echo ""
echo "Next steps:"
echo "  1. git add ."
echo "  2. git commit -m 'Your message'"
echo "  3. git push"
echo ""
