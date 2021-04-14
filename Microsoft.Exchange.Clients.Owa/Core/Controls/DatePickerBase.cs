using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class DatePickerBase
	{
		public static void GetVisibleDateRange(ExDateTime month, out ExDateTime startDate, out ExDateTime endDate, ExTimeZone timeZone)
		{
			Calendar calendar = new GregorianCalendar();
			int weekStartDay = (int)OwaContext.Current.SessionContext.WeekStartDay;
			ExDateTime exDateTime = new ExDateTime(timeZone, new DateTime(month.Year, month.Month, 1, 0, 0, 0, 0, calendar));
			int dayOfWeek = (int)calendar.GetDayOfWeek((DateTime)exDateTime);
			int num = dayOfWeek - weekStartDay;
			num = ((num < 0) ? (7 + num) : num);
			startDate = new ExDateTime(timeZone, calendar.AddDays((DateTime)exDateTime, -1 * num));
			endDate = new ExDateTime(timeZone, calendar.AddDays((DateTime)startDate, 41));
		}

		public const int TotalDays = 42;
	}
}
