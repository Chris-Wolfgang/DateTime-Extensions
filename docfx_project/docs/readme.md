# Wolfgang.Extensions.DateTime Documentation

Welcome to the Wolfgang.Extensions.DateTime documentation! This library provides a comprehensive set of extension methods for the `DateTime` struct in C#, designed to simplify common date and time operations.

## Documentation Overview

This documentation is organized into several sections to help you get the most out of Wolfgang.Extensions.DateTime:

### Core Documentation

#### [Introduction](introduction.md)
Learn about the library's purpose, key features, and design philosophy. This section explains:
- Why use Wolfgang.Extensions.DateTime
- Key features and capabilities
- Target audience
- Design principles

#### [Getting Started](getting-started.md)
A practical guide to start using the library in your project:
- Installation instructions (NuGet, Package Manager)
- Basic usage examples
- Common scenarios and patterns
- Framework compatibility information

#### [Setup Guide](setup.md)
For contributors and developers who want to work on the library itself:
- Development environment setup
- Building the project
- Running tests
- Code quality tools
- Contributing guidelines

### API Reference

The API reference provides detailed documentation for each extension method, including:
- Method signatures
- Parameter descriptions
- Return values
- Usage examples
- Remarks and considerations

Access the API reference at: [Wolfgang.Extensions.DateTime API](../api/Wolfgang.Extensions.DateTime.DateTimeExtensions.yml)

## Quick Reference

### Extension Methods Summary

| Method | Description |
|--------|-------------|
| `TruncateMilliseconds()` | Removes milliseconds, returning a DateTime accurate to the whole second |
| `TruncateSeconds()` | Strips seconds and smaller units, yielding a DateTime rounded to the minute |
| `FirstOfMonth()` | Returns the first day of the month at midnight |
| `EndOfMonth()` | Returns the final tick of the month |
| `FirstOfYear()` | Returns January 1 of the current year at midnight |
| `EndOfYear()` | Returns the final tick of December 31 |
| `FirstOfWeek()` | Returns the first day of the week (culture-aware) |
| `FirstOfWeek(DayOfWeek)` | Returns the first day of the week with explicit start day |
| `EndOfWeek()` | Returns the last tick of the week (culture-aware) |
| `EndOfWeek(DayOfWeek)` | Returns the last tick of the week with explicit start day |

## Library Information

- **Package Name**: Wolfgang.Extensions.DateTime
- **Current Version**: 1.0.0
- **License**: MIT License
- **Author**: Chris Wolfgang
- **Repository**: [GitHub - DateTime-Extensions](https://github.com/Chris-Wolfgang/DateTime-Extensions)

## Supported Frameworks

Wolfgang.Extensions.DateTime supports a wide range of .NET platforms:
- .NET Framework 4.6.2 and higher
- .NET Standard 2.0 (compatible with .NET Core 2.0+, Xamarin, Unity, etc.)
- .NET 8.0
- .NET 10.0

## Getting Help

### Resources
- **GitHub Repository**: [Chris-Wolfgang/DateTime-Extensions](https://github.com/Chris-Wolfgang/DateTime-Extensions)
- **Issue Tracker**: [Report bugs or request features](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues)
- **Discussions**: [Ask questions and share ideas](https://github.com/Chris-Wolfgang/DateTime-Extensions/discussions)

### Common Questions

**Q: Do these methods modify the original DateTime?**  
A: No, all extension methods return new DateTime instances. The original value is never modified.

**Q: Are these methods thread-safe?**  
A: Yes, since DateTime is a value type and all methods create new instances rather than modifying existing ones, they are inherently thread-safe.

**Q: What about performance?**  
A: All methods are optimized for performance with minimal overhead. They use simple calculations and avoid unnecessary allocations.

**Q: Can I use this with DateTimeOffset?**  
A: The current version provides extensions for DateTime only. DateTimeOffset support may be added in future versions.

## Contributing

We welcome contributions! If you'd like to contribute to Wolfgang.Extensions.DateTime:

1. Read the [Setup Guide](setup.md) to prepare your development environment
2. Check the [Contributing Guidelines](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/CONTRIBUTING.md)
3. Follow the [Code of Conduct](https://github.com/Chris-Wolfgang/DateTime-Extensions/blob/main/CODE_OF_CONDUCT.md)
4. Submit your pull request

## Version History

- **1.0.0** - Initial release with core DateTime extension methods

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/Chris-Wolfgang/Wolfgang.Extensions.DateTime/blob/main/LICENSE) file for details.

---

*Last updated: January 2026*