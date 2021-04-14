using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal static class CalendarInterop
	{
		public static void ExecutePendingInteropActions(IStoreSession session, ICalendarItemSeries calendarItemSeries, IStorageTranslator<ICalendarItemBase, Event> eventTranslator = null, IXSOFactory xsoFactory = null, ICalendarInteropLog logger = null)
		{
			xsoFactory = (xsoFactory ?? XSOFactory.Default);
			logger = (logger ?? CalendarInteropLog.Default);
			eventTranslator = (eventTranslator ?? EventTranslator.Instance);
			try
			{
				Event @event = eventTranslator.ConvertToEntity(calendarItemSeries);
				EventReference eventReference = new EventReference(xsoFactory, calendarItemSeries);
				SeriesPendingActionsInterop seriesPendingActionsInterop = new SeriesPendingActionsInterop(logger, null)
				{
					Entity = @event,
					EntityKey = @event.Id,
					Scope = (Events)eventReference.Events
				};
				seriesPendingActionsInterop.Execute(null);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CalendarInteropTracer.TraceError<string, Exception>(0L, "Error executing pending interop actions for series {0}. Error {1}", calendarItemSeries.SeriesId, ex);
				logger.SafeLogEntry(session, ex, false, "Error executing pending interop actions for series {0}.", new object[]
				{
					calendarItemSeries.SeriesId
				});
				throw;
			}
		}
	}
}
