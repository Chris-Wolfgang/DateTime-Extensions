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
    /// <see cref="FirstOfHalf"/>, and the explicit-DayOfWeek overload of
    /// <c>FirstOfWeek</c> (the parameterless overload just delegates to
    /// that one via the current culture's <c>FirstDayOfWeek</c>).
    /// </summary>
    private static System.DateTime MidnightOf(int year, int month, int day, DateTimeKind kind)
        => new(year, month, day, 0, 0, 0, 0, kind);



    /// <summary>
    /// Remove the milliseconds and everything after the milliseconds
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime equal to the passed in value without milliseconds</returns>
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
    public static System.DateTime FirstOfMonth(this System.DateTime dateTime)
        => MidnightOf(dateTime.Year, dateTime.Month, 1, dateTime.Kind);



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// month specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the month.</returns>
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
    public static System.DateTime FirstOfYear(this System.DateTime dateTime)
        => MidnightOf(dateTime.Year, 1, 1, dateTime.Kind);



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// year specified by the DateTime passed in.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the year.</returns>
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
    public static System.DateTime FirstOfWeek(this System.DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        var firstOfWeek = dateTime.Date;
        while (firstOfWeek.DayOfWeek != firstDayOfWeek)
        {
            if (firstOfWeek == System.DateTime.MinValue.Date)
            {
                return new System.DateTime(System.DateTime.MinValue.Ticks, dateTime.Kind);
            }

            firstOfWeek = firstOfWeek.AddDays(-1);
        }

        return MidnightOf(firstOfWeek.Year, firstOfWeek.Month, firstOfWeek.Day, dateTime.Kind);
    }



    /// <summary>
    /// Returns a new DateTime that represents the last day of the
    /// week specified by the DateTime passed in using the thread's
    /// CurrentCulture FirstDayOfWeek.
    /// </summary>
    /// <param name="dateTime">The value to process.</param>
    /// <returns>A new DateTime representing the end of the week.</returns>
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
    public static System.DateTime EndOfWeek(this System.DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        var firstOfWeek = dateTime.FirstOfWeek(firstDayOfWeek);
        var maxTicks = System.DateTime.MaxValue.Ticks;
        var sevenDaysTicks = TimeSpan.FromDays(7).Ticks;

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
    public static System.DateTime EndOfHalf(this System.DateTime dateTime)
    {
        var firstOfHalf = dateTime.FirstOfHalf();

        return firstOfHalf.Month == 7 && firstOfHalf.Year == 9999
            ? new System.DateTime(System.DateTime.MaxValue.Ticks, dateTime.Kind)
            : firstOfHalf.AddMonths(6).AddTicks(-1);
    }
}
