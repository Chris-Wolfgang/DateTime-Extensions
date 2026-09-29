namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>
/// Every public method must hand back the <see cref="DateTimeKind"/> it was given. The methods
/// are enumerated by reflection so a new one is covered without anyone remembering to add it,
/// and the samples reach the <see cref="DateTime.MinValue"/> / <see cref="DateTime.MaxValue"/>
/// clamp branches, which construct their result from raw ticks and are the easiest place to
/// drop the Kind.
/// </summary>
/// <remarks>
/// <see cref="DateTime.Equals(DateTime)"/> ignores <see cref="DateTime.Kind"/>, so the
/// value-equality assertions elsewhere in this suite cannot catch a lost Kind; this class is
/// the one that does.
/// </remarks>
public class KindPreservationTests
{
    /// <summary>
    /// A mid-range instant, one inside the year-1 window where the week methods clamp to
    /// MinValue, and one inside the year-9999 window where every End* method clamps to MaxValue.
    /// </summary>
    private static readonly long[] SampleTicks =
    {
        new DateTime(2026, 9, 28, 14, 30, 45, 123).Ticks,
        new DateTime(1, 1, 3, 12, 0, 0).Ticks,
        new DateTime(9999, 12, 28, 12, 0, 0).Ticks,
    };



    private static readonly DateTimeKind[] Kinds =
    {
        DateTimeKind.Utc,
        DateTimeKind.Local,
        DateTimeKind.Unspecified,
    };



    public static IEnumerable<object[]> Cases()
    {
        return
            from method in PublicMethods()
            from kind in Kinds
            from ticks in SampleTicks
            select new object[] { method.Name, method.GetParameters().Length, kind, ticks };
    }



    [Theory]
    [MemberData(nameof(Cases))]
    public void Every_public_method_preserves_the_Kind_of_its_input(string methodName, int parameterCount, DateTimeKind kind, long ticks)
    {
        var input = new DateTime(ticks, kind);
        var method = PublicMethods().Single
        (
            candidate => string.Equals(candidate.Name, methodName, StringComparison.Ordinal) && candidate.GetParameters().Length == parameterCount
        );

        // Saturday puts 0001-01-03 (a Wednesday) into the MinValue clamp and
        // 9999-12-28 (a Tuesday) into the MaxValue clamp for the week methods.
        var arguments = parameterCount == 1
            ? new object[] { input }
            : new object[] { input, DayOfWeek.Saturday };

        var result = (DateTime)method.Invoke(null, arguments)!;

        Assert.Equal(kind, result.Kind);
    }



    [Fact]
    public void Cases_cover_the_whole_public_surface()
    {
        var methods = PublicMethods().ToList();

        Assert.Equal(14, methods.Count);
        Assert.Equal
        (
            methods.Count * Kinds.Length * SampleTicks.Length,
            Cases().Count()
        );
    }



    private static IEnumerable<MethodInfo> PublicMethods()
    {
        return typeof(DateTimeExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.ReturnType == typeof(DateTime))
            .OrderBy(method => method.Name, StringComparer.Ordinal)
            .ThenBy(method => method.GetParameters().Length);
    }
}
