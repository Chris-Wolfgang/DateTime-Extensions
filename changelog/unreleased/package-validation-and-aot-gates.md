type: internal

The package now builds with `EnablePackageValidation`, so an unintentional binary-breaking change against the last published version fails the build, and the net8.0/net10.0 assemblies are marked `IsTrimmable` and `IsAotCompatible` for consumers doing trimmed or native-AOT publishes.
