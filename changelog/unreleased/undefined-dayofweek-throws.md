type: fix

`FirstOfWeek(DateTime, DayOfWeek)` and `EndOfWeek(DateTime, DayOfWeek)` now throw `ArgumentOutOfRangeException` for a `DayOfWeek` value outside `Sunday`..`Saturday` instead of silently walking back to `DateTime.MinValue`.
