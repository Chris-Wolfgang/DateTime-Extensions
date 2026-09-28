namespace Wolfgang.Extensions.DateTime.Tests.Unit.Globalization;


using Xunit;

/// <summary>
/// Collection for tests that change the ambient culture.
/// </summary>
/// <remarks>
/// <see cref="System.Globalization.CultureInfo.CurrentCulture"/> is ambient state. xunit runs test classes in parallel
/// by default, and a continuation can resume on a thread another test is using, so a test that
/// swaps the culture and one that reads it must not run at the same time. Parallelisation is
/// disabled for this collection rather than for the whole assembly, which would slow every other
/// test down to fix a problem they do not have.
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class CultureCollection
{
    public const string Name = "Culture";
}
