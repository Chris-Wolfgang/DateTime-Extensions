using System;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.DateTime;

namespace Wolfgang.Extensions.DateTime.Benchmarks;

/// <summary>
/// Microbenchmarks for the public extension methods. Every public method
/// on <see cref="DateTimeExtensions"/> has a corresponding [Benchmark]
/// here — fourteen methods at present (<c>TruncateMilliseconds</c>,
/// <c>TruncateSeconds</c>, <c>FirstOfMonth</c> / <c>EndOfMonth</c>,
/// <c>FirstOfYear</c> / <c>EndOfYear</c>, <c>FirstOfQuarter</c> /
/// <c>EndOfQuarter</c>, <c>FirstOfHalf</c> / <c>EndOfHalf</c>, plus
/// both overloads of <c>FirstOfWeek</c> and <c>EndOfWeek</c>). They
/// allocate a small number of DateTime structs and short loops; each
/// should be well under a microsecond. The MemoryDiagnoser is enabled
/// so any future refactor that introduces allocation surfaces in the
/// gh-pages benchmark chart immediately.
/// </summary>
[MemoryDiagnoser]
public class DateTimeExtensionsBenchmarks
{
    // A representative mid-range UTC value — late enough that the MinValue
    // boundary checks in FirstOfWeek do not short-circuit, but well clear of
    // the MaxValue overflow guard so EndOfMonth / EndOfYear take the common
    // fast path.
    private static readonly System.DateTime Sample =
        new(2026, 5, 26, 13, 45, 30, 123, DateTimeKind.Utc);



    [Benchmark]
    public System.DateTime TruncateMilliseconds() => Sample.TruncateMilliseconds();



    [Benchmark]
    public System.DateTime TruncateSeconds() => Sample.TruncateSeconds();



    [Benchmark]
    public System.DateTime FirstOfMonth() => Sample.FirstOfMonth();



    [Benchmark]
    public System.DateTime EndOfMonth() => Sample.EndOfMonth();



    [Benchmark]
    public System.DateTime FirstOfYear() => Sample.FirstOfYear();



    [Benchmark]
    public System.DateTime EndOfYear() => Sample.EndOfYear();



    [Benchmark]
    public System.DateTime FirstOfWeek_Sunday() => Sample.FirstOfWeek(DayOfWeek.Sunday);



    [Benchmark]
    public System.DateTime EndOfWeek_Sunday() => Sample.EndOfWeek(DayOfWeek.Sunday);



    // The parameterless overloads add a `CultureInfo.CurrentCulture
    // .DateTimeFormat.FirstDayOfWeek` lookup that the explicit-DayOfWeek
    // overloads above don't exercise. Benchmark separately so a regression
    // in the culture-lookup path is visible in its own chart series.
    [Benchmark]
    public System.DateTime FirstOfWeek_CurrentCulture() => Sample.FirstOfWeek();



    [Benchmark]
    public System.DateTime EndOfWeek_CurrentCulture() => Sample.EndOfWeek();



    [Benchmark]
    public System.DateTime FirstOfQuarter() => Sample.FirstOfQuarter();



    [Benchmark]
    public System.DateTime EndOfQuarter() => Sample.EndOfQuarter();



    [Benchmark]
    public System.DateTime FirstOfHalf() => Sample.FirstOfHalf();



    [Benchmark]
    public System.DateTime EndOfHalf() => Sample.EndOfHalf();
}
