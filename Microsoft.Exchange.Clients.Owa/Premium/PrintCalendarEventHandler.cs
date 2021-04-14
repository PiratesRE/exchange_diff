using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("PrintCalendar")]
	[OwaEventSegmentation(Feature.Calendar)]
	internal sealed class PrintCalendarEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(PrintCalendarEventHandler));
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("GetEventList")]
		[OwaEventParameter("days", typeof(ExDateTime), true)]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEventParameter("SH", typeof(int))]
		[OwaEventParameter("EH", typeof(int))]
		[OwaEventParameter("wo", typeof(bool), false, true)]
		public void GetEventList()
		{
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			ExDateTime[] days = (ExDateTime[])base.GetParameter("days");
			CalendarViewType viewType = (CalendarViewType)base.GetParameter("vt");
			int startHour = (int)base.GetParameter("SH");
			int endHour = (int)base.GetParameter("EH");
			bool workingDayOnly = false;
			if (base.IsParameterSet("wo"))
			{
				workingDayOnly = (bool)base.GetParameter("wo");
			}
			using (CalendarAdapter calendarAdapter = new CalendarAdapter(base.UserContext, folderId))
			{
				int num;
				ReadingPanePosition readingPanePosition;
				calendarAdapter.LoadData(CalendarUtilities.PrintQueryProperties, days, false, startHour, endHour, ref viewType, out num, out readingPanePosition);
				if (!calendarAdapter.UserCanReadItem)
				{
					throw new OwaInvalidRequestException("no read access to the calendar");
				}
				PrintEventList printEventList = new PrintEventList(base.UserContext, calendarAdapter, viewType, workingDayOnly);
				printEventList.RenderView(this.SanitizingWriter);
			}
		}

		[OwaEvent("GetEventListForPublishedCalendar", false, true)]
		[OwaEventParameter("days", typeof(ExDateTime), true)]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEventParameter("SH", typeof(int))]
		[OwaEventParameter("EH", typeof(int))]
		public void GetEventListForPublishedCalendar()
		{
			ExDateTime[] days = (ExDateTime[])base.GetParameter("days");
			CalendarViewType viewType = (CalendarViewType)base.GetParameter("vt");
			int startHour = (int)base.GetParameter("SH");
			int endHour = (int)base.GetParameter("EH");
			AnonymousSessionContext anonymousSessionContext = base.SessionContext as AnonymousSessionContext;
			if (anonymousSessionContext == null)
			{
				throw new OwaInvalidRequestException("This request can only be sent to Calendar VDir");
			}
			using (PublishedCalendarAdapter publishedCalendarAdapter = new PublishedCalendarAdapter(anonymousSessionContext))
			{
				publishedCalendarAdapter.LoadData(CalendarUtilities.QueryProperties, days, startHour, endHour, viewType);
				if (!publishedCalendarAdapter.UserCanReadItem)
				{
					throw new OwaInvalidRequestException("no read access to the calendar");
				}
				PrintEventList printEventList = new PrintEventList(base.SessionContext, publishedCalendarAdapter, viewType, false);
				printEventList.RenderView(this.SanitizingWriter);
			}
		}

		public const string EventNamespace = "PrintCalendar";

		public const string MethodGetEventList = "GetEventList";

		public const string MethodGetEventListForPublishedCalendar = "GetEventListForPublishedCalendar";

		public const string FolderId = "fId";

		public const string Days = "days";

		public const string ViewType = "vt";

		public const string StartHour = "SH";

		public const string EndHour = "EH";

		public const string WorkingDayOnly = "wo";
	}
}
