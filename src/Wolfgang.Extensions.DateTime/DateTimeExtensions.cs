using System;
using System.Globalization;

namespace Wolfgang.Extensions.DateTime;


/// <summary>
/// A collection of extension methods for DateTime
/// </summary>
public static class DateTimeExtensions
{

    /// <summary>
    /// Returns the midnight DateTime for a specific year/month/day with the
    /// given Kind. Centralises the 8-parameter <see cref="System.DateTime"/>
    /// constructor pattern used by every <c>FirstOf*</c> boundary method
    /// that returns a midnight value: <see cref="FirstOfMonth"/>,
    /// <see cref="FirstOfYear"/>, <see cref="FirstOfQuarter"/> and
    /// <see cref="FirstOfHalf"/>. The week methods construct from ticks
    /// instead, because they work in whole days from the instant.
    /// </summary>
    private static System.DateTime MidnightOf(int year, int month, int day, DateTimeKind kind)
        => new(year, month, day, 0, 0, 0, 0, kind);



    /// <summary>
    /// Whole days from <paramref name="firstDayOfWeek"/> to <paramref name="dayOfWeek"/>,
    /// 0 when they are the same day and never more than 6.
    /// </summary>
    /// <remarks>
    /// The <c>+ 7</c> keeps the left operand of <c>%</c> positive: C#'s remainder takes the sign
    /// of the dividend, so <c>(0 - 6) % 7</c> is -6 rather than 1.
    /// </remarks>
    private static int DaysSinceFirstDayOfWeek(DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek)
        => ((int)dayOfWeek - (int)firstDayOfWeek + 7) % 7;



    /// <summary>
    /// Rejects a <see cref="DayOfWeek"/> that is not one of the seven defined values.
    /// </summary>
    /// <remarks>
    /// An undefined value is a caller mistake rather than a range problem, which is why this
    /// throws <see cref="ArgumentOutOfRangeException"/> while an unrepresentable week boundary
    /// throws <see cref="OverflowException"/>: the first is fixed by passing something else, the
    /// second cannot be fixed by any argument.
    /// </remarks>
    /// <param name="firstDayOfWeek">The value to check.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="firstDayOfWeek"/> is not one of the seven <see cref="DayOfWeek"/> values.
    /// </exception>
    private static void ValidateFirstDayOfWeek(DayOfWeek firstDayOfWeek)
    {
        if (firstDayOfWeek < DayOfWeek.Sunday || firstDayOfWeek > DayOfWeek.Saturday)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(firstDayOfWeek),
                firstDayOfWeek,
                "Not one of the seven DayOfWeek values."
            );
        }
    }



    /// <summary>
    /// Remove the milliseconds and everything after the milliseconds
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime equal to the passed in value without milliseconds</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var truncated = moment.TruncateMilliseconds();
    /// // 2026-09-24 14:30:45.0000000
    /// </code>
    /// </example>
    public static System.DateTime TruncateMilliseconds(this System.DateTime dateTime)
        => new
            (
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                0,
                dateTime.Kind
            );



    /// <summary>
    /// Remove the seconds and everything after the seconds
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime equal to the passed in value without seconds and milliseconds</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var truncated = moment.TruncateSeconds();
    /// // 2026-09-24 14:30:00.0000000
    /// </code>
    /// </example>
    public static System.DateTime TruncateSeconds(this System.DateTime dateTime)
        => new
            (
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                0,
                0,
                dateTime.Kind
            );



    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// month specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the first of the month.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfMonth();
    /// // 2026-09-01 00:00:00.0000000
    /// </code>
    /// </example>
    public static System.DateTime FirstOfMonth(this System.DateTime dateTime)
        => MidnightOf(dateTime.Year, dateTime.Month, 1, dateTime.Kind);



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// month specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the month.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfMonth();
    /// // 2026-09-30 23:59:59.9999999 - the last tick of the month, not midnight
    /// </code>
    /// </example>
    public static System.DateTime EndOfMonth(this System.DateTime dateTime)
    {
        var firstOfMonth = dateTime.FirstOfMonth();

        return firstOfMonth.Month == 12 && firstOfMonth.Year == 9999
            ? new System.DateTime(System.DateTime.MaxValue.Ticks, dateTime.Kind)
            : firstOfMonth.AddMonths(1).AddTicks(-1);
    }




    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// year specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the first of the year.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfYear();
    /// // 2026-01-01 00:00:00.0000000
    /// </code>
    /// </example>
    public static System.DateTime FirstOfYear(this System.DateTime dateTime)
        => MidnightOf(dateTime.Year, 1, 1, dateTime.Kind);



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// year specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the year.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfYear();
    /// // 2026-12-31 23:59:59.9999999
    /// </code>
    /// </example>
    public static System.DateTime EndOfYear(this System.DateTime dateTime)
    {
        var firstOfYear = dateTime.FirstOfYear();

        return firstOfYear.Year == 9999
            ? new System.DateTime(System.DateTime.MaxValue.Ticks, dateTime.Kind)
            : firstOfYear.AddYears(1).AddTicks(-1);
    }



    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// week specified by the DateTime passed in using the thread's
    /// CurrentCulture FirstDayOfWeek.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the first of the week.</returns>
    /// <exception cref="OverflowException">
    /// The week containing <paramref name="dateTime"/> begins before
    /// <see cref="System.DateTime.MinValue"/>. Only reachable in the first days of year 1.
    /// </exception>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfWeek();
    /// // The current culture decides which day starts the week, so this value is
    /// // machine-dependent. Under an invariant culture the week starts on Sunday
    /// // and the result is 2026-09-20. Use the overload below to be explicit.
    /// </code>
    /// </example>
    public static System.DateTime FirstOfWeek(this System.DateTime dateTime)
        => FirstOfWeek(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);



    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// week specified by the DateTime passed in using the specified
    /// firstDayOfWeek
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <param name="firstDayOfWeek">Specifies the first day of the week.</param>
    /// <returns>A new DateTime representing the first of the week.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="firstDayOfWeek"/> is not one of the seven <see cref="DayOfWeek"/> values.
    /// </exception>
    /// <exception cref="OverflowException">
    /// The week containing <paramref name="dateTime"/> and starting on
    /// <paramref name="firstDayOfWeek"/> begins before <see cref="System.DateTime.MinValue"/>.
    /// Only reachable in the first days of year 1.
    /// </exception>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfWeek(DayOfWeek.Monday);
    /// // 2026-09-21 00:00:00.0000000 - the Monday before Thursday 24 September
    /// </code>
    /// </example>
    public static System.DateTime FirstOfWeek(this System.DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        ValidateFirstDayOfWeek(firstDayOfWeek);

        var ticks = dateTime.Date.Ticks
            - (DaysSinceFirstDayOfWeek(dateTime.DayOfWeek, firstDayOfWeek) * TimeSpan.TicksPerDay);

        if (ticks < 0)
        {
            throw new OverflowException
            (
                string.Format
                (
                    CultureInfo.InvariantCulture,
                    "The week containing {0:O} and starting on {1} begins before DateTime.MinValue.",
                    dateTime,
                    firstDayOfWeek
                )
            );
        }

        return new System.DateTime(ticks, dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// week specified by the DateTime passed in using the thread's
    /// CurrentCulture FirstDayOfWeek.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the week.</returns>
    /// <exception cref="OverflowException">
    /// The week containing <paramref name="dateTime"/> ends after
    /// <see cref="System.DateTime.MaxValue"/>. Only reachable in the last days of year 9999.
    /// </exception>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfWeek();
    /// // Culture-dependent, like FirstOfWeek(). Prefer the overload below when the
    /// // result has to be the same on every machine.
    /// </code>
    /// </example>
    public static System.DateTime EndOfWeek(this System.DateTime dateTime)
        => EndOfWeek(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// week specified by the DateTime passed in using the specified
    /// firstDayOfWeek.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <param name="firstDayOfWeek">Specifies the first day of the week. Use the parameterless overload to pick up <see cref="CultureInfo.CurrentCulture"/>'s value automatically.</param>
    /// <returns>A new DateTime representing the end of the week.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="firstDayOfWeek"/> is not one of the seven <see cref="DayOfWeek"/> values.
    /// </exception>
    /// <exception cref="OverflowException">
    /// The week containing <paramref name="dateTime"/> and starting on
    /// <paramref name="firstDayOfWeek"/> ends after <see cref="System.DateTime.MaxValue"/>.
    /// Only reachable in the last days of year 9999.
    /// </exception>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfWeek(DayOfWeek.Monday);
    /// // 2026-09-27 23:59:59.9999999 - the last tick of the Sunday that ends that week
    /// </code>
    /// </example>
    public static System.DateTime EndOfWeek(this System.DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        ValidateFirstDayOfWeek(firstDayOfWeek);

        // Counted FORWARD from the instant, deliberately not as FirstOfWeek() + 7 days. Near
        // DateTime.MinValue the start of the week can be unrepresentable while its end is
        // perfectly representable, and routing through FirstOfWeek would either throw or
        // answer from a week it had to clamp. Each method owns one end of the range.
        var daysRemaining = 7 - DaysSinceFirstDayOfWeek(dateTime.DayOfWeek, firstDayOfWeek);

        // Cannot overflow Int64: the largest value reachable is MaxValue.Ticks plus seven days,
        // about 3.16e18 against long.MaxValue of about 9.22e18.
        var ticks = dateTime.Date.Ticks + (daysRemaining * TimeSpan.TicksPerDay) - 1;

        if (ticks > System.DateTime.MaxValue.Ticks)
        {
            throw new OverflowException
            (
                string.Format
                (
                    CultureInfo.InvariantCulture,
                    "The week containing {0:O} and starting on {1} ends after DateTime.MaxValue.",
                    dateTime,
                    firstDayOfWeek
                )
            );
        }

        return new System.DateTime(ticks, dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// calendar quarter containing the specified DateTime. Quarters are:
    /// Q1 Jan-Mar, Q2 Apr-Jun, Q3 Jul-Sep, Q4 Oct-Dec.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the first of the quarter at 00:00:00.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfQuarter();
    /// // 2026-07-01 00:00:00.0000000 - September is in Q3
    /// </code>
    /// </example>
    public static System.DateTime FirstOfQuarter(this System.DateTime dateTime)
    {
        var quarterStartMonth = (((dateTime.Month - 1) / 3) * 3) + 1;
        return MidnightOf(dateTime.Year, quarterStartMonth, 1, dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the last tick of the
    /// calendar quarter containing the specified DateTime. Clamps at
    /// <see cref="System.DateTime.MaxValue"/> when the quarter is Q4 of
    /// year 9999.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the quarter.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfQuarter();
    /// // 2026-09-30 23:59:59.9999999
    /// </code>
    /// </example>
    public static System.DateTime EndOfQuarter(this System.DateTime dateTime)
    {
        var firstOfQuarter = dateTime.FirstOfQuarter();

        return firstOfQuarter.Month == 10 && firstOfQuarter.Year == 9999
            ? new System.DateTime(System.DateTime.MaxValue.Ticks, dateTime.Kind)
            : firstOfQuarter.AddMonths(3).AddTicks(-1);
    }



    /// <summary>
    /// Returns a new DateTime that represents the first day of the
    /// calendar half-year containing the specified DateTime. Halves are:
    /// H1 Jan-Jun, H2 Jul-Dec.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the first of the half-year at 00:00:00.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var first = moment.FirstOfHalf();
    /// // 2026-07-01 00:00:00.0000000 - September is in the second half
    /// </code>
    /// </example>
    public static System.DateTime FirstOfHalf(this System.DateTime dateTime)
    {
        var halfStartMonth = dateTime.Month <= 6 ? 1 : 7;
        return MidnightOf(dateTime.Year, halfStartMonth, 1, dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the last tick of the
    /// calendar half-year containing the specified DateTime. Clamps at
    /// <see cref="System.DateTime.MaxValue"/> when the half is H2 of
    /// year 9999.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the half-year.</returns>
    /// <example>
    /// <code>
    /// var moment = new DateTime(2026, 9, 24, 14, 30, 45, 678);
    /// var end = moment.EndOfHalf();
    /// // 2026-12-31 23:59:59.9999999
    /// </code>
    /// </example>
    public static System.DateTime EndOfHalf(this System.DateTime dateTime)
    {
        var firstOfHalf = dateTime.FirstOfHalf();

        return firstOfHalf.Month == 7 && firstOfHalf.Year == 9999
            ? new System.DateTime(System.DateTime.MaxValue.Ticks, dateTime.Kind)
            : firstOfHalf.AddMonths(6).AddTicks(-1);
    }
}
