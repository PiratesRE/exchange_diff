using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class O11TimeZoneFormatter
	{
		public static byte[] GetTimeZoneBlob(ExTimeZone timeZone)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (timeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				throw new ArgumentException("timeZone should not be UnspecifiedTimeZone");
			}
			REG_TIMEZONE_INFO timeZoneInfo = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone);
			return O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.ToBytesFromRegistryFormat(timeZoneInfo);
		}

		public static bool TryParseTimeZoneBlob(byte[] bytes, string displayName, out ExTimeZone timeZone)
		{
			timeZone = null;
			if (bytes == null || displayName == null)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "O11TimeZoneParser, time zone blob or display name is null");
				return false;
			}
			if (bytes.Length < O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.Size)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>(0L, "O11TimeZoneParser, corrupted TimeZoneBlob. Length {0} less than standard blob", bytes.Length);
				return false;
			}
			if (bytes.Length > O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.Size)
			{
				ExTraceGlobals.StorageTracer.TraceWarning<int>(0L, "O11TimeZoneParser, corrupted TimeZoneBlob Length {0}, going to trim extra bytes", bytes.Length);
			}
			REG_TIMEZONE_INFO regInfo = O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.ParseToRegistryFormat(bytes);
			try
			{
				timeZone = TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(regInfo, string.Empty, displayName);
			}
			catch (InvalidTimeZoneException ex)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "O11TimeZoneParser, corrupted time zone, blob is not valid registry format. Inner message is {0}", ex.Message);
			}
			return timeZone != null;
		}

		private struct OUTLOOK_TIMEZONE_INFO
		{
			public static REG_TIMEZONE_INFO ParseToRegistryFormat(byte[] bytes)
			{
				if (bytes.Length < O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.Size)
				{
					throw new ArgumentOutOfRangeException();
				}
				return new REG_TIMEZONE_INFO
				{
					Bias = BitConverter.ToInt32(bytes, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.BiasOffset),
					StandardBias = BitConverter.ToInt32(bytes, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.StandardBiasOffset),
					DaylightBias = BitConverter.ToInt32(bytes, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.DaylightBiasOffset),
					StandardDate = NativeMethods.SystemTime.Parse(new ArraySegment<byte>(bytes, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.StandardDateOffset, NativeMethods.SystemTime.Size)),
					DaylightDate = NativeMethods.SystemTime.Parse(new ArraySegment<byte>(bytes, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.DaylightDateOffset, NativeMethods.SystemTime.Size))
				};
			}

			public static byte[] ToBytesFromRegistryFormat(REG_TIMEZONE_INFO timeZoneInfo)
			{
				byte[] array = new byte[O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.Size];
				ExBitConverter.Write(timeZoneInfo.Bias, array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.BiasOffset);
				ExBitConverter.Write(timeZoneInfo.StandardBias, array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.StandardBiasOffset);
				ExBitConverter.Write(timeZoneInfo.DaylightBias, array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.DaylightBiasOffset);
				ExBitConverter.Write(0, array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.StandardYearOffset);
				timeZoneInfo.StandardDate.Write(new ArraySegment<byte>(array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.StandardDateOffset, NativeMethods.SystemTime.Size));
				ExBitConverter.Write(0, array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.DaylightYearOffset);
				timeZoneInfo.DaylightDate.Write(new ArraySegment<byte>(array, O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO.DaylightDateOffset, NativeMethods.SystemTime.Size));
				return array;
			}

			private const int DefaultStandardYear = 0;

			private const int DefaultDaylightYear = 0;

			public static readonly int Size = Marshal.SizeOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO));

			private static readonly int BiasOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "bias");

			private static readonly int StandardBiasOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "standardBias");

			private static readonly int DaylightBiasOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "daylightBias");

			private static readonly int StandardYearOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "standardYear");

			private static readonly int StandardDateOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "standardDate");

			private static readonly int DaylightYearOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "daylightYear");

			private static readonly int DaylightDateOffset = (int)Marshal.OffsetOf(typeof(O11TimeZoneFormatter.OUTLOOK_TIMEZONE_INFO), "daylightDate");

			private int bias;

			private int standardBias;

			private int daylightBias;

			private short standardYear;

			private NativeMethods.SystemTime standardDate;

			private short daylightYear;

			private NativeMethods.SystemTime daylightDate;
		}
	}
}
