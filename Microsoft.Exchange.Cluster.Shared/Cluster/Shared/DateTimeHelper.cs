using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Shared
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DateTimeHelper
	{
		public static bool IsValidDateTime(DateTime dateTime)
		{
			return dateTime > DateTimeHelper.s_minDateTime && dateTime < DateTime.MaxValue;
		}

		public static string ToPersistedString(DateTime dateTime)
		{
			return DateTimeHelper.ToPersistedString(dateTime);
		}

		public static string ToPersistedString(ExDateTime exDateTime)
		{
			return DateTimeHelper.ToPersistedString(exDateTime.UniversalTime);
		}

		public static DateTime ParseIntoDateTime(string dateTimeStr)
		{
			return DateTimeHelper.ParseIntoDateTime(dateTimeStr);
		}

		public static ExDateTime? ParseIntoNullableExDateTimeIfPossible(string dateTimeStr)
		{
			ExDateTime? result = null;
			DateTime dateTime;
			if (DateTimeHelper.TryParseIntoDateTime(dateTimeStr, out dateTime))
			{
				result = DateTimeHelper.ToNullableExDateTime(dateTime);
			}
			return result;
		}

		public static DateTime? ParseIntoNullableDateTimeIfPossible(string dateTimeStr)
		{
			DateTime? result = null;
			DateTime dateTime;
			if (DateTimeHelper.TryParseIntoDateTime(dateTimeStr, out dateTime))
			{
				result = DateTimeHelper.ToNullableDateTime(dateTime);
			}
			return result;
		}

		public static DateTime? ParseIntoNullableLocalDateTimeIfPossible(string dateTimeStr)
		{
			DateTime? result = null;
			DateTime dateTime;
			if (DateTimeHelper.TryParseIntoDateTime(dateTimeStr, out dateTime))
			{
				result = DateTimeHelper.ToNullableLocalDateTime(dateTime);
			}
			return result;
		}

		public static ExDateTime? ToNullableExDateTime(DateTime dateTime)
		{
			if (DateTimeHelper.IsValidDateTime(dateTime))
			{
				return new ExDateTime?(new ExDateTime(ExTimeZone.CurrentTimeZone, dateTime));
			}
			return null;
		}

		public static DateTime? ToNullableDateTime(DateTime dateTime)
		{
			if (DateTimeHelper.IsValidDateTime(dateTime))
			{
				return new DateTime?(dateTime);
			}
			return null;
		}

		public static DateTime? ToNullableLocalDateTime(DateTime dateTime)
		{
			if (DateTimeHelper.IsValidDateTime(dateTime))
			{
				return new DateTime?(dateTime.ToLocalTime());
			}
			return null;
		}

		public static DateTime? ToNullableLocalDateTime(DateTime? dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			return DateTimeHelper.ToNullableLocalDateTime(dateTime.Value);
		}

		public static ExDateTime ParseIntoExDateTime(string dateTimeStr)
		{
			return ExDateTime.Parse(ExTimeZone.CurrentTimeZone, dateTimeStr);
		}

		public static bool TryParseIntoDateTime(string dateTimeStr, out DateTime dateTimeValue)
		{
			return DateTimeHelper.TryParseIntoDateTime(dateTimeStr, ref dateTimeValue);
		}

		public static bool TryParseIntoExDateTime(string dateTimeStr, out ExDateTime dateTimeValue)
		{
			return ExDateTime.TryParse(ExTimeZone.CurrentTimeZone, dateTimeStr, out dateTimeValue);
		}

		public static ExDateTime ToLocalExDateTime(DateTime dateTime)
		{
			return new ExDateTime(ExTimeZone.CurrentTimeZone, dateTime);
		}

		public static string ConvertTimeSpanToShortString(TimeSpan timeSpan)
		{
			return DateTimeHelper.ConvertTimeSpanToShortString(timeSpan);
		}

		public static string ConvertTimeSpanToShortString(EnhancedTimeSpan timeSpan)
		{
			return DateTimeHelper.ConvertTimeSpanToShortString(timeSpan);
		}

		public static DateTime FromFileTimeUtc(NativeMethods.FILETIME ft)
		{
			long fileTime = (long)(((ulong)ft.DateTimeHigh << 32) + (ulong)ft.DateTimeLow);
			return DateTime.FromFileTimeUtc(fileTime);
		}

		private static readonly DateTime s_minDateTime = DateTime.FromFileTimeUtc(0L);
	}
}
