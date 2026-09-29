# Getting Started

This guide will help you quickly get up and running with Wolfgang.Extensions.DateTime.

## Prerequisites

- .NET 10.0 SDK (for development; the library targets .NET Framework 4.6.2, .NET Standard 2.0, .NET 8.0 and .NET 10.0)

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.DateTime
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.DateTime
```

## Quick Start

```csharp
using Wolfgang.Extensions.DateTime;

var now = System.DateTime.Now;

// Truncate to remove sub-second or sub-minute precision
var noMilliseconds = now.TruncateMilliseconds();  // Zeroes out milliseconds
var noSeconds = now.TruncateSeconds();              // Zeroes out seconds and milliseconds

// Navigate to month boundaries
var firstOfMonth = now.FirstOfMonth();  // e.g. 2026-03-01 00:00:00.000
var endOfMonth = now.EndOfMonth();      // e.g. 2026-03-31 23:59:59.9999999

// Navigate to quarter and half-year boundaries
var firstOfQuarter = now.FirstOfQuarter();  // e.g. 2026-01-01 00:00:00.000 for a March date
var endOfQuarter = now.EndOfQuarter();      // e.g. 2026-03-31 23:59:59.9999999
var firstOfHalf = now.FirstOfHalf();        // e.g. 2026-01-01 00:00:00.000
var endOfHalf = now.EndOfHalf();            // e.g. 2026-06-30 23:59:59.9999999

// Navigate to year boundaries
var firstOfYear = now.FirstOfYear();    // e.g. 2026-01-01 00:00:00.000
var endOfYear = now.EndOfYear();        // e.g. 2026-12-31 23:59:59.9999999

// Navigate to week boundaries (culture-aware)
var firstOfWeek = now.FirstOfWeek();                        // Uses CurrentCulture
var firstOfWeekMon = now.FirstOfWeek(DayOfWeek.Monday);     // Explicit start day
var endOfWeek = now.EndOfWeek();
```

## Next Steps

- Explore the [API Reference](https://chris-wolfgang.github.io/DateTime-Extensions/versions/latest/api/Wolfgang.Extensions.DateTime.html) for detailed documentation
- Read the [Introduction](introduction.md) to learn more about Wolfgang.Extensions.DateTime
- Check out example projects in the [GitHub repository](https://github.com/Chris-Wolfgang/DateTime-Extensions)

## Common Issues

### Namespace conflicts with `System.DateTime`

The library namespace is `Wolfgang.Extensions.DateTime`, whose last segment has the same name as the `System.DateTime` type. Consumers are unaffected: `using Wolfgang.Extensions.DateTime;` brings the extension methods into scope and `DateTime` still means `System.DateTime` in your code. Only code declared *inside* the `Wolfgang.Extensions.DateTime` namespace (the library's own source) has to write `System.DateTime` in full - see [ADR-0002](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/docs/adr/0002-namespace-shadows-system-datetime.md).

### DateTimeKind is preserved

All extension methods preserve the `DateTimeKind` of the input. If you pass a UTC DateTime, you get a UTC DateTime back.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
