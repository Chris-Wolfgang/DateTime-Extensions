# Getting Started

This guide will help you quickly integrate Wolfgang.Extensions.DateTime into your project and start using the extension methods.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.DateTime
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.DateTime
```

### Via NuGet Package Manager UI

1. Right-click your project in Visual Studio
2. Select "Manage NuGet Packages"
3. Search for "Wolfgang.Extensions.DateTime"
4. Click "Install"

## Basic Usage

Once installed, add the using directive to your C# file:

```csharp
using Wolfgang.Extensions.DateTime;
```

Now you can use the extension methods on any `DateTime` instance.

## Quick Examples

### Truncate Time Components

```csharp
var now = DateTime.Now; // e.g., 2026-01-29 14:35:42.123

// Remove milliseconds
var withoutMs = now.TruncateMilliseconds(); // 2026-01-29 14:35:42.000

// Remove seconds and milliseconds
var withoutSec = now.TruncateSeconds(); // 2026-01-29 14:35:00.000
```

### Navigate to Month Boundaries

```csharp
var today = new DateTime(2026, 1, 29);

// Get first day of the month
var firstDay = today.FirstOfMonth(); // 2026-01-01 00:00:00

// Get last moment of the month
var lastDay = today.EndOfMonth(); // 2026-01-31 23:59:59.9999999
```

### Navigate to Year Boundaries

```csharp
var today = new DateTime(2026, 6, 15);

// Get first day of the year
var yearStart = today.FirstOfYear(); // 2026-01-01 00:00:00

// Get last moment of the year
var yearEnd = today.EndOfYear(); // 2026-12-31 23:59:59.9999999
```

### Work with Weeks

```csharp
var date = new DateTime(2026, 1, 29); // Thursday

// Get first day of week (using current culture)
var weekStart = date.FirstOfWeek();

// Get first day of week (explicit: Sunday)
var sundayStart = date.FirstOfWeek(DayOfWeek.Sunday); // 2026-01-25

// Get first day of week (explicit: Monday)
var mondayStart = date.FirstOfWeek(DayOfWeek.Monday); // 2026-01-26

// Get last moment of week
var weekEnd = date.EndOfWeek(); // Using current culture
var sundayEnd = date.EndOfWeek(DayOfWeek.Sunday); // 2026-01-31 23:59:59.9999999
```

## Common Scenarios

### Creating Date Ranges

```csharp
var today = DateTime.Today;
var startOfMonth = today.FirstOfMonth();
var endOfMonth = today.EndOfMonth();

// Query data for the current month
var monthlyData = database.GetRecords()
    .Where(r => r.Date >= startOfMonth && r.Date <= endOfMonth);
```

### Normalizing Timestamps

```csharp
// Remove sub-second precision for comparison
var timestamp1 = DateTime.Now.TruncateMilliseconds();
var timestamp2 = OtherSource.GetTimestamp().TruncateMilliseconds();

if (timestamp1 == timestamp2)
{
    // Timestamps match to the second
}
```

### Building Reports

```csharp
var reportDate = DateTime.Now;
var yearStart = reportDate.FirstOfYear();
var yearEnd = reportDate.EndOfYear();

Console.WriteLine($"Annual Report: {yearStart:yyyy-MM-dd} to {yearEnd:yyyy-MM-dd}");
```

## Framework Support

Wolfgang.Extensions.DateTime supports:
- **.NET Framework 4.6.2** and higher
- **.NET Standard 2.0** (compatible with .NET Core 2.0+, Xamarin, etc.)
- **.NET 8.0**
- **.NET 10.0**

The library automatically adapts to your target framework, ensuring optimal compatibility.

## Next Steps

- Explore the [API Reference](../api/Wolfgang.Extensions.DateTime.DateTimeExtensions.yml) for detailed method documentation
- Review the [Examples](../../examples) for more advanced usage patterns
- Check the [README](readme.md) for additional information

## Need Help?

- Report issues on [GitHub Issues](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
- Review the source code on [GitHub](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- Check existing discussions for common questions