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
    /// <remarks>
    /// Deliberately biased towards the two ends. The interesting behaviour lives in the first and
    /// last week of the range, and a uniform draw lands there with probability about 4e-6 - so a
    /// short per-PR run would never exercise a boundary at all, and the branches that handle them
    /// would go untested while the suite looked thorough.
    /// </remarks>
    private static readonly Gen<System.DateTime> AnyInstant = Gen.Frequency
    (
        (80, InstantsBetween(0, MaxTicks)),
        (10, InstantsBetween(0, 14 * TimeSpan.TicksPerDay)),
        (10, InstantsBetween(MaxTicks - (14 * TimeSpan.TicksPerDay), MaxTicks))
    );



    private static readonly Gen<DayOfWeek> AnyDayOfWeek = Gen.Int[0, 6].Select(day => (DayOfWeek)day);



    private static Gen<System.DateTime> InstantsBetween(long fromTicks, long toTicks)
    {
        return Gen.Long[fromTicks, toTicks].Select
        (
            Gen.Int[0, 2],
            (ticks, kind) => new System.DateTime(ticks, (DateTimeKind)kind)
        );
    }



    /// <summary>
    /// The first instant whose week starts inside the representable range for every alignment.
    /// 0001-01-01 is a Monday, so by 0001-01-08 every day of the week has occurred at least once.
    /// </summary>
    private static readonly System.DateTime FirstFullyRepresentableWeek = new(1, 1, 8);



    /// <summary>
    /// The mirror: the last instant whose week ends inside the representable range for every
    /// alignment. Anything later can need up to seven more days than the range holds.
    /// </summary>
    private static readonly System.DateTime LastFullyRepresentableWeek = new(9999, 12, 24, 23, 59, 59);



    /// <summary>
    /// Both week boundaries for <paramref name="instant"/>, or false when either is
    /// unrepresentable and the method throws instead of answering.
    /// </summary>
    private static bool TryWeek
    (
        System.DateTime instant,
        DayOfWeek day,
        out System.DateTime first,
        out System.DateTime end
    )
    {
        first = default;
        end = default;

        try
        {
            first = instant.FirstOfWeek(day);
            end = instant.EndOfWeek(day);

            return true;
        }
        catch (OverflowException)
        {
            return false;
        }
    }



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
    /// A week boundary is unrepresentable only at the very ends of the range, and each method owns
    /// one end: <c>FirstOfWeek</c> throws <see cref="OverflowException"/> only inside the first
    /// seven days of year 1, <c>EndOfWeek</c> only inside the last seven of year 9999.
    /// </summary>
    /// <remarks>
    /// The window is asserted as a DATE RANGE rather than by recomputing the implementation's own
    /// arithmetic, which would only prove that the code agrees with itself. The exact boundary
    /// pairs - 21 at each end - are pinned by name in the unit suite.
    /// </remarks>
    [Fact]
    public void FirstOfWeek_lands_on_the_requested_day_or_throws_only_in_the_first_week_of_year_1()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;

                System.DateTime actual;

                try
                {
                    actual = instant.FirstOfWeek(day);
                }
                catch (OverflowException)
                {
                    return instant < FirstFullyRepresentableWeek;
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



    [Fact]
    public void EndOfWeek_is_a_full_week_after_FirstOfWeek_or_throws_only_at_the_end_of_time()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;

                System.DateTime actual;

                try
                {
                    actual = instant.EndOfWeek(day);
                }
                catch (OverflowException)
                {
                    return instant > LastFullyRepresentableWeek;
                }

                // Near MinValue a week's START can be unrepresentable while its end is not, and
                // EndOfWeek must still answer there - that is what the two implementations being
                // independent buys. So the cross-check against FirstOfWeek is conditional on
                // FirstOfWeek having an answer, not the other way round.
                try
                {
                    // Ticks, not AddDays(7).AddTicks(-1): for a week ending exactly on
                    // DateTime.MaxValue the intermediate "+ 7 days" is 10000-01-01 and
                    // DateTime.AddDays throws before the tick comes back off. Found by this very
                    // property at 9999-12-25, which is a legitimate input the library answers fine.
                    return actual.Kind == instant.Kind
                        && instant <= actual
                        && actual.Ticks == instant.FirstOfWeek(day).Ticks + (7 * TimeSpan.TicksPerDay) - 1;
                }
                catch (OverflowException)
                {
                    return instant < FirstFullyRepresentableWeek;
                }
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    /// <summary>
    /// An undefined <see cref="DayOfWeek"/> is the caller's mistake at every instant, and is
    /// reported as one - never as an <see cref="OverflowException"/>, and never answered.
    /// </summary>
    [Fact]
    public void Week_methods_reject_a_DayOfWeek_that_is_not_one_of_the_seven_values()
    {
        var undefinedDays = Gen.Int[-500, 500].Where(value => value < 0 || value > 6);

        AnyInstant.Select(undefinedDays, (instant, value) => (instant, value)).Sample
        (
            generated =>
            {
                var (instant, value) = generated;
                var day = (DayOfWeek)value;

                // Assert.Throws rather than try/catch: a property that passes never executes its
                // own `return false`, and an unexecutable line cannot meet the 100% coverage bar
                // this repository holds test assemblies to.
                var first = Assert.Throws<ArgumentOutOfRangeException>(() => instant.FirstOfWeek(day));
                var end = Assert.Throws<ArgumentOutOfRangeException>(() => instant.EndOfWeek(day));

                return string.Equals(first.ParamName, "firstDayOfWeek", StringComparison.Ordinal)
                    && string.Equals(end.ParamName, "firstDayOfWeek", StringComparison.Ordinal);
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
    /// <remarks>
    /// This property found issue #436: <c>EndOfWeek</c> used to be built on <c>FirstOfWeek</c> and
    /// inherited a clamp that misaligned the week, so applying it twice moved. With the two
    /// implementations independent there is no exception to carve out - the week methods are
    /// idempotent wherever they answer at all, and the <c>TryWeek</c> guard below only skips the
    /// inputs for which a boundary is unrepresentable and an exception is the answer.
    /// </remarks>
    [Fact]
    public void Every_method_is_idempotent()
    {
        AnyInstant.Select(AnyDayOfWeek, (instant, day) => (instant, day)).Sample
        (
            generated =>
            {
                var (instant, day) = generated;

                var weekIsRepresentable = TryWeek(instant, day, out var first, out var end);

                return (!weekIsRepresentable
                        || (first.FirstOfWeek(day) == first && end.EndOfWeek(day) == end))
                    && instant.FirstOfMonth().FirstOfMonth() == instant.FirstOfMonth()
                    && instant.EndOfMonth().EndOfMonth() == instant.EndOfMonth()
                    && instant.FirstOfQuarter().FirstOfQuarter() == instant.FirstOfQuarter()
                    && instant.EndOfQuarter().EndOfQuarter() == instant.EndOfQuarter()
                    && instant.FirstOfHalf().FirstOfHalf() == instant.FirstOfHalf()
                    && instant.EndOfHalf().EndOfHalf() == instant.EndOfHalf()
                    && instant.FirstOfYear().FirstOfYear() == instant.FirstOfYear()
                    && instant.EndOfYear().EndOfYear() == instant.EndOfYear()
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

                // Only comparable when both instants have both boundaries; at the ends of the
                // range one of them throws instead, which the two properties above cover.
                // Both calls run unconditionally: short-circuiting the second would leave its
                // out parameters unassigned as far as the compiler is concerned.
                var earlierIsRepresentable = TryWeek(earlier, day, out var earlierFirst, out var earlierEnd);
                var laterIsRepresentable = TryWeek(later, day, out var laterFirst, out var laterEnd);
                var bothRepresentable = earlierIsRepresentable && laterIsRepresentable;

                return (!bothRepresentable
                        || (earlierFirst <= laterFirst && earlierEnd <= laterEnd))
                    && earlier.FirstOfMonth() <= later.FirstOfMonth()
                    && earlier.EndOfMonth() <= later.EndOfMonth()
                    && earlier.FirstOfQuarter() <= later.FirstOfQuarter()
                    && earlier.EndOfQuarter() <= later.EndOfQuarter()
                    && earlier.FirstOfHalf() <= later.FirstOfHalf()
                    && earlier.EndOfHalf() <= later.EndOfHalf()
                    && earlier.FirstOfYear() <= later.FirstOfYear()
                    && earlier.EndOfYear() <= later.EndOfYear()
                    && earlier.TruncateMilliseconds() <= later.TruncateMilliseconds()
                    && earlier.TruncateSeconds() <= later.TruncateSeconds();
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    private static TimeSpan LastTickOfADay => TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1);
}
