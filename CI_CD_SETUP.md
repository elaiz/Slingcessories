# ?? CI/CD Setup Guide

## ?? Overview

This project uses **GitHub Actions** for continuous integration and deployment. All workflows are automated and triggered on specific events.

---

## ?? Workflows

### **1. Build and Test** (`build-and-test.yml`)

**Triggers:**
- Push to `main`, `dev/*`, or `feature/*` branches
- Pull requests to `main` or `dev/*`
- Manual dispatch

**What It Does:**
- ? Builds the solution on **Windows**, **Linux**, and **macOS**
- ? Runs all **240+ unit tests**
- ? Generates **code coverage** reports
- ? Publishes **test results** as artifacts
- ? Builds **Blazor WebAssembly** app
- ? Builds **API Service**

**Duration:** ~5-8 minutes

---

### **2. Code Quality** (`code-quality.yml`)

**Triggers:**
- Push to `main` or `dev/*`
- Pull requests

**What It Does:**
- ? Runs **code analysis** and formatting checks
- ? Checks for **security vulnerabilities** in packages
- ? Generates **test coverage report** with badges
- ? Reviews **dependencies** for security issues
- ? Comments **coverage on PRs**

**Duration:** ~3-5 minutes

---

### **3. Deploy** (`deploy.yml`)

**Triggers:**
- Push to `main` branch
- Version tags (`v*.*.*`)
- Manual dispatch with environment selection

**What It Does:**
- ? Runs tests before deployment
- ? Builds production artifacts
- ? Deploys **Blazor app** to hosting
- ? Deploys **API service** to hosting
- ? Creates deployment summary

**Duration:** ~10-15 minutes

---

## ?? Workflow Status Badges

Add these to your `README.md`:

```markdown
![Build Status](https://github.com/elaiz/Slingcessories/workflows/Build%20and%20Test/badge.svg)
![Code Quality](https://github.com/elaiz/Slingcessories/workflows/Code%20Quality/badge.svg)
![Deploy](https://github.com/elaiz/Slingcessories/workflows/Deploy%20to%20Production/badge.svg)
```

---

## ?? Branch Protection Rules

### **Recommended Settings for `main` branch:**

1. **Go to:** Repository ? Settings ? Branches ? Add rule

2. **Branch name pattern:** `main`

3. **Enable the following:**
   - ? **Require a pull request before merging**
     - Require approvals: **1**
     - Dismiss stale pull request approvals when new commits are pushed
   
   - ? **Require status checks to pass before merging**
     - Status checks required:
       - ? `build-and-test / Build & Test on ubuntu-latest`
       - ? `build-and-test / Build & Test on windows-latest`
       - ? `code-quality / Code Analysis`
       - ? `code-quality / Test Coverage Report`
   
   - ? **Require conversation resolution before merging**
   
   - ? **Require signed commits** (optional but recommended)
   
   - ? **Include administrators**

4. **Save changes**

---

## ?? Artifacts

### **Test Results**
All test runs produce artifacts available for **7 days**:
- `test-results-ubuntu-latest`
- `test-results-windows-latest`
- `test-results-macos-latest`

### **Build Artifacts**
Production builds produce:
- `blazor-app` - Blazor WebAssembly output
- `api-service` - API service binaries
- `coverage-report` - Code coverage HTML report

---

## ?? Running Tests Locally

### **All Tests:**
```bash
dotnet test
```

### **With Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### **Generate Coverage Report:**
```bash
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./coverage-report" -reporttypes:Html
```

### **Blazor Tests Only:**
```bash
dotnet test --filter "FullyQualifiedName~Blazor"
```

---

## ?? Configuration

### **Environment Variables**

Set these in GitHub Settings ? Secrets and variables ? Actions:

#### **Required Secrets:**
- `AZURE_CREDENTIALS` - Azure service principal (if using Azure)
- `DEPLOY_TOKEN` - Deployment token for your hosting

#### **Optional Variables:**
- `DOTNET_VERSION` - .NET version (default: `8.0.x`)
- `BUILD_CONFIGURATION` - Build config (default: `Release`)

---

## ?? Deployment

### **Automatic Deployment**

**To Production:**
1. Merge PR to `main` branch
2. Workflow automatically triggers
3. Tests run and must pass
4. Artifacts are built
5. Deployment executes

**To Staging:**
1. Go to Actions ? Deploy to Production
2. Click "Run workflow"
3. Select "staging" environment
4. Confirm and run

### **Manual Deployment**

1. Go to **Actions** tab
2. Select **Deploy to Production** workflow
3. Click **Run workflow**
4. Select:
   - **Branch:** `main`
   - **Environment:** `staging` or `production`
5. Click **Run workflow**

---

## ?? Test Coverage

### **Current Coverage:**
- **Service Layer:** ~95%
- **Blazor Services:** ~100% (240 tests)
- **Controllers:** ~85%
- **Overall:** ~90%

### **Viewing Coverage:**
1. Go to **Actions** ? Latest workflow run
2. Download `coverage-report` artifact
3. Extract and open `index.html`

Or use **Codecov** dashboard (if configured):
- https://codecov.io/gh/elaiz/Slingcessories

---

## ?? Troubleshooting

### **Tests Failing Locally But Pass in CI:**
- Check .NET SDK version: `dotnet --version`
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Rebuild: `dotnet clean && dotnet build`

### **CI Workflow Not Triggering:**
- Check branch protection rules
- Verify workflow file syntax (YAML)
- Check GitHub Actions is enabled

### **Deployment Failures:**
- Check Azure/hosting credentials
- Verify environment secrets are set
- Review deployment logs in Actions tab

---

## ?? Monitoring

### **GitHub Actions Dashboard:**
- View all workflow runs: Repository ? Actions
- Check status badges on README
- Review detailed logs for failures

### **Metrics to Track:**
- ? Test pass rate (should be 100%)
- ? Build time (target: < 10 minutes)
- ? Code coverage (target: > 80%)
- ? Deployment frequency
- ? Mean time to recovery (MTTR)

---

## ?? Continuous Improvement

### **Weekly Review:**
1. Check for **flaky tests** (inconsistent failures)
2. Review **test execution time**
3. Update **dependencies** regularly
4. Monitor **code coverage trends**

### **Monthly Tasks:**
1. Update GitHub Actions versions
2. Review and update branch protection rules
3. Audit security vulnerabilities
4. Optimize workflow performance

---

## ?? Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)
- [Code Coverage with Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Azure DevOps Alternative](https://azure.microsoft.com/en-us/services/devops/)

---

## ?? Contributing

When contributing to this project:

1. ? Create a feature branch from `main`
2. ? Write tests for new functionality
3. ? Ensure all tests pass locally
4. ? Create a pull request with detailed description
5. ? Wait for CI checks to complete
6. ? Address any review comments
7. ? Merge after approval

---

## ?? Support

For CI/CD issues:
- Open an issue with the `ci/cd` label
- Contact DevOps team
- Check GitHub Actions status page

---

**Last Updated:** January 2026
