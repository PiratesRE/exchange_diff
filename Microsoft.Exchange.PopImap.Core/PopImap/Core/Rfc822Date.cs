using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PopImap.Core
{
	public sealed class Rfc822Date
	{
		public static string Format(ExDateTime dateTime)
		{
			return Rfc822Date.Format("dd-MMM-yyyy HH:mm:ss ", dateTime);
		}

		public static string FormatLong(ExDateTime dateTime)
		{
			return Rfc822Date.Format("ddd, d MMM yyyy HH:mm:ss ", dateTime);
		}

		public static bool TryParse(string dateTimeString, out ExDateTime date)
		{
			if (string.IsNullOrEmpty(dateTimeString) || (dateTimeString.Length != 25 && dateTimeString.Length != 26))
			{
				date = ResponseFactory.CurrentExTimeZone.ConvertDateTime(ExDateTime.UtcNow);
				return false;
			}
			string s = dateTimeString.Insert(dateTimeString.Length - 2, ":");
			if (!ExDateTime.TryParseExact(s, "d-MMM-yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowLeadingWhite, out date))
			{
				return false;
			}
			date = ResponseFactory.CurrentExTimeZone.ConvertDateTime(date);
			return true;
		}

		private static string Format(string format, ExDateTime dateTime)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			TimeSpan bias = dateTime.Bias;
			stringBuilder.Append(dateTime.ToString(format, CultureInfo.InvariantCulture));
			stringBuilder.Append((bias.Hours * 100 + bias.Minutes).ToString("+0000;-0000"));
			return stringBuilder.ToString();
		}

		private const string DateTimeFormat = "d-MMM-yyyy HH:mm:ss zzz";
	}
}
