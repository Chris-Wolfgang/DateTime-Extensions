namespace Wolfgang.Extensions.DateTime.Tests.Unit.Globalization;


using System;
using System.Globalization;
using Xunit;

/// <summary>
/// The parameterless <c>FirstOfWeek()</c> / <c>EndOfWeek()</c> overloads read
/// <see cref="CultureInfo.CurrentCulture"/>'s <c>FirstDayOfWeek</c>. These theories set that
/// value and expect the same rows the explicit-<see cref="DayOfWeek"/> overloads are checked
/// against in <see cref="WeekCases"/>.
/// </summary>
/// <remarks>
/// Lives in <see cref="CultureCollection"/> because it swaps the ambient culture: xunit runs
/// classes in parallel, and a test that reads the culture while this one has changed it would
/// observe the change. The culture is restored in <see cref="Dispose"/> on every path.
/// </remarks>
[Collection(CultureCollection.Name)]
public sealed class CurrentCultureWeekOverloadTests : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;



    [Theory]
    [MemberData(nameof(WeekCases.FirstOfWeek), MemberType = typeof(WeekCases))]
    public void FirstOfWeek_when_culture_defines_the_first_day_returns_the_first_day_of_the_containing_week
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        CultureInfo.CurrentCulture = CultureWithFirstDay(firstDayOfWeek);

        var actualResult = testValue.FirstOfWeek();

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [MemberData(nameof(WeekCases.EndOfWeek), MemberType = typeof(WeekCases))]
    public void EndOfWeek_when_culture_defines_the_first_day_returns_the_last_tick_of_the_containing_week
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        CultureInfo.CurrentCulture = CultureWithFirstDay(firstDayOfWeek);

        var actualResult = testValue.EndOfWeek();

        Assert.Equal(expectedResult, actualResult);
    }



    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
    }



    private static CultureInfo CultureWithFirstDay(DayOfWeek firstDayOfWeek)
    {
        return new CultureInfo("en-US")
        {
            DateTimeFormat = { FirstDayOfWeek = firstDayOfWeek }
        };
    }
}
