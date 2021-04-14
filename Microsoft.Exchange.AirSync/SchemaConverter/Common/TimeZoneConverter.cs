using System;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class TimeZoneConverter
	{
		public static byte[] GetBytes(ExTimeZone srcTimeZone, ExDateTime startTime)
		{
			if (srcTimeZone == null)
			{
				throw new ArgumentNullException("srcTimeZone");
			}
			byte[] array = new byte[172];
			int num = 0;
			REG_TIMEZONE_INFO reg_TIMEZONE_INFO;
			if (startTime == ExDateTime.MinValue)
			{
				reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(srcTimeZone);
			}
			else
			{
				reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(srcTimeZone, startTime);
			}
			byte[] bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.Bias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.Bias);
			char[] array2 = srcTimeZone.AlternativeId.ToCharArray();
			int num2 = Math.Min(array2.Length, 32);
			for (int i = 0; i < num2; i++)
			{
				BitConverter.GetBytes(array2[i]).CopyTo(array, num);
				num += 2;
			}
			for (int j = num2; j < 32; j++)
			{
				array[num++] = 0;
				array[num++] = 0;
			}
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Year).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Month).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.DayOfWeek).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Day).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Hour).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Minute).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Second).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardDate.Milliseconds).CopyTo(array, num);
			num += 2;
			bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.StandardBias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.StandardBias);
			array2 = srcTimeZone.LocalizableDisplayName.ToString(CultureInfo.InvariantCulture).ToCharArray();
			num2 = Math.Min(array2.Length, 32);
			for (int k = 0; k < num2; k++)
			{
				BitConverter.GetBytes(array2[k]).CopyTo(array, num);
				num += 2;
			}
			for (int l = num2; l < 32; l++)
			{
				array[num++] = 0;
				array[num++] = 0;
			}
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Year).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Month).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.DayOfWeek).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Day).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Hour).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Minute).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Second).CopyTo(array, num);
			num += 2;
			BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightDate.Milliseconds).CopyTo(array, num);
			num += 2;
			bytes = BitConverter.GetBytes(reg_TIMEZONE_INFO.DaylightBias);
			bytes.CopyTo(array, num);
			num += Marshal.SizeOf(reg_TIMEZONE_INFO.DaylightBias);
			if (num != 172)
			{
				throw new ConversionException("Failed to convert Timezone into bytes. Length=" + num);
			}
			return array;
		}

		public static ExTimeZone GetExTimeZone(byte[] timeZoneInformation)
		{
			if (timeZoneInformation == null)
			{
				throw new ConversionException("Time zone blob is null");
			}
			if (timeZoneInformation.Length != 172)
			{
				throw new ConversionException("Time zone blob length error");
			}
			int num = 0;
			char[] array = new char[32];
			REG_TIMEZONE_INFO regInfo;
			regInfo.Bias = BitConverter.ToInt32(timeZoneInformation, num);
			num += 4;
			for (int i = 0; i < 32; i++)
			{
				array[i] = BitConverter.ToChar(timeZoneInformation, num);
				num += 2;
			}
			string text = new string(array);
			if (string.IsNullOrWhiteSpace(text.Replace('\0', ' ')))
			{
				text = "Customized Time Zone";
			}
			regInfo.StandardDate.Year = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Month = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.DayOfWeek = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Day = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Hour = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Minute = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Second = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardDate.Milliseconds = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.StandardBias = BitConverter.ToInt32(timeZoneInformation, num);
			num += 4;
			for (int j = 0; j < 32; j++)
			{
				array[j] = BitConverter.ToChar(timeZoneInformation, num);
				num += 2;
			}
			new string(array);
			regInfo.DaylightDate.Year = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Month = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.DayOfWeek = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Day = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Hour = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Minute = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Second = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightDate.Milliseconds = (ushort)BitConverter.ToInt16(timeZoneInformation, num);
			num += 2;
			regInfo.DaylightBias = BitConverter.ToInt32(timeZoneInformation, num);
			num += 4;
			if (num != 172)
			{
				throw new ConversionException("Time zone conversion failed! Read=" + num);
			}
			ExTimeZone result;
			try
			{
				result = TimeZoneHelper.CreateExTimeZoneFromRegTimeZoneInfo(regInfo, text);
			}
			catch (InvalidTimeZoneException ex)
			{
				throw new ConversionException("ExTimeZoneFromRegTimeZoneInfo failed: " + ex.Message);
			}
			return result;
		}

		public static bool IsClientTimeZoneEquivalentToServerTimeZoneRule(ExTimeZone clientTZ, ExTimeZone serverTZ, ExDateTime effectiveTime)
		{
			REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(clientTZ);
			REG_TIMEZONE_INFO v = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(serverTZ);
			if (reg_TIMEZONE_INFO.Equals(v))
			{
				return true;
			}
			REG_TIMEZONE_INFO v2 = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(serverTZ, effectiveTime);
			return reg_TIMEZONE_INFO.Equals(v2);
		}

		public static string ToString(ExDateTime dateTime)
		{
			return TimeZoneConverter.ToString(dateTime, "yyyyMMdd\\THHmmss\\Z", 160);
		}

		public static string ToString(ExDateTime dateTime, string format, int protocolVersion)
		{
			if (protocolVersion >= 160)
			{
				string text = dateTime.TimeZone.Id;
				if (text == "tzone://Microsoft/Utc")
				{
					text = "UTC";
				}
				return dateTime.ToString(format, DateTimeFormatInfo.InvariantInfo) + text;
			}
			dateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(dateTime);
			return dateTime.ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		public static ExDateTime Parse(string dateTimeString, string propertyName)
		{
			return TimeZoneConverter.Parse(dateTimeString, "yyyyMMdd\\THHmmss\\Z", 160, propertyName);
		}

		public static ExDateTime Parse(string dateTimeString, string format, int protocolVersion, string propertyName)
		{
			ExDateTime result;
			if (protocolVersion >= 160)
			{
				int length = format.Replace("\\", string.Empty).Length;
				if (dateTimeString.Length <= length)
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTimeFormat, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidFormatIn" + propertyName
					};
				}
				ExTimeZone exTimeZone;
				if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(dateTimeString.Substring(length), out exTimeZone))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidTimezone, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidTimeZoneIn" + propertyName
					};
				}
				if (!ExDateTime.TryParseExact(exTimeZone, dateTimeString.Remove(length), format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeLocal, out result))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeIn" + propertyName
					};
				}
			}
			else if (!ExDateTime.TryParseExact(dateTimeString, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
			{
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
				{
					ErrorStringForProtocolLogger = "InvalidDateTimeIn" + propertyName
				};
			}
			return result;
		}

		private const int TimeZoneInformationStructSize = 172;
	}
}
