using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DateTimeHelper
	{
		public static ExDateTime GetFutureTimestamp(ExDateTime startTime, int afterMinimumDays, DayOfWeek onDayOfWeek, TimeSpan atTimeOfDay, ExTimeZone localTimeZone)
		{
			ExDateTime exDateTime = localTimeZone.ConvertDateTime(startTime);
			int num = (exDateTime.TimeOfDay > atTimeOfDay) ? 1 : 0;
			ExDateTime exDateTime2 = exDateTime.AddDays((double)(afterMinimumDays + num));
			int num2 = onDayOfWeek - exDateTime2.DayOfWeek;
			if (num2 < 0)
			{
				num2 += 7;
			}
			return exDateTime2.AddDays((double)num2).Date.Add(atTimeOfDay).ToUtc();
		}

		public static ExTimeZone GetUserTimeZoneOrUtc(MailboxSession session)
		{
			return DateTimeHelper.GetUserTimeZoneOrDefault(session, ExTimeZone.UtcTimeZone);
		}

		public static ExTimeZone GetUserTimeZoneOrDefault(MailboxSession session, ExTimeZone defaultTimeZone)
		{
			ExTimeZone exTimeZone = session.ExTimeZone;
			try
			{
				exTimeZone = TimeZoneHelper.GetUserTimeZone(session);
			}
			catch (TransientException arg)
			{
				InferenceDiagnosticsLog.Log("NotificationManager", string.Format("Ignoring transient exception caught while loading user time zone and defaulting to UTC time zone. Exception: {0}", arg));
			}
			catch (LocalizedException arg2)
			{
				InferenceDiagnosticsLog.Log("NotificationManager", string.Format("Ignoring localized exception caught while loading user time zone and defaulting to UTC time zone. Exception: {0}", arg2));
			}
			exTimeZone = (exTimeZone ?? defaultTimeZone);
			return exTimeZone;
		}
	}
}
