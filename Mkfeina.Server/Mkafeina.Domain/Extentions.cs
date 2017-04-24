using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkfeina.Domain
{
	public static class Extentions
	{
		public static string ToLogMessage(this string message, long messageNumber)
			=> $"({messageNumber}){message}";

		public static string ToDivisorLine(this string title)
			=> new string('#', (Console.WindowWidth - title.Length) / 2 - 1) + $" {title} " + new string('#', (Console.WindowWidth - title.Length) / 2 - 1);

		public enum FulfillStringMode { LeftAlignment = 0, Centered, RightAlignment }

		public static string AdjustLength(this string str, int finalLength, char fulfiller = ' ', FulfillStringMode mode = FulfillStringMode.LeftAlignment)
		{
			if (string.IsNullOrEmpty(str))
				return new string(fulfiller, finalLength);

			if (str.Length >= finalLength)
				return str.Substring(0, finalLength);

			switch (mode)
			{
				case FulfillStringMode.LeftAlignment:
					return str + " " + new string(fulfiller, finalLength - 1 - str.Length);

				case FulfillStringMode.Centered:

					if (finalLength - str.Length == 1)
						return str.AdjustLength(finalLength);
					else
					{
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

		public static int ParseToInt(this string str) => int.Parse(str);

		public static IEnumerable<string> SplitValueSeparatedBy(this string str, string separator) => str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
																										 .Select(s => s.Trim())
																										 .AsEnumerable();

		private static UnityContainer _container = new UnityContainer();

		public static UnityContainer UnityContainer(this AppDomain domain) => _container;

		public static string FirstLetterToUpper(this string str) => str[0].ToString().ToUpper() + str.Substring(1);

		public static string GenerateNameVersion(this string name)
		{
			int version;
			var last4 = name.Substring(name.Length - 4, 4);
#warning create consts form magic numbers here
			if (last4[0] == '(' && last4[3] == ')' && int.TryParse(last4.Substring(1, 2), out version))
				return name + $"({++version:00})";
			else
				return name + "(01)";
		}

		public static byte[] Encrypt(this byte[] bytes, byte[] key)
		{
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] += key[i % key.Length];
				bytes[i] = (byte)(bytes[i] % 128);
			}
			return bytes;
		}

		public static byte[] Decrypt(this byte[] bytes, byte[] key)
		{
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] -= key[i % key.Length];
				bytes[i] = (byte)(bytes[i] % 128);
			}
			return bytes;
		}

		public static string Encrypt(this string str, byte[] key)
			=> Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(str).Encrypt(key));

		public static string DecryptAndGetString(this byte[] bytes, byte[] key)
			=> Encoding.ASCII.GetString(bytes.Decrypt(key));
	}
}