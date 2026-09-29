# Release Workflow Setup Guide

This guide describes what `.github/workflows/release.yaml` does when a GitHub Release is published,
the one-time configuration it depends on, and how to verify and troubleshoot a release. It is
written for this repository; the same shape applies to every repository created from the template.

## Overview

`release.yaml` triggers on **`release: published`** and runs a validation-then-publish pipeline that:

- rebuilds the tagged commit and checks that the tag matches the `<Version>` in the source project;
- runs every test project on every target framework it declares (Windows runner, so the .NET
  Framework 4.6.2–4.8.1 slices run too) with the same coverage gate as `pr.yaml`
  (`CODECOV_MINIMUM` for production assemblies, 100 % for test assemblies);
- packs the NuGet package, smoke-installs it into a fresh console project, generates the third-party
  notices, a CycloneDX SBOM and the reproducible-build manifest;
- verifies that the DocFX site builds before anything is published;
- publishes to NuGet.org through **OIDC trusted publishing** — there is no stored API key — and
  records a SLSA build-provenance attestation for the package;
- deploys the versioned documentation to GitHub Pages and attaches the packages, provenance bundle
  and coverage report to the GitHub Release.

Releases are serialised (`concurrency: release-<tag>`, never cancelled), so a re-published Release
cannot run two publish pipelines against NuGet and gh-pages at once.

## Required configuration

### NuGet.org trusted publishing (no secret)

`publish-nuget` signs in with `NuGet/login` using the job's OIDC token and pushes with the
short-lived key that action returns. Nothing is stored in repository secrets; the only `secrets.*`
any workflow here reads are `GITHUB_TOKEN` and `SECURITY_ALERTS_TOKEN` (the latter for
`security-alerts.yml`, unrelated to releases).

One-time setup on nuget.org, once per repository:

1. Sign in to nuget.org as the package owner (`Chris-Wolfgang`, the `user:` the login step passes).
2. Account → **Trusted Publishing** → add a policy for the GitHub repository
   `Chris-Wolfgang/DateTime-Extensions` with workflow file `release.yaml` (no environment).
3. For a brand-new package id, the first push has to be made by an owner the policy resolves to; after
   that the policy covers every version.

`publish-nuget` needs `id-token: write` (OIDC) and `attestations: write` (provenance); both are
declared on that job only.

### GitHub Pages

`trigger-docs` calls `docfx.yaml`, which pushes the built site to the `gh-pages` branch. Pages must be
configured to serve from `gh-pages` (root): Settings → Pages → Source "Deploy from a branch",
branch `gh-pages`, folder `/ (root)`. This repository already is.

### Branch ruleset on `main`

`Settings → Rules → Rulesets → Protect main branch`. The release itself does not read the ruleset,
but everything a release ships must have crossed it. As configured today it requires a pull
request, linear history, and these status checks:

- `Stage 1: Linux Tests (.NET 5.0-10.0) + Coverage Gate`
- `Stage 2: Windows Tests (.NET 5.0-10.0, Framework 4.6.2-4.8.1)`
- `Stage 3: macOS Tests (.NET 6.0-10.0)`
- `Security Scan (DevSkim)`, `Security Scan (CodeQL) (csharp)`, `Secrets Scan (gitleaks)`
- `Detect .NET Projects`, `Protected Files Guard`
- `Native AOT publish + run (linux-x64, net8.0)` and `(linux-x64, net10.0)`
- `zizmor`, `actionlint`, `ReSharper InspectCode`

Deletions and force-pushes are blocked. Fleet-wide ruleset changes are made with the scripts in the
`claude-sessions` repository, not from this one.

## Cutting a release

1. Every change since the last release has a fragment in `changelog/unreleased/` (the
   `Changelog Fragment Check` enforces this for PRs that touch `src/`).
2. On the release branch, run `pwsh ./scripts/changelog.ps1 bump` to see the version the fragments
   imply, then `pwsh ./scripts/changelog.ps1 assemble` to fold them into `CHANGELOG.md`.
3. Set `<Version>` in `src/Wolfgang.Extensions.DateTime/Wolfgang.Extensions.DateTime.csproj` to that
   version. Leave `<PackageValidationBaselineVersion>` at the **last published** version — it is bumped
   in a follow-up PR after the new package is on NuGet.org (ADR-0004).
4. Merge the release PR to `main`, then create the tag `vX.Y.Z` on that commit and publish a GitHub
   Release for it. `validate-release` fails immediately if the tag and `<Version>` disagree.

A test run can use a pre-release tag (for example `v0.0.1-test`, marked "pre-release"); the pipeline
runs identically.

## What the workflow does, job by job

```
release: published
        │
        ▼
validate-release (windows)        tag == <Version>; restore; build Release; every test
        │                          project × every TFM; coverage gate (src ≥ CODECOV_MINIMUM,
        │                          tests = 100 %); upload coverage report
        ▼
pack-and-validate (windows)       build; third-party notices; dotnet pack; smoke-install the
        │                          .nupkg into a net8.0 console app; CycloneDX SBOM;
        │                          reproducible-build manifest; upload packages
        ▼
verify-docs-build (windows)       docfx metadata + build, no deploy - a docs failure blocks
        │                          publishing instead of landing after the package is live
        ▼
publish-nuget (windows)           attest build provenance; NuGet/login (OIDC); dotnet nuget push
        │                          --skip-duplicate
        ├──────────────► trigger-docs        docfx.yaml: build and deploy versions/<tag> + latest
        │                                     to gh-pages (only after a successful publish)
        └──────────────► update-release-artifacts (ubuntu)
                                              attach .nupkg/.snupkg, the provenance bundle and
                                              the zipped coverage report to the Release
```

Every job except `trigger-docs` (a reusable-workflow call) carries a `timeout-minutes`.

## Monitoring

- **Actions tab** → `Release on Published Release` shows the run; each job's log names the step that
  failed.
- **Artifacts**: `release-coverage`, `nuget-packages`, `provenance-bundle`.
- **Releases page**: assets appear when `update-release-artifacts` finishes.
- **NuGet.org**: the package is indexed a few minutes after `publish-nuget`; the flat-container CDN
  can lag by 20–30 minutes, so a `dotnet add package` immediately afterwards may still see the old
  version.

## Troubleshooting

### "Release tag does not match csproj version"

`validate-release` compares the tag (without the leading `v`) to `<Version>`. Fix `<Version>` on
`main`, delete the Release and the tag, and publish again — do not re-run the failed workflow, it
would still build the old commit.

### Tests fail on one framework

Read the job log for the TFM named in the failure and reproduce locally with
`dotnet test tests/<project> -c Release -f <tfm>`. The .NET Framework slices only run on Windows.

### "Zero tests ran for <tfm>"

`dotnet test` exits 0 when the adapter finds no tests; the guard fails the job instead. Usually the
`xunit.runner.visualstudio` version pinned for that TFM in the test csproj no longer supports it.

### Coverage gate failed / "ran but produced no coverage row"

Open the `release-coverage` artifact (`Summary.txt`). A module below its threshold needs tests; a test
assembly that ran but has no row means coverage collection broke for it — check that
`coverlet.collector` is still referenced and `coverlet.runsettings` still includes test assemblies.

### `publish-nuget` fails with 401/403

The trusted-publishing policy on nuget.org does not match this repository or workflow file, or the
package id is owned by a different account. Check Account → Trusted Publishing on nuget.org. There is
no `NUGET_API_KEY` secret to check; the workflow never had one.

### Smoke test fails to install the package

The `.nupkg` packed but a fresh net8.0 console project could not restore it. Look at the
`pack-and-validate` log for the NuGet error; typical causes are a dependency that is not on
NuGet.org yet or a `<TargetFrameworks>` change that removed the asset the consumer needed.

### Docs did not deploy

`trigger-docs` runs only after `publish-nuget` succeeded. If the package is live but the site is
stale, re-run the `Deploy DocFX Pages` workflow by hand (`workflow_dispatch`) with the tag name.

## After the release

- Open the baseline PR: `<PackageValidationBaselineVersion>` → the version just published
  (ADR-0004). Wait until the package is on the CDN, or the build cannot download the baseline.
- Prune the release branch; leave `main` and `gh-pages`.
