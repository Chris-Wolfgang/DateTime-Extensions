# ADR-0003: Disable implicit usings in every project

- **Status**: Accepted
- **Date**: 2026-09-23

## Context

Implicit usings were enabled on `src/`, `tests/` and the net8.0 example, and disabled on
`benchmarks/`. The source project enabled them only for net8.0 and net10.0, so the set of `using`
directives a file needed **differed by target framework**.

That made static analysis untrustworthy rather than merely inconsistent. ReSharper InspectCode
analyses one framework slice, so it reported `using System;` in `DateTimeExtensions.cs` as
redundant - true on net8.0/net10.0, where implicit usings supply it. Acting on that alert produced
six `CS0246` errors on net462 and netstandard2.0, which need the directive. The alert was correct
about the slice it saw and wrong about the library.

## Decision

`ImplicitUsings` is `disable` in every project, set in an **unconditional** property group. Every
file states the directives it needs, and needs the same ones on every framework.

## Consequences

- One file had to gain `using System;`: the net8.0 example, which used `Console`, `ConsoleColor`,
  `DayOfWeek` and `DateTimeKind` without importing it.
- "Redundant using" now means the same thing in every slice, so the analyser can be believed.
- New projects must set it explicitly. A conditional property group would reintroduce exactly the
  drift this removes - the setting belongs in the unconditional group even though `disable` is a
  no-op on frameworks that never had implicit usings.
- A future SDK default cannot silently change behaviour on one framework and not another.
