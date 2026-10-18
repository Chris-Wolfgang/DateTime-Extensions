window.BENCHMARK_DATA = {
  "lastUpdate": 1792303993118,
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
      },
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
          "id": "4df090c8d4f7c61190f96632d791336e8078470b",
          "message": "ci: run local tools as pinned, raise the coverage floor to 95% (#461)\n\nscripts/build-pr.ps1: ReportGenerator and DevSkim now run via\n`dotnet <tool>` against the versions pinned in .config/dotnet-tools.json\n(5.5.11 / 1.0.90), matching how pr.yaml runs them, instead of a global\ninstall that silently fell back and could false-pass DevSkim when\n~/.dotnet/tools wasn't on PATH. Ported from Chris-Wolfgang/repo-template#656,\nadapted to this repo's existing structure rather than overwritten wholesale.\n\npr.yaml + build-pr.ps1: CODECOV_MINIMUM raised 90 -> 95, matching the\n2026-09-27 fleet coverage policy. No gaps to close first - every\nassembly already measures 100% locally (verified via the updated\nbuild-pr.ps1, all checks passed).\n\nThe Chris-Wolfgang/repo-template#654 sync (integration-project coverage\ngating) does not apply here - this repo has no *.Tests.Integration.*\nproject.\n\nPart of #459\n\nCo-authored-by: Claude Sonnet 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-28T23:08:42Z",
          "url": "https://github.com/Chris-Wolfgang/DateTime-Extensions/commit/4df090c8d4f7c61190f96632d791336e8078470b"
        },
        "date": 1790638538644,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      },
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
          "id": "462e1e379a608b9ea2f672304881c04fee2162c2",
          "message": "chore: bump PackageValidationBaselineVersion to 1.4.0 (#571)\n\nv1.4.0 is published and indexed on the flatcontainer CDN, so ApiCompat can now\ncompare future changes against it rather than against 1.3.2.\n\nDeliberately not part of the release PR: dotnet pack downloads the baseline, so\nbumping it before the CDN had 1.4.0 would have failed with NU1102.\n\nVerified by packing against the new baseline - it resolves and produces\nWolfgang.Extensions.DateTime.1.4.0.nupkg with no NU1102 and no PackageValidation\nerrors, which is the check that would have failed had this been opened early.\n\nRefs #530.\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-30T18:19:57Z",
          "url": "https://github.com/Chris-Wolfgang/DateTime-Extensions/commit/462e1e379a608b9ea2f672304881c04fee2162c2"
        },
        "date": 1791101103432,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      },
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
          "id": "ff402d6435945366323ef7567dffb118a6af7b25",
          "message": "ci: skip integration projects on Stage 2; coverlet excludes only AutoGeneratedProgram (#582)\n\n* ci: skip integration projects on Stage 2 and narrow coverlet excludes to AutoGeneratedProgram\n\nApplies Chris-Wolfgang/repo-template#701 (pr.yaml Stage 2 Windows skips\n*.Tests.Integration.* projects, keeping the TFM-not-covered warning) and\nChris-Wolfgang/repo-template#702 (coverlet.runsettings <Exclude> is only\n[*]AutoGeneratedProgram; test models and fixtures are measured).\n\nCo-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>\n\n* ci: Stage 2 skips the coverage steps when nothing collected coverage\n\nWith integration projects skipped on Stage 2 (repo-template#701), an\nintegration-only tree leaves no coverage file and an empty ran-assemblies\nledger; the old check-coverage step failed that case. Adopt repo-template\nmain's ledger-aware form: fail only when the ledger lists assemblies that\nran but no coverage file exists; otherwise set has-coverage=false.\n\nCo-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>\n\n---------\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-10-10T22:32:50Z",
          "url": "https://github.com/Chris-Wolfgang/DateTime-Extensions/commit/ff402d6435945366323ef7567dffb118a6af7b25"
        },
        "date": 1791699173918,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      },
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
          "id": "36b8e206d87c9a8687e6d170754dc3b75f80cc99",
          "message": "ci(scorecard): move the SARIF filter and upload out of the publishing job (#593)\n\nscorecard.dev rejects publish_results from a job containing a run: step\n(\"scorecard job must only have steps with uses\"), so results never reached\nthe API and the README badge read \"invalid repo path\". The analysis job now\nholds only uses: steps and hands results.sarif to a new upload job, which runs\nthe existing filter step unchanged and uploads to Code Scanning.\n\nRefs #590\n\nCo-authored-by: Claude Sonnet 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-10-15T17:37:55Z",
          "url": "https://github.com/Chris-Wolfgang/DateTime-Extensions/commit/36b8e206d87c9a8687e6d170754dc3b75f80cc99"
        },
        "date": 1792303991161,
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