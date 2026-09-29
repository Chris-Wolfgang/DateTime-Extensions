# Introduction

Welcome to Wolfgang.Extensions.DateTime!

## Overview

Wolfgang.Extensions.DateTime is a .NET library providing extension methods for the `System.DateTime` type. It adds convenient methods for truncating time precision and navigating to date boundaries (week, month, quarter, half-year, year).

## Key Features

- **TruncateMilliseconds** — removes milliseconds, returning a DateTime precise to the second
- **TruncateSeconds** — removes seconds and milliseconds, returning a DateTime precise to the minute
- **FirstOfMonth / EndOfMonth** — navigates to the first or last instant of the month
- **FirstOfQuarter / EndOfQuarter** — navigates to the first or last instant of the calendar quarter
- **FirstOfHalf / EndOfHalf** — navigates to the first or last instant of the calendar half-year
- **FirstOfYear / EndOfYear** — navigates to the first or last instant of the year
- **FirstOfWeek / EndOfWeek** — navigates to week boundaries, with culture-aware or explicit `DayOfWeek` overloads
- All methods preserve `DateTimeKind` (Local, Utc, Unspecified)

## Getting Help

If you need help with Wolfgang.Extensions.DateTime, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](https://chris-wolfgang.github.io/DateTime-Extensions/versions/latest/api/Wolfgang.Extensions.DateTime.html)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
