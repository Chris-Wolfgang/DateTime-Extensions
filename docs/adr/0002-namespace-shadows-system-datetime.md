# ADR-0002: Accept that the namespace shadows System.DateTime, and qualify in src

- **Status**: Accepted
- **Date**: 2026-05-27

## Context

The library's namespace is `Wolfgang.Extensions.DateTime`, matching the package id and the fleet's
`Wolfgang.Extensions.<Thing>` convention. Inside that namespace the bare name `DateTime` binds to
the **namespace**, not to `System.DateTime`: C# resolves a name against enclosing namespace members
before it consults `using` directives. So `DateTime.MaxValue` does not compile in `src/`.

## Decision

Keep the namespace. Inside `src/`, write **`System.DateTime`** in full and let `using System;` supply
everything else. Do not rename the namespace to dodge the collision, and do not introduce a `using`
alias - an alias moves the surprise somewhere less obvious.

Only `DateTime` itself needs the qualifier, because only `DateTime` collides with a namespace
segment. `DateTimeKind`, `DayOfWeek`, `TimeSpan` and `CultureInfo` are written unqualified, and the
file's `using System;` is what makes them resolve - which is why deleting that directive breaks the
build on net462 and netstandard2.0 (ADR-0003).

## Consequences

- `src/` reads more verbosely than a normal extensions library wherever `DateTime` appears. That
  verbosity is load-bearing; removing it does not compile.
- It does **not** follow that the qualifier is needed everywhere. In
  `tests/Wolfgang.Extensions.DateTime.Tests.Unit` the bare name resolves, because that namespace's
  own `using System;` wins there - which is why the qualifiers were removed from the test project
  and kept in `src/`.
- An analyser that flags the qualifier as redundant is reading a slice where it happens to be
  redundant. Verify against a build of all target frameworks before acting on it; see ADR-0003.
- A consumer is unaffected: from outside, `Wolfgang.Extensions.DateTime` and `System.DateTime` do
  not collide.
