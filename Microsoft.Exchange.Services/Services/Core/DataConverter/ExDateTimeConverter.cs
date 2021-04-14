using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ExDateTimeConverter : BaseConverter
	{
		public static string ToSoapHeaderTimeZoneRelatedXsdDateTime(ExDateTime systemDateTime)
		{
			return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime, ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010));
		}

		public static string ToSoapHeaderTimeZoneRelatedXsdDateTime(ExDateTime systemDateTime, bool supportsExchange2010SchemaVersion)
		{
			if (supportsExchange2010SchemaVersion)
			{
				return ExDateTimeConverter.ToOffsetXsdDateTime(systemDateTime, EWSSettings.RequestTimeZone);
			}
			return systemDateTime.UniversalTime.ToString((EWSSettings.DateTimePrecision == DateTimePrecision.Milliseconds) ? "yyyy-MM-ddTHH:mm:ss.fff\\Z" : "yyyy-MM-ddTHH:mm:ss\\Z", CultureInfo.InvariantCulture);
		}

		public static string ToUtcXsdDateTime(ExDateTime systemDateTime)
		{
			return systemDateTime.UniversalTime.ToString((EWSSettings.DateTimePrecision == DateTimePrecision.Milliseconds) ? "yyyy-MM-ddTHH:mm:ss.fff\\Z" : "yyyy-MM-ddTHH:mm:ss\\Z", CultureInfo.InvariantCulture);
		}

		public static string ToUtcXsdTime(TimeSpan timeSpan)
		{
			return timeSpan.ToString();
		}

		public static ExDateTime Parse(string propertyString)
		{
			ExDateTime result;
			try
			{
				DateTime dateTime = DateTime.Parse(propertyString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
				result = (ExDateTime)dateTime;
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, ArgumentException>(0L, "[ExDateTimeConverter::Parse] DateTime.Parse threw an ArgumentException for string '{0}': {1}", propertyString, ex);
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, ex);
			}
			catch (FormatException ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, FormatException>(0L, "[ExDateTimeConverter::Parse] DateTime.Parse threw a FormatException for string '{0}': {1}", propertyString, ex2);
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, ex2);
			}
			return result;
		}

		public static string ToOffsetXsdDateTime(ExDateTime dateTime, ExTimeZone displayTimeZone)
		{
			ExDateTime exDateTime = displayTimeZone.ConvertDateTime(dateTime);
			if (exDateTime.Bias.Ticks == 0L)
			{
				return exDateTime.ToString((EWSSettings.DateTimePrecision == DateTimePrecision.Milliseconds) ? "yyyy-MM-ddTHH:mm:ss.fff\\Z" : "yyyy-MM-ddTHH:mm:ss\\Z", CultureInfo.InvariantCulture);
			}
			string str = exDateTime.ToString((EWSSettings.DateTimePrecision == DateTimePrecision.Milliseconds) ? "yyyy-MM-ddTHH:mm:ss.fff" : "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
			string str2 = ExDateTimeConverter.ConvertBiasToString(exDateTime.Bias);
			return str + str2;
		}

		public override object ConvertToObject(string propertyString)
		{
			ExDateTime exDateTime = ExDateTimeConverter.ParseTimeZoneRelated(propertyString, EWSSettings.RequestTimeZone);
			exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
			return exDateTime;
		}

		public override string ConvertToString(object propertyValue)
		{
			return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)propertyValue);
		}

		private static string ConvertBiasToString(TimeSpan bias)
		{
			if (bias.Ticks < 0L)
			{
				return (-bias.Hours).ToString("-00", CultureInfo.InvariantCulture) + (-bias.Minutes).ToString(":00", CultureInfo.InvariantCulture);
			}
			return bias.Hours.ToString("+00", CultureInfo.InvariantCulture) + bias.Minutes.ToString(":00", CultureInfo.InvariantCulture);
		}

		public static ExDateTime ParseTimeZoneRelated(string propertyString, ExTimeZone timeZone)
		{
			ExDateTime result;
			try
			{
				DateTime dateTime = XmlConvert.ToDateTime(propertyString, XmlDateTimeSerializationMode.RoundtripKind);
				if (dateTime.Kind == DateTimeKind.Unspecified)
				{
					result = new ExDateTime(timeZone, dateTime);
				}
				else
				{
					if (dateTime.Kind == DateTimeKind.Local)
					{
						dateTime = XmlConvert.ToDateTime(propertyString, XmlDateTimeSerializationMode.Utc);
					}
					result = new ExDateTime(timeZone, dateTime, null);
				}
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, innerException);
			}
			catch (FormatException innerException2)
			{
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, innerException2);
			}
			return result;
		}

		private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

		private const string DateTimeFormatZulu = "yyyy-MM-ddTHH:mm:ss\\Z";

		private const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ss.fff";

		private const string DateTimeFormatPreciseZulu = "yyyy-MM-ddTHH:mm:ss.fff\\Z";
	}
}
