namespace Wolfgang.Extensions.DateTime.Tests.Unit.Globalization;


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>
/// Runs every public method under hostile cultures and asserts that only the two methods
/// documented as culture-sensitive behave differently.
/// </summary>
/// <remarks>
/// <para>
/// The allowlist is <see cref="CultureSensitiveMethods"/>, and it is the same list the README
/// publishes. A method that starts reading the current culture without being added to it fails
/// <see cref="Methods_off_the_allowlist_are_culture_invariant"/>; a method added to the library
/// without a decision either way fails <see cref="The_allowlist_matches_the_public_surface"/>.
/// </para>
/// <para>
/// Both <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>
/// are swapped, and both are restored after every test - xunit constructs the class per test, so
/// <see cref="Dispose"/> runs each time. The collection disables parallelisation because the
/// current culture is ambient state that other tests would otherwise observe mid-change.
/// </para>
/// </remarks>
[Collection(CultureCollection.Name)]
public sealed class CultureInvarianceTests : IDisposable
{
    /// <summary>
    /// The only methods allowed to answer differently per culture, by name and parameter count.
    /// Both read <c>CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek</c> and delegate to
    /// their explicit-<see cref="DayOfWeek"/> overload, which is what
    /// <see cref="The_parameterless_week_overloads_read_only_the_first_day_of_week"/> pins down.
    /// </summary>
    private static readonly HashSet<string> CultureSensitiveMethods = new HashSet<string>
    (
        new[] { "FirstOfWeek/1", "EndOfWeek/1" },
        StringComparer.Ordinal
    );



    /// <summary>
    /// Deliberately awkward instants: a leap day, the last moment of a year, a month end that is
    /// also a quarter end, and one with sub-second precision to exercise the truncations.
    /// </summary>
    private static readonly DateTime[] Samples =
    {
        new DateTime(2024, 2, 29, 13, 45, 30, 123, DateTimeKind.Utc),
        new DateTime(2026, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
        new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified),
        new DateTime(2026, 1, 1, 12, 0, 0, 500, DateTimeKind.Local),
    };



    private readonly CultureInfo _originalCulture;



    private readonly CultureInfo _originalUiCulture;



    public CultureInvarianceTests()
    {
        _originalCulture = CultureInfo.CurrentCulture;
        _originalUiCulture = CultureInfo.CurrentUICulture;
    }



    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalUiCulture;
    }



    /// <summary>
    /// The cultures named in the maintenance issue this test set answers: Turkish for the dotted
    /// I, German for the decimal comma, Chinese for collation, Arabic for digit shaping and a
    /// non-Gregorian default calendar, Japanese for full-width digits. This library formats and
    /// parses nothing, so the interesting one is the calendar - but the point of the matrix is to
    /// stop that being an assumption.
    /// </summary>
    public static TheoryData<string> HostileCultures => new TheoryData<string>
    {
        "en-US",
        "tr-TR",
        "de-DE",
        "zh-CN",
        "ar-SA",
        "ja-JP",
    };



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Methods_off_the_allowlist_are_culture_invariant(string cultureName)
    {
        var expected = DescribeResults(CultureInfo.InvariantCulture);

        var actual = DescribeResults(new CultureInfo(cultureName));

        Assert.Equal
        (
            expected,
            actual
        );
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void The_parameterless_week_overloads_read_only_the_first_day_of_week(string cultureName)
    {
        var culture = new CultureInfo(cultureName);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

        foreach (var sample in Samples)
        {
            Assert.Equal
            (
                sample.FirstOfWeek(firstDayOfWeek),
                sample.FirstOfWeek()
            );

            Assert.Equal
            (
                sample.EndOfWeek(firstDayOfWeek),
                sample.EndOfWeek()
            );
        }
    }



    [Fact]
    public void The_allowlist_matches_the_public_surface()
    {
        var keys = PublicMethods().Select(Key).ToList();

        Assert.Equal
        (
            CultureSensitiveMethods.OrderBy(key => key, StringComparer.Ordinal),
            keys.Where(CultureSensitiveMethods.Contains).OrderBy(key => key, StringComparer.Ordinal)
        );

        Assert.True
        (
            keys.Count == 14,
            $"The public surface has {keys.Count} methods, not the 14 this test set was written against. "
                + "Decide whether the new one is culture-sensitive, then update CultureSensitiveMethods "
                + "and the README's culture-sensitivity table to match."
        );
    }



    private static IEnumerable<MethodInfo> PublicMethods()
    {
        return typeof(DateTimeExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(method => Key(method), StringComparer.Ordinal);
    }



    private static string Key(MethodInfo method)
    {
        return $"{method.Name}/{method.GetParameters().Length}";
    }



    /// <summary>
    /// Invokes every method NOT on the allowlist under <paramref name="culture"/> and renders the
    /// answers as strings, so a failure names the method and instant rather than only the count.
    /// </summary>
    private static List<string> DescribeResults(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        var described = new List<string>();

        foreach (var method in PublicMethods().Where(method => !CultureSensitiveMethods.Contains(Key(method))))
        {
            foreach (var sample in Samples)
            {
                // The explicit-DayOfWeek overloads take a second argument. Wednesday is
                // deliberately not any culture's first day, so a leak into it would show up.
                var arguments = method.GetParameters().Length == 1
                    ? new object[] { sample }
                    : new object[] { sample, DayOfWeek.Wednesday };

                var result = (DateTime)method.Invoke(null, arguments)!;

                described.Add
                (
                    $"{Key(method)}({sample.ToString("O", CultureInfo.InvariantCulture)}) = "
                        + $"{result.ToString("O", CultureInfo.InvariantCulture)} {result.Kind}"
                );
            }
        }

        return described;
    }
}
