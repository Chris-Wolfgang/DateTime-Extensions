#if NETCOREAPP3_0_OR_GREATER

namespace Wolfgang.Extensions.DateTime.Tests.Unit.Performance;


using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

/// <summary>
/// Asserts that every public method allocates nothing.
/// </summary>
/// <remarks>
/// <para>
/// This is not an opt-in hot-path list: every method here takes a <see cref="DateTime"/>, does
/// integer arithmetic on it, and returns a <see cref="DateTime"/>. Nothing on the surface has a
/// reason to allocate, so the expected figure is zero bytes for all of them - which makes a
/// regression easy to state and easy to see.
/// </para>
/// <para>
/// Deliberately not reflection-driven, unlike the culture matrix: <c>MethodInfo.Invoke</c> allocates
/// an argument array and boxes the returned struct, which would swamp the measurement. The delegates
/// below are created once in a static field, so the measured window contains only the call.
/// </para>
/// <para>
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/> arrived in .NET Core 3.0 and has no .NET
/// Framework equivalent, so this file compiles out on the net4x slices rather than asserting
/// something weaker there.
/// </para>
/// </remarks>
public class AllocationTests
{
    private static readonly DateTime Sample = new DateTime(2026, 9, 24, 14, 30, 45, 678, DateTimeKind.Utc);



    /// <summary>
    /// Every public method, invoked the way a consumer would. Both overloads of the week methods
    /// are listed: the parameterless one reads the current culture, which is the one call on this
    /// surface that touches anything ambient, so it is the one most likely to start allocating.
    /// </summary>
    private static readonly (string Name, Func<DateTime> Call)[] Calls =
    {
        ("EndOfHalf()", () => Sample.EndOfHalf()),
        ("EndOfMonth()", () => Sample.EndOfMonth()),
        ("EndOfQuarter()", () => Sample.EndOfQuarter()),
        ("EndOfWeek()", () => Sample.EndOfWeek()),
        ("EndOfWeek(DayOfWeek)", () => Sample.EndOfWeek(DayOfWeek.Monday)),
        ("EndOfYear()", () => Sample.EndOfYear()),
        ("FirstOfHalf()", () => Sample.FirstOfHalf()),
        ("FirstOfMonth()", () => Sample.FirstOfMonth()),
        ("FirstOfQuarter()", () => Sample.FirstOfQuarter()),
        ("FirstOfWeek()", () => Sample.FirstOfWeek()),
        ("FirstOfWeek(DayOfWeek)", () => Sample.FirstOfWeek(DayOfWeek.Monday)),
        ("FirstOfYear()", () => Sample.FirstOfYear()),
        ("TruncateMilliseconds()", () => Sample.TruncateMilliseconds()),
        ("TruncateSeconds()", () => Sample.TruncateSeconds()),
    };



    [Fact]
    public void Every_public_method_allocates_zero_bytes()
    {
        var failures = new List<string>();

        foreach (var (name, call) in Calls)
        {
            // Warm up first. The very first call to a method pays for its JIT, and anything that
            // lands on the GC heap during that would be attributed to the measurement below.
            for (var warmup = 0; warmup < 16; warmup++)
            {
                call();
            }

            var before = GC.GetAllocatedBytesForCurrentThread();

            call();

            var after = GC.GetAllocatedBytesForCurrentThread();

            if (after != before)
            {
                failures.Add
                (
                    string.Format
                    (
                        CultureInfo.InvariantCulture,
                        "{0} allocated {1} byte(s)",
                        name,
                        after - before
                    )
                );
            }
        }

        Assert.Equal
        (
            Array.Empty<string>(),
            failures
        );
    }



    [Fact]
    public void The_measured_list_covers_the_whole_public_surface()
    {
        // A method added to the library without being listed above would never be measured, and
        // nothing else here would notice. The culture matrix enumerates the surface by reflection
        // for the same reason; this file cannot, so it counts instead.
        var methods = typeof(DateTimeExtensions).GetMethods
        (
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
        );

        Assert.Equal
        (
            methods.Length,
            Calls.Length
        );
    }
}

#endif
