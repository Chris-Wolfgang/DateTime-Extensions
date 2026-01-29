# Introduction

## Overview

Wolfgang.Extensions.DateTime is a lightweight, high-performance library that provides a collection of useful extension methods for the `DateTime` struct in C#. These extensions simplify common date and time manipulation tasks, making your code more readable and maintainable.

## Why Use This Library?

Working with dates and times in .NET can be verbose and error-prone. Wolfgang.Extensions.DateTime addresses this by providing intuitive extension methods that:

- **Simplify Common Operations**: Perform frequent date/time tasks with a single method call
- **Improve Code Readability**: Replace complex date calculations with clear, expressive method names
- **Reduce Errors**: Use well-tested, reliable implementations instead of writing custom logic
- **Support Multiple Frameworks**: Compatible with .NET Framework 4.6.2, .NET Standard 2.0, .NET 8.0, and .NET 10.0

## Key Features

### Time Truncation
Remove fractional time components to round DateTime values to specific precision levels:
- **TruncateMilliseconds**: Remove milliseconds, keeping time accurate to the second
- **TruncateSeconds**: Remove seconds and smaller units, rounding to the minute

### Date Boundaries
Quickly navigate to the beginning or end of time periods:
- **FirstOfMonth / EndOfMonth**: Get the first or last moment of a month
- **FirstOfYear / EndOfYear**: Get the first or last moment of a year
- **FirstOfWeek / EndOfWeek**: Get the first or last moment of a week (culture-aware)

### Culture Awareness
Week-related methods respect cultural differences:
- Use current culture's first day of week by default
- Allow explicit specification of the week's starting day

## Target Audience

This library is perfect for:
- .NET developers working with date and time calculations
- Applications requiring calendar-based operations
- Projects needing cross-framework compatibility
- Teams seeking to standardize date/time manipulation patterns

## Philosophy

Wolfgang.Extensions.DateTime follows these principles:
- **Minimal Dependencies**: No external dependencies beyond the .NET framework
- **Performance First**: Efficient implementations with minimal overhead
- **Clear Semantics**: Method names that clearly express their purpose
- **Immutability**: All methods return new DateTime instances, preserving the original value