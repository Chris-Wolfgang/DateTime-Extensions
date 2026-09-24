# Migrating from <OLD> to <NEW>

> Copy this file to `docs/migrations/<OLD>-to-<NEW>.md` during release prep - not after the
> release - and link it from the GitHub Release notes. A migration guide written after consumers
> have already hit the break is a post-mortem, not a migration guide.

`Wolfgang.Extensions.DateTime` <NEW> contains breaking changes. This guide lists every one, what
to use instead, and how long the old form keeps working.

## At a glance

| What changed | Old | New | Action |
|---|---|---|---|
| <renamed method> | `OldName(...)` | `NewName(...)` | rename the call |
| <removed overload> | `Foo(int)` | `Foo(int, DateTimeKind)` | pass the kind explicitly |
| <behaviour change> | returned local time | returns the input's `Kind` | check any caller that assumed local |

## Breaking-change inventory

One section per break. Include the behavioural ones: a method that still compiles but returns
something different is the break consumers find in production rather than at build time.

### <Title of the break>

**What changed.** One or two sentences.

**Why.** The reason it was worth breaking. If it was to fix a bug, say what the bug was.

**Before**

```csharp
System.DateTime moment = new System.DateTime(2026, 9, 24, 14, 30, 0, System.DateTimeKind.Utc);
var result = moment.OldMethod();
```

**After**

```csharp
System.DateTime moment = new System.DateTime(2026, 9, 24, 14, 30, 0, System.DateTimeKind.Utc);
var result = moment.NewMethod(System.DateTimeKind.Utc);
```

> `System.DateTime` is written out in full because this library's own namespace is
> `Wolfgang.Extensions.DateTime`, so the bare name binds to the namespace - see
> [ADR-0002](../adr/0002-namespace-shadows-system-datetime.md). Only `DateTime` collides;
> `System.DateTimeKind` is spelled out here only to keep the snippet uniform. A consumer writing
> outside that namespace needs neither qualifier and can write `DateTime` and `DateTimeKind`.

**Not affected.** Say who can ignore this - it is as useful as the instruction itself.

## Deprecation timeline

| Version | State |
|---|---|
| <OLD> | last release with the old form and no warning |
| <DEPRECATION MINOR> | `[Obsolete]` warning added; old form still works |
| <NEW> | old form removed |

If no intervening MINOR carried the warning, delete that row and say so - a consumer who went
straight from `<OLD>` to `<NEW>` got no build-time notice at all, and that is worth stating
rather than leaving them to infer it.

The fleet convention is to ship the `[Obsolete]` warning in a MINOR release **before** the MAJOR
that removes the API, so a consumer gets a build warning while the old form still works. A break
that appears for the first time in the MAJOR release gives them no such window - note it here
explicitly if that happened.

## Binary compatibility

`EnablePackageValidation` compares each build against the last published package and fails on an
incompatible change ([ADR-0004](../adr/0004-package-validation-baseline.md)). It detects **binary
and API** breaks - a removed or re-signatured member, a changed default, a nullability flip.

Each of those has a matching entry in `CompatibilitySuppressions.xml` naming the rule and target,
so for that class of break the file is the machine-readable form of this document and the two
should agree.

**Behavioural breaks are not in that file and cannot be.** A method whose signature is unchanged
but whose result differs produces no diagnostic, so package validation stays silent and there is
nothing to suppress. Those entries carry their evidence differently - the test that pins the new
behaviour, and the PR that changed it. Do not expect a suppression for every row in the inventory
above; expecting one is how a behavioural break gets quietly dropped from the guide.

`AssemblyVersion` moves to `<NEW MAJOR>.0.0.0` in this release and only in this release
([ADR-0001](../adr/0001-pin-assemblyversion-to-major.md)), so .NET Framework consumers need a
binding redirect crossing this boundary and not within it.
