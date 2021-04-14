using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PrintCalendarView : OwaPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "vt", false);
			int num = 1;
			if (!string.IsNullOrEmpty(queryStringParameter) && (!int.TryParse(queryStringParameter, out num) || num < 1 || num >= 7))
			{
				throw new OwaInvalidRequestException("View type error");
			}
			this.viewType = (CalendarViewType)num;
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "wo", false);
			if (queryStringParameter2 == "1" || this.viewType == CalendarViewType.WorkWeek)
			{
				this.workingDayOnly = true;
			}
			this.days = Utilities.GetQueryStringParameterDateTimeArray(base.Request, "d", base.SessionContext.TimeZone, false, 7);
			if (this.days == null || this.days.Length == 0)
			{
				this.days = new ExDateTime[]
				{
					ExDateTime.Now.Date
				};
			}
			string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "st", false);
			string queryStringParameter4 = Utilities.GetQueryStringParameter(base.Request, "et", false);
			if (!string.IsNullOrEmpty(queryStringParameter3))
			{
				int.TryParse(queryStringParameter3, out this.startTime);
			}
			if (!string.IsNullOrEmpty(queryStringParameter4))
			{
				int.TryParse(queryStringParameter4, out this.endTime);
			}
			if (this.startTime >= this.endTime || this.startTime < 0 || this.endTime > 24)
			{
				throw new OwaInvalidRequestException("start time and end time must be in range and start time must early than end time");
			}
			if (this.workingDayOnly)
			{
				if (this.viewType == CalendarViewType.Weekly)
				{
					this.viewType = CalendarViewType.WorkWeek;
				}
				else if (this.viewType == CalendarViewType.WeeklyAgenda)
				{
					this.viewType = CalendarViewType.WorkWeeklyAgenda;
				}
			}
			if (this.viewType == CalendarViewType.Min)
			{
				string queryStringParameter5 = Utilities.GetQueryStringParameter(base.Request, "rn", false);
				if (queryStringParameter5 == "1")
				{
					this.renderNotes = true;
				}
			}
			string queryStringParameter6 = Utilities.GetQueryStringParameter(base.Request, "el", false);
			if (!string.IsNullOrEmpty(queryStringParameter6) && queryStringParameter6 == "1")
			{
				this.printEventList = true;
			}
			string queryStringParameter7 = Utilities.GetQueryStringParameter(base.Request, "id", false);
			if (base.SessionContext is AnonymousSessionContext)
			{
				PublishedCalendarAdapter publishedCalendarAdapter = new PublishedCalendarAdapter((AnonymousSessionContext)base.SessionContext);
				publishedCalendarAdapter.LoadData(CalendarUtilities.QueryProperties, this.days, this.startTime, this.endTime, this.viewType);
				this.calendarAdapter = publishedCalendarAdapter;
			}
			else
			{
				OwaStoreObjectId owaStoreObjectId;
				if (queryStringParameter7 != null)
				{
					owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter7);
				}
				else
				{
					ExTraceGlobals.CalendarTracer.TraceDebug(0L, "folder Id is null, using default folder");
					owaStoreObjectId = base.UserContext.CalendarFolderOwaId;
				}
				if (owaStoreObjectId == null)
				{
					throw new OwaInvalidRequestException("Invalid folder id");
				}
				CalendarAdapter calendarAdapter = new CalendarAdapter(base.UserContext, owaStoreObjectId);
				calendarAdapter.LoadData(this.printEventList ? CalendarUtilities.PrintQueryProperties : CalendarUtilities.QueryProperties, this.days, false, this.startTime, this.endTime, ref this.viewType, out this.viewWidth, out this.readingPanePosition);
				this.calendarAdapter = calendarAdapter;
			}
			if (!this.calendarAdapter.UserCanReadItem)
			{
				return;
			}
			base.OnLoad(e);
		}

		protected void RenderCssLink()
		{
			base.SanitizingResponse.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			base.SessionContext.RenderThemeFileUrl(base.SanitizingResponse, ThemeFileId.PrintCalendarCss);
			base.SanitizingResponse.Write("\">");
		}

		protected void RenderTitle()
		{
			if (this.calendarAdapter.CalendarTitle != null)
			{
				base.SanitizingResponse.Write(this.calendarAdapter.CalendarTitle);
			}
		}

		protected void RenderPrintView()
		{
			if (this.calendarAdapter.UserCanReadItem)
			{
				IPrintCalendarViewControl view;
				if (this.viewType == CalendarViewType.Monthly)
				{
					view = new PrintMonthlyView(base.SessionContext, this.calendarAdapter, this.workingDayOnly);
				}
				else if (this.viewType == CalendarViewType.WeeklyAgenda || this.viewType == CalendarViewType.WorkWeeklyAgenda)
				{
					string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "al", false);
					bool isHorizontalLayout = queryStringParameter == "1";
					view = new PrintWeeklyAgendaView(base.SessionContext, this.calendarAdapter, this.viewType, isHorizontalLayout);
				}
				else
				{
					view = new PrintDailyView(base.SessionContext, this.calendarAdapter, this.startTime, this.endTime, this.renderNotes);
				}
				this.InternalRenderPrintView(view, 0);
			}
		}

		private void InternalRenderPrintView(IPrintCalendarViewControl view, int pageNumber)
		{
			base.SanitizingResponse.Write("<table class=\"titleTable");
			if (base.SessionContext.BrowserType == BrowserType.Chrome || base.SessionContext.BrowserType == BrowserType.Safari)
			{
				base.SanitizingResponse.Write(" chromeAndSafariTable");
			}
			base.SanitizingResponse.Write("\"");
			if (pageNumber != 0)
			{
				base.SanitizingResponse.Write(" style=\"display: none\"");
			}
			base.SanitizingResponse.Write(" id=\"page");
			base.SanitizingResponse.Write(pageNumber);
			base.SanitizingResponse.Write("\">");
			base.SanitizingResponse.Write("<tr class=\"printTitle\"><td class=\"titleTd\">");
			this.RenderPageHeader(view);
			base.SanitizingResponse.Write("</td></tr><tr><td>");
			this.RenderViewBody(view);
			base.SanitizingResponse.Write("</td></tr></table>");
		}

		private void RenderPageHeader(IPrintCalendarViewControl view)
		{
			if (view != null)
			{
				base.SanitizingResponse.Write("<table class=\"innerTable titleTable\"><tr><td class=\"titleTd\">");
				base.SanitizingResponse.Write("<div class=\"dateDescription\">");
				base.SanitizingResponse.Write(view.DateDescription);
				base.SanitizingResponse.Write("</div><div class=\"calendarName\">");
				base.SanitizingResponse.Write(view.CalendarName);
				base.SanitizingResponse.Write("</div></td>");
				List<ExDateTime> list = new List<ExDateTime>();
				ExDateTime date = this.days[0].Date;
				ExDateTime[] effectiveDates = view.GetEffectiveDates();
				if (this.viewType != CalendarViewType.Min)
				{
					list.Add(effectiveDates[0].AddMonths(1));
				}
				list.Add(date);
				if (this.viewType == CalendarViewType.Monthly)
				{
					list.Add(effectiveDates[0].AddMonths(-1));
				}
				base.SanitizingResponse.Write("<td class=\"titleTd dpTd");
				base.SanitizingResponse.Write(list.Count);
				base.SanitizingResponse.Write("\"><div id=\"divDatePicker\"");
				if (base.SessionContext.BrowserType == BrowserType.Chrome)
				{
					base.SanitizingResponse.Write(" class=\"chromeDatePicker\"");
				}
				base.SanitizingResponse.Write(">");
				if (effectiveDates.Length > 0)
				{
					foreach (ExDateTime exDateTime in list)
					{
						base.SanitizingResponse.Write("<div class=\"datePickerContainer\">");
						DatePicker datePicker;
						if (exDateTime == date)
						{
							datePicker = new DatePicker("divDatePicker", effectiveDates);
						}
						else
						{
							datePicker = new DatePicker(exDateTime);
						}
						datePicker.Render(base.SanitizingResponse);
						base.SanitizingResponse.Write("</div>");
					}
				}
				base.SanitizingResponse.Write("</div></td></tr></table>");
			}
		}

		private void RenderViewBody(IPrintCalendarViewControl view)
		{
			if (view != null)
			{
				view.RenderView(base.SanitizingResponse);
			}
		}

		protected void RenderEventList()
		{
			if (this.printEventList && this.calendarAdapter.UserCanReadItem)
			{
				PrintEventList printEventList = new PrintEventList(base.SessionContext, this.calendarAdapter, this.viewType, this.workingDayOnly);
				printEventList.RenderView(base.SanitizingResponse);
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.calendarAdapter != null)
			{
				this.calendarAdapter.Dispose();
				this.calendarAdapter = null;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected bool PrintEventList
		{
			get
			{
				return this.printEventList;
			}
		}

		private const string FolderIdQueryParameter = "id";

		private const string StartTimeQueryParameter = "st";

		private const string EndTimeQueryParameter = "et";

		private const string ViewTypeQueryParameter = "vt";

		private const string DayQueryParameter = "d";

		private const string FontSizeQueryParameter = "fs";

		private const string WorkingDayOnlyParameter = "wo";

		private const string RenderNotesParameter = "rn";

		private const string AgendaLayoutParameter = "al";

		private const string PrintEventListParameter = "el";

		private int startTime;

		private int endTime = 24;

		private CalendarAdapterBase calendarAdapter;

		private int viewWidth = 450;

		private CalendarViewType viewType;

		private ReadingPanePosition readingPanePosition = ReadingPanePosition.Right;

		private ExDateTime[] days;

		private bool workingDayOnly;

		private bool renderNotes;

		private bool printEventList;
	}
}
