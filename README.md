# Wolfgang.Extensions.DateTime

A collection of extension methods for `DateTime` data type in .Net

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.DateTime.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.Extensions.DateTime)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.Extensions.DateTime.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.Extensions.DateTime)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/DateTime-Extensions/pr.yaml?event=pull_request_target&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/DateTime-Extensions/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/DateTime-Extensions/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/DateTime-Extensions/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/DateTime-Extensions)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Extensions.DateTime
```

**NuGet Package:** [Wolfgang.Extensions.DateTime](https://www.nuget.org/packages/Wolfgang.Extensions.DateTime)

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/DateTime-Extensions](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- **API Documentation:** https://Chris-Wolfgang.github.io/DateTime-Extensions/
- **CHANGELOG:** [CHANGELOG.md](CHANGELOG.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Formatting Guide:** [docs/README-FORMATTING.md](docs/README-FORMATTING.md)
- **DocFX Version Picker Troubleshooting:** [docs/DOCFX-VERSION-PICKER.md](docs/DOCFX-VERSION-PICKER.md)
- **Release Workflow Setup:** [docs/RELEASE-WORKFLOW-SETUP.md](docs/RELEASE-WORKFLOW-SETUP.md)
- **Workflow Security:** [docs/WORKFLOW_SECURITY.md](docs/WORKFLOW_SECURITY.md)

---

## 🚀 Quick Start

```csharp
using System;                          // DateTime, DayOfWeek
using Wolfgang.Extensions.DateTime;

var now = DateTime.UtcNow;

now.TruncateMilliseconds();          // strips fractional seconds
now.TruncateSeconds();               // rounds down to the minute
now.FirstOfMonth();                  // → first day of this month, 00:00:00
now.EndOfMonth();                    // → last tick of this month
now.FirstOfYear();                   // → January 1 of this year, 00:00:00
now.EndOfYear();                     // → last tick of this year
now.FirstOfWeek();                   // → first day of the week (current culture)
now.FirstOfWeek(DayOfWeek.Monday);   // → first day of the week (explicit)
now.EndOfWeek();                     // → last tick of the week (current culture)
now.EndOfWeek(DayOfWeek.Monday);     // → last tick of the week (explicit)
now.FirstOfQuarter();                // → first day of this calendar quarter
now.EndOfQuarter();                  // → last tick of this calendar quarter
now.FirstOfHalf();                   // → first day of this calendar half-year
now.EndOfHalf();                     // → last tick of this calendar half-year
```

All methods are pure: they return a new `DateTime` and never mutate the input. The
returned value preserves the original's `DateTimeKind` (Utc / Local / Unspecified)
unless explicitly noted.

---

## ✨ Features

The table below is a snapshot of the public API at the time of writing. For the
authoritative list (kept in sync with source on every release), see the
[API documentation](https://Chris-Wolfgang.github.io/DateTime-Extensions/api/Wolfgang.Extensions.DateTime.DateTimeExtensions.html).

### Methods
| Method | Description |
|--------|-------------|
| `TruncateMilliseconds` | Removes fractional seconds, returning a DateTime accurate to the whole second. |
| `TruncateSeconds`| Strips seconds and smaller units, yielding a DateTime rounded down to the minute. |
| `FirstOfMonth`| Produces a new `DateTime` set to the first day of the month at midnight of the specified `DateTime`. |
| `EndOfMonth`| Computes the final tick of the month for the specified `DateTime`. |
| `FirstOfYear`| Creates a `DateTime` corresponding to January 1 of the same year as the specified `DateTime`. |
| `EndOfYear`| Returns the final tick of the year of the specified `DateTime`. |
| `FirstOfWeek()`| Uses the current culture’s first day of the week to locate the week’s starting `DateTime`. |
| `FirstOfWeek(DayOfWeek firstDayOfWeek)`| Uses the specified first day of the week to locate the week’s starting `DateTime`. |
| `EndOfWeek()`| Uses the current culture’s first day, calculates the final tick of the week. |
| `EndOfWeek(DayOfWeek firstDayOfWeek)`| Uses the specified first day, calculates the final tick of the week. |
| `FirstOfQuarter`| Returns the first day of the calendar quarter (Q1 Jan-Mar, Q2 Apr-Jun, Q3 Jul-Sep, Q4 Oct-Dec) at 00:00. |
| `EndOfQuarter`| Returns the final tick of the calendar quarter. Clamps at `DateTime.MaxValue` for Q4 of year 9999. |
| `FirstOfHalf`| Returns the first day of the calendar half-year (H1 Jan-Jun, H2 Jul-Dec) at 00:00. |
| `EndOfHalf`| Returns the final tick of the calendar half-year. Clamps at `DateTime.MaxValue` for H2 of year 9999. |



### Examples

```csharp
using System;                          // DateTime, DateTimeKind, DayOfWeek
using Wolfgang.Extensions.DateTime;

var ts = new DateTime(2026, 5, 26, 13, 45, 30, 123, DateTimeKind.Utc);

ts.TruncateMilliseconds();    // 2026-05-26 13:45:30.000 Utc
ts.TruncateSeconds();         // 2026-05-26 13:45:00.000 Utc
ts.FirstOfMonth();            // 2026-05-01 00:00:00.000 Utc
ts.EndOfMonth();              // 2026-05-31 23:59:59.9999999 Utc
ts.FirstOfYear();             // 2026-01-01 00:00:00.000 Utc
ts.EndOfYear();               // 2026-12-31 23:59:59.9999999 Utc

ts.FirstOfWeek(DayOfWeek.Sunday); // 2026-05-24 00:00:00.000 Utc
ts.EndOfWeek(DayOfWeek.Sunday);   // 2026-05-30 23:59:59.9999999 Utc

ts.FirstOfQuarter();              // 2026-04-01 00:00:00.000 Utc  (Q2: Apr-Jun)
ts.EndOfQuarter();                // 2026-06-30 23:59:59.9999999 Utc
ts.FirstOfHalf();                 // 2026-01-01 00:00:00.000 Utc  (H1: Jan-Jun)
ts.EndOfHalf();                   // 2026-06-30 23:59:59.9999999 Utc
```

Boundary safety: `EndOfMonth` / `EndOfYear` / `EndOfWeek` /
`EndOfQuarter` / `EndOfHalf` all clamp at `DateTime.MaxValue` instead
of throwing; `FirstOfWeek` clamps at `DateTime.MinValue`. See
[CHANGELOG.md](CHANGELOG.md) v1.2.0 for the month / year / week
boundary fixes and v1.3.0 for the quarter / half clamping added
with the new methods.

---

## 🎯 Supported Frameworks

This library targets:

- **.NET Framework:** 4.6.2

See the [NuGet package page](https://www.nuget.org/packages/Wolfgang.Extensions.DateTime/) for the authoritative per-TFM compatibility matrix.

## 🔍 Code Quality & Static Analysis

This project enforces **strict code quality standards** through **8 specialized analyzers**, an `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` Release gate, and custom async-first rules:

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** — Built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** — Advanced refactoring and code quality rules
3. **AsyncFixer** — Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** — Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — Prevents usage of banned synchronous APIs (see `BannedSymbols.txt`)
6. **Meziantou.Analyzer** — Comprehensive code quality rules
7. **SonarAnalyzer.CSharp** — Industry-standard code analysis
8. **Microsoft.CodeAnalysis.PublicApiAnalyzers** — Tracks the public API surface via `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`; surfaces additions/removals at compile time as a breaking-change review gate


---

## 🛠️ Building from Source

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) — required for the modern build / test / pack flow used by CI
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for formatting scripts

### Build Steps

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/DateTime-Extensions.git
cd DateTime-Extensions

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run code formatting (PowerShell Core)
pwsh ./format.ps1
```

### Code Formatting

This project uses `.editorconfig` and `dotnet format`:

```bash
# Format code
dotnet format

# Verify formatting (as CI does)
dotnet format --verify-no-changes
```

See [docs/README-FORMATTING.md](docs/README-FORMATTING.md) for detailed formatting guidelines.

### Building Documentation

This project uses [DocFX](https://dotnet.github.io/docfx/) to generate API documentation:

```bash
# Install DocFX (one-time setup)
dotnet tool install -g docfx

# Generate API metadata and build documentation
cd docfx_project
docfx metadata  # Extract API metadata from source code
docfx build     # Build HTML documentation

# Documentation is generated in the docs/ folder at the repository root
```

The documentation is automatically built and deployed to GitHub Pages when changes are pushed to the `main` branch.

**Local Preview:**
```bash
# Serve documentation locally (with live reload)
cd docfx_project
docfx build --serve

# Open http://localhost:8080 in your browser
```

**Documentation Structure:**
- `docfx_project/` - DocFX configuration and source files
- `docs/` - Generated HTML documentation (published to GitHub Pages)
- `docfx_project/index.md` - Main landing page content
- `docfx_project/docs/` - Additional documentation articles
- `docfx_project/api/` - Auto-generated API reference YAML files

---


## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code quality standards
- Build and test instructions
- Pull request guidelines
- Analyzer configuration details
