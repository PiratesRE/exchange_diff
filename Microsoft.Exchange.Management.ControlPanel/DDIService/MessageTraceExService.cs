using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MessageTraceExService
	{
		public static DateTime CalculateDate(string date, string timeZoneName)
		{
			ExTimeZone utcTimeZone;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneName, out utcTimeZone))
			{
				utcTimeZone = ExTimeZone.UtcTimeZone;
			}
			DateTime dateTime = DateTime.ParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
			ExDateTime exDateTime = new ExDateTime(utcTimeZone, dateTime);
			return exDateTime.UniversalTime;
		}
	}
}
