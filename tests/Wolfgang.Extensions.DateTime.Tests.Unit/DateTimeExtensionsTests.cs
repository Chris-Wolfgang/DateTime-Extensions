namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using Xunit;

public class DateTimeExtensionsTests
{

    [Fact]
    public void TruncateMilliseconds_successfully_removes_milliseconds_from_value()
    {
        var now = DateTime.UtcNow;

        var actualResult = now.TruncateMilliseconds();

        var expectedResult = new DateTime
        (
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            0,
            DateTimeKind.Utc
        );

        Assert.Equal(expectedResult, actualResult);
    }



    [Fact]
    public void TruncateMilliseconds_returns_0_milliseconds()
    {
        var now = DateTime.UtcNow;


        var actualResult = now.TruncateMilliseconds();


        Assert.Equal(0, actualResult.Millisecond);
    }



    [Fact]
    public void TruncateSeconds_removes_seconds_from_value()
    {
        var now = DateTime.UtcNow;

        var actualResult = now.TruncateSeconds();

        var expectedResult = new DateTime
        (
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            0,
            DateTimeKind.Utc
        );

        Assert.Equal(expectedResult, actualResult);
    }



    [Fact]
    public void TruncateSeconds_returns_0_milliseconds()
    {
        var now = DateTime.UtcNow;


        var actualResult = now.TruncateSeconds();


        Assert.Equal(0, actualResult.Millisecond);
    }



    [Fact]
    public void TruncateSeconds_returns_0_seconds()
    {
        var now = DateTime.UtcNow;


        var actualResult = now.TruncateSeconds();


        Assert.Equal(0, actualResult.Second);
    }



    [Fact]
    public void TruncateSeconds_removes_seconds_and_everything_after_seconds()
    {
        var testValue = new DateTime
                (
                    2018,
                    4,
                    23,
                    12,
                    47,
                    59,
                    0,
                    DateTimeKind.Utc
                )
            .AddMilliseconds(253)
            .AddTicks(15);

        var expectedResult = new DateTime
            (
                2018,
                4,
                23,
                12,
                47,
                0,
                0,
                DateTimeKind.Utc
            );


        var actualResult = testValue.TruncateSeconds();


        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [InlineData("2016/5/13", "2016/5/1")]
    [InlineData("2016/2/13", "2016/2/1")]
    [InlineData("2016/2/1", "2016/2/1")]
    [InlineData("2016/2/29", "2016/2/1")]
    [InlineData("2016/12/31 23:59:59.9999999", "2016/12/1")]
    public void FirstOfMonth_returns_the_first_of_the_specified_month
    (
        DateTime testValue,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.FirstOfMonth();

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [InlineData("2016/5/13", "2016/5/31 23:59:59.9999999")]
    [InlineData("2016/2/13", "2016/2/29 23:59:59.9999999")]
    [InlineData("2017/2/1", "2017/2/28 23:59:59.9999999")]
    [InlineData("2016/2/29 23:59:59.9999999", "2016/2/29 23:59:59.9999999")]
    [InlineData("2016/12/31 23:59:59.9999999", "2016/12/31 23:59:59.9999999")]
    public void EndOfMonth_returns_the_last_date_and_time_of_the_specified_month
    (
        DateTime testValue,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.EndOfMonth();

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [InlineData("2016/5/13", "2016/1/1")]
    [InlineData("2020/2/29", "2020/1/1")]
    [InlineData("2018/12/31 23:59:59.9999999", "2018/1/1")]
    public void FirstOfYear_returns_the_first_of_the_specified_year
    (
        DateTime testValue,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.FirstOfYear();

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [InlineData("2016/5/13", "2016/12/31 23:59:59.9999999")]
    [InlineData("2020/2/29", "2020/12/31 23:59:59.9999999")]
    [InlineData("2018/12/31 23:59:59.9999999", "2018/12/31 23:59:59.9999999")]
    public void EndOfYear_returns_the_last_date_and_time_of_the_specified_year
    (
        DateTime testValue,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.EndOfYear();

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [MemberData(nameof(WeekCases.FirstOfWeek), MemberType = typeof(WeekCases))]
    public void FirstOfWeek_DayOfWeek_is_specified_returns_the_DateTime_of_the_first_day_of_the_week_containing_the_specified_DateTime
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.FirstOfWeek(firstDayOfWeek);

        Assert.Equal(expectedResult, actualResult);
    }



    [Theory]
    [MemberData(nameof(WeekCases.EndOfWeek), MemberType = typeof(WeekCases))]
    public void EndOfWeek_DayOfWeek_is_specified_returns_the_DateTime_of_the_last_day_of_the_week_containing_the_specified_DateTime
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        var actualResult = testValue.EndOfWeek(firstDayOfWeek);

        Assert.Equal(expectedResult, actualResult);
    }



    [Fact]
    public void EndOfMonth_when_DateTime_MaxValue_returns_max_ticks()
    {
        var result = DateTime.MaxValue.EndOfMonth();

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
    }



    [Fact]
    public void EndOfYear_when_DateTime_MaxValue_returns_max_ticks()
    {
        var result = DateTime.MaxValue.EndOfYear();

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
    }



    [Theory]
    [InlineData((DayOfWeek)7)]
    [InlineData((DayOfWeek)(-1))]
    public void FirstOfWeek_when_firstDayOfWeek_is_undefined_throws_ArgumentOutOfRangeException(DayOfWeek firstDayOfWeek)
    {
        var input = new DateTime(2026, 9, 28, 12, 0, 0, DateTimeKind.Utc);

        var exception = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => input.FirstOfWeek(firstDayOfWeek)
        );

        Assert.Equal("firstDayOfWeek", exception.ParamName);

        Assert.StartsWith
        (
            "Value must be a defined DayOfWeek.",
            exception.Message,
            StringComparison.Ordinal
        );
    }



    [Theory]
    [InlineData((DayOfWeek)7)]
    [InlineData((DayOfWeek)(-1))]
    public void EndOfWeek_when_firstDayOfWeek_is_undefined_throws_ArgumentOutOfRangeException(DayOfWeek firstDayOfWeek)
    {
        var input = new DateTime(2026, 9, 28, 12, 0, 0, DateTimeKind.Utc);

        var exception = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => input.EndOfWeek(firstDayOfWeek)
        );

        Assert.Equal("firstDayOfWeek", exception.ParamName);

        Assert.StartsWith
        (
            "Value must be a defined DayOfWeek.",
            exception.Message,
            StringComparison.Ordinal
        );
    }



    [Fact]
    public void TruncateMilliseconds_when_value_has_sub_millisecond_ticks_drops_them_too()
    {
        var input = new DateTime(2026, 9, 28, 14, 30, 45, DateTimeKind.Utc)
            .AddMilliseconds(253)
            .AddTicks(15);

        var result = input.TruncateMilliseconds();

        Assert.Equal
        (
            new DateTime(2026, 9, 28, 14, 30, 45, DateTimeKind.Utc),
            result
        );
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Theory]
    [InlineData(1, DayOfWeek.Sunday)]
    [InlineData(3, DayOfWeek.Saturday)]
    [InlineData(6, DayOfWeek.Sunday)]
    public void FirstOfWeek_when_requested_day_is_before_MinValue_clamps_to_MinValue(int day, DayOfWeek firstDayOfWeek)
    {
        var input = new DateTime(1, 1, day, 12, 0, 0, DateTimeKind.Local);

        var result = input.FirstOfWeek(firstDayOfWeek);

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
        Assert.Equal(DateTimeKind.Local, result.Kind);
    }



    [Fact]
    public void FirstOfWeek_when_requested_day_is_exactly_MinValue_returns_MinValue_without_clamping()
    {
        // 0001-01-01 is a Monday: a Monday-started week from 0001-01-03 lands
        // exactly on MinValue, the last case that does not need the clamp.
        var input = new DateTime(1, 1, 3, 12, 0, 0, DateTimeKind.Unspecified);

        var result = input.FirstOfWeek(DayOfWeek.Monday);

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
    }



    [Fact]
    public void FirstOfWeek_when_DateTime_MinValue_does_not_throw()
    {
        var result = DateTime.MinValue.FirstOfWeek(DayOfWeek.Sunday);

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
    }



    [Fact]
    public void FirstOfMonth_when_DateTime_MinValue_does_not_throw()
    {
        var result = DateTime.MinValue.FirstOfMonth();

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
    }



    [Fact]
    public void FirstOfYear_when_DateTime_MinValue_does_not_throw()
    {
        var result = DateTime.MinValue.FirstOfYear();

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
    }



    [Fact]
    public void FirstOfQuarter_when_DateTime_MinValue_does_not_throw()
    {
        var result = DateTime.MinValue.FirstOfQuarter();

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
    }



    [Fact]
    public void FirstOfHalf_when_DateTime_MinValue_does_not_throw()
    {
        var result = DateTime.MinValue.FirstOfHalf();

        Assert.Equal(DateTime.MinValue.Ticks, result.Ticks);
    }



    [Fact]
    public void EndOfWeek_when_DateTime_MaxValue_returns_max_ticks()
    {
        var result = DateTime.MaxValue.EndOfWeek(DayOfWeek.Monday);

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
    }



    [Fact]
    public void EndOfWeek_when_week_ends_before_MaxValue_does_not_clamp()
    {
        // Eight days of headroom: the week containing this instant ends before MaxValue,
        // so the saturating clamp must not engage and the ordinary seven-day week applies.
        var input = DateTime.MaxValue.AddDays(-8);

        var result = input.EndOfWeek(DayOfWeek.Monday);

        Assert.Equal(input.FirstOfWeek(DayOfWeek.Monday).AddDays(7).AddTicks(-1), result);
        Assert.NotEqual(DateTime.MaxValue, result);
    }



    [Theory]
    [InlineData(1, DayOfWeek.Tuesday, 1)]
    [InlineData(2, DayOfWeek.Wednesday, 2)]
    [InlineData(3, DayOfWeek.Saturday, 5)]
    [InlineData(6, DayOfWeek.Sunday, 6)]
    public void EndOfWeek_when_FirstOfWeek_would_clamp_still_returns_the_correct_day(int day, DayOfWeek firstDayOfWeek, int expectedDay)
    {
        // 0001-01-01 is a Monday. For these pairs the week START lies before MinValue (so
        // FirstOfWeek clamps), but the week END is representable and must be exact.
        var input = new DateTime(1, 1, day, 12, 0, 0, DateTimeKind.Utc);

        var result = input.EndOfWeek(firstDayOfWeek);

        Assert.Equal
        (
            new DateTime(1, 1, expectedDay).AddDays(1).AddTicks(-1),
            result
        );
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Fact]
    public void EndOfWeek_when_in_the_first_week_of_year_1_is_idempotent()
    {
        // The #436 finding: EndOfWeek(Saturday) applied to its own result used to move.
        var input = new DateTime(1, 1, 1, 0, 8, 54, DateTimeKind.Unspecified);

        var once = input.EndOfWeek(DayOfWeek.Saturday);
        var twice = once.EndOfWeek(DayOfWeek.Saturday);

        Assert.Equal(new DateTime(1, 1, 6).AddTicks(-1), once);
        Assert.Equal(once, twice);
    }



    [Fact]
    public void FirstOfWeek_when_time_is_nonzero_returns_midnight()
    {
        var input = new DateTime(2024, 6, 12, 15, 30, 45, DateTimeKind.Utc);

        var result = input.FirstOfWeek(DayOfWeek.Monday);

        Assert.Equal(0, result.Hour);
        Assert.Equal(0, result.Minute);
        Assert.Equal(0, result.Second);
        Assert.Equal(0, result.Millisecond);
    }



    [Fact]
    public void EndOfMonth_when_DateTime_MaxValue_preserves_Kind()
    {
        var input = new DateTime(9999, 12, 15, 10, 0, 0, DateTimeKind.Local);

        var result = input.EndOfMonth();

        Assert.Equal(DateTimeKind.Local, result.Kind);
    }



    // ----- FirstOfQuarter / EndOfQuarter -----

    [Theory]
    [InlineData(1,  1)]   // Jan → Q1 starts Jan 1
    [InlineData(2,  1)]   // Feb → Q1
    [InlineData(3,  1)]   // Mar → Q1
    [InlineData(4,  4)]   // Apr → Q2 starts Apr 1
    [InlineData(5,  4)]
    [InlineData(6,  4)]
    [InlineData(7,  7)]   // Jul → Q3
    [InlineData(8,  7)]
    [InlineData(9,  7)]
    [InlineData(10, 10)]  // Oct → Q4
    [InlineData(11, 10)]
    [InlineData(12, 10)]
    public void FirstOfQuarter_returns_the_first_day_of_the_quarter(int inputMonth, int expectedQuarterStartMonth)
    {
        var input = new DateTime(2026, inputMonth, 15, 14, 30, 45, 123, DateTimeKind.Utc);

        var result = input.FirstOfQuarter();

        Assert.Equal(2026, result.Year);
        Assert.Equal(expectedQuarterStartMonth, result.Month);
        Assert.Equal(1, result.Day);
        Assert.Equal(0, result.Hour);
        Assert.Equal(0, result.Minute);
        Assert.Equal(0, result.Second);
        Assert.Equal(0, result.Millisecond);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Theory]
    [InlineData(1,  3,  31)]   // Q1 ends Mar 31
    [InlineData(4,  6,  30)]   // Q2 ends Jun 30
    [InlineData(7,  9,  30)]   // Q3 ends Sep 30
    [InlineData(10, 12, 31)]   // Q4 ends Dec 31
    public void EndOfQuarter_returns_the_last_tick_of_the_quarter(int inputMonth, int expectedEndMonth, int expectedEndDay)
    {
        var input = new DateTime(2026, inputMonth, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = input.EndOfQuarter();

        Assert.Equal(2026, result.Year);
        Assert.Equal(expectedEndMonth, result.Month);
        Assert.Equal(expectedEndDay, result.Day);
        Assert.Equal(23, result.Hour);
        Assert.Equal(59, result.Minute);
        Assert.Equal(59, result.Second);
        Assert.Equal(TimeSpan.TicksPerSecond - 1, result.Ticks % TimeSpan.TicksPerSecond);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Fact]
    public void EndOfQuarter_when_DateTime_MaxValue_clamps_to_MaxValue()
    {
        var input = new DateTime(9999, 11, 15, 10, 0, 0, DateTimeKind.Utc);

        var result = input.EndOfQuarter();

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Fact]
    public void FirstOfQuarter_preserves_Kind()
    {
        var input = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Local);

        var result = input.FirstOfQuarter();

        Assert.Equal(DateTimeKind.Local, result.Kind);
    }



    // ----- FirstOfHalf / EndOfHalf -----

    [Theory]
    [InlineData(1,  1)]   // Jan → H1
    [InlineData(2,  1)]
    [InlineData(3,  1)]
    [InlineData(4,  1)]
    [InlineData(5,  1)]
    [InlineData(6,  1)]
    [InlineData(7,  7)]   // Jul → H2
    [InlineData(8,  7)]
    [InlineData(9,  7)]
    [InlineData(10, 7)]
    [InlineData(11, 7)]
    [InlineData(12, 7)]
    public void FirstOfHalf_returns_the_first_day_of_the_half_year(int inputMonth, int expectedHalfStartMonth)
    {
        var input = new DateTime(2026, inputMonth, 15, 14, 30, 45, 123, DateTimeKind.Utc);

        var result = input.FirstOfHalf();

        Assert.Equal(2026, result.Year);
        Assert.Equal(expectedHalfStartMonth, result.Month);
        Assert.Equal(1, result.Day);
        Assert.Equal(0, result.Hour);
        Assert.Equal(0, result.Minute);
        Assert.Equal(0, result.Second);
        Assert.Equal(0, result.Millisecond);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Theory]
    [InlineData(1, 6,  30)]   // H1 ends Jun 30
    [InlineData(7, 12, 31)]   // H2 ends Dec 31
    public void EndOfHalf_returns_the_last_tick_of_the_half_year(int inputMonth, int expectedEndMonth, int expectedEndDay)
    {
        var input = new DateTime(2026, inputMonth, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = input.EndOfHalf();

        Assert.Equal(2026, result.Year);
        Assert.Equal(expectedEndMonth, result.Month);
        Assert.Equal(expectedEndDay, result.Day);
        Assert.Equal(23, result.Hour);
        Assert.Equal(59, result.Minute);
        Assert.Equal(59, result.Second);
        Assert.Equal(TimeSpan.TicksPerSecond - 1, result.Ticks % TimeSpan.TicksPerSecond);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Fact]
    public void EndOfHalf_when_DateTime_MaxValue_clamps_to_MaxValue()
    {
        var input = new DateTime(9999, 9, 15, 10, 0, 0, DateTimeKind.Utc);

        var result = input.EndOfHalf();

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }



    [Fact]
    public void FirstOfHalf_preserves_Kind()
    {
        var input = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Local);

        var result = input.FirstOfHalf();

        Assert.Equal(DateTimeKind.Local, result.Kind);
    }
}
