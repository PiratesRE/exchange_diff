using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class ObjectExtension
	{
		public static bool IsNullValue(this object item)
		{
			return item == null || DBNull.Value.Equals(item);
		}

		public static bool IsTrue(this object item)
		{
			if (item.IsNullValue())
			{
				return false;
			}
			if (item is bool)
			{
				return (bool)item;
			}
			throw new ArgumentException("item is not bool or Nullable<bool>");
		}

		public static bool IsFalse(this object item)
		{
			if (item.IsNullValue())
			{
				return false;
			}
			if (item is bool)
			{
				return !(bool)item;
			}
			throw new ArgumentException("value is not bool or Nullable<bool>");
		}

		public static void Perform<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (T obj in sequence)
			{
				action(obj);
			}
		}

		public static string FromMB(this string size)
		{
			if (string.IsNullOrWhiteSpace(size))
			{
				throw new ArgumentNullException("size");
			}
			string result;
			try
			{
				result = ByteQuantifiedSize.FromBytes(checked((ulong)Math.Round(unchecked(Convert.ToDouble(size) * 1048576.0), 0))).ToString();
			}
			catch (FormatException)
			{
				throw new ArgumentException("String '" + size + "' is not of the expected number format.");
			}
			catch (OverflowException)
			{
				throw new ArgumentException("String '" + size + "' is outside the allowable numeric range.");
			}
			return result;
		}

		public static string ToMB(this ByteQuantifiedSize size, int precision)
		{
			return Math.Round(size.ToBytes() / 1048576.0, precision).ToString();
		}

		public static string FromTimeSpan(this string span, TimeUnit factor)
		{
			if (string.IsNullOrWhiteSpace(span))
			{
				throw new ArgumentNullException("span");
			}
			if (factor < TimeUnit.Second || factor > TimeUnit.Day)
			{
				throw new ArgumentOutOfRangeException("factor");
			}
			string result;
			try
			{
				switch (factor)
				{
				case TimeUnit.Second:
					result = EnhancedTimeSpan.FromSeconds(Math.Round(Convert.ToDouble(span), 0)).ToString();
					break;
				case TimeUnit.Minute:
					result = EnhancedTimeSpan.FromSeconds(Math.Round(Convert.ToDouble(span) * 60.0, 0)).ToString();
					break;
				case TimeUnit.Hour:
					result = EnhancedTimeSpan.FromSeconds(Math.Round(Convert.ToDouble(span) * 3600.0, 0)).ToString();
					break;
				default:
					result = EnhancedTimeSpan.FromSeconds(Math.Round(Convert.ToDouble(span) * 86400.0, 0)).ToString();
					break;
				}
			}
			catch (FormatException)
			{
				throw new ArgumentException("String '" + span + "' is not of the expected number format.");
			}
			catch (OverflowException)
			{
				throw new ArgumentException("String '" + span + "' is outside the allowable numeric range.");
			}
			return result;
		}

		public static string ToString(this EnhancedTimeSpan span, TimeUnit factor, int precision)
		{
			if (factor < TimeUnit.Second || factor > TimeUnit.Day)
			{
				throw new ArgumentOutOfRangeException("factor");
			}
			switch (factor)
			{
			case TimeUnit.Second:
				return Math.Round(span.TotalSeconds, precision).ToString();
			case TimeUnit.Minute:
				return Math.Round(span.TotalMinutes, precision).ToString();
			case TimeUnit.Hour:
				return Math.Round(span.TotalHours, precision).ToString();
			default:
				return Math.Round(span.TotalDays, precision).ToString();
			}
		}

		public static bool? Or(this bool? oldVal, bool? newVal)
		{
			if (newVal != null)
			{
				if (oldVal != null)
				{
					oldVal = new bool?(oldVal.Value || newVal.Value);
				}
				else
				{
					oldVal = newVal;
				}
			}
			return oldVal;
		}

		public static bool? And(this bool? oldVal, bool? newVal)
		{
			if (newVal != null)
			{
				if (oldVal != null)
				{
					oldVal = new bool?(oldVal.Value && newVal.Value);
				}
				else
				{
					oldVal = newVal;
				}
			}
			return oldVal;
		}

		private const double DaysUnitFactor = 86400.0;

		private const double HoursUnitFactor = 3600.0;

		private const double MinutesUnitFactor = 60.0;

		private const double MBUnitFactor = 1048576.0;
	}
}
