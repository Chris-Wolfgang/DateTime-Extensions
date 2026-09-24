# Architecture Decision Records

Short records of the choices in this repository that are not obvious from the code, and would
otherwise be re-derived - usually worse - six months later.

A new ADR lands in the same pull request as the decision it describes, so the reasoning is
reviewed alongside the change rather than reconstructed afterwards. Start from
[TEMPLATE.md](TEMPLATE.md).

| ADR | Decision | Status | Date |
|---|---|---|---|
| [ADR-0001](0001-pin-assemblyversion-to-major.md) | Pin AssemblyVersion to MAJOR.0.0.0 | Accepted | 2026-05-27 |
| [ADR-0002](0002-namespace-shadows-system-datetime.md) | Accept that the namespace shadows System.DateTime, and qualify in src | Accepted | 2026-05-27 |
| [ADR-0003](0003-disable-implicit-usings.md) | Disable implicit usings in every project | Accepted | 2026-09-23 |
| [ADR-0004](0004-package-validation-baseline.md) | Gate the package on binary compatibility against the last published version | Accepted | 2026-09-23 |

A superseded ADR is kept, not deleted: its Status points at the record that replaced it, so the
history of a reversal stays legible.
