namespace Wolfgang.Extensions.DateTime.Tests.DocExamples;

/// <summary>
/// Compiles every XML-doc &lt;example&gt;&lt;code&gt; block in the library's public
/// API against the real assembly, so a renamed/removed member the docs
/// still reference fails the build instead of drifting silently (#237).
/// </summary>
/// <remarks>
/// Limitation: only identifiers explicitly stubbed as typed fields on
/// <c>DocExampleContext</c> (in <c>DocExampleCompiler</c>) resolve.
/// If a future example introduces a new placeholder identifier, add a
/// typed field for it there — the compile error you'd otherwise see
/// (<c>CS0103: name does not exist in the current context</c>) is
/// indistinguishable from a real doc-rot failure without checking.
/// </remarks>
public sealed class DocExampleTests
{
    private static readonly IReadOnlyList<DocExample> Examples = DocExampleSource.ExtractAll();



    public static IEnumerable<object[]> ExampleCases() => Examples.Select(example => new object[] { example });



    [Theory]
    [MemberData(nameof(ExampleCases))]
    public void Example_compiles(DocExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        var errors = DocExampleCompiler.Compile(example);

        Assert.True
        (
            errors.Count == 0,
            $"{example} failed to compile:{Environment.NewLine}" +
            string.Join(Environment.NewLine, errors)
        );
    }



    [Fact]
    public void Example_compiles_when_code_contains_yield_uses_iterator_signature()
    {
        // Exercises BuildWrapperSource's "yield" branch directly. This library is
        // synchronous DateTime arithmetic, so no real example is an iterator and the
        // branch is unreachable through the Theory above.
        var example = new DocExample("synthetic.cs", 1, "yield return \"ok\";");

        var errors = DocExampleCompiler.Compile(example);

        Assert.Empty(errors);
    }



    [Fact]
    public void Example_compiles_when_code_has_no_await_or_yield_uses_sync_void_signature()
    {
        // The synchronous branch every real example here takes. Kept as a direct
        // test so a change to the branch order is caught by name rather than by a
        // confusing failure in the Theory above.
        var example = new DocExample("synthetic.cs", 1, "var x = 1 + 1;");

        var errors = DocExampleCompiler.Compile(example);

        Assert.Empty(errors);
    }



    [Fact]
    public void FindSrcDirectory_when_no_ancestor_has_a_src_directory_throws_DirectoryNotFoundException()
    {
        // A fresh temp subdirectory has no src/Wolfgang.Extensions.DateTime/
        // anywhere in its ancestor chain, unlike AppContext.BaseDirectory in every
        // real test run — this is the only way to reach the not-found path.
        var isolatedStart = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var ex = Assert.Throws<DirectoryNotFoundException>
        (
            () => DocExampleSource.FindSrcDirectory(isolatedStart)
        );

        Assert.Contains(isolatedStart, ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void ExtractAll_when_examples_exist_finds_at_least_the_known_count()
    {
        // Floor guard: if the extractor's <example>/<code> line-matching
        // ever breaks (e.g. a doc-comment format change it doesn't
        // handle), this fails LOUD instead of the Theory above silently
        // running zero cases and reporting a vacuous pass.
        Assert.True
        (
            Examples.Count >= 14,
            $"Expected at least 14 <example> blocks, found {Examples.Count}. " +
            "If this is a real removal, lower the floor deliberately — don't just delete this test."
        );
    }

    [Fact]
    public void Example_compiles_when_code_contains_await_uses_async_task_signature()
    {
        // Exercises BuildWrapperSource's "await" branch. Nothing in this library is
        // asynchronous, so no real example reaches it - but the branch exists and an
        // untested branch in the harness is a harness nobody can trust.
        var example = new DocExample("synthetic.cs", 1, "await Task.Yield();");

        var errors = DocExampleCompiler.Compile(example);

        Assert.Empty(errors);
    }



    [Fact]
    public void Compile_when_the_snippet_does_not_compile_reports_the_errors()
    {
        // The property that makes this whole project worth having: it must be able to
        // FAIL. A detector that only ever passes is indistinguishable from one that
        // checks nothing, which is exactly what this project would have been before
        // the library had any <example> blocks at all.
        var example = new DocExample("synthetic.cs", 1, "var x = NoSuchMethodExists();");

        var errors = DocExampleCompiler.Compile(example);

        Assert.NotEmpty(errors);
        Assert.Contains
        (
            errors,
            error => error.ToString().Contains("NoSuchMethodExists", StringComparison.Ordinal)
        );
    }

    [Fact]
    public void Compile_when_yield_appears_only_in_a_comment_uses_the_synchronous_signature()
    {
        // Regression guard for substring classification. Deciding the wrapper signature
        // by searching the raw text gave this snippet an IEnumerable<string> signature it
        // cannot satisfy - no yield statement, so "not all code paths return a value" -
        // and the example failed for a reason unrelated to the example. Classifying by
        // syntax sees no YieldStatementSyntax and picks the synchronous branch.
        var example = new DocExample("synthetic.cs", 1, "var x = 1; // yield and await, in a comment");

        var errors = DocExampleCompiler.Compile(example);

        Assert.Empty(errors);
    }
}
