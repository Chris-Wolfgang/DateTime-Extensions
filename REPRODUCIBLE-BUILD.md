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
  operating system from two different directories.
- Every `.dll` and `.pdb` for the portable target frameworks (`netstandard2.0`, `net8.0`, `net10.0`)
  is byte-identical when built on Linux and on Windows.
- Every **entry inside** the `.nupkg` and `.snupkg` is byte-identical across those environments.

These are not aspirations. `.github/workflows/reproducible-build.yaml` builds every commit three
ways — Windows in one directory, Windows in a deliberately deeper directory, and Linux — and fails
the build if any comparable output differs, naming the file. Measured on the current `main`: **35 of
35 fingerprints identical** (8 assembly/PDB outputs across four target frameworks, 27 package
entries).

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

**The SDK version is not pinned.** There is no `global.json`, so each runner installs whatever
`10.0.x` resolves to when it runs. Within a single workflow run the three legs agree, which is what
makes the comparison meaningful; across *months* they will not. The compare job detects this and
says so explicitly instead of reporting forty mismatched hashes. See [Known limits](#known-limits).

## Verify a published release yourself

### 1. Match the toolchain

The compiler version folds into a deterministic assembly's identity, so a different SDK patch
produces a different `.dll` from identical source. Use the SDK the release used: open the
`Release` workflow run for the tag and read the `Setup .NET` step.

`net462` requires Windows. On Linux you can verify the `netstandard2.0`, `net8.0` and `net10.0`
slices.

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

## Reporting a discrepancy

If you get a mismatch that is **not** explained by the SDK version, please
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

1. **No `global.json`.** The claim holds *within* a workflow run, not across time. Pinning the SDK
   would make an old tag reproducible years later; it affects every workflow in the repository, so
   it is a separate change.
2. **`net462` is not cross-OS verified** (see above).
3. **The reproducibility check is not the provenance check.** They answer different questions and
   neither substitutes for the other: reproducibility says *this source produces this binary*,
   provenance says *this binary came out of this repository's release workflow*. For the second one,
   see "Verifying a release" in [SECURITY.md](SECURITY.md).
