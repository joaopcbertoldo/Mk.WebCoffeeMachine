using System;
using System.Collections.Generic;

namespace WebCoffeeMachine.Domain
{
    public static class Extensions
    {
        public static string ToLogMessage(this string message, long messageNumber)
            => $"({messageNumber}){message}";

        public static string ToDivisorLine(this string title)
            => new string('#', (Console.WindowWidth - title.Length) / 2 - 1) + $" {title} " + new string('#', (Console.WindowWidth - title.Length) / 2 - 1);

        public static string PanelToString(this List<string> panel)
        {
            var str = "";
            panel.ForEach(line => str += line + "\n");
            return str;
        }

        public enum FulfillStringMode { LeftAlignment = 0, Centered, RightAlignment }

        public static string AdjustLength(this string str, int finalLength, char fulfiller = ' ', FulfillStringMode mode = FulfillStringMode.LeftAlignment)
        {
            if (string.IsNullOrEmpty(str))
                return new string(fulfiller, finalLength);

            if (str.Length >= finalLength)
                return str.Substring(0, finalLength);

            switch (mode) {
                case FulfillStringMode.LeftAlignment:
                    return str + " " + new string(fulfiller, finalLength - 1 - str.Length);

                case FulfillStringMode.Centered:

                    if (finalLength - str.Length == 1)
                        return str.AdjustLength(finalLength);
                    else {
                        int toFulfill = finalLength - 2 - str.Length;
                        int toFulfillOneSide = toFulfill / 2;
                        int reminder = toFulfill % 2;
                        return new string(fulfiller, toFulfillOneSide) + " " + str + " " + new string(fulfiller, toFulfillOneSide + reminder);
                    }

                case FulfillStringMode.RightAlignment:
                    return new string(fulfiller, finalLength - 1 - str.Length) + " " + str;

                default:
                    throw new NotImplementedException();
            }
        }

        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }
}