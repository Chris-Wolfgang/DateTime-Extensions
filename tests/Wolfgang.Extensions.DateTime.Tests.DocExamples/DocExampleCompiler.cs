using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Wolfgang.Extensions.DateTime.Tests.DocExamples;

/// <summary>
/// Compiles an extracted &lt;example&gt;&lt;code&gt; snippet against the real library
/// assembly inside a neutral-context harness, so a diagnostic means the
/// example genuinely no longer compiles — not that the harness is missing
/// a stub.
/// </summary>
public static class DocExampleCompiler
{
    /// <summary>
    /// Only the identifiers the examples actually reference need a typed
    /// field here (see docs/adr or the source XML docs for the full set).
    /// Only their TYPES matter — the harness class is compiled, never
    /// instantiated or run.
    /// </summary>
    private const string HarnessPreamble = """
        using System;
        using System.Globalization;
        using System.Threading.Tasks;
        using Wolfgang.Extensions.DateTime;

        namespace DocExamplesGenerated;

        // Deliberately NOT inside Wolfgang.Extensions.DateTime. A consumer writes plain
        // `DateTime`, and that only resolves outside this library's own namespace - see
        // ADR-0002. Compiling the examples here is therefore the same check a consumer
        // would get, which is the point.
        public abstract class DocExampleContext
        {
        }

        """;



    /// <summary>
    /// Compiles <paramref name="example"/>. Returns every
    /// <see cref="DiagnosticSeverity.Error"/> diagnostic (warnings are
    /// ignored — the example doesn't need to be warning-clean, only
    /// correct).
    /// </summary>
    public static IReadOnlyList<Diagnostic> Compile(DocExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        var wrapperSource = BuildWrapperSource(example);
        var tree = CSharpSyntaxTree.ParseText(wrapperSource, path: "wrapper.cs");

        var compilation = CSharpCompilation.Create(
            assemblyName: "DocExampleCompilation",
            syntaxTrees: [tree],
            references: BuildReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return
        [
            .. compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
        ];
    }



    private static string BuildWrapperSource(DocExample example)
    {
        string signature;
        if (example.Code.Contains("yield", StringComparison.Ordinal))
        {
            signature = "System.Collections.Generic.IEnumerable<string> RunAsync()";
        }
        else if (example.Code.Contains("await", StringComparison.Ordinal))
        {
            signature = "async Task RunAsync()";
        }
        else
        {
            signature = "void RunAsync()";
        }

        // Forward slashes only — #line paths with backslashes are fragile
        // across tooling (see the release.yaml manifest-step gotcha
        // recorded in reference_thorough_review_impl_patterns).
        var normalizedPath = example.FilePath.Replace('\\', '/');

        return $$"""
            {{HarnessPreamble}}
            public sealed class DocExampleHarness : DocExampleContext
            {
                public {{signature}}
                {
            #line {{example.FirstCodeLine}} "{{normalizedPath}}"
            {{example.Code}}
            #line default
                }
            }
            """;
    }



    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        var paths = new HashSet<string>(trustedAssembliesPaths, StringComparer.OrdinalIgnoreCase)
        {
            typeof(DateTimeExtensions).Assembly.Location
        };

        return [.. paths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
    }
}
