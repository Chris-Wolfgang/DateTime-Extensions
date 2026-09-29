namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using Xunit;

/// <summary>
/// Week-boundary rows shared by the explicit-<see cref="DayOfWeek"/> overload theories in
/// <see cref="DateTimeExtensionsTests"/> and the current-culture overload theories in
/// <c>Globalization.CurrentCultureWeekOverloadTests</c>, so the two overload families are
/// always checked against the same expectations. February 2020 is a leap month and
/// 2020-02-29 is a Saturday, which puts week starts on every side of the month boundary.
/// </summary>
public static class WeekCases
{
    public static TheoryData<DateTime, DayOfWeek, DateTime> FirstOfWeek => new()
    {
        { new DateTime(2020, 2, 23), DayOfWeek.Sunday, new DateTime(2020, 2, 23) },
        { new DateTime(2020, 2, 29), DayOfWeek.Sunday, new DateTime(2020, 2, 23) },
        { new DateTime(2020, 2, 23), DayOfWeek.Monday, new DateTime(2020, 2, 17) },
        { new DateTime(2020, 2, 29), DayOfWeek.Monday, new DateTime(2020, 2, 24) },
        { new DateTime(2020, 2, 24), DayOfWeek.Monday, new DateTime(2020, 2, 24) },
        { new DateTime(2020, 3, 2), DayOfWeek.Monday, new DateTime(2020, 3, 2) },
        { new DateTime(2020, 2, 23), DayOfWeek.Saturday, new DateTime(2020, 2, 22) },
        { new DateTime(2020, 2, 29), DayOfWeek.Saturday, new DateTime(2020, 2, 29) },
    };



    public static TheoryData<DateTime, DayOfWeek, DateTime> EndOfWeek => new()
    {
        { new DateTime(2020, 2, 23), DayOfWeek.Sunday, LastTickOf(2020, 2, 29) },
        { new DateTime(2020, 2, 29), DayOfWeek.Sunday, LastTickOf(2020, 2, 29) },
        { new DateTime(2020, 2, 23), DayOfWeek.Monday, LastTickOf(2020, 2, 23) },
        { new DateTime(2020, 2, 29), DayOfWeek.Monday, LastTickOf(2020, 3, 1) },
        { new DateTime(2020, 3, 7), DayOfWeek.Monday, LastTickOf(2020, 3, 8) },
        { new DateTime(2020, 2, 24), DayOfWeek.Monday, LastTickOf(2020, 3, 1) },
        { new DateTime(2020, 3, 2), DayOfWeek.Monday, LastTickOf(2020, 3, 8) },
        { new DateTime(2020, 2, 23), DayOfWeek.Saturday, LastTickOf(2020, 2, 28) },
        { new DateTime(2020, 2, 29), DayOfWeek.Saturday, LastTickOf(2020, 3, 6) },
        { new DateTime(2020, 3, 1), DayOfWeek.Sunday, LastTickOf(2020, 3, 7) },
        { new DateTime(2020, 3, 4), DayOfWeek.Sunday, LastTickOf(2020, 3, 7) },
    };



    private static DateTime LastTickOf(int year, int month, int day)
        => new DateTime(year, month, day).AddDays(1).AddTicks(-1);
}
