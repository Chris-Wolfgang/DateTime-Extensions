# Copilot Instructions for Wolfgang.Extensions.DateTime

## Project Overview
- **Package:** Wolfgang.Extensions.DateTime
- **Namespace:** Wolfgang.Extensions.DateTime
- **Purpose:** Extension methods for System.DateTime — truncation and date boundary navigation

## Key Types
- `DateTimeExtensions` — static class with extension methods on `System.DateTime`

## Extension Methods
- `TruncateMilliseconds()` — zeroes out milliseconds
- `TruncateSeconds()` — zeroes out seconds and milliseconds
- `FirstOfMonth()` / `EndOfMonth()` — month boundary navigation
- `FirstOfQuarter()` / `EndOfQuarter()` — calendar quarter boundary navigation (Q1 Jan-Mar, Q2 Apr-Jun, Q3 Jul-Sep, Q4 Oct-Dec)
- `FirstOfHalf()` / `EndOfHalf()` — calendar half-year boundary navigation (H1 Jan-Jun, H2 Jul-Dec)
- `FirstOfYear()` / `EndOfYear()` — year boundary navigation
- `FirstOfWeek()` / `EndOfWeek()` — week boundary navigation (culture-aware + explicit DayOfWeek overloads)

## Important Notes
- All methods preserve `DateTimeKind`
- Use `System.DateTime` (fully qualified) to avoid namespace conflicts with the library namespace
- `End*` methods return the last tick before the next period (`.AddTicks(-1)`) and clamp at `DateTime.MaxValue` when the next period would overflow
- `FirstOfWeek` clamps at `DateTime.MinValue` when walking back to find the firstDayOfWeek would underflow
- Public API is tracked by `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` — additions surface as RS0016 at compile time

## Code Style
- Allman brace style
- 3 blank lines between members
- File-scoped namespaces
- Warnings as errors in Release builds

## Target Frameworks
- net462, netstandard2.0, net8.0, net10.0, net11.0
