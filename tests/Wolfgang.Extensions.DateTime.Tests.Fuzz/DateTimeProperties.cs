namespace Wolfgang.Extensions.DateTime.Tests.Fuzz;


using System;
using CsCheck;
using Xunit;

/// <summary>
/// Property-based fuzz tests over the whole public surface.
/// </summary>
/// <remarks>
/// <para>
/// The unit suite asserts chosen examples. These assert invariants over generated instants across
/// the entire representable range, which is where a date library actually breaks: the last tick of
/// a month, a leap day, the first and last representable instants, a quarter that ends in a
/// 30-day month.
/// </para>
/// <para>
/// Case count comes from <c>FUZZ_ITERATIONS</c> and the per-property time budget from
/// <c>FUZZ_TIME_SECONDS</c>, so the same properties serve as a fast per-PR pass and as the long
/// scheduled run. Defaults are deliberately small so <c>dotnet test</c> does not hang for anyone
/// running it by hand.
/// </para>
/// <para>
/// CsCheck rather than FsCheck: no F# interop, and its shrinking reports the smallest failing
/// instant rather than the first one it happened to generate.
/// </para>
/// <para>
/// <c>Sample</c> evaluates cases on several threads. Every property below is a pure function of its
/// generated input for that reason - a property that touched shared state, or the ambient culture,
/// would be racy rather than merely slow. That is also why the week properties always pass an
/// explicit <see cref="DayOfWeek"/> and never use the parameterless overloads, which read
/// <c>CultureInfo.CurrentCulture</c>; those are covered by the culture matrix in the unit suite.
/// </para>
/// </remarks>
public sealed class DateTimeProperties
{
    private const long MaxTicks = 3155378975999999999L;



    /// <summary>
    /// Instants across the whole representable range, with all three <see cref="DateTimeKind"/>
    /// values. Kind matters: every method is supposed to carry it through untouched.
    /// </summary>
    private static readonly Gen<System.DateTime> AnyInstant =
        Gen.Long[0, MaxTicks].Select
        (
            Gen.Int[0, 2],
            (ticks, kind) => new System.DateTime(ticks, (DateTimeKind)kind)
        );



    private static readonly Gen<DayOfWeek> AnyDayOfWeek = Gen.Int[0, 6].Select(day => (DayOfWeek)day);



    private static int FuzzIterations => GetEnvironmentInt("FUZZ_ITERATIONS", 1_000);



    private static int FuzzTimeSeconds => GetEnvironmentInt("FUZZ_TIME_SECONDS", -1);



    private static int GetEnvironmentInt(string name, int fallback)
    {
        var raw = Environment.GetEnvironmentVariable(name);

        return int.TryParse(raw, out var parsed) && parsed > 0 ? parsed : fallback;
    }



    [Fact]
    public void FirstOfMonth_is_midnight_on_the_first_of_the_same_month()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var actual = instant.FirstOfMonth();

                return actual.Year == instant.Year
                    && actual.Month == instant.Month
                    && actual.Day == 1
                    && actual.TimeOfDay == TimeSpan.Zero
                    && actual.Kind == instant.Kind
                    && actual <= instant;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void EndOfMonth_is_the_last_tick_of_the_same_month()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var actual = instant.EndOfMonth();

                return actual.Year == instant.Year
                    && actual.Month == instant.Month
                    && actual.Day == System.DateTime.DaysInMonth(instant.Year, instant.Month)
                    && actual.TimeOfDay == LastTickOfADay
                    && actual.Kind == instant.Kind
                    && actual >= instant;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void FirstOfYear_and_EndOfYear_bracket_the_instant_within_its_own_year()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var first = instant.FirstOfYear();
                var end = instant.EndOfYear();

                return first.Year == instant.Year
                    && first.Month == 1
                    && first.Day == 1
                    && first.TimeOfDay == TimeSpan.Zero
                    && end.Year == instant.Year
                    && end.Month == 12
                    && end.Day == 31
                    && end.TimeOfDay == LastTickOfADay
                    && first <= instant
                    && instant <= end;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void Quarters_start_in_January_April_July_or_October_and_contain_the_instant()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var first = instant.FirstOfQuarter();
                var end = instant.EndOfQuarter();
                var monthsIntoTheQuarter = instant.Month - first.Month;

                return (first.Month == 1 || first.Month == 4 || first.Month == 7 || first.Month == 10)
                    && first.Day == 1
                    && first.TimeOfDay == TimeSpan.Zero
                    && (end.Month == 3 || end.Month == 6 || end.Month == 9 || end.Month == 12)
                    && end.Day == System.DateTime.DaysInMonth(end.Year, end.Month)
                    && end.TimeOfDay == LastTickOfADay
                    && monthsIntoTheQuarter >= 0
                    && monthsIntoTheQuarter <= 2
                    && first <= instant
                    && instant <= end;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void Halves_start_in_January_or_July_and_contain_the_instant()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var first = instant.FirstOfHalf();
                var end = instant.EndOfHalf();
                var monthsIntoTheHalf = instant.Month - first.Month;

                return (first.Month == 1 || first.Month == 7)
                    && first.Day == 1
                    && first.TimeOfDay == TimeSpan.Zero
                    && (end.Month == 6 || end.Month == 12)
                    && end.Day == System.DateTime.DaysInMonth(end.Year, end.Month)
                    && end.TimeOfDay == LastTickOfADay
                    && monthsIntoTheHalf >= 0
                    && monthsIntoTheHalf <= 5
                    && first <= instant
                    && instant <= end;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    /// <summary>
    /// The week methods are the only ones with a documented clamp: the earliest representable
    /// instant has no earlier Sunday to walk back to, so <c>FirstOfWeek</c> returns
    /// <see cref="System.DateTime.MinValue"/> rather than throwing. The property encodes the clamp
    /// instead of excluding the range that triggers it.
    /// </summary>
    [Fact]
    public void FirstOfWeek_lands_on_the_requested_day_unless_it_clamps_to_MinValue()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;
                var actual = instant.FirstOfWeek(day);

                if (actual.Ticks == 0)
                {
                    return actual.Kind == instant.Kind && instant.Ticks < TimeSpan.TicksPerDay * 7;
                }

                var daysWalkedBack = (instant.Date - actual).Days;

                return actual.DayOfWeek == day
                    && actual.TimeOfDay == TimeSpan.Zero
                    && actual.Kind == instant.Kind
                    && actual <= instant
                    && daysWalkedBack >= 0
                    && daysWalkedBack <= 6;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    /// <summary>
    /// The mirror clamp: a week starting within seven days of the end of time cannot be a full
    /// week long, so <c>EndOfWeek</c> saturates at <see cref="System.DateTime.MaxValue"/>.
    /// </summary>
    [Fact]
    public void EndOfWeek_is_one_tick_short_of_seven_days_after_FirstOfWeek_unless_it_clamps()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;
                var first = instant.FirstOfWeek(day);
                var actual = instant.EndOfWeek(day);

                if (MaxTicks - first.Ticks < TimeSpan.TicksPerDay * 7)
                {
                    return actual.Ticks == MaxTicks && actual.Kind == instant.Kind;
                }

                return actual == first.AddDays(7).AddTicks(-1)
                    && actual.Kind == instant.Kind
                    && instant <= actual;
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void Truncations_clear_the_smaller_units_and_keep_the_larger_ones()
    {
        AnyInstant.Sample
        (
            instant =>
            {
                var milliseconds = instant.TruncateMilliseconds();
                var seconds = instant.TruncateSeconds();

                return milliseconds.Ticks % TimeSpan.TicksPerSecond == 0
                    && milliseconds.Date == instant.Date
                    && milliseconds.Hour == instant.Hour
                    && milliseconds.Minute == instant.Minute
                    && milliseconds.Second == instant.Second
                    && milliseconds.Kind == instant.Kind
                    && instant - milliseconds < TimeSpan.FromSeconds(1)
                    && seconds.Ticks % TimeSpan.TicksPerMinute == 0
                    && seconds.Date == instant.Date
                    && seconds.Hour == instant.Hour
                    && seconds.Minute == instant.Minute
                    && seconds.Kind == instant.Kind
                    && instant - seconds < TimeSpan.FromMinutes(1);
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    /// <summary>
    /// Every method is a projection onto a boundary, so applying it twice must change nothing.
    /// An off-by-one at a boundary usually shows up here first.
    /// </summary>
    [Fact]
    public void Every_method_is_idempotent()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;

                return instant.FirstOfMonth().FirstOfMonth() == instant.FirstOfMonth()
                    && instant.EndOfMonth().EndOfMonth() == instant.EndOfMonth()
                    && instant.FirstOfQuarter().FirstOfQuarter() == instant.FirstOfQuarter()
                    && instant.EndOfQuarter().EndOfQuarter() == instant.EndOfQuarter()
                    && instant.FirstOfHalf().FirstOfHalf() == instant.FirstOfHalf()
                    && instant.EndOfHalf().EndOfHalf() == instant.EndOfHalf()
                    && instant.FirstOfYear().FirstOfYear() == instant.FirstOfYear()
                    && instant.EndOfYear().EndOfYear() == instant.EndOfYear()
                    && instant.FirstOfWeek(day).FirstOfWeek(day) == instant.FirstOfWeek(day)
                    && instant.EndOfWeek(day).EndOfWeek(day) == instant.EndOfWeek(day)
                    && instant.TruncateMilliseconds().TruncateMilliseconds() == instant.TruncateMilliseconds()
                    && instant.TruncateSeconds().TruncateSeconds() == instant.TruncateSeconds();
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    /// <summary>
    /// Every method is monotonic: an earlier instant can never be projected past a later one.
    /// </summary>
    [Fact]
    public void Every_method_is_monotonic()
    {
        AnyInstant.Select(AnyInstant, AnyDayOfWeek, (left, right, day) => (left, right, day)).Sample
        (
            generated =>
            {
                var (left, right, day) = generated;

                var earlier = left <= right ? left : right;
                var later = left <= right ? right : left;

                return earlier.FirstOfMonth() <= later.FirstOfMonth()
                    && earlier.EndOfMonth() <= later.EndOfMonth()
                    && earlier.FirstOfQuarter() <= later.FirstOfQuarter()
                    && earlier.EndOfQuarter() <= later.EndOfQuarter()
                    && earlier.FirstOfHalf() <= later.FirstOfHalf()
                    && earlier.EndOfHalf() <= later.EndOfHalf()
                    && earlier.FirstOfYear() <= later.FirstOfYear()
                    && earlier.EndOfYear() <= later.EndOfYear()
                    && earlier.FirstOfWeek(day) <= later.FirstOfWeek(day)
                    && earlier.EndOfWeek(day) <= later.EndOfWeek(day)
                    && earlier.TruncateMilliseconds() <= later.TruncateMilliseconds()
                    && earlier.TruncateSeconds() <= later.TruncateSeconds();
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    private static TimeSpan LastTickOfADay => TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1);
}
