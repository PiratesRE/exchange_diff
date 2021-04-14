using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class EcpDateTimeHelper
	{
		public static string UtcToUserDateTimeString(this DateTime? dateTimeWithoutTimeZone)
		{
			if (dateTimeWithoutTimeZone == null)
			{
				return string.Empty;
			}
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTimeWithoutTimeZone.Value).ToUserDateTimeString();
		}

		public static string UtcToUserDateTimeString(this DateTime dateTimeWithoutTimeZone)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTimeWithoutTimeZone).ToUserDateTimeString();
		}

		public static string UtcToUserDateString(this DateTime dateTimeWithoutTimeZone)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTimeWithoutTimeZone).ToUserDateString();
		}

		public static string ToUserWeekdayDateString(this ExDateTime? exDateTimeValue)
		{
			if (exDateTimeValue == null)
			{
				return string.Empty;
			}
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return exDateTimeValue.Value.ToUserExDateTime().ToString(EcpDateTimeHelper.GetWeekdayDateFormat(true), currentCulture);
		}

		public static string ToUserDateTimeString(this ExDateTime? exDateTimeValue)
		{
			if (exDateTimeValue == null)
			{
				return string.Empty;
			}
			return exDateTimeValue.Value.ToUserDateTimeString();
		}

		public static string ToUserDateString(this ExDateTime? exDateTimeValue)
		{
			if (exDateTimeValue == null)
			{
				return string.Empty;
			}
			return exDateTimeValue.Value.ToUserDateString();
		}

		public static string ToUserDateTimeString(this ExDateTime dateTimeValue)
		{
			string format = null;
			if (EacRbacPrincipal.Instance.DateFormat != null && EacRbacPrincipal.Instance.TimeFormat != null)
			{
				format = EacRbacPrincipal.Instance.DateFormat + " " + EacRbacPrincipal.Instance.TimeFormat;
			}
			return dateTimeValue.ToUserExDateTime().ToString(format);
		}

		public static string ToUserDateString(this ExDateTime dateTimeValue)
		{
			string format = null;
			if (EacRbacPrincipal.Instance.DateFormat != null)
			{
				format = EacRbacPrincipal.Instance.DateFormat;
			}
			return dateTimeValue.ToUserExDateTime().ToString(format);
		}

		public static string ToLastMonth()
		{
			return ExDateTime.GetNow(EcpDateTimeHelper.GetCurrentUserTimeZone()).AddMonths(-1).ToString(Strings.LastMonthFormat);
		}

		public static string LocalToUserDateTimeString(this DateTime dateTimeInLocalTime)
		{
			return new ExDateTime(ExTimeZone.CurrentTimeZone, dateTimeInLocalTime).ToUserDateTimeString();
		}

		public static string LocalToUserDateTimeGeneralFormatString(this DateTime dateTimeInLocalTime)
		{
			return new ExDateTime(ExTimeZone.CurrentTimeZone, dateTimeInLocalTime).ToUserDateTimeGeneralFormatString();
		}

		public static string ToUserDateTimeGeneralFormatString(this ExDateTime? exDateTimeValue)
		{
			if (exDateTimeValue == null)
			{
				return string.Empty;
			}
			return exDateTimeValue.Value.ToUserDateTimeGeneralFormatString();
		}

		public static string ToUserDateTimeGeneralFormatString(this ExDateTime dateTimeValue)
		{
			return dateTimeValue.ToUserExDateTime().ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public static ExTimeZone GetCurrentUserTimeZone()
		{
			if (EacRbacPrincipal.Instance.UserTimeZone != null)
			{
				return EacRbacPrincipal.Instance.UserTimeZone;
			}
			return ExTimeZone.CurrentTimeZone;
		}

		public static ExDateTime? ToEcpExDateTime(this string dateString)
		{
			return dateString.ToEcpExDateTime("yyyy/MM/dd");
		}

		public static ExDateTime? ToEcpExDateTime(this string dateTimeString, string parseFormat)
		{
			if (dateTimeString.IsNullOrBlank())
			{
				return null;
			}
			ExDateTime? result;
			try
			{
				if (EacRbacPrincipal.Instance.UserTimeZone != null)
				{
					result = new ExDateTime?(ExDateTime.ParseExact(EacRbacPrincipal.Instance.UserTimeZone, dateTimeString, parseFormat, CultureInfo.InvariantCulture));
				}
				else
				{
					int browserTimeZoneOffsetMinutes = EcpDateTimeHelper.GetBrowserTimeZoneOffsetMinutes();
					result = new ExDateTime?(ExDateTime.ParseExact(ExTimeZone.UtcTimeZone, dateTimeString, parseFormat, CultureInfo.InvariantCulture).AddMinutes((double)browserTimeZoneOffsetMinutes));
				}
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.EventLogTracer.TraceError<string, string>(0, 0L, "Fail to parse the date time string: {0}. Got the exception of '{1}'.", dateTimeString, ex.Message);
				result = null;
			}
			return result;
		}

		public static ExDateTime ToUserExDateTime(this ExDateTime dateTimeValue)
		{
			if (EacRbacPrincipal.Instance.UserTimeZone != null)
			{
				return EacRbacPrincipal.Instance.UserTimeZone.ConvertDateTime(dateTimeValue);
			}
			int browserTimeZoneOffsetMinutes = EcpDateTimeHelper.GetBrowserTimeZoneOffsetMinutes();
			return dateTimeValue.ToUtc().AddMinutes((double)(-(double)browserTimeZoneOffsetMinutes));
		}

		private static int GetBrowserTimeZoneOffsetMinutes()
		{
			int result = 0;
			HttpCookie httpCookie = HttpContext.Current.Request.Cookies["TimeOffset"];
			if (httpCookie != null)
			{
				int.TryParse(httpCookie.Value, out result);
			}
			return result;
		}

		public static string GetWeekdayDateFormat(bool serverSideFormat)
		{
			string text = EacRbacPrincipal.Instance.DateFormat ?? string.Empty;
			string text2 = "ddd " + text;
			int lcid = CultureInfo.CurrentCulture.LCID;
			switch (lcid)
			{
			case 1041:
			case 1042:
				break;
			default:
				if (lcid != 1055 && lcid != 1063)
				{
					goto IL_6C;
				}
				break;
			}
			if (text.Contains("ddd"))
			{
				text2 = text;
			}
			else
			{
				text2 = text + " (ddd)";
			}
			IL_6C:
			if (!serverSideFormat)
			{
				if (text2.Contains("ddd"))
				{
					text2 = text2.Replace("ddd", "'ddd'");
				}
				if (text.Contains("MMMM"))
				{
					text = text.Replace("MMMM", "'MMMM'");
				}
				else if (text.Contains("MMM"))
				{
					text = text.Replace("MMM", "'MMM'");
				}
			}
			return text2;
		}

		public static string GetUserDateFormat()
		{
			if (EacRbacPrincipal.Instance.DateFormat != null)
			{
				return EacRbacPrincipal.Instance.DateFormat;
			}
			return "yyyy/MM/dd";
		}

		public static DateTime GetReportStartDate()
		{
			int months = -1;
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow.Day <= 14)
			{
				months = -2;
			}
			return new DateTime(utcNow.Year, utcNow.Month, 1).AddMonths(months);
		}

		public const string BrowserTimeZoneCookieName = "TimeOffset";

		public const string GeneralizedDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

		public const string GeneralizedDateFormat = "yyyy/MM/dd";

		public const string AuditSearchDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
	}
}
