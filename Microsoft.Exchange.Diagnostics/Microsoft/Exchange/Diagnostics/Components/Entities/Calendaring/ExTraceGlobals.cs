using System;

namespace Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring
{
	public static class ExTraceGlobals
	{
		public static Trace EventDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.eventDataProviderTracer == null)
				{
					ExTraceGlobals.eventDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.eventDataProviderTracer;
			}
		}

		public static Trace ReadEventTracer
		{
			get
			{
				if (ExTraceGlobals.readEventTracer == null)
				{
					ExTraceGlobals.readEventTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.readEventTracer;
			}
		}

		public static Trace CreateEventTracer
		{
			get
			{
				if (ExTraceGlobals.createEventTracer == null)
				{
					ExTraceGlobals.createEventTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.createEventTracer;
			}
		}

		public static Trace UpdateEventTracer
		{
			get
			{
				if (ExTraceGlobals.updateEventTracer == null)
				{
					ExTraceGlobals.updateEventTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.updateEventTracer;
			}
		}

		public static Trace DeleteEventTracer
		{
			get
			{
				if (ExTraceGlobals.deleteEventTracer == null)
				{
					ExTraceGlobals.deleteEventTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.deleteEventTracer;
			}
		}

		public static Trace FindEventsTracer
		{
			get
			{
				if (ExTraceGlobals.findEventsTracer == null)
				{
					ExTraceGlobals.findEventsTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.findEventsTracer;
			}
		}

		public static Trace CalendarDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.calendarDataProviderTracer == null)
				{
					ExTraceGlobals.calendarDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.calendarDataProviderTracer;
			}
		}

		public static Trace ReadCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.readCalendarTracer == null)
				{
					ExTraceGlobals.readCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.readCalendarTracer;
			}
		}

		public static Trace CreateCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.createCalendarTracer == null)
				{
					ExTraceGlobals.createCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.createCalendarTracer;
			}
		}

		public static Trace UpdateCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.updateCalendarTracer == null)
				{
					ExTraceGlobals.updateCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.updateCalendarTracer;
			}
		}

		public static Trace DeleteCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.deleteCalendarTracer == null)
				{
					ExTraceGlobals.deleteCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.deleteCalendarTracer;
			}
		}

		public static Trace FindCalendarsTracer
		{
			get
			{
				if (ExTraceGlobals.findCalendarsTracer == null)
				{
					ExTraceGlobals.findCalendarsTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.findCalendarsTracer;
			}
		}

		public static Trace CancelEventTracer
		{
			get
			{
				if (ExTraceGlobals.cancelEventTracer == null)
				{
					ExTraceGlobals.cancelEventTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.cancelEventTracer;
			}
		}

		public static Trace RespondToEventTracer
		{
			get
			{
				if (ExTraceGlobals.respondToEventTracer == null)
				{
					ExTraceGlobals.respondToEventTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.respondToEventTracer;
			}
		}

		public static Trace InstancesQueryTracer
		{
			get
			{
				if (ExTraceGlobals.instancesQueryTracer == null)
				{
					ExTraceGlobals.instancesQueryTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.instancesQueryTracer;
			}
		}

		public static Trace CalendarInteropTracer
		{
			get
			{
				if (ExTraceGlobals.calendarInteropTracer == null)
				{
					ExTraceGlobals.calendarInteropTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.calendarInteropTracer;
			}
		}

		public static Trace CreateSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.createSeriesTracer == null)
				{
					ExTraceGlobals.createSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.createSeriesTracer;
			}
		}

		public static Trace CancelSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.cancelSeriesTracer == null)
				{
					ExTraceGlobals.cancelSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.cancelSeriesTracer;
			}
		}

		public static Trace UpdateSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.updateSeriesTracer == null)
				{
					ExTraceGlobals.updateSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.updateSeriesTracer;
			}
		}

		public static Trace SeriesPendingActionsInteropTracer
		{
			get
			{
				if (ExTraceGlobals.seriesPendingActionsInteropTracer == null)
				{
					ExTraceGlobals.seriesPendingActionsInteropTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.seriesPendingActionsInteropTracer;
			}
		}

		public static Trace SeriesInlineInteropTracer
		{
			get
			{
				if (ExTraceGlobals.seriesInlineInteropTracer == null)
				{
					ExTraceGlobals.seriesInlineInteropTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.seriesInlineInteropTracer;
			}
		}

		public static Trace CreateOccurrenceTracer
		{
			get
			{
				if (ExTraceGlobals.createOccurrenceTracer == null)
				{
					ExTraceGlobals.createOccurrenceTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.createOccurrenceTracer;
			}
		}

		public static Trace ReadCalendarGroupTracer
		{
			get
			{
				if (ExTraceGlobals.readCalendarGroupTracer == null)
				{
					ExTraceGlobals.readCalendarGroupTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.readCalendarGroupTracer;
			}
		}

		public static Trace CreateCalendarGroupTracer
		{
			get
			{
				if (ExTraceGlobals.createCalendarGroupTracer == null)
				{
					ExTraceGlobals.createCalendarGroupTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.createCalendarGroupTracer;
			}
		}

		public static Trace UpdateCalendarGroupTracer
		{
			get
			{
				if (ExTraceGlobals.updateCalendarGroupTracer == null)
				{
					ExTraceGlobals.updateCalendarGroupTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.updateCalendarGroupTracer;
			}
		}

		public static Trace DeleteCalendarGroupTracer
		{
			get
			{
				if (ExTraceGlobals.deleteCalendarGroupTracer == null)
				{
					ExTraceGlobals.deleteCalendarGroupTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.deleteCalendarGroupTracer;
			}
		}

		public static Trace FindCalendarGroupsTracer
		{
			get
			{
				if (ExTraceGlobals.findCalendarGroupsTracer == null)
				{
					ExTraceGlobals.findCalendarGroupsTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.findCalendarGroupsTracer;
			}
		}

		public static Trace MeetingMessageProcessingTracer
		{
			get
			{
				if (ExTraceGlobals.meetingMessageProcessingTracer == null)
				{
					ExTraceGlobals.meetingMessageProcessingTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.meetingMessageProcessingTracer;
			}
		}

		public static Trace CreateReceivedSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.createReceivedSeriesTracer == null)
				{
					ExTraceGlobals.createReceivedSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.createReceivedSeriesTracer;
			}
		}

		public static Trace RespondToSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.respondToSeriesTracer == null)
				{
					ExTraceGlobals.respondToSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.respondToSeriesTracer;
			}
		}

		public static Trace ForwardEventTracer
		{
			get
			{
				if (ExTraceGlobals.forwardEventTracer == null)
				{
					ExTraceGlobals.forwardEventTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.forwardEventTracer;
			}
		}

		public static Trace ForwardSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.forwardSeriesTracer == null)
				{
					ExTraceGlobals.forwardSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.forwardSeriesTracer;
			}
		}

		public static Trace SeriesActionParserTracer
		{
			get
			{
				if (ExTraceGlobals.seriesActionParserTracer == null)
				{
					ExTraceGlobals.seriesActionParserTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.seriesActionParserTracer;
			}
		}

		public static Trace ExpandSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.expandSeriesTracer == null)
				{
					ExTraceGlobals.expandSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.expandSeriesTracer;
			}
		}

		public static Trace MeetingRequestMessageDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.meetingRequestMessageDataProviderTracer == null)
				{
					ExTraceGlobals.meetingRequestMessageDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.meetingRequestMessageDataProviderTracer;
			}
		}

		public static Trace RespondToMeetingRequestTracer
		{
			get
			{
				if (ExTraceGlobals.respondToMeetingRequestTracer == null)
				{
					ExTraceGlobals.respondToMeetingRequestTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.respondToMeetingRequestTracer;
			}
		}

		public static Trace ConvertSingleEventToNprSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.convertSingleEventToNprSeriesTracer == null)
				{
					ExTraceGlobals.convertSingleEventToNprSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.convertSingleEventToNprSeriesTracer;
			}
		}

		public static Trace GetCalendarViewTracer
		{
			get
			{
				if (ExTraceGlobals.getCalendarViewTracer == null)
				{
					ExTraceGlobals.getCalendarViewTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.getCalendarViewTracer;
			}
		}

		public static Trace DeleteSeriesTracer
		{
			get
			{
				if (ExTraceGlobals.deleteSeriesTracer == null)
				{
					ExTraceGlobals.deleteSeriesTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.deleteSeriesTracer;
			}
		}

		private static Guid componentGuid = new Guid("6B844120-1AE2-4E8C-ABDB-F3D7F3E95388");

		private static Trace eventDataProviderTracer = null;

		private static Trace readEventTracer = null;

		private static Trace createEventTracer = null;

		private static Trace updateEventTracer = null;

		private static Trace deleteEventTracer = null;

		private static Trace findEventsTracer = null;

		private static Trace calendarDataProviderTracer = null;

		private static Trace readCalendarTracer = null;

		private static Trace createCalendarTracer = null;

		private static Trace updateCalendarTracer = null;

		private static Trace deleteCalendarTracer = null;

		private static Trace findCalendarsTracer = null;

		private static Trace cancelEventTracer = null;

		private static Trace respondToEventTracer = null;

		private static Trace instancesQueryTracer = null;

		private static Trace calendarInteropTracer = null;

		private static Trace createSeriesTracer = null;

		private static Trace cancelSeriesTracer = null;

		private static Trace updateSeriesTracer = null;

		private static Trace seriesPendingActionsInteropTracer = null;

		private static Trace seriesInlineInteropTracer = null;

		private static Trace createOccurrenceTracer = null;

		private static Trace readCalendarGroupTracer = null;

		private static Trace createCalendarGroupTracer = null;

		private static Trace updateCalendarGroupTracer = null;

		private static Trace deleteCalendarGroupTracer = null;

		private static Trace findCalendarGroupsTracer = null;

		private static Trace meetingMessageProcessingTracer = null;

		private static Trace createReceivedSeriesTracer = null;

		private static Trace respondToSeriesTracer = null;

		private static Trace forwardEventTracer = null;

		private static Trace forwardSeriesTracer = null;

		private static Trace seriesActionParserTracer = null;

		private static Trace expandSeriesTracer = null;

		private static Trace meetingRequestMessageDataProviderTracer = null;

		private static Trace respondToMeetingRequestTracer = null;

		private static Trace convertSingleEventToNprSeriesTracer = null;

		private static Trace getCalendarViewTracer = null;

		private static Trace deleteSeriesTracer = null;
	}
}
