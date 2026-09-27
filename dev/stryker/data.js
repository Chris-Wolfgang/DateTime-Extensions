window.BENCHMARK_DATA = {
  "lastUpdate": 1790489488108,
  "repoUrl": "https://github.com/Chris-Wolfgang/DateTime-Extensions",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "96114fbff682ad645735d7c73714ad184da10bbe",
          "message": "test: add an AOT/trim smoke consumer that checks answers, not just exit codes (#431)\n\nThe net8.0 and net10.0 assemblies are marked IsTrimmable and IsAotCompatible,\nwhich turns the analysers on but proves nothing about a published binary. The\ntrimmer's failure mode is not a link error: it is a method that still exists\nand returns the wrong thing, or a culture that quietly became invariant.\n\ntests/AotSmoke is a console consumer that calls all 14 public methods once\nand compares each answer against a value measured from a normal build. The\nexpectations are round-trip (\"O\") strings, so a lost DateTimeKind fails too -\nthe input is deliberately Utc, which \"O\" renders as the trailing Z.\n\nIt does NOT set InvariantGlobalization, unlike the equivalent harness in\nIAsyncEnumerable-Extensions. The parameterless FirstOfWeek/EndOfWeek read\nCultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek, so a published\nbinary without culture data would hand a German consumer Sunday-based weeks\nand raise nothing. The harness asserts de-DE starts on Monday to catch exactly\nthat, with en-US and ar-SA alongside - ar-SA because ICU says Sunday for it,\nwhich is easy to assume wrongly and would hide the same failure. The fixed\n14-case table pins CurrentCulture to invariant first, or those two overloads\nwould depend on the runner's locale.\n\nTwo things found by running it rather than by reading about it:\n\n  - PublishAot/PublishTrimmed must stay IN the project file. Passed as -p: on\n    the command line they become global properties, flow into the referenced\n    library, and fail NETSDK1124 on its net462 slice. The csproj says so where\n    someone would otherwise \"simplify\" it onto the publish command.\n  - Local verification stops at the native link step (\"Platform linker not\n    found\") without the C++ workload, but ILCompiler runs first and emitted\n    ZERO trim/AOT warnings for this library, and the harness passes on both\n    net8.0 and net10.0 as a normal build.\n\npr.yaml already skips non-test projects under tests/ (it names an AOT smoke\nexecutable as the case it is skipping), so this needs no change there and does\nnot trip the zero-tests guard. The workflow that publishes and runs it\nfollows separately - .github/workflows is a protected path.\n\nPart of #234\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-25T14:54:18Z",
          "url": "https://github.com/Chris-Wolfgang/DateTime-Extensions/commit/96114fbff682ad645735d7c73714ad184da10bbe"
        },
        "date": 1790489484117,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      }
    ]
  }
}