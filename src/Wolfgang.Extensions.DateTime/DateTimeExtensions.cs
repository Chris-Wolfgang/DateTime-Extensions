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
    /// <see cref="FirstOfYear"/>, <see cref="FirstOfQuarter"/>,
    /// <see cref="FirstOfHalf"/>. The week methods work on ticks instead.
    /// </summary>
    private static System.DateTime MidnightOf(int year, int month, int day, DateTimeKind kind)
        => new(year, month, day, 0, 0, 0, 0, kind);



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
        => new(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond), dateTime.Kind);



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
        => new(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerMinute), dateTime.Kind);



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
    /// Returns a new DateTime that represents the last tick of the
    /// month specified by the DateTime passed in. Clamps at
    /// <see cref="System.DateTime.MaxValue"/> when the month is December
    /// of year 9999.
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
    /// Returns a new DateTime that represents the last tick of the
    /// year specified by the DateTime passed in. Clamps at
    /// <see cref="System.DateTime.MaxValue"/> when the year is 9999.
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
    /// <param name="firstDayOfWeek">Specifies the first day of the week. Use the parameterless overload to pick up <see cref="CultureInfo.CurrentCulture"/>'s value automatically.</param>
    /// <returns>A new DateTime representing the first of the week.</returns>
    /// <remarks>
    /// Within the first six days of year 1 - <c>0001-01-01</c> through <c>0001-01-06</c> - walking
    /// back to <paramref name="firstDayOfWeek"/> can underflow the representable range. Rather
    /// than throw, this method clamps to <see cref="System.DateTime.MinValue"/>, which is itself
    /// a Monday and so is not guaranteed to fall on <paramref name="firstDayOfWeek"/>. See
    /// <see cref="EndOfWeek(System.DateTime, DayOfWeek)"/>'s remarks for the consequence.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="firstDayOfWeek"/> is not a defined <see cref="DayOfWeek"/> value.
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
        if (firstDayOfWeek < DayOfWeek.Sunday || firstDayOfWeek > DayOfWeek.Saturday)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(firstDayOfWeek),
                firstDayOfWeek,
                "Value must be a defined DayOfWeek."
            );
        }

        var date = dateTime.Date;
        var daysBack = ((int)date.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var ticksBack = daysBack * TimeSpan.TicksPerDay;

        // Math.Max is the MinValue clamp: a week start before 0001-01-01 is not
        // representable, so the earliest instant stands in for it (see remarks).
        return new System.DateTime(Math.Max(0L, date.Ticks - ticksBack), dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the last tick of the
    /// week specified by the DateTime passed in using the thread's
    /// CurrentCulture FirstDayOfWeek. Clamps at
    /// <see cref="System.DateTime.MaxValue"/> when the week would extend
    /// past the end of the representable range.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the week.</returns>
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
    /// Returns a new DateTime that represents the last tick of the
    /// week specified by the DateTime passed in using the specified
    /// firstDayOfWeek. Clamps at <see cref="System.DateTime.MaxValue"/>
    /// when the week would extend past the end of the representable range.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <param name="firstDayOfWeek">Specifies the first day of the week. Use the parameterless overload to pick up <see cref="CultureInfo.CurrentCulture"/>'s value automatically.</param>
    /// <returns>A new DateTime representing the end of the week.</returns>
    /// <remarks>
    /// <para>
    /// Not idempotent within the first six days of year 1 - <c>0001-01-01</c> through
    /// <c>0001-01-06</c>. This method computes seven days from
    /// <see cref="FirstOfWeek(System.DateTime, DayOfWeek)"/>'s result, but that result clamps to
    /// <see cref="System.DateTime.MinValue"/> rather than underflowing, and <c>MinValue</c> does
    /// not necessarily fall on <paramref name="firstDayOfWeek"/> (see its remarks). Calling
    /// <c>EndOfWeek</c> a second time, on the first call's own result, can therefore return a
    /// later value than the first call did.
    /// </para>
    /// <para>
    /// No plausible application reaches this - it needs an instant in the first six days of
    /// year 1 - so the behaviour is documented rather than changed. Deliberate, see
    /// <see href="https://github.com/Chris-Wolfgang/DateTime-Extensions/issues/436">#436</see>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="firstDayOfWeek"/> is not a defined <see cref="DayOfWeek"/> value.
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
        var firstOfWeek = dateTime.FirstOfWeek(firstDayOfWeek);
        var maxTicks = System.DateTime.MaxValue.Ticks;
        var sevenDaysTicks = 7 * TimeSpan.TicksPerDay;

        // Stryker disable once Equality : equivalent mutant. firstOfWeek is
        // always midnight (firstOfWeek.Ticks is a multiple of TicksPerDay),
        // while MaxValue.Ticks ≡ -1 (mod TicksPerDay). The difference
        // (maxTicks - firstOfWeek.Ticks) therefore always equals
        // n * TicksPerDay - 1 for some integer n ≥ 0 — never an exact
        // multiple of TicksPerDay, and in particular never equal to
        // sevenDaysTicks (= 7 * TicksPerDay). The equality boundary
        // `< sevenDaysTicks` vs `<= sevenDaysTicks` is therefore
        // unreachable through the public API and both forms produce
        // identical output. The invariant is asserted by
        // EndOfWeek_seven_day_boundary_returns_firstOfWeek_plus_7_minus_1_tick.
        return maxTicks - firstOfWeek.Ticks < sevenDaysTicks
            ? new System.DateTime(maxTicks, dateTime.Kind)
            : firstOfWeek.AddDays(7).AddTicks(-1);
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
