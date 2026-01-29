# Setup Guide

This guide will help you set up your development environment to contribute to Wolfgang.Extensions.DateTime.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

1. **.NET SDK**
   - .NET 8.0 SDK or later (includes .NET 10.0 support)
   - Download from: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
   - Verify installation: `dotnet --version`

2. **Git**
   - Git 2.x or later
   - Download from: [https://git-scm.com/](https://git-scm.com/)
   - Verify installation: `git --version`

3. **IDE/Editor** (Choose one)
   - **Visual Studio 2022** (recommended for Windows)
     - Community, Professional, or Enterprise edition
     - Include ".NET desktop development" workload
   - **Visual Studio Code**
     - Install C# extension
     - Install .NET Core Extension Pack
   - **JetBrains Rider**
     - Full-featured .NET IDE
   - **Visual Studio for Mac**
     - For macOS development

### Optional Tools

1. **DocFX** (for documentation generation)
   - Install: `dotnet tool install -g docfx`
   - Verify: `docfx --version`

2. **ReportGenerator** (for code coverage reports)
   - Install: `dotnet tool install -g dotnet-reportgenerator-globaltool`
   - Verify: `reportgenerator -version`

3. **DevSkim CLI** (for security scanning)
   - Install: `dotnet tool install --global Microsoft.CST.DevSkim.CLI`
   - Verify: `devskim --version`

## Getting the Source Code

### Clone the Repository

```bash
# Using HTTPS
git clone https://github.com/Chris-Wolfgang/DateTime-Extensions.git

# Or using SSH (if you have SSH keys configured)
git clone git@github.com:Chris-Wolfgang/DateTime-Extensions.git

# Navigate to the repository
cd DateTime-Extensions
```

### Repository Structure

```
DateTime-Extensions/
├── src/                              # Source code
│   └── Wolfgang.Extensions.DateTime/ # Main library project
├── tests/                            # Test projects
│   └── Wolfgang.Extensions.DateTime.Tests.Unit/
├── benchmarks/                       # Performance benchmarks (if any)
├── examples/                         # Example projects
├── docs/                             # Additional documentation
├── docfx_project/                    # DocFX documentation
│   ├── docs/                         # Documentation articles
│   └── api/                          # Generated API documentation
├── .github/                          # GitHub workflows and templates
├── DateTime Extensions.slnx          # Solution file
└── README.md                         # Main readme
```

## Building the Project

### Restore Dependencies

Before building, restore all NuGet packages:

```bash
dotnet restore
```

### Build the Solution

Build the entire solution:

```bash
# Debug build
dotnet build

# Release build
dotnet build --configuration Release
```

### Build Specific Project

Build only the main library:

```bash
cd src/Wolfgang.Extensions.DateTime
dotnet build
```

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests with Code Coverage

```bash
# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage" --results-directory "./TestResults"

# Generate HTML coverage report
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" \
                -targetdir:"CoverageReport" \
                -reporttypes:"Html;TextSummary"

# View the report (open in browser)
# On Windows: start CoverageReport/index.html
# On macOS: open CoverageReport/index.html
# On Linux: xdg-open CoverageReport/index.html
```

### Run Specific Tests

```bash
# Run tests from a specific test class
dotnet test --filter ClassName=DateTimeExtensionsTests

# Run a specific test method
dotnet test --filter MethodName=TruncateMilliseconds_RemovesMilliseconds
```

## Code Quality

### Code Style

The project uses `.editorconfig` for consistent code style. Most IDEs will automatically apply these settings.

Key style guidelines:
- Use file-scoped namespaces
- Use explicit types (`int`, `string`) instead of `var` where clarity is important
- Follow C# naming conventions
- Enable nullable reference types

### Running Linters

```bash
# Analyze code for issues
dotnet build /p:TreatWarningsAsErrors=true

# Run code analysis
dotnet build /p:EnableNETAnalyzers=true /p:AnalysisLevel=latest
```

### Security Scanning

Run DevSkim to check for security issues:

```bash
devskim analyze --source-code . -f text --output-file devskim-results.txt
cat devskim-results.txt
```

## Building Documentation

### Generate API Documentation

```bash
cd docfx_project
docfx build

# Serve documentation locally
docfx serve _site
# Open http://localhost:8080 in your browser
```

### Documentation Files

Documentation is written in Markdown and located in:
- `docfx_project/docs/` - User guides and tutorials
- `docfx_project/index.md` - Documentation homepage
- API docs are auto-generated from XML comments

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Changes

- Edit code in `src/Wolfgang.Extensions.DateTime/`
- Add/update tests in `tests/Wolfgang.Extensions.DateTime.Tests/`
- Update documentation if needed

### 3. Test Your Changes

```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Check code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 4. Commit Your Changes

```bash
git add .
git commit -m "Add: Brief description of your changes"
```

Follow conventional commit format:
- `Add:` for new features
- `Fix:` for bug fixes
- `Update:` for modifications to existing features
- `Docs:` for documentation changes
- `Test:` for test-related changes

### 5. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a pull request on GitHub.

## Running the CI Pipeline Locally

The project uses GitHub Actions for CI/CD. To verify your changes will pass CI:

```bash
# Restore dependencies
dotnet restore

# Build in Release configuration
dotnet build --no-restore --configuration Release

# Run all tests
dotnet test --no-build --configuration Release

# Generate coverage report
dotnet test --no-build --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory "./TestResults"

reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:"Html;TextSummary;MarkdownSummaryGithub;CsvSummary"

# Run security scan
devskim analyze --source-code . -f text --output-file devskim-results.txt -E
```

## Troubleshooting

### Common Issues

**Issue**: Build fails with "SDK not found"  
**Solution**: Ensure .NET SDK 8.0 or later is installed. Run `dotnet --list-sdks` to verify.

**Issue**: Tests fail with culture-related errors  
**Solution**: Some tests may be culture-sensitive. The library handles this, but test setup may need culture specification.

**Issue**: Coverage report generation fails  
**Solution**: Ensure `reportgenerator` tool is installed globally: `dotnet tool install -g dotnet-reportgenerator-globaltool`

**Issue**: DocFX build fails  
**Solution**: 
1. Ensure DocFX is installed: `dotnet tool install -g docfx`
2. Check that all referenced files exist
3. Update `docfx.json` project paths if needed

### Getting Help

- Check existing [GitHub Issues](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
- Create a new issue with detailed error information
- Join discussions in [GitHub Discussions](https://github.com/Chris-Wolfgang/DateTime-Extensions/discussions)

## Code Review Guidelines

Before submitting a pull request:

1. ✅ All tests pass
2. ✅ Code coverage meets requirements (aim for >80%)
3. ✅ No security vulnerabilities detected by DevSkim
4. ✅ Code follows project style guidelines
5. ✅ XML documentation comments added for public APIs
6. ✅ Documentation updated if behavior changes
7. ✅ Commit messages follow conventional format

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [C# Coding Conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [DocFX Documentation](https://dotnet.github.io/docfx/)
- [GitHub Flow](https://guides.github.com/introduction/flow/)

---

Happy coding! Thank you for contributing to Wolfgang.Extensions.DateTime! 🎉