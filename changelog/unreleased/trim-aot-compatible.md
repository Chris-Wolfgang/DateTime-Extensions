type: feature

The net8.0 and net10.0 assemblies are marked `IsTrimmable` and `IsAotCompatible`, so consumers publishing trimmed or native-AOT builds get the analysers and the metadata that lets the trimmer trust this library.
