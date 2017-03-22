using System;
using System.Collections.Generic;

namespace WebCoffeeMachine.Domain
{
    public static class Extensions
    {
        public static string ToLogMessage(this string message, long messageNumber)
            => $"{messageNumber} :: {DateTime.Now} :: {message}";

        public static string ToDivisorLine(this string title)
            => new string('#', (Console.WindowWidth - title.Length) / 2 - 1) + $" {title} " + new string('#', (Console.WindowWidth - title.Length) / 2 - 1);

        public static string PanelToString(this List<string> panel)
        {
            var str = "";
            panel.ForEach(line => str += line + "\n");
            return str;
        }
    }
}