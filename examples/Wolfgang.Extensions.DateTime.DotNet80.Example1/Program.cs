using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.Extensions.DateTime.DotNet8.Example1;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static void Main()
    {
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine("\n");

        Console.WriteLine($"\tFirst of the current month: {System.DateTime.Today.FirstOfMonth()}.");
        Console.WriteLine($"\t  End of the current month: {System.DateTime.Today.EndOfMonth()}.");
        Console.WriteLine($"\t  End of the Feb 2020 (leap year): {new System.DateTime(2020, 2, 15).EndOfMonth()}.");


        Console.WriteLine("\n");
        Console.WriteLine($"\tFirst of the current year: {System.DateTime.Today.FirstOfYear()}.");
        Console.WriteLine($"\t  End of the current year: {System.DateTime.Today.EndOfYear()}.");


        Console.WriteLine("\n");
        Console.WriteLine($"\tFirst of the current week: {System.DateTime.Today.FirstOfWeek()} (current culture first of week).");
        Console.WriteLine($"\t  End of the current week: {System.DateTime.Today.EndOfWeek()} (current culture first of week).");


        Console.WriteLine("\n");
        Console.WriteLine($"\tFirst of the current week: {System.DateTime.Today.FirstOfWeek(DayOfWeek.Saturday)} (Saturday as first of week).");
        Console.WriteLine($"\t  End of the current week: {System.DateTime.Today.EndOfWeek(DayOfWeek.Saturday)} (Saturday as first of week).");


        var now = System.DateTime.Now;
        Console.WriteLine($"\t                 Now: {now:yyyy-MM-dd hh:mm:ss.fff tt}");
        Console.WriteLine($"\tTruncateMilliseconds: {now.TruncateMilliseconds():yyyy-MM-dd hh:mm:ss.fff tt}");
        Console.WriteLine($"\t     TruncateSeconds: {now.TruncateSeconds():yyyy-MM-dd hh:mm:ss.fff tt}");

        Console.ResetColor();
    }
}
