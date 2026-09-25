using System;
using System.Collections.Generic;
using System.Globalization;
using Wolfgang.Extensions.DateTime;

namespace AotSmoke;

/// <summary>
/// Calls every public method of <c>Wolfgang.Extensions.DateTime</c> once and checks the answer,
/// so that a native-AOT or trimmed publish that breaks the library fails here instead of in a
/// consumer's application.
/// </summary>
/// <remarks>
/// <para>
/// A publish that merely succeeds proves very little: the trimmer's failure mode is a method that
/// still exists and returns the wrong thing - or nothing - so every call is compared against a
/// value measured from a normal build.
/// </para>
/// <para>
/// Expectations are round-trip ("O") strings, which encode <see cref="DateTimeKind"/> as well as
/// the instant, so a lost <c>Kind</c> also fails.
/// </para>
/// </remarks>
internal static class Program
{
    // Kind is deliberately Utc: "O" renders it as the trailing Z, so a publish that loses the
    // Kind shows up as a mismatch rather than passing quietly.
    private static readonly System.DateTime Moment = new(2026, 9, 24, 14, 30, 45, 678, DateTimeKind.Utc);



    private static int Main()
    {
        // The parameterless FirstOfWeek/EndOfWeek read the current culture, so the fixed
        // expectations below are only meaningful against a known one. The invariant culture's
        // first day of the week is Sunday.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var failures = new List<string>();

        Check(failures, "EndOfHalf", Moment.EndOfHalf(), "2026-12-31T23:59:59.9999999Z");
        Check(failures, "EndOfMonth", Moment.EndOfMonth(), "2026-09-30T23:59:59.9999999Z");
        Check(failures, "EndOfQuarter", Moment.EndOfQuarter(), "2026-09-30T23:59:59.9999999Z");
        Check(failures, "EndOfWeek", Moment.EndOfWeek(), "2026-09-26T23:59:59.9999999Z");
        Check(failures, "EndOfWeek(Monday)", Moment.EndOfWeek(DayOfWeek.Monday), "2026-09-27T23:59:59.9999999Z");
        Check(failures, "EndOfYear", Moment.EndOfYear(), "2026-12-31T23:59:59.9999999Z");
        Check(failures, "FirstOfHalf", Moment.FirstOfHalf(), "2026-07-01T00:00:00.0000000Z");
        Check(failures, "FirstOfMonth", Moment.FirstOfMonth(), "2026-09-01T00:00:00.0000000Z");
        Check(failures, "FirstOfQuarter", Moment.FirstOfQuarter(), "2026-07-01T00:00:00.0000000Z");
        Check(failures, "FirstOfWeek", Moment.FirstOfWeek(), "2026-09-20T00:00:00.0000000Z");
        Check(failures, "FirstOfWeek(Monday)", Moment.FirstOfWeek(DayOfWeek.Monday), "2026-09-21T00:00:00.0000000Z");
        Check(failures, "FirstOfYear", Moment.FirstOfYear(), "2026-01-01T00:00:00.0000000Z");
        Check(failures, "TruncateMilliseconds", Moment.TruncateMilliseconds(), "2026-09-24T14:30:45.0000000Z");
        Check(failures, "TruncateSeconds", Moment.TruncateSeconds(), "2026-09-24T14:30:00.0000000Z");

        // A trimmed or AOT-published application that lost its culture data answers as though
        // every culture were invariant - and the parameterless overloads would then silently
        // return Sunday-based weeks to a German consumer. de-DE starting on Monday is the check
        // that the data is really there; ar-SA is included because ICU says Sunday for it, which
        // is easy to assume wrongly and would hide the same failure.
        CheckCulture(failures, "de-DE", DayOfWeek.Monday, "2026-09-21T00:00:00.0000000Z", "2026-09-27T23:59:59.9999999Z");
        CheckCulture(failures, "en-US", DayOfWeek.Sunday, "2026-09-20T00:00:00.0000000Z", "2026-09-26T23:59:59.9999999Z");
        CheckCulture(failures, "ar-SA", DayOfWeek.Sunday, "2026-09-20T00:00:00.0000000Z", "2026-09-26T23:59:59.9999999Z");

        if (failures.Count > 0)
        {
            Console.Error.WriteLine($"AOT smoke test FAILED with {failures.Count} problem(s):");

            foreach (var failure in failures)
            {
                Console.Error.WriteLine($"  {failure}");
            }

            return 1;
        }

        Console.WriteLine($"AOT smoke test passed on {RuntimeInformationSummary()}.");

        return 0;
    }



    private static void Check(List<string> failures, string name, System.DateTime actual, string expected)
    {
        var rendered = actual.ToString("O", CultureInfo.InvariantCulture);

        if (!string.Equals(rendered, expected, StringComparison.Ordinal))
        {
            failures.Add($"{name}: expected {expected}, got {rendered}");
        }
    }



    private static void CheckCulture
    (
        List<string> failures,
        string culture,
        DayOfWeek expectedFirstDayOfWeek,
        string expectedFirstOfWeek,
        string expectedEndOfWeek
    )
    {
        CultureInfo cultureInfo;

        try
        {
            cultureInfo = new CultureInfo(culture);
        }
        catch (CultureNotFoundException exception)
        {
            failures.Add($"{culture}: the culture is not available at all ({exception.Message})");

            return;
        }

        CultureInfo.CurrentCulture = cultureInfo;

        try
        {
            var actualFirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            if (actualFirstDayOfWeek != expectedFirstDayOfWeek)
            {
                failures.Add
                (
                    $"{culture}: FirstDayOfWeek is {actualFirstDayOfWeek}, expected {expectedFirstDayOfWeek} - "
                        + "culture data is missing or invariant-only in this published binary"
                );
            }

            Check(failures, $"{culture} FirstOfWeek", Moment.FirstOfWeek(), expectedFirstOfWeek);
            Check(failures, $"{culture} EndOfWeek", Moment.EndOfWeek(), expectedEndOfWeek);
        }
        finally
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }



    private static string RuntimeInformationSummary()
    {
        return $"{System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier} / "
            + $"{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";
    }
}
