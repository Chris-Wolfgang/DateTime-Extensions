type: fix

`EndOfWeek(DateTime, DayOfWeek)` now returns the correct week end in the first six days of year 1 - it no longer inherits `FirstOfWeek`'s `DateTime.MinValue` clamp, which made it report a day up to six days late for 21 `(date, firstDayOfWeek)` pairs - and is idempotent over the whole representable range (#436).
