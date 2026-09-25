# Security Policy

## Supported Versions

Security fixes are released for the **latest published version** only. If you are on an older
version, upgrade to the latest release to receive the fix. Pre-1.0 releases (0.x) follow the same
rule: the newest 0.x release is the supported one.

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. Open the private report form: https://github.com/Chris-Wolfgang/DateTime-Extensions/security/advisories/new
   (or, from the repository's **Security** tab, click **Report a vulnerability**).
   GitHub's guide to this process: https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability
3. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

This is a single-maintainer project; the commitments below are realistic for that, not aspirational.

| Stage | Target |
|-------|--------|
| Acknowledgement of the report | Within 48 hours |
| Initial assessment (confirmed / not a vulnerability / need more information) and severity | Within 7 days |
| Fix for **Critical** severity | Within 30 days |
| Fix for **High** or **Medium** severity | Within 90 days |
| Fix for **Low** severity | Next scheduled release |

If a target is going to slip, you will hear that from the maintainer in the advisory thread before
the deadline passes, not after.

## Disclosure Process

1. **Report** — you open a private advisory (above). It is not public, but it is not a
   two-person conversation either. GitHub grants access to the repository owner, to organization
   owners, to security managers, and to anyone with the repository's admin role, plus any
   collaborator explicitly added to the advisory. Repository write access alone does not grant it.
   Nobody outside that set sees the report until it is published.
2. **Triage** — the maintainer confirms the issue and assigns a severity, discussed with you in the
   advisory thread.
3. **Fix** — the fix is developed in a temporary private fork attached to the advisory, so nothing
   about the vulnerability is visible in public pull requests until the fix is released.
4. **Release** — a new version ships with the fix. The release notes say a security issue was fixed
   without giving details that would help exploit unpatched versions.
5. **Publish** — the advisory is published, which notifies dependents through Dependabot. A CVE is
   requested from GitHub for confirmed vulnerabilities; assignment is GitHub's decision, so the
   advisory may publish without one. Publication happens at release time, or after 90 days from the
   initial report if no fix is possible, whichever comes first.

Please keep the details private until the advisory is published.

## Credit

Reporters are credited in the published advisory and in the release notes, unless you ask not to be.
Tell us in the report how you would like to be named.

## Verifying a release

A GitHub Release carries the package, a SLSA build-provenance bundle (`*.intoto.jsonl`) and,
normally, a CycloneDX SBOM per project (`*.bom.json`). The SBOM is best-effort: `release.yaml`
warns and continues if CycloneDX fails, and the artifact upload does not treat a missing file as
an error, so a release can complete without one. Its absence is not evidence of tampering.
Nothing below needs a key from us - verification uses GitHub's own transparency log.

**Provenance - did this package come from this repository's release workflow?**

```bash
gh attestation verify Wolfgang.Extensions.DateTime.1.3.2.nupkg \
    --repo Chris-Wolfgang/DateTime-Extensions \
    --signer-workflow Chris-Wolfgang/DateTime-Extensions/.github/workflows/release.yaml
```

`--repo` alone only proves *some* workflow in this repository attested the artifact.
`--signer-workflow` is what pins it to the release workflow, which is what the heading above
actually claims - keep both.

If you are verifying offline, or want to check the exact bundle attached to the release rather than
whatever GitHub currently holds, download the `.intoto.jsonl` asset and point at it:

```bash
gh attestation verify Wolfgang.Extensions.DateTime.1.3.2.nupkg \
    --repo Chris-Wolfgang/DateTime-Extensions \
    --signer-workflow Chris-Wolfgang/DateTime-Extensions/.github/workflows/release.yaml \
    --bundle DateTime-Extensions-v1.3.2.intoto.jsonl
```

A failure here is meaningful: it means the file you hold is not the file that workflow produced.

**Contents - what does the package actually depend on?**

The `*.bom.json` asset is a CycloneDX SBOM listing the transitive closure at build time. It is a
plain JSON document; read it, or feed it to whatever your organisation uses for dependency review.

Note what it is **not**: the attestation's subjects are the `.nupkg` files only, so the SBOM is
co-published alongside the package rather than covered by its provenance. Verifying the package
does not authenticate the SBOM. If that distinction matters to you, read the SBOM as a
convenience and derive the dependency set from the verified package itself.

**What this does NOT prove.** The package is not author-signed: `nuget verify` checks an author
signature, and this package does not carry one - that is blocked on a code-signing certificate
(#280). Provenance answers *"which workflow built this, from which commit"*; an author signature
would answer *"who vouches for it"*. They are different claims and only the first is available
today.

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic
incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories,
unlisting NuGet packages) are not duplicated here - GitHub's and NuGet's own docs update faster
than a checked-in runbook.

- **Release path**: OIDC / NuGet Trusted Publishing via `NuGet/login@v1` in
  `.github/workflows/release.yaml`. The workflow mints an ephemeral push token per run via OIDC -
  the release path does not depend on a long-lived API key stored in GitHub secrets or on the
  NuGet account. During an incident, check the NuGet account for long-lived API keys anyway (they
  can be created outside CI) and delete anything you do not recognise.
- **Fallback**: none. If Trusted Publishing is compromised the incident is at the GitHub-account
  level; the OIDC identity is `Chris-Wolfgang/DateTime-Extensions`.
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: no Wolfgang.* package depends on this one. It is a leaf library, so
  the blast radius is direct consumers on nuget.org, who are not enumerable from here.
- **Package coordinates for unlisting**: one package,
  [`Wolfgang.Extensions.DateTime`](https://www.nuget.org/packages/Wolfgang.Extensions.DateTime/).

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.
