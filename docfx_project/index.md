---
_layout: landing
---

# Wolfgang.Extensions.DateTime

A powerful, lightweight library of extension methods for working with DateTime in C#.

## What is Wolfgang.Extensions.DateTime?

Wolfgang.Extensions.DateTime is a carefully crafted collection of extension methods that simplify common date and time operations in .NET applications. Whether you're building web applications, desktop software, or backend services, these extensions make working with dates more intuitive and less error-prone.

## Key Features

### 🎯 Time Precision Control
Easily truncate time components to achieve the precision you need:
- Remove milliseconds for second-level accuracy
- Remove seconds for minute-level accuracy

### 📅 Boundary Navigation
Navigate effortlessly to the beginning or end of time periods:
- **Monthly boundaries**: First and last day of any month
- **Yearly boundaries**: First and last day of any year
- **Weekly boundaries**: First and last day of any week (culture-aware)

### 🌍 Culture-Aware
Week-related operations respect cultural differences, automatically using the current culture's first day of the week, with options to override when needed.

### ⚡ High Performance
All methods are optimized for performance with zero external dependencies beyond the .NET framework itself.

## Supported Frameworks

- .NET Framework 4.6.2+
- .NET Standard 2.0 (supports .NET Core 2.0+, Xamarin, Unity, etc.)
- .NET 8.0
- .NET 10.0

## Quick Example

```csharp
using Wolfgang.Extensions.DateTime;

var today = DateTime.Now;

// Get the first day of the current month
var monthStart = today.FirstOfMonth();

// Get the last moment of the current month
var monthEnd = today.EndOfMonth();

// Get the start of the current week (Sunday by default in en-US)
var weekStart = today.FirstOfWeek();

// Truncate to minute precision
var roundedTime = today.TruncateSeconds();
```

## Getting Started

Ready to enhance your DateTime operations? Check out the [Getting Started Guide](docs/getting-started.md) for installation instructions and basic usage examples.

## Documentation

- **[Introduction](docs/introduction.md)** - Learn about the library and its philosophy
- **[Getting Started](docs/getting-started.md)** - Installation and quick start guide
- **[Setup Guide](docs/setup.md)** - Development environment setup for contributors
- **[API Reference](api/Wolfgang.Extensions.DateTime.DateTimeExtensions.yml)** - Complete method documentation
- **[README](docs/readme.md)** - Additional documentation overview

## Available Methods

### Truncation Methods
- `TruncateMilliseconds()` - Remove fractional seconds
- `TruncateSeconds()` - Remove seconds and smaller units

### Month Operations
- `FirstOfMonth()` - Get the first day of the month at midnight
- `EndOfMonth()` - Get the last tick of the month

### Year Operations
- `FirstOfYear()` - Get January 1st of the current year
- `EndOfYear()` - Get the last tick of December 31st

### Week Operations
- `FirstOfWeek()` - Get the first day of the week (culture-aware)
- `FirstOfWeek(DayOfWeek)` - Get the first day of the week with explicit start day
- `EndOfWeek()` - Get the last tick of the week (culture-aware)
- `EndOfWeek(DayOfWeek)` - Get the last tick of the week with explicit start day

## Installation

```bash
dotnet add package Wolfgang.Extensions.DateTime
```

Or via NuGet Package Manager:

```powershell
Install-Package Wolfgang.Extensions.DateTime
```

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/CONTRIBUTING.md) for details.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/LICENSE) file for details.

## Author

Created and maintained by [Chris Wolfgang](https://github.com/Chris-Wolfgang)

## Support

- 🐛 [Report Issues](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
- 📖 [View Source](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- 💬 [Discussions](https://github.com/Chris-Wolfgang/DateTime-Extensions/discussions)
