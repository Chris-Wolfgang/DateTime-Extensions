# Wolfgang.Extensions.DateTime

A collection of extension methods for the `DateTime` struct in C#.

## Methods
- `TruncateMilliseconds`: Removes fractional seconds, returning a DateTime accurate to the whole second.
- `TruncateSeconds`: Strips seconds and smaller units, yielding a DateTime rounded down to the minute.
- `FirstOfMonth`: Produces a new `DateTime` set to the first day of the month at midnight of the specified `DateTime`.
- `EndOfMonth`: Computes the final tick of the monthfor the specified `DateTime`.
- `FirstOfYear`: Creates a `DateTime` corresponding to January 1 of the same year as the specified `DateTime`.
- `EndOfYear`: Returns the final tick of the year of the specified `DateTime`.
- `FirstOfWeek()`: Uses the current culture’s first day of the week to locate the week’s starting `DateTime`.
- `FirstOfWeek(DayOfWeek firstDayOfWeek)`: Uses the specified first day of the week to locate the week’s starting `DateTime`.
- `EndOfWeek()`: Uses the current culture’s first day, calculates the final tick of the week.
- `EndOfWeek(DayOfWeek firstDayOfWeek)`: Uses the specified first day, calculates the final tick of the week.

