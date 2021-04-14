using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class InternalCalendarQuery
	{
		internal static CalendarEvent[] GetCalendarEvents(EmailAddress emailAddress, CalendarFolder calendarFolder, ExDateTime windowStart, ExDateTime windowEnd, FreeBusyViewType accessAllowed, bool canActAsMailboxOwner, ExchangeVersionType exchangeVersion)
		{
			CalendarEvent[] array = null;
			int num = 0;
			object[][] calendarView = calendarFolder.GetCalendarView(windowStart, windowEnd, InternalCalendarQuery.CalendarQueryProperties);
			InternalCalendarQuery.CalendarViewTracer.TraceDebug(0L, "{0}: Query for {1} found {2} appointment entries between {3} and {4}", new object[]
			{
				TraceContext.Get(),
				emailAddress,
				calendarView.Length,
				windowStart,
				windowEnd
			});
			num += calendarView.Length;
			if (num > Configuration.MaximumResultSetSize)
			{
				LocalizedException ex = new ResultSetTooBigException(Configuration.MaximumResultSetSize, num);
				InternalCalendarQuery.CalendarViewTracer.TraceError<object, EmailAddress, LocalizedException>(0L, "{0}: Query for {1} got exception getting Calendar Data. Exception: {2}", TraceContext.Get(), emailAddress, ex);
				throw ex;
			}
			int length = calendarView.GetLength(0);
			if (length > 0)
			{
				array = new CalendarEvent[length];
				for (int i = 0; i < length; i++)
				{
					object[] properties = calendarView[i];
					array[i] = CalendarEvent.CreateFromQueryData(emailAddress, properties, accessAllowed, canActAsMailboxOwner, exchangeVersion);
				}
			}
			return array;
		}

		private static readonly Trace CalendarViewTracer = ExTraceGlobals.CalendarViewTracer;

		private static readonly PropertyDefinition[] CalendarQueryProperties = new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			CalendarItemBaseSchema.FreeBusyStatus,
			ItemSchema.Id,
			ItemSchema.Sensitivity,
			ItemSchema.Subject,
			CalendarItemBaseSchema.Location,
			ItemSchema.ReminderIsSet,
			CalendarItemBaseSchema.AppointmentState,
			CalendarItemBaseSchema.CalendarItemType,
			CalendarItemBaseSchema.GlobalObjectId
		};
	}
}
