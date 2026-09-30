namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>
/// The AOT smoke harness is the only thing that exercises this library inside a trimmed,
/// natively-published binary, and the list of calls in it is maintained by hand. This asserts
/// that list still covers the whole public surface, so a method added later cannot quietly go
/// untested under AOT.
/// </summary>
/// <remarks>
/// <para>
/// The check deliberately lives here rather than inside <c>AotSmoke</c> itself. A reflection
/// guard running inside the published binary cannot work: the trimmer removes any public method
/// nothing calls, so <see cref="Type.GetMethods()"/> there would return only the methods already
/// exercised and the guard would pass vacuously for precisely the method it exists to catch.
/// Running untrimmed, as the unit suite does, is what makes the whole surface visible.
/// </para>
/// <para>
/// It reads the harness as text rather than referencing the project, because <c>AotSmoke</c> is
/// deliberately outside the solution - it is published on its own by <c>aot-smoke.yaml</c>.
/// </para>
/// </remarks>
public class AotSmokeCoverageTests
{

    [Fact]
    public void The_AOT_smoke_harness_calls_every_public_method()
    {
        var harness = File.ReadAllText(FindAotSmokeProgram(AppContext.BaseDirectory));

        var uncovered = typeof(DateTimeExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.DeclaringType == typeof(DateTimeExtensions))
            .Select(method => method.Name)
            .Distinct(StringComparer.Ordinal)
            .Where(name => harness.IndexOf("." + name + "(", StringComparison.Ordinal) < 0)
            .ToList();

        // Sorted with List.Sort rather than .OrderBy(name => name, ...): an OrderBy key selector
        // only runs when the sequence is non-empty, i.e. only when this guard is FAILING, so that
        // lambda is uncoverable while the harness is complete - and the suite is gated at 100%.
        uncovered.Sort(StringComparer.Ordinal);

        Assert.True
        (
            uncovered.Count == 0,
            $"tests/AotSmoke/Program.cs never calls {string.Join(", ", uncovered)}. "
                + "The AOT smoke harness is the only check that the library still behaves once "
                + "trimmed and natively published, so every public method has to appear there. "
                + "Add a Check(...) line with a value measured from a normal build."
        );
    }



    /// <summary>
    /// Walks up from <paramref name="startDirectory"/> looking for <c>tests/AotSmoke/Program.cs</c>.
    /// Deliberately not <c>[CallerFilePath]</c>, which bakes in a build-machine path that resolves
    /// to a deterministic-build placeholder under CI rather than a real one - the same reasoning
    /// the DocExamples harness records.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// No ancestor of <paramref name="startDirectory"/> contains <c>tests/AotSmoke/Program.cs</c>.
    /// </exception>
    internal static string FindAotSmokeProgram(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "tests", "AotSmoke", "Program.cs");

            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException
        (
            "No ancestor of '" + startDirectory + "' contains tests/AotSmoke/Program.cs."
        );
    }



    /// <summary>
    /// Covers the not-found path with a synthetic starting directory, rather than leaving it as an
    /// unreachable throw that the 100% test-coverage gate would then fail on.
    /// </summary>
    [Fact]
    public void FindAotSmokeProgram_when_no_ancestor_holds_the_harness_throws_FileNotFoundException()
    {
        // A directory that cannot exist, so the walk runs all the way to the filesystem root
        // without finding the harness. Path.GetPathRoot would be the obvious choice but returns
        // string?, and nullable is enabled here.
        var missing = Path.Combine(Path.GetTempPath(), "no-aot-smoke-" + Guid.NewGuid().ToString("N"));

        Assert.Throws<FileNotFoundException>
        (
            () => FindAotSmokeProgram(missing)
        );
    }
}
