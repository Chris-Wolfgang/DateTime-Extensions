# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [1.3.2] - 2026-07-06

### Fixed

- `EndOfWeek`: kill equivalent Stryker mutant on the 7-day boundary (#217).

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [1.3.1] - 2026-05-28

### Added

- Docs site: version picker dropdown on the published DocFX site, with a
  landing-page redirect to the latest released version.
- Examples: `FirstOfQuarter` / `EndOfQuarter` / `FirstOfHalf` / `EndOfHalf`
  demos in both example apps.
- README: Quick Start + Examples now demonstrate the Q/H methods.

### Changed

- Internal: extracted a private `MidnightOf` helper to DRY up the
  `FirstOf*` midnight-construction pattern. No public-API impact.
- CHANGELOG / `DateTime-Extensions.slnx` housekeeping.
- CI: canonical workflow sync (Stage 2 coverage regex, ruleset
  enforcement, version-picker.js header).

### Fixed

- **Binding stability on .NET Framework consumers.** v1.3.0 inadvertently
  shipped with `AssemblyVersion=1.3.0.0` (SDK-derived from `<Version>`
  after the explicit `<AssemblyVersion>1.0.0.0</AssemblyVersion>` line was
  dropped in the v1.3.0 csproj cleanup). That changed the assembly's
  binding identity vs the 1.0 / 1.1 / 1.2 line, which all shipped with
  `AssemblyVersion=1.0.0.0`. Without a binding redirect, .NET Framework
  consumers upgrading from any of those to v1.3.0 would have hit
  `FileLoadException: Could not load file or assembly 'Wolfgang.Extensions.DateTime, Version=1.3.0.0, ...'`.

  v1.3.1 restores the explicit pin: `<AssemblyVersion>1.0.0.0</AssemblyVersion>`.
  Future minor/patch bumps on the 1.x line will keep this binding identity;
  `AssemblyVersion` will only change on a deliberate breaking API change
  (i.e., a 2.0 release). `<FileVersion>` now explicitly tracks
  `$(Version).0` so file-version metadata still reflects every release.

  Pre-1.3.0 → 1.3.1 consumers do not need a binding redirect (the binding
  identity is the same `1.0.0.0` they already linked against). v1.3.0 →
  1.3.1 consumers also do not need one going forward, but anyone on v1.3.0
  today is referencing `Version=1.3.0.0` and will need either a binding
  redirect or a re-reference + recompile to pick up 1.3.1. The damage from
  v1.3.0 cannot be fully undone after release; v1.3.1 stops the bleeding
  for everyone who hasn't yet adopted v1.3.0.
- `PublicAPI.Shipped.txt`: promoted the four Q/H entries that were left in
  `PublicAPI.Unshipped.txt` at the v1.3.0 tag. The methods themselves
  shipped in v1.3.0; this is a metadata correction, not an API change.

## [1.3.0] - 2026-05-26

### Added

- `FirstOfQuarter()` / `EndOfQuarter()` extension methods returning the
  first day (at 00:00) and last tick of the calendar quarter containing
  the input. Quarters: Q1 Jan-Mar, Q2 Apr-Jun, Q3 Jul-Sep, Q4 Oct-Dec.
  `EndOfQuarter()` clamps at `DateTime.MaxValue` for Q4 of year 9999.
- `FirstOfHalf()` / `EndOfHalf()` extension methods returning the
  first day (at 00:00) and last tick of the calendar half-year
  containing the input. Halves: H1 Jan-Jun, H2 Jul-Dec. `EndOfHalf()`
  clamps at `DateTime.MaxValue` for H2 of year 9999.
- All four new methods preserve the input's `DateTimeKind` and are
  covered by unit tests across every TFM (Theory cases for all 12
  input months on Quarter, all 12 on Half, plus boundary + Kind tests)
  and by gh-pages benchmark chart series.
- `PublicApiAnalyzer` baseline activated. `PublicAPI.Shipped.txt`
  captures the v1.3.0 public surface (14 extension method overloads +
  the static class); future surface changes will surface as RS0016
  / *REMOVED* diffs at compile time.
- BenchmarkDotNet baseline project
  (`benchmarks/Wolfgang.Extensions.DateTime.Benchmarks`) plus
  `benchmarks.yaml` workflow that publishes trend charts to gh-pages
  `/dev/bench/`.
- `<RepositoryUrl>` and `<PackageTags>` package metadata in src csproj.

### Changed

- Analyzer `PackageReference`s consolidated back into `Directory.Build.props`
  (reverses the v1.2.0 per-project split — see the v1.2.0 entry below). The
  consolidated location is the single source of truth for the canonical
  template-sync flow; per-project opt-out is still available via overrides
  in a project's own csproj.
- README rewritten: filled Quick Start + Examples, added NuGet / build /
  release badges, moved DocFX troubleshooting out of README into
  `docs/DOCFX-VERSION-PICKER.md`, corrected analyzer count (7 → 8 with
  PublicApiAnalyzer), bumped build prereq from .NET 8.0 → 10.0 SDK.
- `<AssemblyVersion>` dropped from src csproj — let SDK derive from
  `<Version>` so all version metadata stays in lock-step on every bump
  (the previously-hardcoded `1.0.0` was stale relative to released
  package versions from v1.1.0 onward).
- CHANGELOG seeded with v1.0.0 / v1.1.0 / v1.2.0 entries from git tag
  history.

### Removed

- `REPO-INSTRUCTIONS.md` template scaffolding (its own opening said
  "delete this file after setup"; the repo has been live since v1.0.0).
- `TEMPLATE-PLACEHOLDERS.md` and `TEMPLATE-REPO-PLACEHOLDERS-EXPLANATION.md`
  template scaffolding files (also from initial setup, no longer needed).

### Fixed

### Security

## [1.2.0] - 2026-04-27

### Fixed

- Boundary crashes in `FirstOfMonth`, `EndOfMonth`, `FirstOfYear`,
  `EndOfYear`, `FirstOfWeek`, `EndOfWeek` near `DateTime.MaxValue` and
  `DateTime.MinValue` — methods now return the boundary value instead of
  throwing.
- `FirstOfWeek` no longer carries the input's time-of-day into the
  returned value (now consistently returns `00:00:00.0000000`).

### Changed

- Analyzer `PackageReference`s moved from `Directory.Build.props` into
  individual csproj files for this release (later reverted in
  `[Unreleased]` — see above — when the canonical template-sync settled
  on `Directory.Build.props` as the fleet-wide source of truth).

## [1.1.0] - 2026-03-31

### Changed

- Dropped `netcoreapp3.1` target. Supported frameworks are now
  `net462`, `netstandard2.0`, `net8.0`, and `net10.0`.

### Fixed

- `TruncateMilliseconds` and `TruncateSeconds` now use the 8-parameter
  `DateTime` constructor with explicit `millisecond=0`, removing an
  ambiguity that surfaced under some culture settings.
- XML documentation corrections (`EndOfWeek` previously said "end of
  the month").

### Removed

- `StyleCop.Analyzers` PackageReference removed in favour of the
  consolidated analyzer set (Roslynator, Meziantou, SonarAnalyzer).

## [1.0.0] - 2026-01-18

### Added

- Initial release with `TruncateMilliseconds`, `TruncateSeconds`,
  `FirstOfMonth`, `EndOfMonth`, `FirstOfYear`, `EndOfYear`,
  `FirstOfWeek`, `EndOfWeek` extension methods on `System.DateTime`.
- Multi-framework targeting: `net462`, `netstandard2.0`,
  `netcoreapp3.1`, `net8.0`, `net10.0`.

[Unreleased]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.3.2...HEAD
[1.3.2]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.3.1...v1.3.2
[1.3.1]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Chris-Wolfgang/DateTime-Extensions/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Chris-Wolfgang/DateTime-Extensions/releases/tag/v1.0.0
