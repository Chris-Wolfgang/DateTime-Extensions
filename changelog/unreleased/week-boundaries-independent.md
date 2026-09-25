type: breaking

`FirstOfWeek` and `EndOfWeek` no longer clamp at the ends of `DateTime`'s range: `EndOfWeek` now returns the correct week end for dates in the first week of year 1 (it was up to six days late), an unrepresentable week boundary throws `OverflowException` instead of returning `MinValue`/`MaxValue`, and a `firstDayOfWeek` outside the seven `DayOfWeek` values throws `ArgumentOutOfRangeException` instead of answering `0001-01-01`. See [docs/migrations/1.3.2-to-2.0.0.md](docs/migrations/1.3.2-to-2.0.0.md).
