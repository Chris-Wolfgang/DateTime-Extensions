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

- Every `.dll` and `.pdb` this repository produces is **byte-identical** when built from a
  different directory, on a machine that has never built it before, **on Windows or on Linux** —
  every target framework, `net462` included. The same files inside the `.nupkg` and `.snupkg` are
  byte-identical too.
- On the **same** operating system, every **entry inside** the `.nupkg` and `.snupkg` is
  byte-identical, not only the assemblies and symbols.

The guarantee covers the assemblies and symbols — the files that carry the code — and **not the
package files themselves**, and not the package's metadata entries across operating systems. See
[What is not guaranteed](#what-is-not-guaranteed-and-why).

These are not aspirations. `.github/workflows/reproducible-build.yaml` builds and packs every commit
three ways — Windows in one directory, Windows in a deliberately deeper directory, and Linux — and
**fails the build** if the two Windows builds disagree about anything, or if any `.dll` or `.pdb`
(built, or inside a package) differs between Linux and Windows, naming the file. The other package
entries are compared across operating systems too, and reported rather than gated.

The inputs that buy this:

- `Directory.Build.props`: `ContinuousIntegrationBuild` (active whenever `CI=true`), which
  normalises the source paths embedded in the assembly and PDB, plus Roslyn's deterministic
  compilation, which is on by default.
- `Directory.Build.targets`: rewrites the sources the SDK generates for each project to LF before
  compilation (see [why](#why-the-generated-sources-are-normalised)).
- `Directory.Build.props`: `net4*` builds of `src/` compile against the
  `Microsoft.NETFramework.ReferenceAssemblies` package on every operating system, instead of the
  targeting pack installed on Windows, which is a different build of the same reference
  assemblies.

**Releases up to and including `v1.4.0` predate the last two**, so for those tags the cross-OS
guarantee does not hold: rebuild them on Windows, as their release was.

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

**Across operating systems, the package's metadata entries differ.** The XML documentation files,
the `.nuspec`, `_rels/.rels`, `[Content_Types].xml` and the OPC core properties are written by the
compiler and by NuGet with the platform's newline. They carry no code, so the workflow reports them
instead of failing on them; the `.dll` and `.pdb` entries next to them must match.

### Why the generated sources are normalised

Every project compiles a handful of sources the SDK generates for it — `AssemblyInfo.cs`,
`GlobalUsings.g.cs`, `.AssemblyAttributes.cs` — and the SDK writes them with **the platform's
newline**: CRLF on Windows, LF on Linux. The compiler hashes every source file it compiles, that
hash reaches the PDB, and the PDB feeds the assembly's MVID. So without intervention the same code,
compiled on two operating systems, gets a different assembly *identity* while the IL is the same.

The effect is measurable on a single machine. Converting one tracked source file from LF to CRLF and
rebuilding:

```
LF source     1c98a76adf41d3eb53c285cdccde26f71bfad2d69529f487dbc9f38335f95e48
CRLF source   8166399fa1b85c911bc336c195d6359ecb8da16d47a861a6ce8c652f429c44a7

same size (74,752 bytes), 71 differing bytes in 6 runs of 4, 7, 8, 4, 16 and 32 bytes
```

Same signature as the cross-OS difference used to have: the PE timestamp, the MVID and the
debug-directory entries.

The repository's `.gitattributes` normalises tracked files to LF on every platform, so the tracked
sources are not the problem; the generated ones were, and there is no supported switch for them.
`Directory.Build.targets` therefore rewrites them to LF between generation and `CoreCompile`. Together
with the reference-assemblies package for `net4*`, that removed every cross-OS difference in the
`.dll` and `.pdb` files: the Linux and Windows builds of one commit now match byte-for-byte, and the
workflow fails if they stop doing so.

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
gh release download v1.4.0 --repo Chris-Wolfgang/DateTime-Extensions \
  --pattern 'reproducible-build-manifest.json'
pwsh -c '(Get-Content reproducible-build-manifest.json | ConvertFrom-Json).toolchain'
```

```
sdk      os      continuousIntegrationBuild
---      --      --------------------------
10.0.401 Windows                       True
```

Releases made before that asset existed do not carry it. For those, open the `Release` workflow run
for the tag and read the `Setup .NET` step.

**Pick the operating system by tag.** For releases made after the cross-OS fixes (any tag after
`v1.4.0`), Windows and Linux both reproduce every `.dll` and `.pdb` entry byte-for-byte; on Linux,
expect the XML documentation, `.nuspec` and OPC metadata entries to differ in their newlines only.
For `v1.4.0` and earlier, use Windows: those releases were built before the generated sources were
normalised, so a Linux rebuild differs in the four `.dll` entries' identity fields — read
[What you should see](#4-what-you-should-see) for how to tell that apart from a real discrepancy.

### 2. Clone at the tag and pack

`CI=true` is not optional — without it `ContinuousIntegrationBuild` is off, your checkout path is
embedded verbatim, and nothing will match.

```bash
git clone --branch v1.4.0 https://github.com/Chris-Wolfgang/DateTime-Extensions.git
cd DateTime-Extensions
CI=true dotnet restore
CI=true pwsh ./scripts/third-party-notices.ps1
CI=true dotnet pack src/Wolfgang.Extensions.DateTime/Wolfgang.Extensions.DateTime.csproj \
  -c Release -o packages --no-restore
```

Tags that contain `scripts/third-party-notices.ps1` (every release after `v1.3.2`) ship a generated
`THIRD-PARTY-NOTICES.md` at the package root. The release workflow renders it between restore and
pack, and `Directory.Build.props` only packs it when the file exists, so a plain `pack` leaves that
entry out and cannot match - hence the three steps above. For `v1.3.2` and earlier, which predate
the script, a plain pack is the whole build:

```bash
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
curl -sLO https://api.nuget.org/v3-flatcontainer/wolfgang.extensions.datetime/1.4.0/wolfgang.extensions.datetime.1.4.0.nupkg

pwsh ./scripts/compare-package-entries.ps1 \
  -Mine ./packages/Wolfgang.Extensions.DateTime.1.4.0.nupkg \
  -Published ./wolfgang.extensions.datetime.1.4.0.nupkg
```

The script handles two entries specially, neither of which comes from the build:

| Entry | How it is handled |
|---|---|
| `.signature.p7s` | **Skipped.** nuget.org's repository signature, added after the build. Present in the downloaded package, absent from yours. |
| `*.psmdcp` | **Compared**, under a stable key rather than by name: OPC core properties, whose *file name* is a GUID on some NuGet versions and the fixed `nuget.psmdcp` on others. A difference in its content is reported. |

### 3b. Or compare against the manifest

Each release also carries `reproducible-build-manifest.json`, which lists a SHA-256 for every entry
inside every package it shipped. The same script reads it, so you can check your rebuild without
downloading anything from nuget.org. You downloaded the manifest in step 1, before cloning, so it
is one directory above the clone:

```bash
pwsh ./scripts/compare-package-entries.ps1 \
  -Mine ./packages/Wolfgang.Extensions.DateTime.1.4.0.nupkg \
  -Manifest ../reproducible-build-manifest.json
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
  identity hash. All IL is identical, and so is every metadata value other than those identity
  fields (the MVID is itself stored in metadata).
- `_rels/.rels` and the OPC core properties differ in their generated ids only.

A compiler change looks like that: a small, bounded residue in the identity fields. Changed source
does not — it moves IL, and the differing byte count is not 72.

**A different operating system looked the same way** for releases up to `v1.4.0`, for the same
underlying reason — the generated sources carried the platform newline, so their checksums differed
and the identity fields followed. For later releases the `.dll` and `.pdb` entries match across
operating systems; only the newline-bearing metadata entries listed above differ.

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
2. **Cross-OS byte-identity covers assemblies and symbols only.** Package files never match
   (zip timestamps), and across operating systems the package's metadata entries differ in their
   newlines. Releases up to and including `v1.4.0` predate the cross-OS fixes and reproduce on
   Windows only.
3. **The reproducibility check is not the provenance check.** They answer different questions and
   neither substitutes for the other: reproducibility says *this source produces this binary*,
   provenance says *this binary came out of this repository's release workflow*. For the second one,
   see "Verifying a release" in [SECURITY.md](SECURITY.md).
