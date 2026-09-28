type: docs

Documented that `FirstOfWeek(DateTime, DayOfWeek)` and `EndOfWeek(DateTime, DayOfWeek)` are not idempotent within the first week of year 1 (`0001-01-01` through `0001-01-07`): the `MinValue` clamp that avoids underflow does not necessarily land on the requested `firstDayOfWeek`, so `EndOfWeek` applied to its own result can return a later value. Found by fuzz testing (#436); no plausible application reaches it, so the behaviour is documented rather than changed.
