# Workflow Security

## Overview

This document describes how the GitHub Actions workflows in this repository keep untrusted
pull-request code away from anything it could abuse, and how a pull request is prevented from
weakening the checks it is subject to. It describes the design as implemented; when a workflow
changes, change this file in the same PR.

## The model in one paragraph

`pr.yaml` runs on **`pull_request`**. It builds and tests the PR's own code with the PR's own copies
of the workflow and of every analyzer and coverage configuration file, so a PR is validated exactly
as it would merge. GitHub gives a `pull_request` run a read-only `GITHUB_TOKEN` and, for forks, no
secrets at all; `pr.yaml` uses no secret and declares `permissions: contents: read`, so there is
nothing for untrusted code to reach. Gate integrity comes from a second workflow,
`protected-files.yaml`, which runs on **`pull_request_target`** from `main`, never checks out the PR,
and fails any PR that mixes a change to a protected file with any other change. A PR therefore either
leaves the gates alone, or changes *only* gates and is reviewed as such by a maintainer.

## 1. Validation runs the PR's code with the PR's config — deliberately

**Mechanism:** `pull_request` trigger (`.github/workflows/pr.yaml`)

```yaml
on:
  pull_request:
    branches:
      - main
  push:
    branches:
      - main
```

A `pull_request` run executes the workflow file *as it exists in the PR*, checks out the merge
commit, and gets a read-only token. That is the property that makes it safe to build and run
arbitrary contributed code: the run can read the repository and nothing else. The `push` trigger
re-runs the same gates on `main` after a merge, which also keeps the InspectCode alert set on `main`
current.

Consequence: a PR can edit `pr.yaml`, `.editorconfig`, `Directory.Build.props`, `BannedSymbols.txt`,
`coverlet.runsettings`, `.config/dotnet-tools.json` and the scripts the workflow runs, and its
validation will use those edited copies. Section 2 is what stops that from being exploitable.

## 2. Protected files cannot change alongside anything else

**Mechanism:** `.github/workflows/protected-files.yaml`, required check `Protected Files Guard`

This is the one workflow that runs on `pull_request_target`, and the reason it may: it never checks
out or executes anything from the PR. It runs from `main` (so a PR cannot edit it), lists the PR's
changed files through the REST API, and classifies the PR:

| PR changes | Result |
|---|---|
| no protected file | pass — `pr.yaml` validated everything |
| only protected files | pass with a notice — a maintainer reviews the configuration diff on its own |
| protected files **and** anything else | **fail**, and nothing bypasses it: split the PR |

"Protected" is every file that shapes what CI checks: the workflows (`.github/workflows/*.yml|yaml`),
analyzer configuration (`.editorconfig`, `*.globalconfig`, `*.ruleset`, `*.DotSettings`,
`BannedSymbols.txt`), build props (`Directory.Build.props`, `Directory.Build.targets`), coverage
configuration (`coverlet.runsettings`), the Stryker floor (`stryker-config.json`), the gitleaks
config, the CI tool manifest (`.config/dotnet-tools.json`), the hash-pinned Python requirements
(`.github/requirements/*`), the license-audit policy (`.github/license-audit/*.json`) and the CI
scripts `pr.yaml` and `license-audit.yaml` run (`scripts/changelog.ps1`, `tfm-parity.ps1`,
`build-pr.ps1`, `third-party-notices.ps1`). The authoritative list is the `case` statement in the
workflow itself.

Dependabot is exempt: its bumps to `Directory.Build.props` and the requirements files are legitimate,
and its identity is GitHub-controlled and not spoofable.

Why this is sufficient: a PR that weakens a gate is protected-only and therefore *cannot* also
contain the code the weaker gate would have let through; a PR that contains code cannot touch the
gates. The two halves must merge separately, each reviewed for what it is.

## 3. Write scopes never coexist with PR code

Every job that checks out or builds pull-request code runs with `contents: read` and nothing more.
Where a workflow needs a write scope, it is on a separate job that does not execute repository code:

- `pr.yaml`: `inspectcode-upload` (`security-events: write`, `actions: read`) uploads the SARIF the
  `inspectcode` job produced; it never checks out PR code.
- `stryker.yaml`: the `stryker` job that compiles and mutates the code is read-only; `publish`
  (`contents: write`, `issues: write`) runs only for non-PR events and reads the run's artifact.
- `release.yaml`: `publish-nuget` (`id-token: write`, `attestations: write`), `trigger-docs` and
  `update-release-artifacts` (`contents: write`) run on `release: published` from a tagged commit
  on `main`, never on PR code.

The default for every workflow file is `permissions: contents: read`.

## 4. Credentials are not left on disk

**Mechanism:** `persist-credentials: false` on every `actions/checkout`

```yaml
- name: Checkout code
  uses: actions/checkout@<sha>  # vX.Y.Z
  with:
    persist-credentials: false
```

This stops the checkout token from being written into `.git/config`, where a later step (or the
built code) could read it. It does **not** prevent a step from using `${{ github.token }}` when a
workflow explicitly passes it; that is why the previous section matters.

## 5. Supply chain

- Every third-party action is pinned to a full commit SHA with a `# vX.Y.Z` comment;
  `actions-audit.yaml` (zizmor + actionlint) and Scorecard's Pinned-Dependencies check enforce it.
- .NET tools used by CI (`docfx`, `dotnet-stryker`, `dotnet-reportgenerator-globaltool`,
  `microsoft.cst.devskim.cli`, `cyclonedx`, `nuget-license`, `sourcelink`,
  `jetbrains.resharper.globaltools`) are pinned in `.config/dotnet-tools.json` and restored with
  `dotnet tool restore`; nothing is installed unversioned.
- Python tools (`semgrep`, `zizmor`, `defusedxml`) are installed with `--require-hashes` from
  `.github/requirements/*.txt`.
- No `${{ github.event.* }}` value is interpolated into a `run:` script; untrusted strings go through
  `env:`.

## Attack scenarios

| Attack | Why it fails |
|---|---|
| PR edits `pr.yaml` to skip a stage and adds code that stage would reject | The PR is mixed → `Protected Files Guard` fails. As protected-only, the weakened workflow merges alone and is reviewed as a workflow change. |
| PR edits `.editorconfig` / `BannedSymbols.txt` to allow a banned API and uses it | Same: mixed → fails. |
| PR code exfiltrates the token during `dotnet test` | The token is read-only and no secret is exposed; `persist-credentials: false` keeps it out of `.git/config`. |
| PR alters `protected-files.yaml` itself | It runs on `pull_request_target`, i.e. from `main`; the PR's copy is never executed. |
| PR lowers `stryker-config.json`'s break floor in the same change that drops the score | `stryker-config.json` is protected → mixed → fails. |

## Maintenance

### Changing a protected file

Open a PR that touches **only** protected files. `Protected Files Guard` passes with a notice; review
it as a configuration change. If the new configuration is needed by a code change, land the
configuration first and rebase the code PR onto it.

### Adding a protected file

1. Add its path (or glob) to the `case` statement in `.github/workflows/protected-files.yaml`.
2. Update the list in section 2 of this document in the same PR (both files are protected, so the PR
   stays protected-only).

### Adding a job that needs a write scope

Put it in its own job that does not check out or build PR code, declare the scope on that job only,
and add it to section 3.

## References

- [GitHub Actions security hardening](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [Keeping your GitHub Actions and workflows secure: preventing pwn requests](https://securitylab.github.com/research/github-actions-preventing-pwn-requests/)
- [OpenSSF Scorecard — Dangerous-Workflow and Pinned-Dependencies checks](https://github.com/ossf/scorecard/blob/main/docs/checks.md)
