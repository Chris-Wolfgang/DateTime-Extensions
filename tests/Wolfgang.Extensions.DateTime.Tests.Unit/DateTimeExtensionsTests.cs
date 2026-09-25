namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using System.Globalization;
using System.Threading;
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
    [InlineData("2016/2/13", "2016/2/29  23:59:59.9999999")]
    [InlineData("2017/2/1", "2017/2/28  23:59:59.9999999")]
    [InlineData("2016/2/29  23:59:59.9999999", "2016/2/29  23:59:59.9999999")]
    [InlineData("2016/12/31 23:59:59.9999999", "2016/12/31  23:59:59.9999999")]
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
    [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/23")]
    [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/23")]
    [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/17")]
    [InlineData("2020/2/29", DayOfWeek.Monday, "2020/2/24")]
    [InlineData("2020/2/24", DayOfWeek.Monday, "2020/2/24")]
    [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/2")]
    [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/22")]
    [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/2/29")]
    public void FirstOfWeek_returns_the_date_time_of_the_first_day_of_the_week_containing_the_specified_DateTime
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        var backup = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US")
            {
                DateTimeFormat = { FirstDayOfWeek = firstDayOfWeek }
            };

            var actualResult = testValue.FirstOfWeek();

            Assert.Equal(expectedResult, actualResult);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = backup;
        }
    }



    [Theory]
    [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/23")]
    [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/23")]
    [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/17")]
    [InlineData("2020/2/29", DayOfWeek.Monday, "2020/2/24")]
    [InlineData("2020/2/24", DayOfWeek.Monday, "2020/2/24")]
    [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/2")]
    [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/22")]
    [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/2/29")]
    public void
        FirstOfWeek_DayOfWeek_is_specified_returns_the_DateTime_of_the_first_day_of_the_week_containing_the_specified_DateTime
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
    [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
    [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/23 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
    [InlineData("2020/3/7", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
    [InlineData("2020/2/24", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
    [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
    [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/28 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/3/6 23:59:59.9999999")]
    [InlineData("2020/3/1", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
    [InlineData("2020/3/4", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
    public void EndOfWeek_returns_the_DateTime_of_the_last_day_of_the_week_containing_the_specified_DateTime
    (
        DateTime testValue,
        DayOfWeek firstDayOfWeek,
        DateTime expectedResult
    )
    {
        var backup = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US")
            {
                DateTimeFormat = { FirstDayOfWeek = firstDayOfWeek }
            };

            var actualResult = testValue.EndOfWeek();

            Assert.Equal(expectedResult, actualResult);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = backup;
        }
    }



    [Theory]
    [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
    [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/23 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
    [InlineData("2020/3/7", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
    [InlineData("2020/2/24", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
    [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
    [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/28 23:59:59.9999999")]
    [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/3/6 23:59:59.9999999")]
    [InlineData("2020/3/1", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
    [InlineData("2020/3/4", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
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



    [Fact]
    public void FirstOfWeek_when_the_week_starts_before_DateTime_MinValue_throws_OverflowException()
    {
        // DateTime.MinValue is a Monday, so the Sunday-aligned week containing it began on
        // 0000-12-31, which DateTime cannot represent. This used to return MinValue - a value
        // whose DayOfWeek was not the day that was asked for. See issue #436.
        var exception = Assert.Throws<OverflowException>
        (
            () => DateTime.MinValue.FirstOfWeek(DayOfWeek.Sunday)
        );

        Assert.Contains("begins before DateTime.MinValue", exception.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void FirstOfWeek_when_DateTime_MinValue_and_the_week_starts_on_Monday_returns_MinValue()
    {
        // The one alignment that IS representable at the bottom of the range, because
        // 0001-01-01 is itself a Monday.
        var result = DateTime.MinValue.FirstOfWeek(DayOfWeek.Monday);

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
    public void EndOfWeek_when_the_week_ends_after_DateTime_MaxValue_throws_OverflowException()
    {
        // DateTime.MaxValue is a Friday, so a Monday-aligned week containing it ends on
        // 10000-01-02, which DateTime cannot represent. This used to return MaxValue, quietly
        // describing a week under two days long. See issue #436.
        var exception = Assert.Throws<OverflowException>
        (
            () => DateTime.MaxValue.EndOfWeek(DayOfWeek.Monday)
        );

        Assert.Contains("ends after DateTime.MaxValue", exception.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void EndOfWeek_when_DateTime_MaxValue_and_the_week_starts_on_Saturday_returns_MaxValue()
    {
        // The one alignment that IS representable at the top of the range: the Saturday week
        // 9999-12-25..9999-12-31 ends exactly on the last representable tick.
        var result = DateTime.MaxValue.EndOfWeek(DayOfWeek.Saturday);

        Assert.Equal(DateTime.MaxValue.Ticks, result.Ticks);
    }



    [Fact]
    public void EndOfWeek_when_the_week_starts_before_DateTime_MinValue_still_returns_the_week_end()
    {
        // The regression this split exists for. The Saturday-aligned week containing
        // 0001-01-03 runs 0000-12-30..0001-01-05: its START is unrepresentable, its END is
        // not. EndOfWeek used to route through FirstOfWeek, inherit that clamp, and answer
        // 0001-01-07 - two days into the following week. Counting forward from the instant
        // instead gives the correct answer and never consults the week's start. Issue #436.
        var result = new DateTime(1, 1, 3, 6, 0, 0).EndOfWeek(DayOfWeek.Saturday);

        Assert.Equal
        (
            new DateTime(1, 1, 5, 23, 59, 59).AddTicks(TimeSpan.TicksPerSecond - 1),
            result
        );
    }



    [Fact]
    public void FirstOfWeek_when_the_week_ends_after_DateTime_MaxValue_still_returns_the_week_start()
    {
        // The mirror of the test above: each method owns one end of the range, so the one
        // that is not at its own limit must keep working.
        var result = DateTime.MaxValue.FirstOfWeek(DayOfWeek.Monday);

        Assert.Equal(new DateTime(9999, 12, 27), result);
    }



    [Fact]
    public void EndOfWeek_a_week_clear_of_both_limits_is_exactly_seven_days_long()
    {
        // Cross-checks the two now-independent implementations against each other: away from
        // both limits, EndOfWeek must land exactly one tick before FirstOfWeek plus seven days.
        var input = DateTime.MaxValue.AddDays(-8);
        var firstOfWeek = input.FirstOfWeek(DayOfWeek.Monday);

        var result = input.EndOfWeek(DayOfWeek.Monday);

        Assert.Equal(firstOfWeek.AddDays(7).AddTicks(-1), result);
        Assert.NotEqual(DateTime.MaxValue, result);
    }



    public static TheoryData<DateTime, DayOfWeek> WeeksStartingBeforeMinValue
    {
        get
        {
            // 0001-01-01 is a Monday, so on the 1st every day except Monday needs an earlier
            // week start than DateTime can hold; by the 7th (a Sunday) every day has occurred
            // at least once and nothing is unreachable. Six dates, 21 pairs.
            var data = new TheoryData<DateTime, DayOfWeek>();

            for (var day = 1; day <= 6; day++)
            {
                var date = new DateTime(1, 1, day, 12, 0, 0);

                foreach (DayOfWeek firstDayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var daysBack = ((int)date.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

                    // Only (day - 1) days exist before this date, so a larger step back leaves
                    // the representable range.
                    if (daysBack > day - 1)
                    {
                        data.Add(date, firstDayOfWeek);
                    }
                }
            }

            return data;
        }
    }



    [Theory]
    [MemberData(nameof(WeeksStartingBeforeMinValue))]
    public void FirstOfWeek_throws_for_every_week_that_starts_before_MinValue
    (
        DateTime dateTime,
        DayOfWeek firstDayOfWeek
    )
    {
        Assert.Throws<OverflowException>(() => dateTime.FirstOfWeek(firstDayOfWeek));

        // The other method owns the other end of the range, so it must still answer here.
        var endOfWeek = dateTime.EndOfWeek(firstDayOfWeek);

        Assert.True(endOfWeek >= dateTime, $"EndOfWeek returned {endOfWeek:O} for {dateTime:O}");
    }



    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    [InlineData(99)]
    public void Week_methods_throw_for_a_DayOfWeek_that_is_not_one_of_the_seven_values(int value)
    {
        var dateTime = new DateTime(2026, 9, 24);
        var firstDayOfWeek = (DayOfWeek)value;

        // Previously these walked back to year 1 - some 740,000 iterations - and answered
        // 0001-01-01.
        var first = Assert.Throws<ArgumentOutOfRangeException>(() => dateTime.FirstOfWeek(firstDayOfWeek));
        var end = Assert.Throws<ArgumentOutOfRangeException>(() => dateTime.EndOfWeek(firstDayOfWeek));

        Assert.Equal("firstDayOfWeek", first.ParamName);
        Assert.Equal("firstDayOfWeek", end.ParamName);
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
