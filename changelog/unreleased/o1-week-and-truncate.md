type: internal

`FirstOfWeek`, `EndOfWeek`, `TruncateMilliseconds` and `TruncateSeconds` now use constant-time tick arithmetic instead of a day-by-day walk or a full calendar decomposition; results are unchanged.
