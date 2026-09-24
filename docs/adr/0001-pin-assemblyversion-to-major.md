# ADR-0001: Pin AssemblyVersion to MAJOR.0.0.0

- **Status**: Accepted
- **Date**: 2026-05-27

## Context

.NET Framework resolves assembly references by strong-name identity, and `AssemblyVersion` is part
of that identity. If it changes on every release, a net462 consumer that compiled against 1.2.0
fails to load 1.2.1 unless the application adds a binding redirect. That burden lands on the
consumer for a change that broke nothing.

This library ships net462 and netstandard2.0, so it is exposed to that behaviour in a way a
net8.0-only package is not.

## Decision

`AssemblyVersion` is pinned to `MAJOR.0.0.0` and changes only on a deliberate breaking change -
a MAJOR bump under SemVer. `FileVersion` and `InformationalVersion` carry the real release
version; the latter is derived from `<Version>` plus the SourceLink commit.

## Consequences

- A net462 consumer upgrades within a major version with no binding redirect.
- Tooling that reports `AssemblyVersion` shows `1.0.0.0` for every 1.x release. Diagnose the exact
  build from `InformationalVersion`, not from the assembly version.
- Changing it is a release-gating decision, not a routine bump. `EnablePackageValidation`
  (ADR-0004) is what makes an accidental break visible before the version question arises.
- v1.0.0, v1.1.0 and v1.2.0 shipped with `1.0.0.0`, so the pin is consistent with what is already
  published rather than a change of direction.
