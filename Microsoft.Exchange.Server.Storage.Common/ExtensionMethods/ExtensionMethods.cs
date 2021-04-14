using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Common.ExtensionMethods
{
	public static class ExtensionMethods
	{
		public static void AppendAsString(this StringBuilder sb, byte[] bytes, int offset, int length)
		{
			ToStringHelper.AppendAsString(bytes, offset, length, sb);
		}

		public static void AppendAsString<T>(this StringBuilder sb, T value)
		{
			ToStringHelper.AppendAsString<T>(value, sb);
		}

		public static string GetAsString<T>(this T value)
		{
			return ToStringHelper.GetAsString<T>(value);
		}

		public static void SortAndRemoveDuplicates(this List<string> list, CompareInfo compareInfo, CompareOptions compareOptions)
		{
			ValueHelper.SortAndRemoveDuplicates(list, compareInfo, compareOptions);
		}

		public static void SortAndRemoveDuplicates<T>(this List<T> list) where T : IComparable<T>, IEquatable<T>
		{
			ValueHelper.SortAndRemoveDuplicates<T>(list);
		}

		public static bool Any(this BitArray source, bool predicate)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] == predicate)
				{
					return true;
				}
			}
			return false;
		}

		public static bool All(this BitArray source, bool predicate)
		{
			return !source.Any(!predicate);
		}

		public static string ValueOrEmpty(this string value)
		{
			return value ?? string.Empty;
		}

		public static object GetBoxed(this bool value)
		{
			if (!value)
			{
				return SerializedValue.BoxedFalse;
			}
			return SerializedValue.BoxedTrue;
		}

		public static double TotalMicroseconds(this TimeSpan value)
		{
			return (double)value.Ticks / 10.0;
		}

		public static TimeSpan ToTimeSpan(this Stopwatch sw)
		{
			return TimeSpan.FromTicks(StopwatchStamp.ToTimeSpanTicks(sw.ElapsedTicks));
		}

		private const long MicrosecondsPerMillisecond = 1000L;

		private const long TicksPerMicrosecond = 10L;
	}
}
