using System;
using Wolfgang.Extensions.DateTime;

// End-to-end SourceLink "step into" fixture. The debugger sets a breakpoint on the
// marked line below and issues a step-into (the F11 a consumer would press). If
// SourceLink is intact the debugger resolves the library's real source (from GitHub)
// inside DateTimeExtensions.TruncateMilliseconds, instead of a decompiled placeholder.
// TruncateMilliseconds is a plain, non-async public method with a real body, which
// makes it a clean and stable step-into target.
//
// System.DateTime is written out in full because this library's own namespace is
// Wolfgang.Extensions.DateTime, so the bare name binds to the namespace here.

var moment = new System.DateTime(2026, 9, 23, 14, 30, 45, 678, System.DateTimeKind.Utc);
var truncated = moment.TruncateMilliseconds(); // STEP_INTO_TARGET
Console.WriteLine(truncated.ToString("O"));
