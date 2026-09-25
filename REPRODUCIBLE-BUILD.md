# Reproducible builds

This document states exactly what this repository guarantees about rebuilding a release, what it
does **not** guarantee, and how you can check the claim yourself without trusting us.

Two words that get used interchangeably and should not be:

- **Deterministic** — the same inputs on the same machine produce the same outputs.
- **Reproducible** — the same commit produces the same outputs in a *different* environment: another
  operating system, another directory, a machine that has never built this project.

Only the second one is useful to you. Determinism means our CI agrees with itself; reproducibility
means you can tie a published `.nupkg` back to a commit you have read.

## What is guaranteed

For any commit on `main`:

- Every `.dll` and `.pdb` this repository produces is **byte-identical** when built on the same
  operating system from a different directory, on a machine that has never built it before.
- Every **entry inside** the `.nupkg` and `.snupkg` is byte-identical under those conditions.

Releases are built on Windows, so a verifier on Windows can reach byte-identity. **Across operating
systems you cannot** — for a reason in the SDK rather than in this repository. See
[Why a Linux rebuild does not match byte-for-byte](#why-a-linux-rebuild-does-not-match-byte-for-byte).

These are not aspirations. `.github/workflows/reproducible-build.yaml` builds every commit three
ways — Windows in one directory, Windows in a deliberately deeper directory, and Linux — and **fails
the build** if the two Windows builds disagree about anything, naming the file. The Linux comparison
is printed rather than gated, for the same reason. Measured on the current `main`: **35 of 35
fingerprints identical** between the two Windows builds (8 assembly/PDB outputs across four target
frameworks, 27 package entries).

The inputs that buy this are in `Directory.Build.props`: `ContinuousIntegrationBuild` (active
whenever `CI=true`), which normalises the source paths embedded in the assembly and PDB, plus
Roslyn's deterministic compilation, which is on by default.

## What is not guaranteed, and why

**The `.nupkg` file itself is not byte-reproducible.** A `.nupkg` is a zip archive, and NuGet stamps
every entry with its file modification time. Two packs of the same commit **on the same machine,
seconds apart** produce different files:

```
pack #1   f51f80bab7c89c6f5b29964297ca7c5e605cf38e34825300680963d6fb6128d9
pack #2   1a892e1c59107505372a5be6e034a13759957ab36f5c86f0726868cb2122b3ce
```

…while all 19 entries inside matched by name, order, CRC and content. The only difference was the
zip entry timestamps, four seconds apart.

So a `sha256` of the package file is **not** a reproducibility check and never can be with
`dotnet pack` as it stands. Compare the entries instead; `scripts/compare-package-entries.ps1` does.

**The `.NET Framework` slices are verified on Windows only.** `net462` needs the .NET Framework
reference assemblies, which are not present on the Linux runners, so those slices are covered by the
two-directory Windows comparison rather than the cross-OS one. The rest of this repository's CI
treats them the same way (`build-all-versions.yaml` is `windows-latest` for the same reason).

### Why a Linux rebuild does not match byte-for-byte

Every project compiles a handful of sources the SDK generates for it — `AssemblyInfo.cs`,
`GlobalUsings.g.cs`, `.AssemblyAttributes.cs` — and the SDK writes them with **the platform's
newline**: CRLF on Windows, LF on Linux. The compiler hashes every source file it compiles, that
hash reaches the PDB, and the PDB feeds the assembly's MVID. So the same code, compiled on two
operating systems, gets a different assembly *identity* while the IL is the same.

The effect is measurable on a single machine. Converting one tracked source file from LF to CRLF and
rebuilding:

```
LF source     1c98a76adf41d3eb53c285cdccde26f71bfad2d69529f487dbc9f38335f95e48
CRLF source   8166399fa1b85c911bc336c195d6359ecb8da16d47a861a6ce8c652f429c44a7

same size (74,752 bytes), 71 differing bytes in 6 runs of 4, 7, 8, 4, 16 and 32 bytes
```

Same signature as the cross-OS difference: the PE timestamp, the MVID and the debug-directory
entries. This is why the Linux leg of the workflow reports rather than gates — gating on it would
fail every pull request forever without saying anything about the code.

The repository's `.gitattributes` normalises tracked files to LF on every platform, so the tracked
sources are not the problem; the generated ones are, and there is no supported switch for them. An
MSBuild target could rewrite them before compilation, which *might* close the gap — nobody has shown
that it is sufficient, so it is not claimed here.

**The SDK version is not pinned.** There is no `global.json`, so each runner installs whatever
`10.0.x` resolves to when it runs. Within a single workflow run the three legs agree, which is what
makes the comparison meaningful; across *months* they will not. The compare job detects this and
says so explicitly instead of reporting forty mismatched hashes. See [Known limits](#known-limits).

## Verify a published release yourself

### 1. Match the toolchain

The compiler version folds into a deterministic assembly's identity, so a different SDK patch
produces a different `.dll` from identical source. Use the SDK the release used — it is recorded in
the release's `reproducible-build-manifest.json` asset:

```bash
gh release download v1.3.2 --repo Chris-Wolfgang/DateTime-Extensions \
  --pattern 'reproducible-build-manifest.json'
pwsh -c '(Get-Content reproducible-build-manifest.json | ConvertFrom-Json).toolchain'
```

```
sdk      os      continuousIntegrationBuild
---      --      --------------------------
10.0.400 Windows                       True
```

Releases made before that asset existed do not carry it. For those, open the `Release` workflow run
for the tag and read the `Setup .NET` step.

**Use Windows.** Releases are built on Windows, and a rebuild on Linux or macOS cannot match
byte-for-byte even with the right SDK — see
[Why a Linux rebuild does not match byte-for-byte](#why-a-linux-rebuild-does-not-match-byte-for-byte).
`net462` needs Windows regardless, for its reference assemblies. Verifying from Linux is still worth
doing; just expect the four `.dll` entries to differ in their identity fields, and read
[What you should see](#4-what-you-should-see) for how to tell that apart from a real discrepancy.

### 2. Clone at the tag and pack

`CI=true` is not optional — without it `ContinuousIntegrationBuild` is off, your checkout path is
embedded verbatim, and nothing will match.

```bash
git clone --branch v1.3.2 https://github.com/Chris-Wolfgang/DateTime-Extensions.git
cd DateTime-Extensions
CI=true dotnet pack src/Wolfgang.Extensions.DateTime/Wolfgang.Extensions.DateTime.csproj \
  -c Release -o packages
```

If the restore fails with `NU1902`/`NU1903`, that is expected on older tags and is not a build
problem: NuGet's vulnerability audit is a *time-dependent* input, so a tag that restored clean on
release day can fail today because an advisory was published since. Add `-p:NuGetAudit=false` to
reproduce the release as it was, and read the advisory separately. (Concretely: `v1.3.2` no longer
restores clean, because `Microsoft.Build.Tasks.Git` 10.0.300 was later flagged by
[GHSA-23fw-v26w-5fgq](https://github.com/advisories/GHSA-23fw-v26w-5fgq).)

### 3. Compare entry by entry

```bash
curl -sLO https://api.nuget.org/v3-flatcontainer/wolfgang.extensions.datetime/1.3.2/wolfgang.extensions.datetime.1.3.2.nupkg

pwsh ./scripts/compare-package-entries.ps1 \
  -Mine ./packages/Wolfgang.Extensions.DateTime.1.3.2.nupkg \
  -Published ./wolfgang.extensions.datetime.1.3.2.nupkg
```

The script skips two entries deliberately, neither of which comes from the build:

| Entry | Why it is not compared |
|---|---|
| `.signature.p7s` | nuget.org's repository signature, added after the build. Present in the downloaded package, absent from yours. |
| `*.psmdcp` | OPC core properties. The *file name* is a GUID on some NuGet versions and the fixed `nuget.psmdcp` on others, so it is compared under a stable key rather than by name. |

### 3b. Or compare against the manifest

Each release also carries `reproducible-build-manifest.json`, which lists a SHA-256 for every entry
inside every package it shipped. The same script reads it, so you can check your rebuild without
downloading anything from nuget.org:

```bash
pwsh ./scripts/compare-package-entries.ps1 \
  -Mine ./packages/Wolfgang.Extensions.DateTime.1.3.2.nupkg \
  -Manifest ./reproducible-build-manifest.json
```

Two things to know before reading a difference as one:

- **`sha256IsReproducible` is `false` on each package's own file hash.** It is recorded for
  completeness. Do not compare it — see [What is not guaranteed](#what-is-not-guaranteed-and-why).
- **The manifest does not list every entry.** The `.psmdcp` is omitted, because its entry name is not
  stable across NuGet versions, and `.signature.p7s` does not exist yet when the manifest is written.
  The manifest says both in its own `verification.entriesNotListed`.

The manifest is written by the same workflow run that produced the packages, so it is a convenience,
not independent evidence. If the question is whether *we* published what we said we did, compare
against nuget.org as in step 3, or verify the provenance attestation, which is signed
([SECURITY.md](SECURITY.md#verifying-a-release)).

### 4. What you should see

With a matching SDK, every entry matches and the script exits 0.

With a *different* SDK you will see the four `lib/**/*.dll` entries differ and nothing else of
substance. That is worth knowing in detail, because it is the difference between "a different
compiler built this" and "different source built this". Repacking `v1.3.2` with SDK 10.0.400 against
the published package:

- 13 of 19 entries identical — including the `.nuspec`, the `README.md`, the icons, and **every
  XML documentation file**.
- The four `.dll` entries differ in **72 of 74,752 bytes**, in five runs of 4, 16, 4, 16 and 32
  bytes. The sizes and positions are consistent with the PE header timestamp, the MVID, and the two
  debug-directory entries carrying the PDB id and checksum — the fields that *are* the compilation's
  identity hash. All IL and all metadata is identical.
- `_rels/.rels` and the OPC core properties differ in their generated ids only.

A compiler change looks like that: a small, bounded residue in the identity fields. Changed source
does not — it moves IL, and the differing byte count is not 72.

**A different operating system looks the same way**, and for the same underlying reason — the
generated sources carry the platform newline, so their checksums differ and the identity fields
follow. A Linux rebuild against a Windows-built release lands in this shape, not in a clean match.

## Reporting a discrepancy

If you get a mismatch that is **not** explained by the SDK version or the operating system, please
[open an issue](https://github.com/Chris-Wolfgang/DateTime-Extensions/issues/new/choose) with:

- the tag you built, and the SDK version (`dotnet --version`) and OS you built on,
- the script's output — the list of entries that differ,
- for a differing `.dll`, its size and the number of differing bytes, so it is immediately clear
  whether the residue is bounded like the case above or not.

If you would rather publish your result independently than file it here, the
[Reproducible Builds](https://reproducible-builds.org/) project's conventions apply; a link in an
issue is enough for us to reference it from the release notes.

Treat a mismatch you cannot explain as a **security report** and follow
[SECURITY.md](SECURITY.md) instead — do not open a public issue for it.

## Known limits

1. **No `global.json`.** The claim holds *within* a workflow run, not across time. Each release's
   manifest records the SDK that built it, so an older tag can still be reproduced by installing
   that SDK — but nothing in the repository pins it. Pinning it would make that automatic; it
   affects every workflow here, so it is a separate change.
2. **Byte-identity is same-OS only**, because of the generated sources' newline
   ([why](#why-a-linux-rebuild-does-not-match-byte-for-byte)). An MSBuild target normalising them
   before compilation might close the gap; that is unverified, so it is not promised. `net462` is
   additionally Windows-only, for its reference assemblies.
3. **The reproducibility check is not the provenance check.** They answer different questions and
   neither substitutes for the other: reproducibility says *this source produces this binary*,
   provenance says *this binary came out of this repository's release workflow*. For the second one,
   see "Verifying a release" in [SECURITY.md](SECURITY.md).
