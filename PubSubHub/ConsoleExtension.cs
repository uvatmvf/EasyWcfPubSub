using System;

namespace PubSubService2
{
    internal static class ConsoleExtension
    {
        internal static void WriteError(Exception e)
            => WriteColor(e.ToString(), ConsoleColor.Red);

        internal static void WriteDrop(string s = "Subscriber dropped")
            => WriteColor(s, ConsoleColor.DarkYellow);

        internal static void WriteAdd(string s = "Subscriber added")
            => WriteColor(s, ConsoleColor.Green);

        internal static void WriteColor(string s, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}