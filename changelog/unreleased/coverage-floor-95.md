type: internal

Raised the production-code coverage floor (`CODECOV_MINIMUM`) from 90% to 95%, per the fleet coverage policy. Also synced `scripts/build-pr.ps1` to run ReportGenerator and DevSkim as the pinned local tools (`dotnet reportgenerator`/`dotnet devskim`) instead of unreliable global installs, so a missing tool now fails loudly locally instead of silently reporting a false pass.
