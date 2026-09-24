# ADR-0004: Gate the package on binary compatibility against the last published version

- **Status**: Accepted
- **Date**: 2026-09-23

## Context

`PublicAPI.Shipped.txt` and the PublicApiAnalyzers catch added or removed signatures, because they
compare declared API text. They do not catch a default-value change, a nullability annotation flip,
or a binary-layout shift - breaks a consumer's already-compiled assembly hits at runtime rather
than at compile time.

## Decision

`EnablePackageValidation` with `PackageValidationBaselineVersion` pinned to the last version
**published to NuGet**. The build downloads that package and compares the compiled surface against
it. A deliberate break is waived in `CompatibilitySuppressions.xml`, naming the rule and the target.

## Consequences

- The check runs at pack time, so it fails on any build that packs - including a pull request -
  rather than only at release.
- The baseline must be moved to the newly published version **after** each release, and never to
  the version being prepared. Left stale, the gate silently compares against the wrong thing.
- It overlaps with the PublicAPI baseline, and that is fine: they catch different failures. A
  method made `internal` trips `RS0017` at compile time long before validation runs, which is what
  happened when this gate was first tested.
- Waiving a break is a recorded, reviewable act rather than a silent one.
